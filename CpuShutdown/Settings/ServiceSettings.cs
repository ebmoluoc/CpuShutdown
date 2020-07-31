using System;
using System.ComponentModel.DataAnnotations;

namespace CpuShutdown.Settings
{

    public sealed class ServiceSettings : SettingsBase
    {

        private int _pollingInterval;
        [Required(ErrorMessage = "{0} is required"), Range(1, 5, ErrorMessage = "{0} should be between 1 and 5 seconds")]
        public int PollingInterval
        {
            get { return _pollingInterval; }
            set { SetProperty(ref _pollingInterval, value); }
        }


        private int _yellowTemperature;
        [Required(ErrorMessage = "{0} is required"), Range(40, 120, ErrorMessage = "{0} should be between 40 and 120 degrees Celsius")]
        public int YellowTemperature
        {
            get { return _yellowTemperature; }
            set { SetProperty(ref _yellowTemperature, value); }
        }


        private int _redTemperature;
        [Required(ErrorMessage = "{0} is required"), Range(40, 120, ErrorMessage = "{0} should be between 40 and 120 degrees Celsius")]
        public int RedTemperature
        {
            get { return _redTemperature; }
            set { SetProperty(ref _redTemperature, value); }
        }


        private int _alertTemperature;
        [Required(ErrorMessage = "{0} is required"), Range(40, 120, ErrorMessage = "{0} should be between 40 and 120 degrees Celsius")]
        public int AlertTemperature
        {
            get { return _alertTemperature; }
            set { SetProperty(ref _alertTemperature, value); }
        }


        private int _alertHysteresis;
        [Required(ErrorMessage = "{0} is required"), Range(5, 30, ErrorMessage = "{0} should be between 5 and 30 degrees Celsius")]
        public int AlertHysteresis
        {
            get { return _alertHysteresis; }
            set { SetProperty(ref _alertHysteresis, value); }
        }


        private int _shutdownTemperature;
        [Required(ErrorMessage = "{0} is required"), Range(40, 120, ErrorMessage = "{0} should be between 40 and 120 degrees Celsius")]
        public int ShutdownTemperature
        {
            get { return _shutdownTemperature; }
            set { SetProperty(ref _shutdownTemperature, value); }
        }


        private int _shutdownTimeout;
        [Required(ErrorMessage = "{0} is required"), Range(10, 60, ErrorMessage = "{0} should be between 10 and 60 seconds")]
        public int ShutdownTimeout
        {
            get { return _shutdownTimeout; }
            set { SetProperty(ref _shutdownTimeout, value); }
        }

    }

}
