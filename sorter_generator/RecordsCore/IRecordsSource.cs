using System;
using System.Collections.Generic;

namespace RecordsCore
{
    public interface IRecordsSource : IDisposable
    {
        IEnumerable<Record> GetRecords();

        Record GetNextRecord();

        IEnumerable<Record> GetNextRecords(int recordsCount);

        IEnumerable<Record> GetNextRecordsBySize(long chunkSize);
    }
}