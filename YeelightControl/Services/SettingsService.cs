using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using Catel.Runtime.Serialization.Xml;
using YeelightControl.Models;
using YeelightControl.Services.Interfaces;

namespace YeelightControl.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _path;
        private readonly XmlSerializationConfiguration _serializationConfiguration;

        public SettingsService()
        {
            var directory = Catel.IO.Path.GetApplicationDataDirectory("YeelightControl");

            _path = Path.Combine(directory, "settings.xml");
            _serializationConfiguration = new XmlSerializationConfiguration();
        }

        public Settings LoadSettings()
        {
            using (var fs = File.OpenRead(_path))
            {
                return Settings.Load(fs, SerializationMode.Xml, _serializationConfiguration);
            }
        }

        public void SaveSettings(Settings settings)
        {
            using (var fs = File.OpenWrite(_path))
            {
                settings.Save(fs, SerializationMode.Xml, _serializationConfiguration);
            }
        }
    }
}