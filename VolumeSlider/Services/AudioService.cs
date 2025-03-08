using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using VolumeSlider.Models;

namespace VolumeSlider.Services
{
    public class AudioService : IAudioService, IMMNotificationClient
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private MMDevice _device;
        private bool _disposed;

        public event EventHandler<float>? VolumeChanged;
        public event EventHandler<bool>? MuteStateChanged;
        public event EventHandler<AudioDevice>? DefaultDeviceChanged;
        public event EventHandler? DeviceListChanged;

        public AudioService()
        {
            try
            {
                _deviceEnumerator = new MMDeviceEnumerator();
                _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                
                // Subscribe to volume changes
                _device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

                // Register for device notifications
                _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize audio service.", ex);
            }
        }

        public float GetMasterVolume()
        {
            ThrowIfDisposed();
            return _device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        public void SetMasterVolume(float volume)
        {
            ThrowIfDisposed();
            volume = Math.Clamp(volume, 0f, 1f);
            _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
        }

        public bool IsMuted()
        {
            ThrowIfDisposed();
            return _device.AudioEndpointVolume.Mute;
        }

        public void SetMute(bool isMuted)
        {
            ThrowIfDisposed();
            _device.AudioEndpointVolume.Mute = isMuted;
        }

        public IEnumerable<AudioDevice> GetPlaybackDevices()
        {
            ThrowIfDisposed();
            var defaultDevice = _device.ID;
            return _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active | DeviceState.Disabled)
                .Select(device => new AudioDevice(
                    device.ID,
                    device.FriendlyName,
                    device.ID == defaultDevice,
                    device.State));
        }

        public AudioDevice GetDefaultDevice()
        {
            ThrowIfDisposed();
            return new AudioDevice(
                _device.ID,
                _device.FriendlyName,
                true,
                _device.State);
        }

        public void SetDefaultDevice(string deviceId)
        {
            ThrowIfDisposed();
            
            var device = _deviceEnumerator.GetDevice(deviceId);
            if (device != null && device.State == DeviceState.Active)
            {
                // Unsubscribe from old device
                _device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                
                // Update device
                _device = device;
                
                // Subscribe to new device
                _device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
                
                // Notify of changes
                VolumeChanged?.Invoke(this, GetMasterVolume());
                MuteStateChanged?.Invoke(this, IsMuted());
            }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            VolumeChanged?.Invoke(this, data.MasterVolume);
            MuteStateChanged?.Invoke(this, data.Muted);
        }

        #region IMMNotificationClient Implementation

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render && role == Role.Multimedia)
            {
                SetDefaultDevice(defaultDeviceId);
                var newDevice = GetDefaultDevice();
                DefaultDeviceChanged?.Invoke(this, newDevice);
            }
        }

        public void OnDeviceAdded(string pwstrDeviceId) => DeviceListChanged?.Invoke(this, EventArgs.Empty);
        public void OnDeviceRemoved(string pwstrDeviceId) => DeviceListChanged?.Invoke(this, EventArgs.Empty);
        public void OnDeviceStateChanged(string pwstrDeviceId, DeviceState dwNewState) => DeviceListChanged?.Invoke(this, EventArgs.Empty);
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

        #endregion

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AudioService));
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_device != null)
                {
                    _device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                    _deviceEnumerator.UnregisterEndpointNotificationCallback(this);
                    _device.Dispose();
                }
                _deviceEnumerator?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
} 