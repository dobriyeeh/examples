using System;

namespace RecordsGenerator.Internal
{
    internal class RandomValueGenerator : IValueGenerator<int>
    {
        private readonly int _duplatesFrequncy;
        private readonly Random _randomValueGenerator;

        public RandomValueGenerator(int duplatesFrequncy)
        {
            _duplatesFrequncy = duplatesFrequncy;
            _randomValueGenerator = new Random();
        }

        public int Next()
        {
            return _randomValueGenerator.Next(_duplatesFrequncy);
        }
    }
}
