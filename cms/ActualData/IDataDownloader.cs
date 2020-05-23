using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public interface IDataDownloader
    {
        Task<DownloadResult> DownloadAsync(long entityNumber, bool downloadFullInformation, CancellationToken? cancelToken = null);
    }
}
