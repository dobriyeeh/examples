using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RecordsCore;
using RecordsGenerator;
using RecordsSorter.Internal;

namespace RecordsSorterTests
{
    [TestFixture]
    public class FileSourceFixture
    {
        private const double Frequncy = 0.1;
        private const int ItemsCount = 10;

        private string _recordsFilePath;

        [SetUp]
        public void SetUp()
        {
            _recordsFilePath = Path.GetTempFileName();

            Generator.CreateRecordingFile(_recordsFilePath, ItemsCount, Frequncy);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_recordsFilePath))
            {
                File.Delete(_recordsFilePath);
            }
        }

        [Test]
        public void TestReader()
        {
            using (var fileSource = new RecordsFileSource(_recordsFilePath, new RecordConverter()))
            {
                var records = fileSource.GetRecords().ToArray();

                Assert.AreEqual(records.Length, ItemsCount);
                Assert.True(records.All(r => !string.IsNullOrEmpty(r.Text) && r.Number > 0));
            }
        }

        [Test]
        public void TestReaderNItems()
        {
            using (var fileSource = new RecordsFileSource(_recordsFilePath, new RecordConverter()))
            {
                var records1 = fileSource.GetNextRecords(4).ToArray();
                Assert.AreEqual(4, records1.Length);

                var records2 = fileSource.GetNextRecords(4).ToArray();
                Assert.AreEqual(4, records2.Length);

                var records3 = fileSource.GetNextRecords(4).ToArray();
                Assert.AreEqual(2, records3.Length);
            }
        }      
        
        [TestCase(100)]
        [TestCase(200)]
        public void TestReaderNBytes(long chunkSize)
        {
            using (var fileSource = new RecordsFileSource(_recordsFilePath, new RecordConverter()))
            {
                var records = fileSource.GetNextRecordsBySize(chunkSize).ToArray();

                long totalSize = 0;
                foreach (var currRecord in records)
                {
                    totalSize += (sizeof(long) + sizeof(char) * currRecord.Text.Length);
                }

                Assert.Less(totalSize, chunkSize);
            }
        }      

    }
}
