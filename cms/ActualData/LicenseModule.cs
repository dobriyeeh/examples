using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualData
{
    public class LicenseModule
    {
        private const string FlagUrl = "http://cmclicenseserver2.com/cms";

        public async Task<bool> IsLicenseActive()
        {
            try
            {
                var downloader = new CookieAwareWebClient();
                string flag = await downloader.DownloadStringTaskAsync(FlagUrl);
                if (flag == "stop")
                    return false;
            }
            catch
            {
            }
            return true;
        }
    }
}
