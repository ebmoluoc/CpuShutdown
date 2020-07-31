using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CpuShutdown.Settings
{

    public abstract class SettingsBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {

        private readonly Dictionary<string, List<ValidationResult>> _errors = new Dictionary<string, List<ValidationResult>>();


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        [JsonIgnore]
        public bool HasErrors
        {
            get { return _errors.Count != 0; }
        }


        public IEnumerable GetErrors(string propertyName)
        {
            propertyName ??= "";
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }


        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            propertyName ??= "";

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this) { MemberName = propertyName };

            if (Validator.TryValidateProperty(value, validationContext, validationResults))
                _errors.Remove(propertyName);
            else if (_errors.ContainsKey(propertyName))
                _errors[propertyName] = validationResults;
            else
                _errors.Add(propertyName, validationResults);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
