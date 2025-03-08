using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using Point = System.Drawing.Point;
using Application = System.Windows.Application;

namespace VolumeSlider.Services
{
    public class TrayService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _contextMenu;
        private readonly Window _mainWindow;
        private bool _disposed;

        public event EventHandler? ExitRequested;

        public TrayService(Window mainWindow)
        {
            _mainWindow = mainWindow;

            // Use the application's icon
            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath ?? throw new InvalidOperationException("Could not get process path")),
                Visible = true
            };

            // Create context menu
            _contextMenu = new ContextMenuStrip();
            var openItem = new ToolStripMenuItem("Open");
            var exitItem = new ToolStripMenuItem("Exit");

            openItem.Click += (s, e) => ShowMainWindow();
            exitItem.Click += (s, e) => ExitRequested?.Invoke(this, EventArgs.Empty);

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                openItem,
                new ToolStripSeparator(),
                exitItem
            });

            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.DoubleClick += (s, e) => ShowMainWindow();

            // Handle window state changes
            mainWindow.StateChanged += (s, e) =>
            {
                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.Hide();
                }
            };

            // Handle closing to minimize to tray instead
            mainWindow.Closing += (s, e) =>
            {
                if (!_disposed)
                {
                    e.Cancel = true;
                    mainWindow.Hide();
                }
            };
        }

        public void UpdateTooltip(string text)
        {
            if (!_disposed)
            {
                _notifyIcon.Text = text.Length > 63 ? text[..63] : text; // Windows limits tooltip to 63 chars
            }
        }

        private void ShowMainWindow()
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Icon?.Dispose();
                _notifyIcon.Dispose();
                _contextMenu.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 