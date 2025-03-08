using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VolumeSlider.Models;
using VolumeSlider.Services;

namespace VolumeSlider.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel, IDisposable
    {
        private readonly IAudioService _audioService;
        private readonly ThemeService _themeService;
        private readonly SettingsService _settingsService;
        private TrayService? _trayService;
        private bool _disposed;
        private bool _isUpdatingVolume;

        [ObservableProperty]
        private string _title = "Volume Slider";

        [ObservableProperty]
        private float _volume;

        [ObservableProperty]
        private bool _isMuted;

        [ObservableProperty]
        private AudioDevice? _selectedDevice;

        [ObservableProperty]
        private bool _isDarkTheme;

        [ObservableProperty]
        private bool _minimizeToTray;

        [ObservableProperty]
        private Models.StartupBehavior _startupBehavior;

        public ObservableCollection<AudioDevice> AvailableDevices { get; } = new();

        [RelayCommand]
        private void ToggleMute()
        {
            _audioService.SetMute(!IsMuted);
        }

        [RelayCommand]
        private void RefreshDevices()
        {
            UpdateDeviceList();
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            IsDarkTheme = !IsDarkTheme;
            _themeService.SetTheme(IsDarkTheme ? Theme.Dark : Theme.Light);
            _settingsService.Settings.IsDarkTheme = IsDarkTheme;
            _settingsService.SaveSettings();
        }

        public MainWindowViewModel()
        {
            _audioService = new AudioService();
            _themeService = new ThemeService();
            _settingsService = new SettingsService();
            
            // Load settings
            IsDarkTheme = _settingsService.Settings.IsDarkTheme;
            MinimizeToTray = _settingsService.Settings.MinimizeToTray;
            StartupBehavior = _settingsService.Settings.StartupBehavior;

            // Initialize values
            Volume = _audioService.GetMasterVolume();
            IsMuted = _audioService.IsMuted();
            _themeService.SetTheme(IsDarkTheme ? Theme.Dark : Theme.Light);

            // Subscribe to changes
            _audioService.VolumeChanged += OnVolumeChanged;
            _audioService.MuteStateChanged += (s, muted) => IsMuted = muted;
            _audioService.DeviceListChanged += (s, e) => UpdateDeviceList();
            _audioService.DefaultDeviceChanged += (s, device) => SelectedDevice = device;

            // Initialize device list
            UpdateDeviceList();
        }

        public void InitializeTrayService(Window mainWindow)
        {
            _trayService = new TrayService(mainWindow);
            _trayService.ExitRequested += (s, e) =>
            {
                _disposed = true;
                System.Windows.Application.Current.Shutdown();
            };

            UpdateTrayTooltip();

            // Apply startup behavior
            if (StartupBehavior == Models.StartupBehavior.Minimized)
            {
                mainWindow.WindowState = WindowState.Minimized;
            }
            else if (StartupBehavior == Models.StartupBehavior.MinimizedToTray)
            {
                mainWindow.WindowState = WindowState.Minimized;
                mainWindow.Hide();
            }
        }

        private void OnVolumeChanged(object? sender, float volume)
        {
            if (!_isUpdatingVolume)
            {
                Volume = volume;
                UpdateTrayTooltip();
            }
        }

        private void UpdateTrayTooltip()
        {
            if (_trayService != null)
            {
                string deviceInfo = SelectedDevice != null ? $" - {SelectedDevice}" : "";
                string muteStatus = IsMuted ? " (Muted)" : "";
                _trayService.UpdateTooltip($"Volume: {Volume:P0}{muteStatus}{deviceInfo}");
            }
        }

        private void UpdateDeviceList()
        {
            var devices = _audioService.GetPlaybackDevices().ToList();
            AvailableDevices.Clear();
            foreach (var device in devices)
            {
                AvailableDevices.Add(device);
                if (device.IsDefault || (device.Id == _settingsService.Settings.LastAudioDeviceId))
                {
                    SelectedDevice = device;
                }
            }
            UpdateTrayTooltip();
        }

        partial void OnSelectedDeviceChanged(AudioDevice? value)
        {
            if (value != null)
            {
                if (!value.IsDefault)
                {
                    _audioService.SetDefaultDevice(value.Id);
                }
                _settingsService.Settings.LastAudioDeviceId = value.Id;
                _settingsService.SaveSettings();
            }
            UpdateTrayTooltip();
        }

        partial void OnMinimizeToTrayChanged(bool value)
        {
            _settingsService.Settings.MinimizeToTray = value;
            _settingsService.SaveSettings();
        }

        partial void OnStartupBehaviorChanged(Models.StartupBehavior value)
        {
            _settingsService.Settings.StartupBehavior = value;
            _settingsService.SaveSettings();
        }

        partial void OnVolumeChanged(float value)
        {
            if (!_disposed)
            {
                _isUpdatingVolume = true;
                try
                {
                    _audioService.SetMasterVolume(value);
                }
                finally
                {
                    _isUpdatingVolume = false;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _audioService.VolumeChanged -= OnVolumeChanged;
                _audioService.Dispose();
                _themeService.Dispose();
                _trayService?.Dispose();
                _settingsService.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 