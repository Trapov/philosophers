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

        public Task Eat(object leftFork, object rightFork, TimeSpan @for) =>
            Task.Run(() =>
            {
                try
                {
                    (TaskId, ThreadId) = (Task.CurrentId, Thread.CurrentThread.ManagedThreadId);

                    var leftForkLocked = false;
                    do
                    {
                        if(leftForkLocked)
                            Monitor.Exit(leftFork);
                        do
                        {
                        }
                        while (!(leftForkLocked = 
                            Monitor.TryEnter(leftFork, TimeSpan.FromMilliseconds(1))));
                    }
                    while (!Monitor.TryEnter(rightFork, TimeSpan.FromMilliseconds(1)));

                    State = PhilosopherStates.Eating;

                    Task.Delay(@for).GetAwaiter().GetResult();

                    (TaskId, ThreadId) = (Task.CurrentId, Thread.CurrentThread.ManagedThreadId);

                    State = PhilosopherStates.Thinking;
                    EatCount++;
                }
                finally
                {
                    Monitor.Exit(leftFork);
                    Monitor.Exit(rightFork);
                }
            });
    }
}
