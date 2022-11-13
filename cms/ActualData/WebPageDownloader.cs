using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public class WebPageDownloader : IWebPageDownloader
    {
        public WebProxy WebProxy { set; get; }

        public async Task<EntityWebPages> DownloadAsync(long entityNumber, bool downloadFullInformation, CancellationToken? cancelToken = null)
        {
            using (var webClient = new CookieAwareWebClient())
            {
                cancelToken?.Register(webClient.CancelAsync);

                if (WebProxy != null)
                    webClient.Proxy = WebProxy;

                string mainPageUrl = EntityWebPagesHelper.GetEntityMainPageUrl(entityNumber);
                string detailPageUrl = EntityWebPagesHelper.GetEntityDetailPageUrl();

                string entityNumberNormC = EntityNumberFormat.ToString(entityNumber, GlobalWorkMode.PrefixForEntityNumbers);
                string entityNumberNorm0 = EntityNumberFormat.ToString(entityNumber, GlobalWorkMode.PrefixForEntityNumbers);

                if (cancelToken?.IsCancellationRequested == true)
                    return await Task.FromResult<EntityWebPages>(null);

                string mainPage = await webClient.DownloadStringTaskAsync(mainPageUrl);
                if (EntityWebPagesHelper.IsItNotFoundPage(mainPage))
                    return await Task.FromResult<EntityWebPages>(null);

                if (!downloadFullInformation)
                    return new EntityWebPages(mainPage, null, false);

                string requestVerificationToken = EntityWebPagesHelper.ExtractRequestVerificationToken(mainPage);

                var requestParms = new NameValueCollection
                {
                    { "__RequestVerificationToken", requestVerificationToken },
                    { "SearchType", "NUMBER" },
                    { "SearchSubType", "Exact" },
                    { "enitityTable_length", "10" },
                    { "EntityId", entityNumberNorm0 }
                };

                webClient.Headers[HttpRequestHeader.Referer] = mainPageUrl;

                if (cancelToken?.IsCancellationRequested == true)
                    return await Task.FromResult<EntityWebPages>(null);

                byte[] responsebytes = await webClient.UploadValuesTaskAsync(detailPageUrl, "POST", requestParms);
                string detailPage = Encoding.UTF8.GetString(responsebytes);

                return new EntityWebPages(mainPage, detailPage);
            }
        }
    }
}
