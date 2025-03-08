using System;
using System.Collections.Generic;
using VolumeSlider.Models;

namespace VolumeSlider.Services
{
    public interface IAudioService : IDisposable
    {
        // Existing volume control methods
        float GetMasterVolume();
        void SetMasterVolume(float volume);
        bool IsMuted();
        void SetMute(bool isMuted);
        event EventHandler<float> VolumeChanged;
        event EventHandler<bool> MuteStateChanged;

        // New device management methods
        IEnumerable<AudioDevice> GetPlaybackDevices();
        AudioDevice GetDefaultDevice();
        void SetDefaultDevice(string deviceId);
        event EventHandler<AudioDevice> DefaultDeviceChanged;
        event EventHandler DeviceListChanged;
    }
} 