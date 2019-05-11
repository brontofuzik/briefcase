using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiThreadQueueLib
{
    public class CancellationToken : IDisposable
    {
        /// <summary>
        /// The event for cancellation.
        /// </summary>
        private ManualResetEvent cancelEvent = new ManualResetEvent(false);

        public ManualResetEvent Token
        {
            get
            {
                return cancelEvent;
            }
        }

        public void Cancel()
        {
            cancelEvent.Set();
        }

        public void Reset()
        {
            cancelEvent.Reset();
        }

        public void Dispose()
        {
            if (cancelEvent != null)
            {
                cancelEvent.Close();
            }
        }
    }
}
