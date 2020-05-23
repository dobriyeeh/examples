using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RecordsSorter;
using RecordsSorter.Internal;

namespace RecordsSorterTests
{
    [TestFixture]
    public class SortFileHelperFixture
    {
        private List<string> _resultFiles = new List<string>();

        [TearDown]
        public void TearDown()
        {
            if (_resultFiles != null)
            {
                foreach (var currFile in _resultFiles)
                {
                    if (File.Exists(currFile))
                    {
                        File.Delete(currFile);
                    }
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestSplitIntoSortedFiles(bool parallelSort)
        {
            const long chunkSize = 32;

            var sortedFiles = SortFileHelper.SplitToSortedFiles(TestContext.CurrentContext.TestDirectory + "\\Data\\TestData1.txt", chunkSize, parallelSort);
            _resultFiles = sortedFiles.ToList();

            Assert.AreEqual(3, _resultFiles.Count);

            var resultFromFiles = new List<string[]>();
            for (int i = 0; i < 3; i++)
                resultFromFiles.Add(File.ReadAllLines(_resultFiles[i])); 

            resultFromFiles.Sort((elm1, elm2) => elm1[0].CompareTo(elm2[0]));

            string[] expectedFirstFile = { "0. CCC CCC", "4. DDDD DDDD" };
            string[] expectedSecondFile = { "4. 1DDD DDDD" };
            string[] expectedThirdFile = { "5. AAAAA", "6. AAAAA", "1. BBB" };

            Assert.That(expectedFirstFile, Is.EquivalentTo(resultFromFiles[0]));
            Assert.That(expectedSecondFile, Is.EquivalentTo(resultFromFiles[1]));
            Assert.That(expectedThirdFile, Is.EquivalentTo(resultFromFiles[2]));
        }

        [Test]
        public void TestMergeTwoSortedFiles()
        {
            string firstFile = TestContext.CurrentContext.TestDirectory + "\\Data\\MergeData1.txt";
            string secondFile = TestContext.CurrentContext.TestDirectory + "\\Data\\MergeData2.txt";

            string mergedFile = Path.GetTempFileName();
            _resultFiles.Add(mergedFile);

            SortFileHelper.MergeTwoSortedFiles(firstFile, secondFile, mergedFile);
            
            string[] expectedFile = { "1. A1", "2. A2", "3. A3", "4. A3", "5. A3" };
            var actualFile = File.ReadAllLines(mergedFile);

            Assert.That(expectedFile, Is.EquivalentTo(actualFile));
        }

        [Test]
        public void TestMergeFourSortedFiles()
        {
            string[] origins = { "\\Data\\MergeData1.txt", "\\Data\\MergeData2.txt", "\\Data\\MergeData3.txt", "\\Data\\MergeData4.txt" };
            var tempSorted = new List<string>();
            foreach (var currOrigin in origins)
            {
                string newTempFile = Path.GetTempFileName();
                File.Delete(newTempFile);
                File.Copy(TestContext.CurrentContext.TestDirectory + currOrigin, newTempFile);

                tempSorted.Add(newTempFile);
            }

            string resultFile = Path.GetTempFileName();
            _resultFiles.Add(resultFile);
            SortFileHelper.MergeTempFilesIntoOneSorted(tempSorted, resultFile);
            
            string[] expectedFile = { "0. A0", "1. A1", "2. A2", "3. A3", "4. A3", "5. A3", "4. A4", "5. A5", "8. A8", "9. A9" };
            var actualFile = File.ReadAllLines(resultFile);

            Assert.That(expectedFile, Is.EquivalentTo(actualFile));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestMergeSort(bool parallelSort)
        {
            var sourceFile = TestContext.CurrentContext.TestDirectory + "\\Data\\TestSortLong.txt";
            var rule = new SortingEnviromentRules(20);

            var mergeSorting = new MergeSortingStrategy(rule, parallelSort);

            string resultFile = Path.GetTempFileName();
            File.Delete(resultFile);
            _resultFiles.Add(resultFile);

            mergeSorting.SortFile(sourceFile, resultFile);

            string[] expectedFile = { "0. A0", "1. A1 Q", "2. A2", "3. A3", "5. A3", "4. A3 EWQ", "4. A4 qq", "5. A5Q", "8. A8 AF", "9. A9AA" };
            var actualFile = File.ReadAllLines(resultFile);

            Assert.That(expectedFile, Is.EquivalentTo(actualFile));
        }
    }
}
