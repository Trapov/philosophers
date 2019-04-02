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
                    do { }
                    while (!Monitor.TryEnter(leftFork, TimeSpan.FromMilliseconds(1)));

                    do { }
                    while (!Monitor.TryEnter(rightFork, TimeSpan.FromMilliseconds(1)));

                    State = PhilosopherStates.Eating;

                    Task.Delay(@for).GetAwaiter().GetResult();

                    State = PhilosopherStates.Thinking;
                }
                finally
                {
                    Monitor.Exit(leftFork);
                    Monitor.Exit(rightFork);
                }
            });
    }
}
