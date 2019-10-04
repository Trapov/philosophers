namespace Philosophers.App.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Program
    {
        private static IReadOnlyList<Philosopher> _philosophers;

        public static async Task Main()
        {
            using var token = new CancellationTokenSource();

            await Task
                .WhenAll(Render(token.Token), PhilosophersLogic(token.Token))
                .ConfigureAwait(true);

            token.Cancel(throwOnFirstException: true);
            Console.ResetColor();
            Console.Clear();
        }

        private static async Task PhilosophersLogic(CancellationToken token)
        {
            var arrangedTable = Table.ArrangeFor("Sartre", "Plato", "Socrates", "Kant", "Camus");

            _philosophers = arrangedTable.Philosophers;
            while (!token.IsCancellationRequested)
                await Task.WhenAll(arrangedTable.Serve(TimeSpan.FromMilliseconds(10))).ConfigureAwait(false);
        }

        private static async Task Render(CancellationToken cancellationToken)
        {
            var process = Process.GetCurrentProcess();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(true);
                Console.Clear();

                var stringBuilder =
                    new StringBuilder()
                        .AppendLine("-------------------------")
                        .AppendLine("[         (0_0)          ]")
                        .AppendLine(@"[         / 0 \          ]")
                        .AppendLine(@"[         \ 0 /          ]")
                        .AppendLine($"[      Memory: {process.WorkingSet64 / 1024 / 1024}Mb      ]")
                        .AppendLine($"[      Threads: {process.Threads.Count}       ]")
                        .AppendLine("[                        ]")
                        .AppendLine("[                        ]")
                        .AppendLine("-------------------------");

                Console.Write(stringBuilder.ToString());

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
            }
        }
    }
}
