namespace Philosophers.App.Console
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Program
    {
        private static IReadOnlyList<Philosopher> _philosophers;
        private static CircullarList<object> _forks;

        public static void Main()
        {
            var token = new CancellationTokenSource();

            var arrangedTable = Table.ArrangeFor("Sartre", "Plato", "Socrates", "Kant", "Camus");

            _philosophers = arrangedTable.Philosophers;
            _forks = arrangedTable.Forks;

            Task.Run(() =>
            {
                while(!token.IsCancellationRequested) 
                    Task.WhenAll(arrangedTable.Serve(TimeSpan.FromMilliseconds(200))).GetAwaiter().GetResult();
            }, token.Token);

            System.Console.CancelKeyPress += (_, __) => 
            {
                token.Cancel();
                System.Console.WriteLine("Cancellation has been requested.");
            };

            while(!token.IsCancellationRequested)
                Render();
        }

        private static void Render()
        {
            
            System.Console.Write("Forks : [");
            
            for (var i = 0; i < _forks.Count; i++)
            {
                var isLocked = !Monitor.TryEnter(_forks.At(i));

                if(!isLocked)
                    Monitor.Exit(_forks.At(i));
                
                System.Console.ForegroundColor = isLocked ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
                
                System.Console.Write($" {i}, ");
                
                System.Console.ResetColor();
            }
            
            System.Console.Write("] \n");

            System.Console.WriteLine("--------------------");

            foreach(var philosopher in _philosophers)
            {
                System.Console.Write($"{philosopher.Name} is ");
                System.Console.ForegroundColor = philosopher.State == PhilosopherStates.Thinking ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
                System.Console.Write($"{philosopher.State} \n");
                System.Console.ResetColor();
            }
            Task.Delay(10).GetAwaiter().GetResult();

            System.Console.Clear();
        }
    }
}
