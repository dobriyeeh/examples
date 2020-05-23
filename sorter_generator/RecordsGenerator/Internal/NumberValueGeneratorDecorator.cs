using System.Threading;

namespace RecordsGenerator.Internal
{
    internal class NumberValueGeneratorDecorator : IValueGenerator<long>
    {
        private long _counter;

        public long Next()
        {
            return Interlocked.Increment(ref _counter);
        }
    }
}
