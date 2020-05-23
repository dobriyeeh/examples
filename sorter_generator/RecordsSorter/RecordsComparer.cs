using System;
using System.Collections.Generic;
using RecordsCore;

namespace RecordsSorter
{
    public class RecordsComparer : IComparer<Record>
    {
        public int Compare(Record x, Record y)
        {
            int compartion = String.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase);
            if (compartion == 0)
            {
                compartion = (int)((x.Number - y.Number) & -1);
            }

            return compartion;
        }
    }
}
