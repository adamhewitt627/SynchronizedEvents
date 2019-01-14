using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizedEvents
{
    public abstract class ContextEventBase<THandler, TArgs>
    {
        private readonly List<(SynchronizationContext context, THandler handler)> _handlers = new List<(SynchronizationContext context, THandler handler)>(0);

        public void Add(THandler handler)
        {
            if (handler == null) return;
            lock (_handlers) _handlers.Add((SynchronizationContext.Current, handler));
        }
        public void Remove(THandler handler)
        {
            if (handler == null) return;
            lock (_handlers)
            {
                var i = 0;
                foreach (var item in _handlers)
                {
                    if (item.handler.Equals(handler)) { _handlers.RemoveAt(i); break; }
                    i++;
                }
            }
        }

        public async Task Invoke(object sender, TArgs args)
        {
            (SynchronizationContext context, THandler handler)[] handlers;
            lock (_handlers) handlers = _handlers.ToArray();

            var tasks = handlers
                .GroupBy(x => x.context, x => x.handler)
                .Select(g => invokeContext(g.Key, g))
                .ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            async Task invokeContext(SynchronizationContext context, IEnumerable<THandler> l)
            {
                if (context != null)
                {
                    var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    context.Post(o =>
                    {
                        try { invokeHandlers(l); tcs.TrySetResult(true); }
                        catch (Exception e) { tcs.TrySetException(e); }
                    }, null);
                    await tcs.Task.ConfigureAwait(false);
                }
                else
                {
                    await Task.Run(() => invokeHandlers(l)).ConfigureAwait(false);
                }
            }
            void invokeHandlers(IEnumerable<THandler> l)
            {
                foreach (var h in l) Invoke(h, sender, args);
            }
        }

        protected abstract void Invoke(THandler handler, object sender, TArgs args);
    }
}
