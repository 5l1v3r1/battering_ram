using System;
using System.Collections.Generic;
using System.Threading;

namespace battering_ram
{
    internal class QueuePump<T>
    {
        public event EventHandler<T> Popped;

        protected Queue<T> queue = new Queue<T>();
        protected bool running = false;
        protected object queuelock = new object();

        public void Push(T o)
        {
            queue.Enqueue(o);
        }

        public T Pop()
        {
            return queue.Dequeue();
        }

        public int GetQueue()
        {
            return queue.Count;
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((object o) => Pump(), null);
        }

        private void Pump()
        {
            if (running)
                return;

            running = true;

            do
            {
                T item;
                lock (queuelock)
                {
                    if (GetQueue() == 0)
                        break;

                    item = Pop();
                }
                OnPopped(item);

            } while (true);

            running = false;
        }

        protected void OnPopped(T item)
        {
            EventHandler<T> handler = Popped;
            if (!Equals(handler, null))
                handler(this, item);
        }
    }
}