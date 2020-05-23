using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;
using RecordsSorter;
using RecordsSorter.Internal;

namespace RecordsSorterTests
{
    [TestFixture]
    public class SortPerformanceFixture
    {
        public enum SortMethod
        {   
            ReadIntoMemoryAndSortStrategy,
            InsertIntoSortedCollectionDuringReadingStrategy,
            MergeSortingStrategySingleThread,
            MergeSortingStrategyMultithread,
        }

        const double Frequncy = 0.1;

        private string _originalFilePath;
        private string _sortedFilePath;

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_originalFilePath))
            {
                File.Delete(_originalFilePath);
            }

            if (File.Exists(_sortedFilePath))
            {
                File.Delete(_sortedFilePath);
            }
        }

        [TestCase(1000, SortMethod.ReadIntoMemoryAndSortStrategy)]
        [TestCase(1000, SortMethod.InsertIntoSortedCollectionDuringReadingStrategy)]
        [TestCase(1000, SortMethod.MergeSortingStrategySingleThread)]
        [TestCase(1000, SortMethod.MergeSortingStrategyMultithread)]
        [TestCase(10000, SortMethod.ReadIntoMemoryAndSortStrategy)]
        [TestCase(10000, SortMethod.InsertIntoSortedCollectionDuringReadingStrategy)]
        [TestCase(10000, SortMethod.MergeSortingStrategySingleThread)]
        [TestCase(10000, SortMethod.MergeSortingStrategyMultithread)]
        [TestCase(100000, SortMethod.ReadIntoMemoryAndSortStrategy)]
        [TestCase(100000, SortMethod.InsertIntoSortedCollectionDuringReadingStrategy)]
        [TestCase(100000, SortMethod.MergeSortingStrategySingleThread)]
        [TestCase(100000, SortMethod.MergeSortingStrategyMultithread)]
        [TestCase(1000000, SortMethod.ReadIntoMemoryAndSortStrategy)]
        [TestCase(1000000, SortMethod.InsertIntoSortedCollectionDuringReadingStrategy)]
        [TestCase(1000000, SortMethod.MergeSortingStrategySingleThread)]
        [TestCase(1000000, SortMethod.MergeSortingStrategyMultithread)]
        [TestCase(10000000, SortMethod.ReadIntoMemoryAndSortStrategy)]
        [TestCase(10000000, SortMethod.InsertIntoSortedCollectionDuringReadingStrategy)]
        [TestCase(10000000, SortMethod.MergeSortingStrategySingleThread)]
        [TestCase(10000000, SortMethod.MergeSortingStrategyMultithread)]
        [TestCase(100000000, SortMethod.ReadIntoMemoryAndSortStrategy)]
        [TestCase(100000000, SortMethod.InsertIntoSortedCollectionDuringReadingStrategy)]
        [TestCase(100000000, SortMethod.MergeSortingStrategySingleThread)]
        [TestCase(100000000, SortMethod.MergeSortingStrategyMultithread)]
        public void TestMemorySorting(int recordsCount, SortMethod sortMethod)
        {
            IFileSorter sorter = null;
            switch (sortMethod)
            {
                case SortMethod.ReadIntoMemoryAndSortStrategy:
                    sorter = new ReadIntoMemoryAndSortStrategy();
                    break;
                case SortMethod.InsertIntoSortedCollectionDuringReadingStrategy:
                    sorter = new InsertIntoSortedCollectionDuringReadingStrategy();
                    break;
                case SortMethod.MergeSortingStrategySingleThread:
                    sorter = new MergeSortingStrategy(false);
                    break;
                case SortMethod.MergeSortingStrategyMultithread:
                    sorter = new MergeSortingStrategy(true);
                    break;
            }

            Generator.CreateRecordingFile(_originalFilePath = Path.GetTempFileName(), recordsCount, Frequncy);

            _sortedFilePath = Path.GetTempFileName();
            sorter.SortFile(_originalFilePath, _sortedFilePath);
        }
    }
}
