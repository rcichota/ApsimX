using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using APSIM.Documentation.Models;
using APSIM.Shared.Utilities;
using Models.Core;

namespace AMEI;

/// <summary>
///
/// </summary>
public class Crop2ML
{
    /// <summary>
    /// Write an xml file.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sourcFileName"></param>
    /// <param name="fileName"></param>
    public static void WriteXmlFile(Type type, string sourcFileName, string fileName)
    {
        var modelUnit = GetModelDescription(type, sourcFileName);
        var serializer = new XmlSerializer(typeof(ModelUnit));

        XmlWriterSettings settings = new();
        settings.Indent = true;

        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        using StreamWriter writer = new(fileName);
        using XmlWriter xmlWriter = XmlWriter.Create(writer, settings);

        serializer.Serialize(xmlWriter, modelUnit, ns);
    }

    /// <summary>Get a model description.</summary>
    /// <param name="type">The type to document.</param>
    public static ModelUnit GetModelDescription(Type type, string sourcFileName)
    {
        List<(string oldName, string newName)> replacements = new();  // map of oldname, newname
        ModelUnit modelUnit = new();
        modelUnit.Inputs = new();
        modelUnit.Inputs.Input = new();
        modelUnit.Outputs = new();
        modelUnit.Outputs.Output = new();

        string sourceCode = File.ReadAllText(sourcFileName);

        // Exogenous
        modelUnit.Inputs.Input.AddRange(
            GetExogenous(type, sourceCode).Select(m => new Input()
            {
                Name = m.Name,
                Description = m.Description,
                Inputtype = "variable",
                Variablecategory = "exogenous",
                Datatype = GetDataTypeString(m.DataType),
                Unit = m.Unit
            }));

        // Parameters
        modelUnit.Inputs.Input.AddRange(
            GetParameters(type).Select(m => new Input()
            {
                Name = m.Name,
                Inputtype = "parameter",
                Parametercategory = "constant",
                Description = m.Description,
                Datatype = GetDataTypeString(m.DataType),
                Unit = m.Unit
            }));

        // Input states
        modelUnit.Inputs.Input.AddRange(
            GetInputStates(type).Select(m => new Input()
            {
                Name = m.Name,
                Inputtype = "variable",
                Variablecategory = "state",
                Description = m.Description,
                Datatype = GetDataTypeString(m.DataType),
                Unit = ""
            }));

        // Output states
        modelUnit.Outputs.Output.AddRange(
            GetOutputStates(type).Select(m => new Output()
                {
                    Name = m.Name,
                    Variablecategory = "state",
                    Description = m.Description,
                    Datatype = GetDataTypeString(m.DataType),
                    Unit = m.Unit
                }));

        // Functions
        modelUnit.Function.AddRange(
            GetMethods(type).Select(m => new Function()
                {
                    Name = m.Name,
                    Type = "external",
                    Description = m.Description,
                    Filename = $"algo/pyx/{m.Name}.pyx",
                    Language = "cyml"
                }));

        modelUnit.Modelid = $"AP_{type.Name}";
        modelUnit.Name = type.Name;
        modelUnit.Timestep = "1";
        modelUnit.Version = "1.0";

        modelUnit.Description = new()
        {
            Title = type.Name,
            Authors = "APSIM Initiative",
            Institution = "APSIM Initiative",
            URI = "www.apsim.info",
            ShortDescription = AutoDocumentation.GetSummary(type)
        };

        modelUnit.Initialization = new()
        {
            Name = $"init_{type.Name}",
            Language = "cyml",
            Filename = $"algo/pyx/init.{type.Name}.pyx"
        };

        modelUnit.Algorithm = new()
        {
            Language = "cyml",
            Filename = $"algo/pyx/init.{type.Name}.pyx"
        };

        //string modifiedSourceCode = ConverSourceCodeToCrop2MLFriendly(replacements, modelUnit, sourceCode);

        //File.WriteAllText(Path.ChangeExtension(sourcFileName, ".modified.cs"), modifiedSourceCode);
        return modelUnit;
    }


    private static IEnumerable<MemberDetails> GetExogenous(Type t, string sourceCode)
    {
        // Iterate though all [Link] fields.
        foreach (FieldInfo linkField in t.GetFields(BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance |
                                                    BindingFlags.FlattenHierarchy)
                                            .Where(f => f.GetCustomAttribute<LinkAttribute>() != null))
        {
            string instanceName = linkField.Name;

            // Now iterate through all instances where this instance is used in the source code.
            foreach (string usedPropertyName in EnumerateFieldUsage(linkField.Name, sourceCode))
            {
                // Find the declaration of the property that was used.
                PropertyInfo property = linkField.FieldType.GetProperty(usedPropertyName, BindingFlags.Public |
                                                                                          BindingFlags.NonPublic |
                                                                                          BindingFlags.Instance |
                                                                                          BindingFlags.FlattenHierarchy);
                if (property != null)
                {
                    VariableProperty variableProperty = new(null, property);

                    // Add an exogenous variable to model unit.
                    string description = variableProperty.Description ?? usedPropertyName;
                    yield return new()
                    {
                        Name = $"{linkField.FieldType.Name}.{usedPropertyName}",
                        Description = GetDescription(property),
                        DataType = property.PropertyType,
                        Unit = variableProperty.Units
                    };
                }
            }
        }
    }

    /// <summary>
    /// Get a collection of model parameters.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private static IEnumerable<MemberDetails> GetParameters(Type t)
    {
        return t.GetProperties(BindingFlags.Public |
                               BindingFlags.Instance |
                               BindingFlags.DeclaredOnly)
                .Where(p => p.CanWrite)
                .Select(p => new MemberDetails()
                {
                    Name = p.Name,
                    Description = GetDescription(p),
                    DataType = p.PropertyType,
                    Unit = GetUnits(p)
                });
    }

