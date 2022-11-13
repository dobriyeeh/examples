using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordsCore;

namespace RecordsSorter
{
    /// <summary>
    /// Special kind of comparer to allow using duplicates in some collection like SortedHash
    /// </summary>
    internal class RecordsComparerWithoutDuplicates : IComparer<Record>
    {
        public int Compare(Record x, Record y)
        {
            int compartion = String.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase);
            if (compartion == 0)
            {
                compartion = x.Number < y.Number ? -1 : 1;
            }

            return compartion;
        }
    }
}
