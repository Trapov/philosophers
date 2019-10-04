namespace Philosophers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System;
    using System.Threading;

    public static class Table
    {
        public sealed class ArrangedTable
        {
            public ArrangedTable(IReadOnlyList<SemaphoreSlim> forks, IReadOnlyList<Philosopher> philosophers)
                => (Forks, Philosophers) = (new CircullarList<SemaphoreSlim>(forks), philosophers);

            public CircullarList<SemaphoreSlim> Forks { get; }
            public IReadOnlyList<Philosopher> Philosophers { get; internal set; }

            public Task[] Serve() => Serve(TimeSpan.FromMilliseconds(200));
            
            public Task[] Serve(TimeSpan @for)
            {
                var tasks = new Task[Philosophers.Count];
                for (var i = 0; i < Philosophers.Count; i++)
                {
                    tasks[i] = Philosophers[i].Eat(
                        leftFork: Forks.Before(index: in i),
                        rightFork: Forks.At(index: in i),
                        @for: @for
                    );
                }

                return tasks;
            }
        }

        public static ArrangedTable ArrangeFor(params Philosopher[] philosophers) => new ArrangedTable(
                Enumerable
                    .Range(0, philosophers.Count())
                    .Select(x => new SemaphoreSlim(1, 1))
                .ToList(),
                philosophers
            );

        public static ArrangedTable ArrangeFor(params string[] philosophersNames) => new ArrangedTable(
                Enumerable
                    .Range(0, philosophersNames.Count())
                    .Select(x => new SemaphoreSlim(1, 1))
                .ToList(),
                philosophersNames
                    .Select(x => new Philosopher(x))
                .ToList()
            );
    }
}
