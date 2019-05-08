using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiThreadQueueLib
{
    /// <summary>
    /// Generic class to define a multi-thread queue
    /// </summary>
    /// <typeparam name="V">The generic type</typeparam>
    public class MultiThreadQueue<V> : IMultiThreadQueue, IDisposable
    {
        /// <summary>
        /// The queue
        /// </summary>
        protected Queue<V> list = new Queue<V>();
        /// <summary>
        /// Lock to synch parallel read access.
        /// </summary>
        private ManualResetEvent newElementEvent = new ManualResetEvent(false);

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiThreadQueue()
        {
        }

        /// <summary>
        /// Enqueue a new element in the queue. If the queue is empty the manual resent event is setted in order to allow readers 
        /// to process the incoming elements.
        /// </summary>
        /// <param name="value">The value to insert in the queue.</param>
        public void Enqueue(V value)
        {
            lock (list)
            {
                list.Enqueue(value);
                if (list.Count == 1)
                {
                    newElementEvent.Set();
                }
            }
        }

        /// <summary>
        /// Check if the queue already contains the value.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the queue contains the value, otherwise false.</returns>
        public bool Contains(V value)
        {
            lock (list)
            {
                return list.Contains(value);
            }
        }

        /// <summary>
        /// Wait for new element and unqueue it. When the queue is empty the manual reset event is reset.
        /// A timeout for the waiting could be defined.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout for waiting a new element.</param>
        /// <returns>The new element to process. Null in case there's not a new element to process.</returns>
        public object Unqueue(int millisecondsTimeout)
        {
            bool flag = this.newElementEvent.WaitOne(millisecondsTimeout);
            if (!flag)
            {
                return null;
            }
            lock (list)
            {
                if (list.Count == 0)
                {
                    return null;
                }
                V ret = list.Dequeue();
                if (list.Count == 0)
                {
                    this.newElementEvent.Reset();
                }
                return ret;
            }
        }

        /// <summary>
        /// Wait for new element and unqueue it. When the queue is empty the manual reset event is reset.
        /// The cancellation token allows to exit from this method without waiting for a new element.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new element to process. Null in case there's not a new element to process.</returns>
        public object Unqueue(CancellationToken cancellationToken)
        {
            int idx = System.Threading.WaitHandle.WaitAny(new WaitHandle[] {this.newElementEvent, cancellationToken.Token});
            if (idx == 1)
            {
                cancellationToken.Reset();
                return null;
            }
            lock (list)
            {
                if (list.Count == 0)
                {
                    return null;
                }
                V ret = list.Dequeue();
                if (list.Count == 0)
                {
                    this.newElementEvent.Reset();
                }
                return ret;
            }
        }

        /// <summary>
        /// Wait for new element and unqueue it. When the queue is empty the manual reset event is reset.
        /// This method has not timeout.
        /// </summary>
        /// <returns>The new element to process. Null in case there's not a new element to process.</returns>
        public object Unqueue()
        {
            return this.Unqueue(-1);
        }

        /// <summary>
        /// The queue count
        /// </summary>
        public int Count
        {
            get
            {
                lock (list)
                {
                    return this.list.Count;
                }
            }
        }

        public void Dispose()
        {
            list = null;
            if (newElementEvent != null)
            {
                newElementEvent.Close();
            }
        }
    }
}
