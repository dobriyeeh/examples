using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ActualData;
using DataModel;

namespace ActualDataTest
{
    [TestFixture]
    public class EntityInfoRangeFixture
    {
        [Test]
        public async Task EntityInfoRangeShouldCorrentFindFirst2017Item()
        {
            var entityInfoRange = new EntityInfoRange();
            var downloader = new DataDownloader();

            long entityNumber2017 = await entityInfoRange.FindFirstEntityNumberByYearAsync(2017);
            var entity2017 = await downloader.DownloadAsync(entityNumber2017, true);
            var entity2016 = await downloader.DownloadAsync(entityNumber2017 - 1, true);

            Assert.AreEqual(entity2017.Status, DownloadStatus.Ok);
            Assert.AreEqual(2017, entity2017.Entity.RegistrationDate.Year);

            Assert.AreEqual(entity2016.Status, DownloadStatus.Ok);
            Assert.AreEqual(2016, entity2016.Entity.RegistrationDate.Year);
        }

        [Test]
        public async Task EntityInfoRangeShouldAvoidGap()
        {
            var entityInfoRange = new EntityInfoRange();

            long lastEntity = await entityInfoRange.FindLastEntityNumberAsync(3979808);
           
            Assert.AreNotEqual(3979808, lastEntity);
        }


        [Test]
        public async Task EntityInfoRangeShouldCorrentFindFirst2016Item()
        {
            var entityInfoRange = new EntityInfoRange();
            var downloader = new DataDownloader();

            long entityNumber2016 = await entityInfoRange.FindFirstEntityNumberByYearAsync(2016);
            var entity2016 = await downloader.DownloadAsync(entityNumber2016, true);
            var entity2015 = await downloader.DownloadAsync(entityNumber2016 - 1, true);

            Assert.AreEqual(entity2016.Status, DownloadStatus.Ok);
            Assert.AreEqual(2016, entity2016.Entity.RegistrationDate.Year);

            Assert.AreEqual(entity2015.Status, DownloadStatus.Ok);
            Assert.AreEqual(2015, entity2015.Entity.RegistrationDate.Year);
        }

        [Test]
        public async Task EntityInfoRangeShouldCorrectFindLastEntityIndex()
        {
            var entityInfoRange = new EntityInfoRange();
            long start2017 = await entityInfoRange.FindFirstEntityNumberByYearAsync(2017);

            long lastEntityNumber = await entityInfoRange.FindLastEntityNumberAsync(start2017);

            var downloader = new DataDownloader();
            var lastEntity = await downloader.DownloadAsync(lastEntityNumber, true);
            var nonExistEntity = await downloader.DownloadAsync(lastEntityNumber + 1, true);

            Assert.IsNotNull(lastEntity.Entity);
            Assert.AreEqual(DownloadStatus.Ok, lastEntity.Status);

            Assert.IsNull(nonExistEntity.Entity);
            Assert.AreEqual(DownloadStatus.EntityIsNotExists, nonExistEntity.Status);
        }

        [Test]
        public async Task EntityInfoRangeShouldSupportCacheForLastNumber()
        {
            var entityInfoRange = new EntityInfoRange();

            long start2017 = await entityInfoRange.FindFirstEntityNumberByYearAsync(2017);

            long last2017 = await entityInfoRange.FindLastEntityNumberAsync(start2017);

            Assert.AreNotEqual(-1, last2017);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            long lastAgain2017 = await entityInfoRange.FindLastEntityNumberAsync(start2017);
            stopWatch.Stop();

            Assert.AreEqual(last2017, lastAgain2017);
            Assert.IsTrue(stopWatch.Elapsed.Milliseconds < 100);
        }

        [Test]
        public async Task EntityInfoRangeShouldCorrectFindLastSameEntity()
        {
            var entityInfoRange = new EntityInfoRange();

            long lastEntityNumber1 = await entityInfoRange.FindLastEntityNumberAsync();
            EntityInfoRange.ResetCache();
            long lastEntityNumber2 = await entityInfoRange.FindLastEntityNumberAsync(lastEntityNumber1);

            Assert.AreEqual(lastEntityNumber1, lastEntityNumber2);

        }
    }
}
