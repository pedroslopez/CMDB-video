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
                    break;
                case "2":
                    Console.WriteLine("REPORTE DE DEPRECAR");
                    break;
                case "3":
                    return;
            }

            PrintReportMenu();

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
