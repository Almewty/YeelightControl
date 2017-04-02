using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using YeelightControl.Models.Enums;

namespace YeelightControl.Models
{
    public class Device : ModelBase
    {
        public Uri Location { get; set; }
        public DateTime LastUpdate { get; set; }
        public long Id { get; set; }
        public Model Model { get; set; }
        public uint FirmwareVersion { get; set; }
        public ObservableCollection<string> SupportedMethods { get; set; }
        public bool IsOn { get; set; }
        public int Brightness { get; set; }
        public ColorMode ColorMode { get; set; }
        public int ColorTemperature { get; set; }
        public Color Color { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Device;
            if (other == null)
                return false;
            return other.Id == Id;
        }

        protected bool Equals(Device other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Id.GetHashCode();
            }
        }
    }
}