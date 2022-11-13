using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace CMSController
{
    internal class EntitiesContext : DbContext
    {
        public EntitiesContext() : base("name=DbConnectionString")
        {
            Database.SetInitializer<EntitiesContext>(null);

            Database.Initialize(true);
        }

        public DbSet<EntityInformation> Entities { get; set; }

    }
}
