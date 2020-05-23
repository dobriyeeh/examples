using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecordsCore;

namespace RecordsSorter.Internal
{
    internal static class SortFileHelper
    {
        public static IEnumerable<string> SplitToSortedFiles(string sourceFilePath, long maxChunkSize, bool parallelSort, int maxConcurrency = 4)
        {
            var sortedFiles = new ConcurrentQueue<string>();
            var sortTasks = new List<Task>();

            using (var originalRecordsSource = new RecordsFileSource(sourceFilePath, new RecordConverter()))
            {
                using (Semaphore concurrencySemaphore = new Semaphore(maxConcurrency, maxConcurrency))
                {
                    bool anyRecords;

                    do
                    {
                        var chunkRecords = originalRecordsSource.GetNextRecordsBySize(maxChunkSize).ToList();

                        anyRecords = chunkRecords.Any();
                        if (anyRecords)
                        {
                            Action sorting = () =>
                            {
                                chunkRecords.Sort(new RecordsComparer());

                                var sortedChunkFilePath = Path.GetTempFileName();

                                using (var output = new RecordsFileOutput(sortedChunkFilePath, new RecordConverter()))
                                {
                                    output.Write(chunkRecords);
                                }

                                sortedFiles.Enqueue(sortedChunkFilePath);

                                if (parallelSort)
                                {
                                    concurrencySemaphore.Release();
                                }
                            };

                            if (!parallelSort)
                            {
                                sorting();
                            }
                            else
                            {
                                concurrencySemaphore.WaitOne();

                                sortTasks.Add(Task.Run(sorting));
                            }

                        }
                    }
                    while (anyRecords);

                    if (parallelSort)
                    {
                        Task.WaitAll(sortTasks.ToArray());
                    }
                }
            }

            return sortedFiles;
        }

        private static Record ChooseLessValue(ref Record first, ref Record second, RecordsComparer comparer)
        {
            Record result;

            if (first == null)
            {
                result = second;
                second = null;
                return result;
            }

            if (second == null)
            {
                result = first;
                first = null;
                return result;
            }

            if (comparer.Compare(first, second) <= 0)
            {
                result = first;
                first = null;
            }
            else
            {
                result = second;
                second = null;
            }

            return result;
        }

        public static void MergeTwoSortedFiles(string firstFile, string secondFile, string destination)
        {
            var converter = new RecordConverter();
            var comparer = new RecordsComparer();

            using (var output = new RecordsFileOutput(destination, converter))
            using (var firstSource = new RecordsFileSource(firstFile, converter))
            using (var secondSource = new RecordsFileSource(secondFile, converter))
            {
                var firstRecord = firstSource.GetNextRecord();
                var secondRecord = secondSource.GetNextRecord();

                var lessRecords = ChooseLessValue(ref firstRecord, ref secondRecord, comparer);

                while (lessRecords != null)
                {
                    output.Write(lessRecords);

                    if (firstRecord == null)
                    {
                        firstRecord =  firstSource.GetNextRecord();
                    }

                    if (secondRecord == null)
                    {
                        secondRecord = secondSource.GetNextRecord();
                    }

                    lessRecords = ChooseLessValue(ref firstRecord, ref secondRecord, comparer);
                }
            }
        }

        public static void MergeTempFilesIntoOneSorted(IEnumerable<string> tempSortedShortFiles, string destinationFilePath)
        {
            int fileCount = tempSortedShortFiles.Count();

            if (fileCount == 0)
                throw new ArgumentException("No files");

            if (fileCount == 1)
            {
                File.Delete(destinationFilePath);
                File.Move(tempSortedShortFiles.First(), destinationFilePath);
                return;
            }

            var workingFiles = new List<string>();
            workingFiles.AddRange(tempSortedShortFiles);

            while (workingFiles.Count > 1)
            {
                var sortedBySize = workingFiles.OrderBy(file => new FileInfo(file).Length).ToArray();
                    
                string newTempMergedFile = Path.GetTempFileName();
                string first = sortedBySize[0];
                string second = sortedBySize[1];

                MergeTwoSortedFiles(first, second, newTempMergedFile);

                File.Delete(first);
                workingFiles.Remove(first);
                File.Delete(second);
                workingFiles.Remove(second);

                workingFiles.Add(newTempMergedFile);
            }

            File.Delete(destinationFilePath);
            File.Move(workingFiles.First(), destinationFilePath);
        }
    }
}
