using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Shema_USK.WinHighlightSync
{
    public interface IThemeSyncEngine
    {
        void Start();
        void Stop();
        bool IsRunning { get; }
    }

    public class ThemeSyncEngine : IThemeSyncEngine
    {
        [DllImport("user32.dll")]
        public static extern bool SetSysColors(int cElements, int[] lpaElements, uint[] lpaRgbValues);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        private const int COLOR_HIGHLIGHT = 13;
        private const int COLOR_HOTLIGHT = 26;
        private const uint WM_SETTINGCHANGE = 0x1A;

        private int _lastColor = 0;
        private System.Threading.Timer? _timer;
        public bool IsRunning { get; private set; }

        public void Start()
        {
            if (IsRunning) return;
            _timer = new System.Threading.Timer(CheckAndSync, null, 0, 500);
            IsRunning = true;
        }

        public void Stop()
        {
            _timer?.Dispose();
            IsRunning = false;
        }

        private void CheckAndSync(object? state)
        {
            try
            {
                using var dwmKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");
                if (dwmKey == null) return;

                var accentColor = (int)dwmKey.GetValue("AccentColor");
                if (accentColor == _lastColor) return;

                _lastColor = accentColor;

                byte r = (byte)(accentColor & 0xFF);
                byte g = (byte)((accentColor >> 8) & 0xFF);
                byte b = (byte)((accentColor >> 16) & 0xFF);
                uint colorRef = (uint)((b << 16) | (g << 8) | r);

                SetSysColors(2, new int[] { COLOR_HIGHLIGHT, COLOR_HOTLIGHT }, new uint[] { colorRef, colorRef });

                using var colorsKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true);
                string rgb = $"{r} {g} {b}";
                colorsKey.SetValue("Hilight", rgb);
                colorsKey.SetValue("HotTrackingColor", rgb);

                SendMessageTimeout((IntPtr)0xffff, WM_SETTINGCHANGE, IntPtr.Zero, "Colors", 0x0002, 5000, out _);
            }
            catch {}
        }
    }


    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly IThemeSyncEngine _syncEngine;

        public TrayApplicationContext(IThemeSyncEngine syncEngine)
        {
            _syncEngine = syncEngine;

            _trayIcon = new NotifyIcon
            {
                Icon = new Icon("WinHighlightSync.ico"),
                Text = "WinHighlightSync",
                Visible = true,
                ContextMenuStrip = CreateContextMenu()
            };

            _syncEngine.Start();
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip();

            var toggleItem = new ToolStripMenuItem("Pause Sync", null, (s, e) => ToggleSync((ToolStripMenuItem)s));
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) => Exit());

            menu.Items.Add(toggleItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exitItem);

            return menu;
        }

        private void ToggleSync(ToolStripMenuItem item)
        {
            if (_syncEngine.IsRunning)
            {
                _syncEngine.Stop();
                item.Text = "Resume Sync";
            }
            else
            {
                _syncEngine.Start();
                item.Text = "Pause Sync";
            }
        }

        private void Exit()
        {
            _syncEngine.Stop();
            _trayIcon.Visible = false;
            System.Windows.Forms.Application.Exit();
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            IThemeSyncEngine engine = new ThemeSyncEngine();
            System.Windows.Forms.Application.Run(new TrayApplicationContext(engine));
        }
    }
}