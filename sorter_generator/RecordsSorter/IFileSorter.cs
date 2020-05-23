using System;

namespace RecordsSorter
{
    public interface IFileSorter
    {
        void SortFile(string sourceFilePath, string destinationFilePath);
    }
}