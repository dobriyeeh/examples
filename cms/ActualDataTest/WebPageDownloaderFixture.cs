using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ActualData;
using NUnit.Framework;
using DataModel;

namespace ActualDataTest
{
    [TestFixture]
    public class WebPageDownloaderFixture
    {
        [Test]
        public void UrlShouldBeCorrentForAnyEntityNumber()
        {
            string expectedMainPageUrl = "https://auditbusiness.sos.ca.gov/CBS/SearchResults?SearchType=NUMBER&SearchCriteria=C3422388";
            string expectedDetailPageUrl = "https://auditbusiness.sos.ca.gov/CBS/Detail";

            Assert.AreEqual(expectedMainPageUrl, EntityWebPagesHelper.GetEntityMainPageUrl(3422388));
            Assert.AreEqual(expectedDetailPageUrl, EntityWebPagesHelper.GetEntityDetailPageUrl());
        }

        [Test]
        public void HelperShouldExtractVerificationToken()
        {
            string html =
"<div class=\"container\" id=\"body-content\">																			" +
"    <div class=\"row\">                                                                                                " +
"		<div class=\"col-xs-12 col-sm-9 col-sm-push-3\" id=\"maincontent\">                                             " +
"			<form action=\"/Document/RetrievePDF\" id=\"formDetails\"                                                   " +
"			method=\"post\" target=\"_blank\"><input name=\"__RequestVerificationToken\"                                " +
"			type=\"hidden\"                                                                                             " +
"			value=\"SKSbSPp5ac7LYH42DplYkcRiUTZqxJcpIhQJ9fjoK9vMIjYaTPj_JzB7eX0nUun6UvV5yRSu0Y97aanq8HAgAj5ranw1\" />   " +
"            <h1>                                                                                                       " +
"                <img src=\"http://bpd.cdn.sos.ca.gov/images/cbs-search-icon.jpg\" alt=\"\" role=\"presentation\" />    " +
"                                                                                                                       ";

            string expectedRequestVerificationToken =
                "SKSbSPp5ac7LYH42DplYkcRiUTZqxJcpIhQJ9fjoK9vMIjYaTPj_JzB7eX0nUun6UvV5yRSu0Y97aanq8HAgAj5ranw1";

            Assert.AreEqual(expectedRequestVerificationToken, EntityWebPagesHelper.ExtractRequestVerificationToken(html));
        }

        [Test]
        public void TestEntityNumberNormalization()
        {
            Assert.AreEqual("C12345", EntityNumberFormat.NormalizeEntityNumber("C12345"));
            Assert.AreEqual("C12345", EntityNumberFormat.NormalizeEntityNumber("12345"));
            Assert.AreEqual("C12345", EntityNumberFormat.NormalizeEntityNumber("Q12345"));
            Assert.AreEqual("012345", EntityNumberFormat.NormalizeEntityNumber("C12345", '0'));
            Assert.AreEqual("012345", EntityNumberFormat.NormalizeEntityNumber("012345", '0'));
        }

        [Test]
        public async Task WebPagesDownloaderShouldGet2Pages()
        {
            var downloader = new WebPageDownloader();
            var result = await downloader.DownloadAsync(3911411, true);

            Assert.IsNotNull(result);
        }

        [Test]
        public void WebPagesDownloaderShouldPassExceptionsIfNetworkIssues()
        {
            var downloader = new WebPageDownloader();
            downloader.WebProxy = new WebProxy("http://proxyserver:80/", true);

            Assert.ThrowsAsync<WebException>(async () => await downloader.DownloadAsync(11111, true));
        }
    }
}
