using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RecordsCore;
using RecordsSorter;
using RecordsSorter.Internal;

namespace RecordsSorterTests
{
    [TestFixture]
    public class FileSortingFixture
    {
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

        [Test]
        public void TestComparer()
        {
            var comparer = new RecordsComparer();

            var record1 = new Record(1, "AAA");
            var record2 = new Record(1, "ABB");
            var record3 = new Record(2, "ABB");
            var record4 = new Record(4, "CCC");
            var record5 = new Record(4, "CCC");

            Assert.IsTrue(comparer.Compare(record1, record2) < 0);
            Assert.IsTrue(comparer.Compare(record2, record1) > 0);
            Assert.IsTrue(comparer.Compare(record2, record3) < 0);
            Assert.IsTrue(comparer.Compare(record3, record2) > 0);
            Assert.IsTrue(comparer.Compare(record3, record4) < 0);
            Assert.IsTrue(comparer.Compare(record4, record3) > 0);
            Assert.IsTrue(comparer.Compare(record4, record5) == 0);
            Assert.IsTrue(comparer.Compare(record5, record4) == 0);
        }

        [Test]
        public void TestComparerWithoutEquality()
        {
            var comparer = new RecordsComparerWithoutDuplicates();

            var record1 = new Record(1, "AAA");
            var record2 = new Record(1, "ABB");
            var record3 = new Record(2, "ABB");
            var record4 = new Record(2, "ABB");

            Assert.IsTrue(comparer.Compare(record1, record2) < 0);
            Assert.IsTrue(comparer.Compare(record2, record1) > 0);
            Assert.IsTrue(comparer.Compare(record2, record3) < 0);
            Assert.IsTrue(comparer.Compare(record3, record4) > 0);
            Assert.IsTrue(comparer.Compare(record4, record3) > 0);
        }

        [Test]
        public void TestReader()
        {
            const int RecordsCount = 10;

            Generator.CreateRecordingFile(_originalFilePath = Path.GetTempFileName(), RecordsCount, Frequncy);

            var sorter = new ReadIntoMemoryAndSortStrategy();
            
            sorter.SortFile(_originalFilePath, _sortedFilePath = Path.GetTempFileName());

            var comparer = new RecordsComparer();

            bool isSorted = true;

            using (var sortedSource = new RecordsFileSource(_sortedFilePath, new RecordConverter()))
            {
                var records = sortedSource.GetRecords();

                Record prevRec = null;
                foreach (var currRec in records)
                {
                    if (prevRec != null)
                    {
                        if (comparer.Compare(prevRec, currRec) > 0)
                        {
                            isSorted = false;
                            break;
                        }
                    }

                    prevRec = currRec;
                }
            }

            Assert.IsTrue(isSorted);
        }
    }
}
