using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;
/// <summary>
/// Provides helper methods for retrieving localized strings from resource files.
/// </summary>
/// <remarks>This class is designed to simplify access to localized resources by leveraging a static cache for 
/// <see cref="ResourceManager"/> instances associated with specific types. It supports retrieving  localized strings
/// based on the current UI culture, with a fallback to the key if no matching  resource is found.</remarks>
public static class LocalizerHelper
{

    private static Dictionary<Assembly, ResourceManager> _resourceManagers = new Dictionary<Assembly, ResourceManager>();

    /// <summary>
    /// Retrieves a localized string for the specified key from the resource file associated with the specified type.
    /// </summary>
    /// <remarks>This method uses the current UI culture to determine the appropriate localized string. If no
    /// matching string is found in the resource file, the method returns the key as a fallback.</remarks>
    /// <typeparam name="T">The type used to determine the assembly containing the resource file. Typically, this is a class within the same
    /// assembly as the resource file.</typeparam>
    /// <param name="key">The key identifying the string to retrieve from the resource file.</param>
    /// <returns>The localized string corresponding to the specified key if found; otherwise, the key itself.</returns>
    public static string Localize<T>(string key)
    {

        var assembly = typeof(T).Assembly;
        if (!_resourceManagers.TryGetValue(assembly, out var resourceManager))
        {
            resourceManager = new ResourceManager($"{assembly.GetName().Name}.Resources.Strings", Assembly.GetExecutingAssembly());
            _resourceManagers[assembly] = resourceManager;
        }
        try
        {
            string localized = resourceManager.GetString(key);
            return !string.IsNullOrEmpty(localized) ? localized : key;
        }
        catch (Exception)
        {
            return key;
        }
    }
}
