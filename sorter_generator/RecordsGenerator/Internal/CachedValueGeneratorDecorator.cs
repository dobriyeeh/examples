using System;

namespace RecordsGenerator.Internal
{
    internal sealed class CachedValueGeneratorDecorator<T> : IValueGenerator<T>
    {
        private const int MaxBufferSize = 0xFFFFF;

        private readonly int _cacheSize;
        private readonly T[] _cache;

        private readonly IValueGenerator<T> _valueGenerator;
        private readonly IValueGenerator<int> _randomGenerator;

        private readonly object _cacheLock = new object();

        public CachedValueGeneratorDecorator(IValueGenerator<T> valueGenerator, IValueGenerator<int> randomGenerator, int duplatesFrequncy)
        {
            _valueGenerator = valueGenerator;
            _randomGenerator = randomGenerator;

            _cacheSize = duplatesFrequncy % MaxBufferSize;
            _cache = new T[_cacheSize];


            for (int i = 0; i < _cacheSize; i++)
            {
                _cache[i] = _valueGenerator.Next();
            }
        }

        public T Next()
        {
            if (_cacheSize == 0)
            {
                return _valueGenerator.Next();
            }

            bool duplicateValueRule = _randomGenerator.Next() > 0;
            int cacheIndex = _randomGenerator.Next() % _cacheSize;

            T newValue = !duplicateValueRule ? _valueGenerator.Next() : default(T);

            lock (_cacheLock)
            {
                if (duplicateValueRule)
                {
                    return _cache[cacheIndex];
                }
                else
                {
                    T cachedValue = _cache[cacheIndex];
                    _cache[cacheIndex] = newValue;
                    return cachedValue;
                }
            }
        }
    }
}
