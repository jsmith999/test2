using System;
using System.Collections.Generic;
using System.Threading;

namespace Conta.UiController {
    public interface IObservableService<T> : IDisposable {
        void Post(T message);
        IDisposable Register(Action<T> listener);
    }

    public class ObservableService<T> : IObservableService<T> {
        private readonly object listenersLock = new object();
        private List<Unsubscriber> listeners = new List<Unsubscriber>();

        public void Post(T message) {
            lock (listenersLock) {
                //var cachedListeners = new List<Unsubscriber>(listeners);
                //foreach (var listener in cachedListeners)
                //    listener.Listener(message);

                for (var index = 0; index < listeners.Count;index++ )
                    listeners[index].Listener(message);
            }
        }

        public IDisposable Register(Action<T> listener) {
            if (listener == null)
                return null;

            var result = new Unsubscriber(this, listener);
            lock (listenersLock) {
                listeners.Add(result);
            }
            return result;
        }

        public void Dispose() {
            lock (listenersLock) {
                while (listeners.Count > 0) {
                    var item = listeners[listeners.Count - 1];
                    item.Dispose();
                }
            }
        }

        class Unsubscriber : IDisposable {
            private ObservableService<T> observable;
            public Unsubscriber(ObservableService<T> observable, Action<T> listener) {
                this.observable = observable;
                this.Listener = listener;
            }

            public Action<T> Listener { get; set; }

            public void Dispose() {
                if (this.observable == null)
                    return;

                ThreadPool.QueueUserWorkItem(_ => {
                    lock (this.observable.listenersLock) {
                        Listener = null;
                        this.observable.listeners.Remove(this);
                        this.observable = null;
                    }
                });
            }
        }
    }
}

