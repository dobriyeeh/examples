using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActualData
{
    public interface IWebPagesEntityInfoParser
    {
        DownloadResult Parse(EntityWebPages entityWebPages);
    }
}
