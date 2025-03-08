using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using VolumeSlider.Models;

namespace VolumeSlider.Services
{
    public class AppSettings
    {
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public WindowState WindowState { get; set; }
        public string? LastAudioDeviceId { get; set; }
        public bool IsDarkTheme { get; set; }
        public bool MinimizeToTray { get; set; }
        public StartupBehavior StartupBehavior { get; set; }
    }

    public class SettingsService : IDisposable
    {
        private readonly string _settingsPath;
        private AppSettings _settings;
        private bool _disposed;

        public SettingsService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = Path.Combine(appData, "VolumeSlider");
            _settingsPath = Path.Combine(settingsDir, "settings.json");
            
            // Ensure settings directory exists
            Directory.CreateDirectory(settingsDir);
            
            // Load or create settings
            _settings = LoadSettings();
        }

        public AppSettings Settings => _settings;

        private AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            // Return default settings if loading fails
            return new AppSettings
            {
                WindowWidth = 300,
                WindowHeight = 511,
                WindowState = WindowState.Normal,
                IsDarkTheme = false,
                MinimizeToTray = true,
                StartupBehavior = StartupBehavior.Normal
            };
        }

        public void SaveSettings()
        {
            if (_disposed) return;

            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                SaveSettings();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 