# Volume Slider

A modern volume control application for Windows with system tray integration.

## Features

- Modern WPF UI with light/dark theme support
- System tray integration with volume control
- Remember window position and settings
- Audio device selection and management
- Single executable deployment

## Requirements

- Windows 10 or later
- No additional dependencies (self-contained application)

## Installation

1. Download the latest release
2. Run the executable - no installation required
3. (Optional) Add to startup programs for automatic launch

## Usage

- Use the slider to adjust volume
- Click the system tray icon for quick access
- Right-click the system tray icon for options
- Access settings via the gear icon
- Minimize to system tray for background operation

## Building from Source

1. Clone the repository
2. Open in Visual Studio or use .NET CLI
3. Build and run:
```bash
dotnet build
dotnet run --project VolumeSlider
```

For publishing:
```bash
dotnet publish VolumeSlider -c Release
```

## License

MIT License - See LICENSE file for details 