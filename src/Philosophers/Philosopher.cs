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
            set
            {
                _state = value;
                OnStateChange?.Invoke(_state);
            }
        }

        public event StateChangeCallback OnStateChange;

        public Task Eat(object leftFork, object rightFork) =>
            Task.Run(() =>
            {
                try
                {
                    do { }
                    while (!Monitor.TryEnter(leftFork, TimeSpan.FromMilliseconds(100)));

                    do { }
                    while (!Monitor.TryEnter(rightFork, TimeSpan.FromMilliseconds(100)));

                    State = PhilosopherStates.Eating;

                    Task.Delay(TimeSpan.FromMilliseconds(300)).GetAwaiter().GetResult();

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
