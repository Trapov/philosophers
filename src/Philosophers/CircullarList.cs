namespace Philosophers
{
    using System.Collections.Generic;

    public sealed class CircullarList<T>
    {
        private readonly IReadOnlyList<T> _data;
        public CircullarList(IReadOnlyList<T> data) => _data = data;

        public int Count => _data.Count;

        public T At(in int index) => _data[index];
        public T NextAfter(in int index) => _data[(index + 1) % _data.Count];
        public T Before(in int index) => (index - 1) > 0 ? _data[index - 1] : _data[_data.Count - 1];
    }
}
