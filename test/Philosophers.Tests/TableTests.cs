namespace Philosophers.Tests
{
    using Philosophers;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class TableTests
    {
        [Fact]
        public void When_Table_Is_Aranged_Then_True()
        {
            var arrangedTable = Table.ArrangeFor("Sartre", "Plato", "Socrates", "Kant", "Camus");

            Assert.NotEmpty(arrangedTable.Philosophers);
            Assert.Equal(
                expected: arrangedTable.Philosophers.Count,
                actual: arrangedTable.Forks.Count
            );
        }

        [Fact]
        public async Task When_Table_Is_Served_Then_True()
        {
            var arrangedTable = Table.ArrangeFor("Sartre", "Plato", "Socrates", "Kant", "Camus");

            var eatingCounter = 0;
            var thinkingCounter = 0;

            foreach (var philosopher in arrangedTable.Philosophers)
            {
                philosopher.OnStateChange += (state) =>
                {
                    if (state == PhilosopherStates.Eating)
                        Interlocked.Increment(ref eatingCounter);
                    else
                    {
                        Interlocked.Increment(ref thinkingCounter);
                    }
                };
            }

            await Task.WhenAll(arrangedTable.Serve());

            Assert.Equal(5, eatingCounter);
            Assert.Equal(5, thinkingCounter);
        }
    }
}
