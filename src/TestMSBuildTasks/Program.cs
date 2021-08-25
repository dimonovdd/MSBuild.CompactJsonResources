using System;
using System.Linq;

namespace TestMSBuildTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            var resources = EmbeddedResourceProvider.GetAllJsonResources().ToArray();
     
            foreach(var item in resources)
            {
                Console.WriteLine(item.name);
                Console.WriteLine(item.data);
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
