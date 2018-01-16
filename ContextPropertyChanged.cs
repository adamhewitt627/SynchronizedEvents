using System.ComponentModel;

namespace SynchronizedEvents
{
    public class ContextPropertyChanged : ContextEventBase<PropertyChangedEventHandler, PropertyChangedEventArgs>
    {
        protected override void Invoke(PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs args)
            => handler(sender, args);
    }
}
