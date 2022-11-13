using System.Threading;
using RecordsCore;

namespace RecordsGenerator
{
    public interface IRecordsSequenceGenerator
    {
        void Generate(long recordsCount, IValueGenerator<Record> recordGenerator, IRecordsOutput recordsOutput, CancellationToken cancellationToken);
    }
}