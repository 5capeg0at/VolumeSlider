using System;
using System.Windows;
using Microsoft.Win32;
using System.Linq;

namespace VolumeSlider.Services
{
    public enum Theme
    {
        Light,
        Dark
    }

    public class ThemeService : IDisposable
    {
        private const string ThemeRegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string AppsUseLightTheme = "AppsUseLightTheme";
        private bool _disposed;

        public event EventHandler<Theme>? ThemeChanged;

        public Theme CurrentTheme { get; private set; }

        public ThemeService()
        {
            // Start with dark theme
            CurrentTheme = Theme.Dark;
            
            // Watch for system theme changes
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        public void SetTheme(Theme theme)
        {
            if (CurrentTheme != theme)
            {
                CurrentTheme = theme;
                ApplyTheme(theme);
                ThemeChanged?.Invoke(this, theme);
            }
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                var newTheme = GetWindowsTheme();
                if (CurrentTheme != newTheme)
                {
                    SetTheme(newTheme);
                }
            }
        }

        private Theme GetWindowsTheme()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(ThemeRegistryKeyPath);
                var value = key?.GetValue(AppsUseLightTheme);
                return value != null && (int)value > 0 ? Theme.Light : Theme.Dark;
            }
            catch
            {
                return Theme.Light; // Default to light theme if we can't read the registry
            }
        }

        private void ApplyTheme(Theme theme)
        {
            var app = System.Windows.Application.Current;
            if (app != null)
            {
                var themeDictionary = new ResourceDictionary
                {
                    Source = new Uri($"/VolumeSlider;component/Themes/{theme}Theme.xaml", UriKind.Relative)
                };

                // Remove old theme dictionary if it exists
                var oldTheme = app.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source?.OriginalString?.Contains("Theme.xaml") == true);
                if (oldTheme != null)
                {
                    app.Resources.MergedDictionaries.Remove(oldTheme);
                }

                // Add new theme dictionary
                app.Resources.MergedDictionaries.Add(themeDictionary);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 