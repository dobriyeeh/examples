using System.Threading;
using System.Threading.Tasks;

namespace ActualData
{
    public interface IWebPageDownloader
    {
        Task<EntityWebPages> DownloadAsync(long entityNumber, bool downloadFullInformation, CancellationToken? cancelToken = null);
    }
}
