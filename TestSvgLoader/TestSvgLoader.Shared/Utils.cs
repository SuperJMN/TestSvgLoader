using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TestSvgLoader.Shared
{
    public class ResourceUtils
    {
        public static Stream GetStreamFromResourceFile(string resourceName, Type typeCalling)
        {
            try
            {
                Assembly assy = typeCalling?.Assembly;
                string[] resources = assy?.GetManifestResourceNames();
                foreach (string sResourceName in resources)
                {
                    Console.WriteLine(sResourceName);
                    if (sResourceName.ToUpperInvariant().Contains(resourceName.ToUpperInvariant()))
                    {
                        // resource found
                        return assy.GetManifestResourceStream(sResourceName);
                    }
                }
            }
            catch
            {
                //
            }
            throw new Exception("Unable to find resource file:" + resourceName);
        }

    }
}
