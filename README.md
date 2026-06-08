# WinHighlightSync 🎨

A lightweight C# utility that automatically synchronizes the Windows 11 selection (highlight) color with the system's current accent color. Perfect for **Wallpaper Engine** users who want a perfectly matching UI experience.

## 🌟 Features
- **Real-time Sync:** Monitors accent color changes and updates the system highlight color instantly.
- **Low Footprint:** Uses less than 0.1% CPU by utilizing event polling and WinAPI.
- **Zero Configuration:** Automatically detects the theme and applies changes.
- **Auto-start:** Includes a built-in feature to run on system boot.

## 🛠 How it Works
Windows 11 often keeps the classic "blue" selection color even when the system accent color changes. This tool bridges that gap by:
1. Monitoring the `AccentColor` value in the Windows Registry (`HKCU\Software\Microsoft\Windows\DWM`).
2. Updating the `Hilight` and `HotTrackingColor` registry keys.
3. Forcing an instant UI update using **WinAPI** functions: `SetSysColors` and `SendMessageTimeout`.

## 📸 Visuals
Before/After:
![photo1(before)](https://github.com/user-attachments/assets/f1964aa0-0994-4b61-aa5c-6689d800aa1d)
![photo2(after)](https://github.com/user-attachments/assets/00ab26c9-66c2-4cb9-b536-80b4026b85bd)

Real-time usage:

![8cd53bae9f194290a9849555ca0a1c10](https://github.com/user-attachments/assets/127c91ca-276a-4b2b-8e03-e51c422dadf4)

App icon: 


![8cd53bae9f194290a9849555ca0a1c10](https://github.com/user-attachments/assets/1268c1bd-8a18-4371-8d2b-34625b6d558f)

## UML Diagrams

Use Case:


<img width="1079" height="233" alt="UML1" src="https://github.com/user-attachments/assets/83464b12-775d-40b5-ba4f-b938c78481e3" />

Class Diagram: 


<img width="641" height="733" alt="UML2" src="https://github.com/user-attachments/assets/0a9c1367-5954-4756-aa40-5b8158757294" />

Sequence Diagram:


<img width="939" height="574" alt="UML3" src="https://github.com/user-attachments/assets/e255220c-e85b-467d-b032-a7792636c732" />


## 🚀 Installation
1. Go to the [Releases] page.
2. Download the `WinHighlightSync.exe`.
3. Run the application. It will automatically add itself to the startup and run in the background.

## 🏗 Requirements
- Windows 11
- .NET 9 Runtime (The Release version is self-contained and doesn't require separate installation).

## 📄 License
This project is licensed under the MIT License.

