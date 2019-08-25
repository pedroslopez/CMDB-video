using CMDB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB.Database
{
    public class ConfigurationItemContext : DbContext
    {

        public ConfigurationItemContext() : base("MyDbCM")
        {
        }

        public IDbSet<ConfigurationItem> ConfigurationItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConfigurationItem>().HasMany(m => m.Dependencies).WithMany();
        }
    }
}