    /// <summary>
    /// Get a collection of model input state variables.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private static IEnumerable<MemberDetails> GetInputStates(Type t)
    {
        return t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.Name.Contains("__BackingField") &&           // don't return backing fields
                            p.FieldType.Name != "EventHandler" &&           // don't return events
                            p.GetCustomAttribute<LinkAttribute>() == null)  // don't return [Link]s
                .Select(p => new MemberDetails()
                {
                    Name = p.Name,
                    Description = GetDescription(p),
                    DataType = p.FieldType,
                    Unit = null
                });
    }

    /// <summary>
    /// Get a collection of model output state variables.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private static IEnumerable<MemberDetails> GetOutputStates(Type t)
    {
        return GetInputStates(t).Concat(t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                         .Where(p => !p.CanWrite)
                                         .Select(p => new MemberDetails()
                                         {
                                             Name = p.Name,
                                             Description = GetDescription(p),
                                             DataType = p.PropertyType,
                                             Unit = GetUnits(p)
                                         }));
    }

    /// <summary>
    /// Get a collection of model methods.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private static IEnumerable<MemberDetails> GetMethods(Type t)
    {
        return t.GetMethods(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                .Where(m => !m.Name.StartsWith("get_") &&    // don't return property getter methods
                            !m.Name.StartsWith("set_") &&    // don't return property setter methods
                            !m.Name.StartsWith("add_") &&    // don't return property eventhandler add methods
                            !m.Name.StartsWith("remove_"))   // don't return property eventhandler remove methods
                .Select(m => new MemberDetails()
                {
                    Name = m.Name,
                    Description = GetDescription(m)
                });
    }

    /// <summary>
    /// Get units for a property.
    /// </summary>
    /// <param name="property">The property.</param>
    private static string GetUnits(PropertyInfo property)
    {
        VariableProperty variableProperty = new(null, property);
        return variableProperty.Units;
    }

    /// <summary>
    /// Get Crop2ML datatype string from a datatype
    /// </summary>
    /// <param name="property">The datatype.</param>
    private static string GetDataTypeString(Type t)
    {
        return t.Name.ToUpper().Replace("[]", "ARRAY");
    }

    /// <summary>
    /// Get Crop2ML datatype string from a datatype
    /// </summary>
    /// <param name="property">The datatype.</param>
    private static string GetDescription(MemberInfo m)
    {
        return AutoDocumentation.GetSummary(m);
    }

    /// <summary>
    /// A private class to store details about a reflected method, field, property
    /// </summary>
    private class MemberDetails
    {
        public string Name { get; set; }
        public string Description {get; set; }
        public Type DataType { get; set; }
        public string Unit { get; set; }
    }

    /// <summary>
    /// Clean up source code to make it more friendly to Crop2ML i.e. replace references to linked properties with class members.
    /// </summary>
    /// <remarks>
    /// e.g. replace weather.MaxT with MaxT - a private class variable.
    /// </remarks>
    /// <param name="replacements"></param>
    /// <param name="modelUnit"></param>
    /// <param name="sourceCode"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static string ConverSourceCodeToCrop2MLFriendly(List<(string oldName, string newName)> replacements, ModelUnit modelUnit, string sourceCode)
    {
        // Remove old variable names.
        foreach (var replacement in replacements)
            sourceCode = sourceCode.Replace(replacement.oldName, replacement.newName);

        // Create an input declaration section that will be inserted into source code.
        StringBuilder builder = new();
        foreach (var input in modelUnit.Inputs.Input)
        {
            if (input.Inputtype == "variable")
            {
                string dataType = input.Datatype;
                if (dataType == "DATETIME")
                    dataType = "DateTime";
                else
                    dataType = dataType.ToLower();
                builder.AppendLine($"private {dataType} i{input.Name};");
            }
        }

        // Add new inputs to code at top of class.
        var match = Regex.Match(sourceCode, @"public class .+\n\s*({)");
        if (!match.Success)
            throw new Exception("Cannot find start of class in sourcecode");
        int posOpenBrace = match.Groups[1].Index;

        // Determine the indent number of characters.
        int indent = posOpenBrace - sourceCode.LastIndexOf('\n', posOpenBrace) + 4;
        string textToInsert = StringUtilities.IndentText(builder.ToString(), indent);
        textToInsert += Environment.NewLine;

        // Insert input declarations into source code
        sourceCode = sourceCode.Insert(posOpenBrace + 3, textToInsert);
        return sourceCode;
    }

    /// <summary>
    /// Enumerate all usages of a object instance.
    /// </summary>
    /// <param name="name">The instance name of the object.</param>
    /// <param name="sourceCode">The source code to search.</param>
    /// <returns>A collection of field names of the instance that were referenced.</returns>
    private static IEnumerable<string> EnumerateFieldUsage(string name, string sourceCode)
    {
        return Regex.Matches(sourceCode, @$"{name}\.(\w+)")
                    .Where(m => !IsCommentedOut(m, sourceCode))
                    .Select(m => m.Groups[1].ToString())
                    .Distinct();
    }

    /// <summary>
    /// Determine if a match is commented out.
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private static bool IsCommentedOut(Match m, string sourceCode)
    {
        // Scan backwards for a '//' or '\n'. If // is found first then the match is commented out.
        int i = Math.Max(sourceCode.LastIndexOf("//", m.Index),
                         sourceCode.LastIndexOf("\n", m.Index));
        return i >= 0 && sourceCode.Substring(i, 2) == "//";
    }
}