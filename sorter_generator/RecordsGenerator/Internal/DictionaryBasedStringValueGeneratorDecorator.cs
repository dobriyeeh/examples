using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordsGenerator.Internal
{
    class DictionaryBasedStringValueGeneratorDecorator : IValueGenerator<string>
    {
        private const int StringLength = 16;

        public string Next()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
