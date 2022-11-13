using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActualData;
using CMSController;
using DataModel;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace CMSControllerTest
{
    [TestFixture]
    public class ExportFixture
    {
        private const string TestFilePath = @"C:\test\export\test.txt";

        [SetUp]
        public void SetUp()
        {
            File.Delete(TestFilePath);
        }

        public async Task<IEnumerable<EntityInformation>> DownloadTestData()
        {
            var knowNumbers = new[] { 3978013, 3978014, 3978015 };

            var downloader = new DataDownloader();
            var entities = new List<EntityInformation>();

            foreach (var currNumber in knowNumbers)
            {
                var result = await downloader.DownloadAsync(currNumber, true);
                if (result.Status == DownloadStatus.Ok)
                {
                    if (result.Entity.EntityNumber == 3978014)
                        result.Entity.AgentForServiceOfProcess = null;

                    entities.Add(result.Entity);
                }

            }

            return entities;
        }

        private async Task ExportToFile(string filePath)
        {
            var entities = await DownloadTestData();

            var export = new ExportEntities();
            export.ToFile(filePath, entities);
        }

        [Test]
        public async Task ExportShouldCreateFile()
        {
            await ExportToFile(TestFilePath);

            Assert.IsTrue(File.Exists(TestFilePath));
        }

        public static IEnumerable<Dictionary<string, object>> ReadFromOldCrapFormatAndSourceCode(string fileName)
        {
            // my old crap code to test compability
            var result = new List<Dictionary<string, object>>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string entityName = reader.ReadLine();
                    string items3 = reader.ReadLine();
                    string jurisdiction = reader.ReadLine();
                    string addressLine = reader.ReadLine();
                    if (!addressLine.Equals("Address", StringComparison.OrdinalIgnoreCase))
                        break;

                    string entityAddress = reader.ReadLine();
                    string entityCityStateZip = reader.ReadLine();
                    string agentVariant = reader.ReadLine();
                    string agent = "";
                    string ckeckEnd = "error";

                    if (!string.IsNullOrEmpty(agentVariant))
                    {
                        if (
                            !agentVariant.Trim()
                                .Equals("Agent for Service of Process", StringComparison.OrdinalIgnoreCase))
                            break;

                        agent = reader.ReadLine();
                        ckeckEnd = reader.ReadLine();
                        if (!string.IsNullOrEmpty(ckeckEnd))
                            break;
                    }

                    string corporate_name = entityName;
                    items3 = items3.ToLower();
                    string[] items3sep = items3.Split(new string[] {"number: ", " date filed: ", " status: "},
                        StringSplitOptions.RemoveEmptyEntries);
                    if (items3sep.Length != 3)
                        continue;

                    string corporate_id = items3sep[0];
                    string date_field = items3sep[1];
                    string[] dateParts = date_field.Split(new char[] {'/'});
                    if (dateParts.Length != 3)
                        continue;

                    DateTime date = DateTime.MinValue;
                    try
                    {
                        int month = Convert.ToInt32(dateParts[0]);
                        int day = Convert.ToInt32(dateParts[1]);
                        int year = Convert.ToInt32(dateParts[2]);
                        date = new DateTime(year, month, day);
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }

                    jurisdiction = jurisdiction.Replace("Jurisdiction: ", "");
                    jurisdiction = jurisdiction.Replace("jurisdiction: ", "");
                    string corp_address = entityAddress;

                    entityCityStateZip = entityCityStateZip.Trim();

                    string[] place = entityCityStateZip.Split(new string[] {", "}, StringSplitOptions.RemoveEmptyEntries);
                    if (place.Length != 2)
                    {
                        if (place.Length == 1)
                        {
                            place = entityCityStateZip.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                            if (place.Length >= 3)
                            {
                                string first = place[0];
                                string second = "";
                                for (int i = 1; i < place.Length - 2; i++)
                                    first = first + " " + place[i];

                                second = place[place.Length - 2] + " " + place[place.Length - 1];

                                place[0] = first;
                                place[1] = second;
                            }
                            else
                                continue;
                        }
                        else
                            continue;
                    }

                    string city = place[0];
                    string state_zip = place[1];
                    string[] last_places = state_zip.Split(new char[] {' '});
                    string state = "";
                    string zip = "";

                    if (last_places.Length >= 1)
                        state = last_places[0];

                    if (last_places.Length >= 2)
                        zip = last_places[1];

                    DateTime imported = DateTime.Now;

                    if ((corporate_name != null) && (corporate_name.Length > 64))
                        corporate_name = corporate_name.Substring(0, 63);

                    if ((corp_address != null) && (corp_address.Length > 64))
                        corp_address = corp_address.Substring(0, 63);

                    if ((city != null) && (city.Length > 32))
                        city = city.Substring(0, 31);

                    if ((corporate_id != null) && (corporate_id.Length > 16))
                        corporate_id = corporate_id.Substring(0, 15);

                    if ((zip != null) && (zip.Length > 16))
                        zip = zip.Substring(0, 15);

                    if ((state != null) && (state.Length > 32))
                        state = state.Substring(0, 31);

                    var resultItem = new Dictionary<string, object>
                    {
                        ["Zip"] = zip,
                        ["Date_field"] = date,
                        ["Corporate_id"] = corporate_id,
                        ["Corporate_name"] = corporate_name,
                        ["Corp_address"] = corp_address,
                        ["City"] = city,
                        ["State"] = state,
                        ["Date_imported"] = imported
                    };

                    result.Add(resultItem);
                }
            }

            return result;
        }

        [Test]
        public async Task ExportShouldBeAbleToReadCrappyFormat()
        {
            await ExportToFile(TestFilePath);

            var entities = ReadFromOldCrapFormatAndSourceCode(TestFilePath);
            Assert.AreEqual(3, entities.Count());
        }
    }
}
