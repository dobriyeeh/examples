using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Transactions;
using ActualData;
using DataModel;
using CMSController;

namespace CMSControllerTest
{
    [TestFixture]
    public class CMSControllerFixture
    {
        private Controller _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new Controller();
            _controller.TestUtils.DeleteDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _controller.TestUtils.DeleteDatabase();
            _controller.Dispose();
        }

        private async Task PrepareDb()
        {
            var entity = new EntityInformation
            {
                EntityName = "Aqwa barwa",
                EntityNumber = 3978013,
                Jurisdiction = "J",
                EntityAddress = "E",
                EntityCityStateZip = "E",
                AgentForServiceOfProcess = "A",
                Status = "Active",
                EntityType = "E",
                LastUpdate = DateTime.Now - TimeSpan.FromDays(1),
                RegistrationDate = DateTime.Now - TimeSpan.FromDays(1),
            };
            await _controller.TestUtils.AddEntity(entity);

            var entity2 = new EntityInformation
            {
                EntityName = "1 aqwa1",
                EntityNumber = 3978015,
                Jurisdiction = "J",
                EntityAddress = "E",
                EntityCityStateZip = "E",
                AgentForServiceOfProcess = "A",
                Status = "Active",
                EntityType = "E",
                LastUpdate = DateTime.Now - TimeSpan.FromDays(1),
                RegistrationDate = DateTime.Now - TimeSpan.FromDays(1),
            };
            await _controller.TestUtils.AddEntity(entity2);

            var entity3 = new EntityInformation
            {
                EntityName = "zhava aa1",
                EntityNumber = 3978018,
                Jurisdiction = "J",
                EntityAddress = "E",
                EntityCityStateZip = "E",
                AgentForServiceOfProcess = "A",
                Status = "Active",
                EntityType = "E",
                LastUpdate = DateTime.Now - TimeSpan.FromDays(1),
                RegistrationDate = DateTime.Now - TimeSpan.FromDays(1),
            };
            await _controller.TestUtils.AddEntity(entity3);
        }

        [Test]
        public async Task TestControllerDatabase()
        {
            var entity = new EntityInformation
            {
                EntityName 					= "EntityName  				",
                EntityNumber 				= new Random().Next(),
                Jurisdiction 				= "Jurisdiction 			",
                EntityAddress 				= "EntityAddress 			",
                EntityCityStateZip 			= "EntityCityStateZip 		",
                AgentForServiceOfProcess 	= "AgentForServiceOfProcess ",
                Status 						= "DataStatus 					",
                EntityType 					= "EntityType 				",
                LastUpdate 					= DateTime.Now,
                RegistrationDate 			= DateTime.Now
            };

            await _controller.TestUtils.AddEntity(entity);

            var list = await _controller.GetListOfAllEntitiesAsync();
            Assert.NotZero(list.Count);
        }

        [Test]
        public async Task ControllerShouldUpdateEntityInformation()
        {
            const long entityNumber = 3978013;

            var entity = new EntityInformation
            {
                EntityName               = "EntityName",
                EntityNumber             = entityNumber,
                Jurisdiction             = "J",
                EntityAddress            = "E",
                EntityCityStateZip       = "E",
                AgentForServiceOfProcess = "A",
                Status                   = "S",
                EntityType               = "E",
                LastUpdate               = DateTime.Now - TimeSpan.FromDays(1),
                RegistrationDate         = DateTime.Now - TimeSpan.FromDays(1),
            };

            await _controller.TestUtils.AddEntity(entity);

            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(entityNumber));

            var updatedEntity = await _controller.GetEntityInformation(entityNumber);
            Assert.NotNull(updatedEntity);

