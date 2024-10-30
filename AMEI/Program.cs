using System.Reflection;

namespace AMEI;

public class Program
{
    /// <summary>
    /// Generate a Crop2ML .xml file for a model.
    /// </summary>
    /// <param name="args"> Command line arguments</param>
    /// <returns> Program exit code (0 for success)</returns>
    public static int Main(string[] args)
    {
        try
        {
            // Get command line arguments
            if (args.Length != 2)
                throw new Exception("Usage: PreProcessForCrop2ML TypeName SourceCodePath");
            string typeName = args[0];
            string sourceCodePath = args[1];

            // Convert the type string passed in to a .NET Type.
            Type t = null;
            foreach (AssemblyName reference in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                Assembly assembly = Assembly.Load(reference);

                t = assembly.GetType(typeName);
                if (t != null)
                    break;
            }
            if (t == null)
                throw new Exception($"Cannot find type {typeName}");

            // Determine the name of the output xml file.
            string outputFileName = Path.ChangeExtension(sourceCodePath, ".xml");

            // Generate the xml file.
            Crop2ML.WriteXmlFile(t, sourceCodePath, outputFileName);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

}
