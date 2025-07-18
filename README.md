# CpuHot – Real-Time CPU Temperature Monitor

A tiny, modern Windows utility that shows your CPU package temperature in real time, changes color according to heat level, and optionally logs or beeps when it gets too hot.

![Screenshot](assets/screenshot.png)

## Features
- **Live temperature** refreshed every second  
- **Color-coded status**  
  - ≤ 45 °C – Green (“COLD”)  
  - 46-55 °C – White (“NORMAL”)  
  - 56-65 °C – Yellow (“WARM”)  
  - ≥ 66 °C – Red (“HOT”) + audible beep  
- **CSV logging** – writes `cpu_hot_log.csv` whenever the CPU is HOT  
- **Resizable dark-themed window**  
- **Single executable** – no installer, no console

## Requirements
- Windows 10/11 64-bit  
- .NET 6 runtime (already included in the self-contained build)

## Quick Start
1. Download the latest release (`CpuHot.exe`).  
2. Run it – administrator rights may be needed on first launch.  
3. Minimize or close with the window’s standard buttons.

## Build from Source
```bash
git clone https://github.com/YourUsername/CpuHot.git
cd CpuHot
dotnet build -c Release
# or self-contained single file:
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true