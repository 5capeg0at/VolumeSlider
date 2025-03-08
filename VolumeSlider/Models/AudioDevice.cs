using NAudio.CoreAudioApi;

namespace VolumeSlider.Models
{
    public class AudioDevice
    {
        public string Id { get; }
        public string Name { get; }
        public bool IsDefault { get; set; }
        public DeviceState State { get; }

        public AudioDevice(string id, string name, bool isDefault, DeviceState state)
        {
            Id = id;
            Name = name;
            IsDefault = isDefault;
            State = state;
        }

        public override string ToString() => Name;
    }
} 