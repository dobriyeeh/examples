using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace CMSController
{
    public class TestUtils
    {
        private readonly Controller _controller;

        internal TestUtils(Controller controller)
        {
            _controller = controller;
        }

        public async Task AddEntity(EntityInformation entity, CancellationToken? cancelToken = null)
        {
            var token = cancelToken ?? default(CancellationToken);
            var context = _controller.EntitiesContext;

            context.Entities.Add(entity);
            await context.SaveChangesAsync(token);
        }

        public void DeleteLastEntities(int lastEntitiesNumber)
        {
            var entities = _controller.EntitiesContext.Entities;

            var lastEntities = entities.OrderBy(ent => ent.EntityNumber).Skip(Math.Max(0, entities.Count() - lastEntitiesNumber));

            entities.RemoveRange(lastEntities);

            _controller.EntitiesContext.SaveChanges();

        }

        public void DeleteDatabase()
        {
            _controller.EntitiesContext.Database.ExecuteSqlCommand("DELETE FROM EntityInformations");
        }

        public async Task<bool> UpdateEntityAsync(long entityNumber, CancellationToken? cancelToken = null)
        {
            return await _controller.UpdateEntityAsync(entityNumber, cancelToken);
        }

        public async Task<long> GetLastKnownEntityNumberAsync(CancellationToken? cancelToken = null)
        {
            return await _controller.GetLastKnownEntityNumberAsync(cancelToken);
        }
    }
}
