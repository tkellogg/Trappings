using System;
using System.Linq;

namespace Trappings
{
    class SetupClassScanner
    {
        public void ScanForSetupTypes()
        {
            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
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
    }
}
