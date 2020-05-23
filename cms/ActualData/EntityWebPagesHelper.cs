using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public static class EntityWebPagesHelper
    {
        private const string mainPageUrlPrefix = "https://auditbusiness.sos.ca.gov/CBS/SearchResults?SearchType=NUMBER&SearchCriteria=";
        private const string detailPageUrl = "https://auditbusiness.sos.ca.gov/CBS/Detail";

        public static bool IsItNotFoundPage(string webPage)
        {
            if (string.IsNullOrEmpty(webPage))
                return true;

            string notFoundSign1 = "No matching entities found";
            string notFoundSign2 = "dataTables_empty";
            string notFoundSign3 = "__RequestVerificationToken";

            return webPage.Contains(notFoundSign1) && webPage.Contains(notFoundSign2) || !webPage.Contains(notFoundSign3);
        }

        public static string ExtractRequestVerificationToken(string webPage)
        {
            if (string.IsNullOrEmpty(webPage))
                return null;

            string tokenKeyword = "__RequestVerificationToken";
            string valueKeyword = "value=\"";

            int tokenKeywordIndex = webPage.IndexOf(tokenKeyword, StringComparison.InvariantCultureIgnoreCase);
            if (tokenKeywordIndex == -1)
                return null;

            int tokenValueIndex = webPage.IndexOf(valueKeyword, tokenKeywordIndex, StringComparison.InvariantCultureIgnoreCase);
            if (tokenValueIndex == -1)
                return null;

            int verificationTokenStartIndex = tokenValueIndex + valueKeyword.Length;

            int verificationTokenEndIndex = webPage.IndexOf("\"", verificationTokenStartIndex, StringComparison.InvariantCultureIgnoreCase);
            if (verificationTokenEndIndex == -1)
                return null;

            string requestVerificationToken = webPage.Substring(verificationTokenStartIndex, verificationTokenEndIndex - verificationTokenStartIndex);
            return requestVerificationToken;
        }

        public static string GetEntityMainPageUrl(long entityNumber)
        {
            return mainPageUrlPrefix + EntityNumberFormat.ToString(entityNumber, GlobalWorkMode.PrefixForEntityNumbers);
        }

        public static string GetEntityDetailPageUrl()
        {
            return detailPageUrl;
        }

    }
}
