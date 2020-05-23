using System;
using System.Collections.Generic;

namespace RecordsCore
{
    public interface IRecordsOutput : IDisposable
    {
        void Write(Record record);
        void Write(IEnumerable<Record> records);
    }
}