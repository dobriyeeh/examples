using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Abstractions;
using RecordsSorter.Internal;

namespace RecordsSorter
{
    public class SortingStrategy
    {
        private readonly IFileSystem _fileSystem;
        private readonly SortingEnviromentRules _enviromentRules;

        public SortingStrategy(SortingEnviromentRules enviromentRules, IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _enviromentRules = enviromentRules;
        }

        public IFileSorter ChooseApproachSortMethod(string sourceFilePath)
        {
            long fileSize = _fileSystem.FileInfo.FromFileName(sourceFilePath).Length;

            if (fileSize < _enviromentRules.MaxChunkSize)
            {
                return new ReadIntoMemoryAndSortStrategy();
            }
            else
            {
                return new MergeSortingStrategy(_enviromentRules, true);
            }
        }

    }
}
