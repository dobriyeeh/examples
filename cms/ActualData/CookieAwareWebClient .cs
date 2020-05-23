using System;
using System.Net;


namespace ActualData
{
    class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer _cookieContainer;

        public CookieAwareWebClient()
        {
            _cookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            var httpRequest = request as HttpWebRequest;

            if (httpRequest == null)
                return request;

            httpRequest.CookieContainer = _cookieContainer;
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
            httpRequest.Timeout = 10000;

            return httpRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            string setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

            if (!string.IsNullOrEmpty(setCookieHeader))
            {
                _cookieContainer.SetCookies(request.RequestUri, setCookieHeader);
            }

            return response;
        }
    }
}
