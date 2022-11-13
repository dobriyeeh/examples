using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public class DownloadResult
    {
        public DownloadResult(DownloadStatus status, EntityInformation entity = null)
        {
            Status = status;
            Entity = entity;
        }

        public DownloadStatus Status
        {
            get; private set;
        }

        public EntityInformation Entity
        {
            get; private set;
        }
    }
}
