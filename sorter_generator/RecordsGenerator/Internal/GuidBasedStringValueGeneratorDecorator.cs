using System;

namespace RecordsGenerator.Internal
{
    internal class GuidBasedStringValueGeneratorDecorator : IValueGenerator<string>
    {
        private const int StringLength = 16;

        public string Next()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
