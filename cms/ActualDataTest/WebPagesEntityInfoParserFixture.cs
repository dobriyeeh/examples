using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActualData;
using NUnit.Framework;

namespace ActualDataTest
{
    [TestFixture]
    public class WebPagesEntityInfoParserFixture
    {
        private string _mainPage;
        private string _detailedPage;

        [SetUp]
        public void SetUp()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _mainPage = File.ReadAllText(dir + "\\TestData\\MainPage.html");
            _detailedPage = File.ReadAllText(dir + "\\TestData\\DetailedPage.html");
        }

        [Test]
        public void TestBadParsing()
        {
            string something = "<erfref>wedfewdwedwed<wedwed>wedwedwedwed";
            var parser = new WebPagesEntityInfoParser();
            var parcedData = parser.Parse(new EntityWebPages(something, something));
            Assert.AreEqual(DownloadStatus.EntityIsNotExists, parcedData.Status);
        }

        [Test]
        public void TestGoodParsing()
        {
            var parser = new WebPagesEntityInfoParser();
            var parcedData = parser.Parse(new EntityWebPages(_mainPage, _detailedPage));
            Assert.AreEqual(DownloadStatus.Ok, parcedData.Status);
        }

        [Test]
        public void TestDataDecoder()
        {
            string decoded = WebPagesEntityInfoParser.DataDecode("  AB     DE JK     LL&amp;    f ");
            Assert.IsTrue(decoded == "AB DE JK LL& f");
        }
    }
}
