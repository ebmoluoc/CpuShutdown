using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;

namespace CpuShutdown.Settings
{

    public sealed partial class AppSettings : SettingsBase
    {

        private ServiceSettings _serviceSettings;
        [Required(ErrorMessage = "{0} is required")]
        public ServiceSettings ServiceSettings
        {
            get { return _serviceSettings; }
            set { SetProperty(ref _serviceSettings, value); }
        }


        public void Save()
        {
            if (HasErrors)
                throw new InvalidOperationException("Cannot save settings containing errors");

            var json = JsonSerializer.Serialize(this);

            File.WriteAllText(AppSettingsJsonPath, json);
        }

    }

}
