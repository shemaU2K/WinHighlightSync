using Microsoft.Win32;
using System.Runtime.InteropServices;

class ThemeSynchronizer
{
    [DllImport("user32.dll")]
    public static extern bool SetSysColors(int cElements, int[] lpaElements, uint[] lpaRgbValues);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

    private const int COLOR_HIGHLIGHT = 13;
    private const int COLOR_HOTLIGHT = 26;
    private const uint WM_SETTINGCHANGE = 0x1A;

    static int lastColor = 0;

    static void Main()
    {
        SetAutoStart();
        while (true)
        {
            CheckAndSync();
            Thread.Sleep(500);
        }
    }

    static void SetAutoStart()
    {
        try
        {
            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key.GetValue("WinThemeSync") == null)
            {
                key.SetValue("WinThemeSync", $"\"{appPath}\"");
            }
        }
        catch {}
    }

    static void CheckAndSync()
    {
        try
        {
            using var dwmKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");
            if (dwmKey == null) return;

            var accentColor = (int)dwmKey.GetValue("AccentColor");
            if (accentColor == lastColor) return;

            lastColor = accentColor;

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
        catch { }
    }
}