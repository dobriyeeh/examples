using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecordsGenerator;

namespace RecordsSorterTests
{
    internal class Generator
    {
        public static void CreateRecordingFile(string filePath, int itemsCount, double frequncy)
        {
            var builder = new RecordGeneratorBuilder();
            var recordGenerator = builder.BuildRecordGenerator((int)(itemsCount * frequncy));
            var recordsSequenceGenerator = RecordsSequenceGeneratorFactory.CreateMultiThreadGenerator();

            using (var output = builder.BuildRecordsOutput(filePath))
            {
                recordsSequenceGenerator?.Generate(itemsCount, recordGenerator, output, CancellationToken.None);
            }
        }
    }
}
