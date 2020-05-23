using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace CMSController
{
    public interface IExportEntities
    {
        Task ToFile(string filePath, IEnumerable<EntityInformation> entities);
    }
}
