namespace Philosophers.Tests
{
    using System.Linq;
    using Xunit;

    public sealed class CircullarListTests
    {
        [Fact]
        public void When_List_Is_Circullar_Then_True()
        {
            var list = new CircullarList<int>(Enumerable.Range(0, 10).ToList());

            Assert.Equal(expected: 9, actual: list.Before(0));
            Assert.Equal(expected: 0, actual: list.NextAfter(9));
        }

        [Fact]
        public void When_List_Returns_Valid_Count_Then_True()
        {
            var list = new CircullarList<int>(new int[]{ 0, 2, 3, 4});

            Assert.Equal(4, list.Count);
        }
    }
}
