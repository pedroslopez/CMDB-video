using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMDB.Business;
using CMDB.Models;

namespace CMDB
{
    class Program
    {
        static ConfigurationManagerBusiness CMBusiness = new ConfigurationManagerBusiness();

        static void Main(string[] args)
        {
            PrintMenu();
        }

        static void PrintMenu()
        {
            Console.WriteLine("---- CONFIGURATION MANAGEMENT DATABASE ----");
            Console.WriteLine("1. Listar Configuration Items");
            Console.WriteLine("2. Agregar Configuration Item");
            Console.WriteLine("3. Establecer Dependencias");
            Console.WriteLine("4. Reportes de Impacto de Cambio");
            Console.WriteLine("5. Salir");
            Console.WriteLine("Seleccionar opcion: ");

            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    ListItems();
                    break;
                case "2":
                    AddItem();
                    break;
                case "3":
                    AddDependencies();
                    break;
                case "4":
                    PrintReportMenu();
                    break;
                case "5":
                    return;
            }

            PrintMenu();
        }

        static void ListItems()
        {
            List<ConfigurationItem> configItems = CMBusiness.GetConfigurationItems().ToList();
            foreach (ConfigurationItem configItem in configItems)
            {
                Console.WriteLine("---------------------");
                Console.WriteLine(configItem);
            }
        }

        static void AddItem()
        {
            Console.WriteLine("Agregando configuration item...");

            Console.WriteLine("Nombre: ");
            string name = Console.ReadLine();

            Console.WriteLine("Descripcion: ");
            string description = Console.ReadLine();

            Console.WriteLine("Version: ");
            string version = Console.ReadLine();

            ConfigurationItem configItem = new ConfigurationItem()
            {
                Name = name,
                Description = description,
                Version = version
            };

            try
            {
                CMBusiness.AddConfigurationItem(configItem);
            }
            catch (DbEntityValidationException ex)
            {
                Console.WriteLine("La version debe seguir el estandar de SemVer. (MAJOR.MINOR.PATCH)");
            }

        }

        static ConfigurationItem SelectItem(List<ConfigurationItem> configItems)
        {
            string stringId = Console.ReadLine();

            bool parsed = int.TryParse(stringId, out int id);
            if (!parsed)
            {
                Console.WriteLine("ERROR: ID invalido!");

                return null;
            }

            ConfigurationItem affectedCI = configItems.FirstOrDefault(e => e.Id == id);
            if (affectedCI == null)
            {
                Console.WriteLine("ERROR: ID no encontrado!");

                return null;
            }

            return affectedCI;
        }

        static void AddDependencies()
        {
            List<ConfigurationItem> configItems = CMBusiness.GetConfigurationItems().ToList();

            if (configItems.Count() <= 1)
            {
                Console.WriteLine("ERROR: Deben existir mas Configuration Items para agregar dependencia.");
                return;
            }

            Console.WriteLine("Agregar dependencia...");
            printCINames(configItems);

            Console.WriteLine("Seleccione el item de configuracion: ");

            var ci = SelectItem(configItems);

            if (ci == null)
            {
                AddDependencies();
                return;
            }


            ConfigurationItem dependency = null;
            bool dependencyExists = false;

            while (dependency == null || dependencyExists)
            {
                Console.WriteLine("Seleccione la dependencia a agregar: ");

                var listedItems = configItems.Where(e => e.Id != ci.Id).ToList();

                dependency = SelectItem(listedItems);

                if (dependency != null)
                    dependencyExists = CMBusiness.ValidateDependencyExists(ci, dependency.Id);

                if (dependencyExists || dependency == null)
                {
                    Console.WriteLine("1. Continuar \n O cualquier otro caracter para salir: ");
                    var resp = Console.ReadLine();

                    if (resp != "1")
                        return;
                }
            }

            CMBusiness.AddDependency(ci.Id, dependency.Id);

            Console.WriteLine("Dependencia agregada!");
        }

        static void PrintReportMenu()
        {
            Console.WriteLine("-- REPORTES --");
            Console.WriteLine("1. Actualizar");
            Console.WriteLine("2. Deprecar");
            Console.WriteLine("3. Salir");
            Console.WriteLine("Seleccionar opcion: ");

            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    Console.WriteLine("REPORTE DE ACTUALIZACION");
                    ImpactChangeUpgradeReport();
                    break;
                case "2":
                    Console.WriteLine("REPORTE DE DEPRECAR");
                    break;
                case "3":
                    return;
            }

            PrintReportMenu();

        }

        static void ImpactChangeUpgradeReport()
        {
            List<ConfigurationItem> configItems = CMBusiness.GetConfigurationItems().ToList();

            if (configItems.Count() == 0)
            {
                Console.WriteLine("ERROR: No existe ningun configuration item.");
                return;
            }

            printCINames(configItems);

            Console.WriteLine("Seleccione el item de configuracion que desee actualizar: ");
            ConfigurationItem updatedItem = SelectItem(configItems);

            string selected;
            while(true)
            {
                Console.WriteLine("Seleccione el tipo de actualizacion:");
                Console.WriteLine("1. Major");
                Console.WriteLine("2. Minor");
                Console.WriteLine("3. Patch");
                Console.WriteLine("4. Cancelar");

                selected = Console.ReadLine();

                if (selected == "4") return;

                if(selected != "1" && selected != "2" && selected != "3")
                {
                    Console.WriteLine("Opcion invalida!");
                } else
                {
                    break;
                }
            }
      
            if(selected == "2" || selected == "3")
            {
                Console.WriteLine("La actualizacion se puede realizar sin conflictos!");
            } else
            {
                var affectedItems = CMBusiness.GetAffectedCIs(updatedItem);
                Console.WriteLine("Al realizar la actualizacion, los siguientes items de configuracion se verian afectados:");
                foreach(ConfigurationItem item in affectedItems)
                {
                    Console.WriteLine(item.ToString());
                    Console.WriteLine("------");
                }
            }


        }

        static void printCINames(ICollection<ConfigurationItem> configurationItems)
        {
            foreach (ConfigurationItem configurationItem in configurationItems)
            {
                Console.WriteLine($"{configurationItem.Id}. {configurationItem.Name}");
            }
        }

    }
}
