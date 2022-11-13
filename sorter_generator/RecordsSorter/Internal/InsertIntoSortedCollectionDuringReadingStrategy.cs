using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordsCore;

namespace RecordsSorter.Internal
{
    internal class InsertIntoSortedCollectionDuringReadingStrategy : IFileSorter
    {
        public void SortFile(string sourceFilePath, string destinationFilePath)
        {
            const int BlockLenth = 0xFF;

            var bufferQueue = new BlockingCollection<List<Record>>(BlockLenth);
            var sortedCollection = new SortedSet<Record>(new RecordsComparerWithoutDuplicates());

            var sorter = Task.Run(() =>
            {
                foreach (var currSetOfRecords in bufferQueue.GetConsumingEnumerable())
                {
                    foreach (var currRecord in currSetOfRecords)
                    {
                        sortedCollection.Add(currRecord);
                    }
                }
            });

            using (var recordsFileSource = new RecordsFileSource(sourceFilePath, new RecordConverter()))
            {
                bool flag = true;
                do
                {
                    var records = recordsFileSource.GetNextRecords(BlockLenth).ToList();
                    if (records.Any())
                    {
                        bufferQueue.Add(records);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                while (flag);
                
                bufferQueue.CompleteAdding();
            }

            sorter.Wait();

            using (var output = new RecordsFileOutput(destinationFilePath, new RecordConverter()))
            {
                output.Write(sortedCollection);
            }
        }
    }
}
