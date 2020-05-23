using System;
using RecordsCore;
using RecordsGenerator;

namespace RecordsGeneratorTests
{
    internal class StaticRecordTestGenerator : IValueGenerator<Record>
    {
        private readonly Record _predefined;

        public StaticRecordTestGenerator()
        {
            _predefined = new Record(1, Guid.NewGuid().ToString());
        }

        public Record Next()
        {
            return _predefined;
        }
    }
}