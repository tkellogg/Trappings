using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trappings
{
    class SetupClassScanner
    {
        public void ScanForSetupTypes()
        {
            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in GetTypes(assembly)
                        where type.IsClass && !type.IsAbstract && typeof (IGlobalSetup).IsAssignableFrom(type)
                        select type;

            foreach (var type in types)
            {
                try
                {
                    var @object = (IGlobalSetup)Activator.CreateInstance(type);
                    @object.SetupOnce();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (TypeLoadException)
            {
                return new Type[0];
            }
        }
    }
}
