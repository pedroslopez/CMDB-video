using CMDB.Database;
using CMDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB.Business
{
    public class ConfigurationManagerBusiness
    {

        ConfigurationItemContext configurationDb;
        public ConfigurationManagerBusiness()
        {
            configurationDb = new ConfigurationItemContext();
        }

        public IQueryable<ConfigurationItem> GetConfigurationItems()
        {
            return configurationDb.ConfigurationItems;
        }

        public void AddConfigurationItem(ConfigurationItem ci)
        {
            
            bool nameExists = configurationDb.ConfigurationItems.Any(x => x.Name == ci.Name);

            if (nameExists)
            {
                Console.WriteLine("ERROR: Este Configuration Item ya existe!");
                return;
            }

            configurationDb.ConfigurationItems.Add(ci);
            configurationDb.SaveChanges();
        }

        public bool ValidateDependencyExists(ConfigurationItem ci , int dependencyId)
        {
            bool isExistsDependency = ci.Dependencies.Any(e => e.Id == dependencyId);

            if (isExistsDependency)
            {
                Console.WriteLine("ERROR: La dependencia ya existe!");
                return true;
            }
            return false;
        }

        public bool AddDependency(int ciId, int dependencyId)
        {
            var ci = configurationDb.ConfigurationItems.Find(ciId);
            var dependency = configurationDb.ConfigurationItems.Find(dependencyId);

            ci.Dependencies.Add(dependency);
            configurationDb.SaveChanges();
            return true;
        }

        public List<ConfigurationItem> GetAffectedCIs(ConfigurationItem item)
        {
            List<ConfigurationItem> affectedCIs = new List<ConfigurationItem>();

            foreach(ConfigurationItem ci in configurationDb.ConfigurationItems)
            {
                if(ci.Dependencies.Contains(item))
                {
                    affectedCIs.Add(ci);
                }
            }

            return affectedCIs;
        }


    }
}
