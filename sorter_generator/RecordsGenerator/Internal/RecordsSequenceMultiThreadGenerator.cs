using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RecordsCore;

namespace RecordsGenerator.Internal
{
    class RecordsSequenceMultiThreadGenerator : IRecordsSequenceGenerator
    {
        private readonly int _degreeOfParallelism = 1;

        private RecordsSequenceMultiThreadGenerator(int degreeOfParallelism)
        {
            _degreeOfParallelism = degreeOfParallelism;
        }

        public static RecordsSequenceMultiThreadGenerator CreateByProcessorCount()
        {
            return new RecordsSequenceMultiThreadGenerator(Environment.ProcessorCount);
        }

        public static RecordsSequenceMultiThreadGenerator CreateAsSingleProducerConsumer()
        {
            return new RecordsSequenceMultiThreadGenerator(1);
        }

        public void Generate(long recordsCount, IValueGenerator<Record> recordGenerator, IRecordsOutput recordsOutput, CancellationToken cancellationToken)
        {
            var bufferQueue = new BlockingCollection<List<Record>>(_degreeOfParallelism * 4);

            var consumerOutput = Task.Run(() =>
                {
                    foreach (var currSetOfRecords in bufferQueue.GetConsumingEnumerable())
                    {
                        recordsOutput.Write(currSetOfRecords);
                    }
                });

            long workerItemsCount = recordsCount / _degreeOfParallelism;

            const int boundOfHandling = 0xFF;

            Parallel.For(
                0, 
                _degreeOfParallelism, 
                new ParallelOptions(),
                workerIndex =>
                {
                    var setOfHandledRecords = new List<Record>(boundOfHandling);
                    long itemsCount = workerIndex < _degreeOfParallelism - 1
                        ? workerItemsCount
                        : recordsCount - (_degreeOfParallelism - 1) * workerItemsCount;

                    for (long i = 0; i < itemsCount; i++)
                    {
                        if ((i % boundOfHandling == 0) && (i > 0))
                        {
                            bufferQueue.Add(setOfHandledRecords);

                            setOfHandledRecords = new List<Record>(boundOfHandling);
                        }

                        setOfHandledRecords.Add(recordGenerator.Next());
                    }

                    bufferQueue.Add(setOfHandledRecords);
                }
                );

            bufferQueue.CompleteAdding();

            consumerOutput.Wait(cancellationToken);
        }
    }
}
