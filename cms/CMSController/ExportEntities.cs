using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace CMSController
{
    public class ExportEntities : IExportEntities
    {
        public async Task ToFile(string filePath, IEnumerable<EntityInformation> entities)
        {
            // this crappy output format is designed for compatibility with the previous application
            using (var writer = new StreamWriter(filePath, true))
            {
                foreach (var currEntity in entities)
                {
                    string status = currEntity.Status?.ToLower() ?? "active";

                    await writer.WriteLineAsync(currEntity.EntityName);
                    await writer.WriteLineAsync($"number: C{currEntity.EntityNumber}  Date Filed: {DateToString(currEntity.RegistrationDate)}  Status: {status} ");
                    await writer.WriteLineAsync($"Jurisdiction: {currEntity.Jurisdiction}");
                    await writer.WriteLineAsync("Address");
                    await writer.WriteLineAsync(currEntity.EntityAddress);
                    await writer.WriteLineAsync(currEntity.EntityCityStateZip);

                    if (!string.IsNullOrEmpty(currEntity.AgentForServiceOfProcess))
                    {
                        await writer.WriteLineAsync("Agent for Service of Process ");
                        await writer.WriteLineAsync(currEntity.AgentForServiceOfProcess);
                    }
                    await writer.WriteLineAsync();
                }
            }
        }

        private static string DateToString(DateTime dateTime)
        {
            return $"{dateTime.Month}/{dateTime.Day}/{dateTime.Year}";
        }
    }
}
