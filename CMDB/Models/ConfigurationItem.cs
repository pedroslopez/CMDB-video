using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB.Models
{
    public class ConfigurationItem 
    {
        public ConfigurationItem()
        {
            Dependencies = new HashSet<ConfigurationItem>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
      
        [VersionValidationAttribute]
        public string Version { get; set; }

        public virtual ICollection<ConfigurationItem> Dependencies { get; set; }

        public override string ToString()
        {
            var dependencies = string.Join(", ", Dependencies.Select(e => e.Name));

            return
                $"Id: {Id}\n" +
                $"Nombre: {Name}\n" +
                $"Description: {Description}\n" +
                $"Version: {Version}\n" +
                $"Dependencias: {dependencies}";
        }
    }
}
