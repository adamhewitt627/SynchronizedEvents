using System.Collections.Specialized;

namespace SynchronizedEvents
{
    public class ContextCollectionChanged : ContextEventBase<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
    {
        protected override void Invoke(NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs args)
            => handler(sender, args);
    }
}
