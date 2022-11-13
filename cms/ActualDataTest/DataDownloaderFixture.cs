using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using ActualData;
using DataModel;

namespace ActualDataTest
{
    [TestFixture]
    public class DataDownloaderFixture
    {
        [Test]
        public async Task DownloaderShouldDownloadAllDataCorrectly()
        {
            long entityNumber = 3422388;

            var downloader = new DataDownloader();
            var result = await downloader.DownloadAsync(entityNumber, true);

            Assert.AreEqual(DownloadStatus.Ok, result.Status);
            Assert.AreEqual("INDIANA", result.Entity.Jurisdiction);
        }

        [Test]
        public async Task DownloaderShouldReturnNullForIncorrect()
        {
            long entityNumber = 999999999;

            var downloader = new DataDownloader();
            var result = await downloader.DownloadAsync(entityNumber, true);

            Assert.AreEqual(DownloadStatus.EntityIsNotExists, result.Status);
        }

        [Test]
        public async Task DownloaderShouldSupportCancelation()
        {
            long entityNumber = 3422388;

            var cancelToken = new CancellationTokenSource();
            var downloader = new DataDownloader();
            cancelToken.Cancel();
            var result = await downloader.DownloadAsync(entityNumber, true, cancelToken.Token);
            
            Assert.AreEqual(DownloadStatus.Canceled, result.Status);
        }

        [Test]
        public async Task DownloaderShouldDownloadJustMainDataIfNeed()
        {
            long entityNumber = 3422388;

            var downloader = new DataDownloader();
            var result = await downloader.DownloadAsync(entityNumber, false);

            Assert.AreEqual(DownloadStatus.Ok, result.Status);
            Assert.AreEqual(entityNumber, result.Entity.EntityNumber);
            Assert.AreEqual(new DateTime(2011, 10, 24), result.Entity.RegistrationDate);
            Assert.IsNull(result.Entity.EntityCityStateZip);
            
        }

        [Test]
        public async Task DownloaderShouldSetLastUpdateTime()
        {
            long entityNumber = 3422388;

            var downloader = new DataDownloader();
            var result = await downloader.DownloadAsync(entityNumber, true);

            Assert.AreEqual(DownloadStatus.Ok, result.Status);
            Assert.IsTrue((DateTime.Now - result.Entity.LastUpdate).Seconds < 10);
        }

        [Test]
        public async Task DownloaderShouldCorrentHandlesNameWihoutHtmlSymbols()
        {
            long entityNumber = 3978011;

            var downloader = new DataDownloader();
            var result = await downloader.DownloadAsync(entityNumber, true);

            Assert.AreEqual(DownloadStatus.Ok, result.Status);
            Assert.IsFalse(result.Entity.EntityName.Contains("&") && result.Entity.EntityName.Contains(";"));
        }

        [Test]
        public async Task TaskLicense()
        {
            var license = new LicenseModule();
            Assert.IsTrue(await license.IsLicenseActive());
        }
    }
}
