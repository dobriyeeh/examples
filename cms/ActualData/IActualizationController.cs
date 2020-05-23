using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public interface IActualizationController
    {
        Task<bool> UpdateEntityStatusAsync(IEnumerable<EntityInformation> entities, CancellationToken? cancellationToken = null);

        // return true if all entityes are downloaded
        Task<bool> DownloadNewEntitiesAsync(
            int lastKnownEntityNumber, 
            Action<EntityInformation> newEntityHandler, 
            IProgress<double> progress, 
            CancellationToken? cancellationToken = null);
    }
}
