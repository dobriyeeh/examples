using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RecordsSorter;
using System.IO.Abstractions;
using NSubstitute;
using RecordsSorter.Internal;

namespace RecordsSorterTests
{
    [TestFixture]
    public class SortingStrategyFixture
    {
        [Test]
        public void ShouldBeUsedMemoryStrategyForNotBigFiles()
        {
            var enviromentRules = new SortingEnviromentRules();

            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.FileInfo.FromFileName(Arg.Any<string>()).Length.Returns(enviromentRules.MaxConcurrency / 10);

            var strategy = new SortingStrategy(new SortingEnviromentRules(), mockFileSystem);
            var sorter = strategy.ChooseApproachSortMethod(@"a:\test.txt");

            Assert.IsTrue(sorter is ReadIntoMemoryAndSortStrategy);
        }

        [Test]
        public void ShouldBeUsedMergeSortStrategyForBigFiles()
        {
            var enviromentRules = new SortingEnviromentRules();

            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.FileInfo.FromFileName(Arg.Any<string>()).Length.Returns(enviromentRules.MaxConcurrency + 1);

            var strategy = new SortingStrategy(enviromentRules, mockFileSystem);
            var sorter = strategy.ChooseApproachSortMethod(@"a:\test.txt");

            Assert.IsTrue(sorter is MergeSortingStrategy);
        }

    }
}
