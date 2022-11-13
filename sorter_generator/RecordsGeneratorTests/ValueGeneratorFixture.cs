using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RecordsGenerator;
using RecordsGenerator.Internal;

namespace RecordsGeneratorTests
{
    [TestFixture]
    public class ValueGeneratorFixture
    {
        [Test]
        public void NumberValueGeneratorShouldMakeOrderedSequence()
        {
            var numberValueGenerator = new NumberValueGeneratorDecorator();
            var numbers = new List<long>();

            for (int i = 0; i < 5; i++)
            {
                numbers.Add(numberValueGenerator.Next());
            }

            Assert.That(numbers, Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
        }
    }
}
