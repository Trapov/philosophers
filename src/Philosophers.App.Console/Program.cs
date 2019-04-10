namespace Philosophers.App.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Program
    {
        private static IReadOnlyList<Philosopher> _philosophers;

        public static void Main()
        {
            var token = new CancellationTokenSource();

            var arrangedTable = Table.ArrangeFor("Sartre", "Plato", "Socrates", "Kant", "Camus");

            _philosophers = arrangedTable.Philosophers;

            var tableServingTask = Task.Run(() =>
            {
                while(!token.IsCancellationRequested) 
                    Task.WhenAll(arrangedTable.Serve(TimeSpan.FromMilliseconds(400))).GetAwaiter().GetResult();
            }, token.Token);

            Console.CancelKeyPress += (_, __) => 
            {
                token.Cancel(throwOnFirstException: true);
                Console.ResetColor();
                Console.Clear();
            };

            while(!token.IsCancellationRequested)
                Render(token.Token);
        }

        private static void Render(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var process = Process.GetCurrentProcess();
            Console.WriteLine("-------------------------");
            Console.WriteLine("[         (0_0)          ]");
            Console.WriteLine(@"[         / 0 \          ]");
            Console.WriteLine(@"[         \ 0 /          ]");
            Console.WriteLine($"[      Memory: {process.WorkingSet64 / 1024 / 1024}Mb      ]");
            Console.WriteLine($"[      Threads: {process.Threads.Count}       ]");
            Console.WriteLine("[                        ]");
            Console.WriteLine("[                        ]");
            Console.WriteLine("-------------------------");


            foreach (var philosopher in _philosophers)
            {
                Console.Write($"{philosopher.Name} is ");
                Console.ForegroundColor = philosopher.State == PhilosopherStates.Thinking ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
                Console.Write(philosopher.State);
                Console.ResetColor();
                Console.Write(" => Has eaten ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(philosopher.EatCount);
                Console.ResetColor();
                Console.Write($" times [Task:{philosopher.TaskId}, Thread:{philosopher.ThreadId}] \n");
            }
            Task.Delay(65).GetAwaiter().GetResult();

            Console.Clear();
        }
    }
}
