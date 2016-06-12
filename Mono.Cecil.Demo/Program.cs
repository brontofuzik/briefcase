using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mono.Cecil.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get assembly
            Assembly assembly = Assembly.GetAssembly(typeof(Foo));
            string assemblyPath = assembly.Location;
            AssemblyDefinition assemblyDef = AssemblyDefinition.ReadAssembly(assemblyPath);
            PrintTypes(assemblyDef);

            // Get type
            TypeDefinition typeDef = assemblyDef.MainModule.GetType("Mono.Cecil.Demo.Foo");

            AssemblyDefinition newAssemblyDef = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Foo", new Version("1.0.0.0")), "Module", ModuleKind.Dll);
            newAssemblyDef.MainModule.Types.Add(typeDef);
            PrintTypes(newAssemblyDef);
        }

        static void PrintTypes(AssemblyDefinition assembly)
        {
            Console.WriteLine(assembly.Name);
            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                Console.WriteLine(type.Name);
            }
            Console.WriteLine();
        }
    }
}