            Assert.AreEqual(entity.EntityNumber, updatedEntity.EntityNumber);
            Assert.AreNotEqual(entity.EntityName, "EntityName");
            Assert.AreNotEqual(entity.Jurisdiction, "J");
            Assert.AreNotEqual(entity.Status, "S");
        }

        [Test]
        public async Task ControllerShouldSelectInformation()
        {
            await PrepareDb();

            var items = await _controller.QueryEntities("aqw", false, null);

            Assert.IsTrue(items.Count == 2);
            Assert.IsTrue(items[0].EntityNumber == 3978013);
            Assert.IsTrue(items[1].EntityNumber == 3978015);
        }

        [Test]
        public async Task TestControllerSync()
        {
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978013));

            var list = await _controller.GetListOfAllEntitiesAsync();
            Assert.NotZero(list.Count);
        }

        [Test]
        public async Task ControllerShouldFindLastEntity()
        {
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978013));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978012));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978014));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978011));

            long lastEntity = await _controller.TestUtils.GetLastKnownEntityNumberAsync();
            Assert.AreEqual(lastEntity, 3978014);
        }

        [Test]
        public async Task ControllerShouldReturnEntitiesCountCorrectly()
        {
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978013));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978012));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(3978014));

            Assert.AreEqual(3, _controller.EntitiesInDatabase);
        }

        [Test]
        public async Task ControllerShouldDetectsIsNewEntitiesAvailable()
        {
            var range = new EntityInfoRange();
            long lastNumber = await range.FindLastEntityNumberAsync();

            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber - 2));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber - 1));

            Assert.IsTrue(await _controller.SyncStatusAsync());
            Assert.IsTrue(_controller.DataStatus == DataStatus.NeedToUpdate);
        }

        [Test]
        public async Task ControllerShouldSyncEntities()
        {
            var range = new EntityInfoRange();
            long lastNumber = await range.FindLastEntityNumberAsync();

            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber - 3));
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber - 2));
            
            Assert.IsTrue(await _controller.SyncDataAsync());
            Assert.IsTrue(_controller.DataStatus == DataStatus.Synced);

            var entities = await _controller.GetListOfAllEntitiesAsync();
            int lastIndex = entities.Count - 1;

            Assert.AreEqual(lastNumber - 3, entities[lastIndex -3].EntityNumber);
            Assert.AreEqual(lastNumber - 2, entities[lastIndex - 2].EntityNumber);
            Assert.AreEqual(lastNumber - 1, entities[lastIndex - 1].EntityNumber);
            Assert.AreEqual(lastNumber, entities[lastIndex].EntityNumber);

            Assert.IsTrue(await _controller.SyncStatusAsync());
            Assert.IsTrue(_controller.DataStatus == DataStatus.Synced);
        }

        [Test]
        public async Task ControllerShouldReportProgress()
        {
            var range = new EntityInfoRange();
            long lastNumber = await range.FindLastEntityNumberAsync();

            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber - 2));

            var progressValues = new List<double>();
            var progressHandler = new Progress<double>(progress => progressValues.Add(progress));

            Assert.IsTrue(await _controller.SyncDataAsync(progressHandler));

            await Task.Delay(512);

            Assert.AreEqual(5, progressValues.Count);

            var isOrdered = progressValues.
                Zip(progressValues.Skip(1), (a, b) => new { a, b })
                .All(p => p.a < p.b);

            Assert.IsTrue(isOrdered);
        }

        [Test]
        public async Task ControllerShouldBeAbleToNotifyState()
        {
            var range = new EntityInfoRange();
            long lastNumber = await range.FindLastEntityNumberAsync();

            var statuses = new List<DataStatus>();
            _controller.DataStatusChanged += status => statuses.Add(status);

            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber - 1));

            Assert.IsTrue(await _controller.SyncStatusAsync());

            Assert.IsTrue(await _controller.SyncDataAsync());

            var expected = new[]
            {
                DataStatus.StatusUpdating,
                DataStatus.NeedToUpdate,
                DataStatus.DataUpdating,
                DataStatus.Synced
            };

            Assert.AreEqual(expected.Length, statuses.Count);
            Assert.IsTrue(Enumerable.SequenceEqual<DataStatus>(expected, statuses));
        }

        [Test]
        public async Task ControllerShouldCorrectUpdateIfNoNew()
        {
            var range = new EntityInfoRange();
            long lastNumber = await range.FindLastEntityNumberAsync();
            Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(lastNumber));

            Assert.IsTrue(await _controller.SyncDataAsync());
            Assert.IsTrue(_controller.DataStatus == DataStatus.Synced);
        }

        [Test]
        public async Task Cabability()
        {
            var listNumbers = new[]
            {
                3936693,
                3937210,
                3938011,
                3938419,
                3925173,
                3940508,
                3929461,
                3923490,
                3940780,
                3926461,
                3935643,
                3941125,
                3929729,
                3932196,
                3936494,
                3932594,
                3926218,
                3924405,
                3929898,
                3939126,
                3936875,
                3925069,
                3939710,
                3930414,
                3938351,
                3929610,
                3929916,
                3923927,
                3935213,
                3929655,
                3936495,
                3937347,
                3925768,
                3933827,
                3929405,
                3939005,
                3932277,
                3934880,
                3931800,
                3933723,
                3930134,
                3927095,
                3926316,
                3941336,
                3932344,
                3926360,
                3924929,
                3927140,
                3930486,
                3934583,
                3929597,
                3925941,
                3674245,
                3931481,
                3931843,
                3938766,
                3940994,
                3925074,
                3929109,
                3928389,
                3934487,
                3932754,
                3941607,
                3925261,
                3937851,
                3940469,
                3934602,
                3926879,
                3925632,
                3934384,
                3937790,
                3927280,
                3924970,
                3933831,
                3927341,
                3926220,
                3923711,
                3933567,
                3934439,
                3925421,
                3940905,
                3940822,
                3930262,
                3929165,
                3927891,
                3936436,
                3938787,
                3931511,
                3934768,
                3939224,
                3938727,
                3924267,
                3929850,
                3938214,
                3938792,
                3940219,
                3934770,
                3933431,
                3927954,
                3924608
            };

            var pathNew = @"C:\project\CMS2\old_data\Andy-2_new.txt";
            if (File.Exists(pathNew))
                File.Delete(pathNew);

            var list = new List<EntityInformation>();

            foreach (var currNumber in listNumbers)
            {
                Assert.IsTrue(await _controller.TestUtils.UpdateEntityAsync(currNumber));
                var updatedEntity = await _controller.GetEntityInformation(currNumber);
                Assert.IsNotNull(updatedEntity);

                list.Add(updatedEntity);
            }

            var exportFile = new ExportEntities();
            await exportFile.ToFile(pathNew, list);
        }
    }
}
