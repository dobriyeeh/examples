using System.Collections.Generic;
using RecordsCore;
using RecordsGenerator;

namespace RecordsGeneratorTests
{
    internal class DummyRecordsOutput : IRecordsOutput
    {
        public void Dispose()
        {
        }

        public void Write(Record record)
        {
        }

        public void Write(IEnumerable<Record> records)
        {
        }
    }
}