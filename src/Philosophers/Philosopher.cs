namespace Philosophers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate void StateChangeCallback(PhilosopherStates state);

    public sealed class Philosopher
    {
        private PhilosopherStates _state;

        public Philosopher(string name) => Name = name;

        public int? TaskId { get; private set; }
        public int? ThreadId { get; private set; }

        public int EatCount { get; private set; }

        public string Name { get; }

        public PhilosopherStates State
        {
            get => _state;
            private set
            {
                _state = value;
                OnStateChange?.Invoke(_state);
            }
        }

        public event StateChangeCallback OnStateChange;

        public async Task Eat(SemaphoreSlim leftFork, SemaphoreSlim rightFork, TimeSpan @for)
        {
            try
            {
                (TaskId, ThreadId) = (Task.CurrentId, Thread.CurrentThread.ManagedThreadId);

                var done = false;
                do
                {
                    if (await leftFork.WaitAsync(TimeSpan.FromMilliseconds(1)))
                        if (await rightFork.WaitAsync(TimeSpan.FromMilliseconds(1)))
                            done = true;
                        else
                            leftFork.Release(1);
                }
                while (!done);

                State = PhilosopherStates.Eating;

                await Task.Delay(@for);

                (TaskId, ThreadId) = (Task.CurrentId, Thread.CurrentThread.ManagedThreadId);

                State = PhilosopherStates.Thinking;
                EatCount++;
            }
            finally
            {
                leftFork.Release(1);
                rightFork.Release(1);
            }
        }
    }
}
