using System.Collections.Generic;
using System.IO;
using System.Linq;
using RecordsCore;

namespace RecordsSorter.Internal
{
    internal class MergeSortingStrategy : IFileSorter
    {
        private readonly SortingEnviromentRules _sortingEnviromentRules;
        private readonly bool _parallelSort;

        public MergeSortingStrategy(bool parallelSort) : this(new SortingEnviromentRules(), parallelSort)
        {
        }

        public MergeSortingStrategy(SortingEnviromentRules sortingEnviromentRules, bool parallelSort)
        {
            _sortingEnviromentRules = sortingEnviromentRules;
            _parallelSort = parallelSort;
        }

        public void SortFile(string sourceFilePath, string destinationFilePath)
        {
            var sortedFiles = SortFileHelper.SplitToSortedFiles(sourceFilePath, _sortingEnviromentRules.MaxChunkSize, _parallelSort, _sortingEnviromentRules.MaxConcurrency);

            SortFileHelper.MergeTempFilesIntoOneSorted(sortedFiles, destinationFilePath);
        }
    }
}