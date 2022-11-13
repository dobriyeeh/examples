using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using DataModel;
using HtmlAgilityPack;

namespace ActualData
{
    public class WebPagesEntityInfoParser : IWebPagesEntityInfoParser
    {
        public static DateTime FromString(string stringTime)
        {
            if (string.IsNullOrEmpty(stringTime))
                return default(DateTime);

            var parts = stringTime.Trim().Split('/');
            if (parts.Length != 3)
                return default(DateTime);

            string day = parts[1];
            string month = parts[0];
            string year = parts[2];

            return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
        }

        public static string DataDecode(string original)
        {
            string decoded = WebUtility.HtmlDecode(original ?? "");

            decoded = decoded.Trim();

            while (decoded.IndexOf("  ", StringComparison.Ordinal) != -1)
            {
                decoded = decoded.Replace("  ", " ");
            }

            return decoded;
        }

        public static string AddressStateFix(string originalAddress)
        {
            string californiaState = " CA ";

            int? lastIndex = originalAddress?.LastIndexOf(californiaState, StringComparison.OrdinalIgnoreCase);
            if ((lastIndex ?? -1) > 0)
            {
                string result = originalAddress?.Insert((int)lastIndex, ",");
                return result;
            }

            return originalAddress;
        }

        private bool TryParseMainPage(EntityWebPages entityWebPages, ref EntityInformation entity)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(entityWebPages.MainPage);

            var documentNode = htmlDoc.DocumentNode;

            var dataNodes = documentNode?.SelectNodes("//div[@id='container-entity-grid']//tbody/tr/td");
            if (dataNodes?.Count >= 6)
            {
                entity.EntityNumber             = EntityNumberFormat.ToNumeric(dataNodes[0].InnerText?.Trim());
                entity.RegistrationDate         = FromString(dataNodes[1].InnerText?.Trim());
                entity.Status                   = dataNodes[2].InnerText?.Trim();
                entity.Status                   = DataDecode(entity.Status);
                entity.Jurisdiction             = dataNodes[4].InnerText?.Trim();
                entity.Jurisdiction             = DataDecode(entity.Jurisdiction);
                entity.AgentForServiceOfProcess = dataNodes[5].InnerText?.Trim();
                entity.AgentForServiceOfProcess = DataDecode(entity.AgentForServiceOfProcess);

                return true;
            }

            return false;
        }

        private bool TryParseDetailPage(EntityWebPages entityWebPages, ref EntityInformation entity)
        {
            HtmlNode.ElementsFlags.Remove("form");

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(entityWebPages.DetailedPage);

            var documentNode = htmlDoc.DocumentNode;

            var nameNode = documentNode?.SelectSingleNode("//div[@class='row']//h2");
            if (nameNode == null)
                return false;

            string nameText = nameNode.InnerText?.Trim();
            if (string.IsNullOrEmpty(nameText))
                return false;
            
            if (entity.EntityNumber > 0)
            {
                string withoutNumber = nameText.Replace(EntityNumberFormat.ToString(entity.EntityNumber, GlobalWorkMode.PrefixForEntityNumbers), "");
                string withoutSpaces = withoutNumber.Replace("&nbsp;", "").Trim();
                entity.EntityName = withoutSpaces;
            }
            else
            {
                var lexems = nameText.Split(new[] { "&nbsp; &nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                if (lexems.Length != 2)
                    return false;

                entity.EntityName = lexems.LastOrDefault(word => word.Trim().Length > 0)?.Trim();
            }
            entity.EntityName = DataDecode(entity.EntityName);

            var rowsNode = documentNode?.SelectNodes("//div[@class='row'][./div/strong]");
            if (rowsNode == null)
                return false;

            foreach (var currNode in rowsNode)
            {
                var labelNode = currNode.SelectSingleNode(".//strong");
                var valueNode = currNode.SelectSingleNode(".//div[not(.//strong)]");

                if ((labelNode == null) || (valueNode == null))
                    continue;

                string labelName = labelNode?.InnerText?.Trim();

                switch (labelName)
                {
                    case "Registration Date:":
                        entity.RegistrationDate = FromString(valueNode.InnerText?.Trim());
                        break;

                    case "Jurisdiction:":
                        entity.Jurisdiction = valueNode.InnerText?.Trim();
                        entity.Jurisdiction = DataDecode(entity.Jurisdiction);
                        break;

                    case "Entity Type:":
                        entity.EntityType = valueNode.InnerText?.Trim();
                        entity.EntityType = DataDecode(entity.EntityType);
                        break;

                    case "Status:":
                        entity.Status = valueNode.InnerText?.Trim();
                        entity.Status = DataDecode(entity.Status);
                        break;

                    case "Agent for Service of Process:":
                        entity.AgentForServiceOfProcess = valueNode.SelectSingleNode(".//text()")?.InnerText?.Trim();
                        entity.AgentForServiceOfProcess = DataDecode(entity.AgentForServiceOfProcess);
                        break;

                    case "Entity Mailing Address:":
                        entity.EntityAddress = valueNode.SelectSingleNode(".//label[1]/following-sibling::text()")?.InnerText?.Trim();
                        entity.EntityCityStateZip = valueNode.SelectSingleNode(".//label[2]/following-sibling::text()")?.InnerText?.Trim();
                        entity.EntityCityStateZip = DataDecode(entity.EntityCityStateZip);
                        entity.EntityCityStateZip = AddressStateFix(entity.EntityCityStateZip);
                        break;
                }
            }

            return true;
        }

        public DownloadResult Parse(EntityWebPages entityWebPages)
        {
            if (entityWebPages.DetailedPageRequired)
            {
                if (string.IsNullOrEmpty(entityWebPages.MainPage) || string.IsNullOrEmpty(entityWebPages.DetailedPage))
                    return new DownloadResult(DownloadStatus.EntityIsNotExists);
            }
            else
            {
                if (string.IsNullOrEmpty(entityWebPages.MainPage))
                    return new DownloadResult(DownloadStatus.EntityIsNotExists);
            }
            
            var entity = new EntityInformation();

            try
            {
                if (TryParseMainPage(entityWebPages, ref entity))
                {
                    if (!entityWebPages.DetailedPageRequired || TryParseDetailPage(entityWebPages, ref entity))
                        return new DownloadResult(DownloadStatus.Ok, entity);
                }
            }
            catch (HtmlWebException)
            {
            }

            return new DownloadResult(DownloadStatus.EntityIsNotExists);
        }
    }
}
