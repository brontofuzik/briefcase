using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Briefcase.Utils;

namespace Briefcase
{
    public abstract class ActiveObject<T> : IDisposable
    {
        private readonly Thread thread;
        private readonly Queue<Operation> queue = new Queue<Operation>();

        protected ActiveObject(T passive)
        {
            Passive = passive;

            thread = new Thread(Run);
            thread.Start();
        }

        public T Passive { get; }

        public Task<TResult> CallFunction1<TResult>(Func<object> operation)
        {
            var tcs = new TaskCompletionSource<object>();
            Enqueue(Operation.Function(operation, tcs));
            return tcs.Task.Then(r => (TResult)r);
        }

        public Task<TResult> CallFunction2<TResult>(Func<TResult> operation)
        {
            var tcs = new TaskCompletionSource<object>();
            Enqueue(Operation.Function(() => operation(), tcs));
            return tcs.Task.Then(r => (TResult)r);
        }

        public Task CallAction(Action operation)
        {
            var tcs = new TaskCompletionSource<object>();
            Enqueue(Operation.Action(() => operation(), tcs));
            return tcs.Task;
        }

        private void Run()
        {
            while (true)
            {
                var operation = Dequeue();

                if (operation.IsNull)
                    throw new NotImplementedException("Shutting down not implemented!");

                operation.Invoke();
            }
        }

        private void Enqueue(Operation operation)
        {
            lock (queue)
            {
                queue.Enqueue(operation);
                Monitor.Pulse(queue);
            }
        }

        private Operation Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                    Monitor.Wait(queue);
                return queue.Dequeue();
            }
        }

        private class Operation
        {
            private Func<object> function;
            private Action action;

            private TaskCompletionSource<object> tcs;

            public static Operation Function(Func<object> function, TaskCompletionSource<object> tcs)
                => new Operation
                {
                    function = function,
                    tcs = tcs
                };

            public static Operation Action(Action action, TaskCompletionSource<object> tcs)
                => new Operation
                {
                    action = action,
                    tcs = tcs
                };

            public bool IsNull => function == null && action == null;

            public void Invoke()
            {
                try
                {
                    if (function != null)
                    {
                        var result = function.Invoke();
                        tcs.SetResult(result);
                    }
                    else if (action != null)
                    {
                        action.Invoke();
                        tcs.SetResult(null);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
        }

        public void Dispose()
        {
            thread.Abort();
        }
    }
}
