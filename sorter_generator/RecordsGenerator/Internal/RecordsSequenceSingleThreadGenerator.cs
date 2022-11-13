using System.Threading;
using RecordsCore;

namespace RecordsGenerator.Internal
{
    internal class RecordsSequenceSingleThreadGenerator : IRecordsSequenceGenerator
    {
        public void Generate(long recordsCount, IValueGenerator<Record> recordGenerator, IRecordsOutput recordsOutput, CancellationToken cancellationToken)
        {
            for (long i = 0; i < recordsCount; i++)
            {
                if (i % 255 == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                recordsOutput.Write(recordGenerator.Next());
            }
        }
    }
}
