using System;
using System.Collections.Generic;
using System.Text;

namespace MultiThreadQueueLib
{
    /// <summary>
    /// Interface for a multi-thread queue
    /// </summary>
    public interface IMultiThreadQueue
    {
        /// <summary>
        /// Wait and unqueue a new element. The wait has a timeout in ms (-1 is infinite).
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in ms.</param>
        /// <returns>The unqueue element. Null in there's not a new element to unqueue</returns>
        object Unqueue(int millisecondsTimeout);
        /// <summary>
        /// Wait and unqueue a new element. The wait is infinite.
        /// </summary>
        /// <returns>The unqueue element.</returns>
        object Unqueue();
    }
}
