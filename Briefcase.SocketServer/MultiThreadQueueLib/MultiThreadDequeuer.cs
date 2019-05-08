using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiThreadQueueLib
{
    /// <summary>
    /// Abstract class to define a dequeuer thread
    /// </summary>
    public abstract class MultiThreadDequeuer : IDisposable
    {
        /// <summary>
        /// The thread that process the reading elements
        /// </summary>
        private Thread th = null;
        /// <summary>
        /// The multi-thread queue.
        /// </summary>
        private IMultiThreadQueue sourceQueue = null;
        /// <summary>
        /// Flag to exit from the thread
        /// </summary>
        protected bool shutdown = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="queue">The multi-thread queue</param>
        public MultiThreadDequeuer(IMultiThreadQueue queue)
        {
            this.sourceQueue = queue;
        }

        /// <summary>
        /// Start the dequeuer thread.
        /// </summary>
        public virtual void Start()
        {
            th = new Thread(this.Run);
            th.Name = this.GetType().FullName;
            th.Start();
        }

        /// <summary>
        /// Stop the dequeuer thread
        /// </summary>
        /// <param name="timeOutMs"></param>
        public virtual void Shutdown(int timeOutMs)
        {
            lock (this)
            {
                if (shutdown)
                {
                    return;
                }
                shutdown = true;
                if (th != null)
                {
                    if (!th.Join(timeOutMs))
                    {
                        th.Interrupt();
                    }
                    if (!th.Join(timeOutMs))
                    {
                        th.Abort();
                    }
                    th = null;
                }
            }
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public virtual void Dispose()
        {
            this.Shutdown(1000);
        }

        /// <summary>
        /// The thread state.
        /// </summary>
        /// <returns>The thread state enum.</returns>
        public ThreadState GetThreadState()
        {
            return this.th.ThreadState;
        }

        /// <summary>
        /// Thread loop. Wait for a new incoming element and in case the threag get the new element, the mehod OnNewElement is called.
        /// </summary>
        protected virtual void Run()
        {
            while (!shutdown)
            {
                try
                {
                    object newElement = this.sourceQueue.Unqueue();
                    if (newElement != null)
                    {
                        OnNewElement(newElement);
                    }
                }
                catch (ThreadInterruptedException thInt)
                {
                }
                catch (ThreadAbortException thAbort)
                {
                    shutdown = true;
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex);
                }
            }
            Console.Out.WriteLine("Exit from Run. Thread is stopped.");
        }

        internal void Interrupt(bool withShutdown)
        {
            shutdown = withShutdown;
            th.Interrupt();
        }

        /// <summary>
        /// Override this abstact method in order to define the process to elaborate the received element.
        /// </summary>
        /// <param name="element">The new received element.</param>
        protected abstract void OnNewElement(object element);
    }
}
