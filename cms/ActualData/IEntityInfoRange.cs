using System;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{

    public interface IEntityInfoRange
    {
        Task<long> FindLastEntityNumberAsync(long startSearchFrom, CancellationToken? cancellationToken = null);
        Task<long> FindFirstEntityNumberByYearAsync(long year, CancellationToken? cancellationToken = null);
    }
}