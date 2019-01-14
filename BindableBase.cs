using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SynchronizedEvents
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        private readonly ContextPropertyChanged _propertyChanged = new ContextPropertyChanged();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _propertyChanged.Add(value);
            remove => _propertyChanged.Remove(value);
        }

        protected bool SetProperty<T>(ref T originalValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(originalValue, newValue))
            {
                originalValue = newValue;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected async void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => await _propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName)).ConfigureAwait(false);
    }
}