using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public class DataDownloader : IDataDownloader
    {
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly IWebPagesEntityInfoParser _entityInfoParser;


        public DataDownloader() : this(new WebPageDownloader(), new WebPagesEntityInfoParser())
        {
        }

        public DataDownloader(IWebPageDownloader webPageDownloader, IWebPagesEntityInfoParser entityInfoParser)
        {
            _webPageDownloader = webPageDownloader;
            _entityInfoParser = entityInfoParser;
        }


        public async Task<DownloadResult> DownloadAsync(long entityNumber, bool downloadFullInformation, CancellationToken? cancelToken = null)
        {
            try
            {
                if (cancelToken?.IsCancellationRequested == true)
                    return new DownloadResult(DownloadStatus.Canceled);

                var webPages = await _webPageDownloader.DownloadAsync(entityNumber, downloadFullInformation, cancelToken);
                if (cancelToken?.IsCancellationRequested == true)
                    return new DownloadResult(DownloadStatus.Canceled);

                var downloadResult = _entityInfoParser.Parse(webPages);

                if (downloadResult?.Status == DownloadStatus.Ok)
                {
                    downloadResult.Entity.LastUpdate = DateTime.Now;
                }

                return downloadResult;
            }
            catch (WebException)
            {
            }

            return new DownloadResult(DownloadStatus.NetworkProblem);
        }
    }
}
