using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VolumeSlider.Services;
using VolumeSlider.ViewModels;
using VolumeSlider.Views;

namespace VolumeSlider;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Storyboard _volumeChangeAnimation;
    private readonly SettingsService _settingsService;
    private float _lastVolume;
    private bool _isClosing;

    public MainWindow()
    {
        InitializeComponent();
        _volumeChangeAnimation = (Storyboard)FindResource("VolumeChangeAnimation");
        _settingsService = new SettingsService();
        
        // Restore window position and size
        if (_settingsService.Settings.WindowLeft != 0 || _settingsService.Settings.WindowTop != 0)
        {
            Left = _settingsService.Settings.WindowLeft;
            Top = _settingsService.Settings.WindowTop;
        }
        if (_settingsService.Settings.WindowWidth > 0 && _settingsService.Settings.WindowHeight > 0)
        {
            Width = _settingsService.Settings.WindowWidth;
            Height = _settingsService.Settings.WindowHeight;
        }
        
        // Handle mouse wheel events for the entire slider area
        VolumeSlider.PreviewMouseWheel += (sender, e) =>
        {
            double change = e.Delta > 0 ? 0.02 : -0.02; // 2% change per wheel tick
            VolumeSlider.Value = Math.Clamp(VolumeSlider.Value + change, 0, 1);
            e.Handled = true;
        };

        // Handle volume changes
        VolumeSlider.ValueChanged += (s, e) =>
        {
            if (Math.Abs((float)e.NewValue - _lastVolume) > 0.01f)
            {
                _lastVolume = (float)e.NewValue;
                _volumeChangeAnimation.Begin(VolumeText);
            }
        };

        // Save window position and size when changed
        LocationChanged += (s, e) => SaveWindowSettings();
        SizeChanged += (s, e) => SaveWindowSettings();
        StateChanged += (s, e) => SaveWindowSettings();

        // Initialize tray service
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.InitializeTrayService(this);
        }
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow
        {
            Owner = this,
            DataContext = DataContext
        };
        settingsWindow.ShowDialog();
    }

    private void SaveWindowSettings()
    {
        if (_isClosing) return;

        _settingsService.Settings.WindowState = WindowState;
        
        if (WindowState == WindowState.Normal)
        {
            _settingsService.Settings.WindowLeft = Left;
            _settingsService.Settings.WindowTop = Top;
            _settingsService.Settings.WindowWidth = Width;
            _settingsService.Settings.WindowHeight = Height;
        }
        
        _settingsService.SaveSettings();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        _isClosing = true;
        base.OnClosing(e);
    }
}