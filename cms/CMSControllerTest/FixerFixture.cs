using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSController;
using DataModel;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace CMSControllerTest
{
    [TestFixture]
    public class FixerFixture
    {
        [Test]
        public void TestFixDuplicatedSpace()
        {
            var entity = new EntityInformation
            {
                EntityName = "zhava   aa  bb c",
                EntityAddress = "Addr     1      2 3",
                EntityCityStateZip = "EZip     a   b     c    d",
                AgentForServiceOfProcess = "Age    6 3    7",
            };

            var fixer = EntityInformationFixerFactory.Create(FixedIssues.DuplicatedSpace);

            var fixedEntity = fixer.Fix(entity);
            Assert.IsTrue(fixedEntity.EntityName == "zhava aa bb c");
            Assert.IsTrue(fixedEntity.EntityAddress == "Addr 1 2 3");
            Assert.IsTrue(fixedEntity.EntityCityStateZip == "EZip a b c d");
            Assert.IsTrue(fixedEntity.AgentForServiceOfProcess == "Age 6 3 7");
        }

        [Test]
        public void TestFixCommaAfterCities()
        {
            var entity = new EntityInformation
            {
                EntityAddress = "2390 HUNTINGTON CA DR",
                EntityCityStateZip = "SAN MARINO CA 91108",
            };

            var fixer = EntityInformationFixerFactory.Create(FixedIssues.CommaAfterCities);

            var fixedEntity = fixer.Fix(entity);
            Assert.IsTrue(fixedEntity.EntityAddress == "2390 HUNTINGTON CA DR");
            Assert.IsTrue(fixedEntity.EntityCityStateZip == "SAN MARINO, CA 91108");
        }
    }
}