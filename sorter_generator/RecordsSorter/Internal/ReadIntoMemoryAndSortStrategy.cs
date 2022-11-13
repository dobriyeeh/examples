using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordsCore;

namespace RecordsSorter.Internal
{
    internal class ReadIntoMemoryAndSortStrategy : IFileSorter
    {
        public void SortFile(string sourceFilePath, string destinationFilePath)
        {
            var records = new List<Record>();

            using (var recordsFileSource = new RecordsFileSource(sourceFilePath, new RecordConverter()))
            {
                records.AddRange(recordsFileSource.GetRecords());    
            }

            records.Sort(new RecordsComparer());

            using (var output = new RecordsFileOutput(destinationFilePath, new RecordConverter()))
            {
                output.Write(records);
            }
        }
    }
}
