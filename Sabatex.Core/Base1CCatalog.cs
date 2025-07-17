using sabatex.BakeryWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;


/// <summary>
/// Base abstract class for directory (catalog) entities.
/// Serves as a marker for catalog objects and may be extended with common catalog behavior.
/// </summary>
public abstract class Base1CCatalog : Base1CObject
{
    // Optional functionality (e.g. Description, Code) is defined via separate interfaces:
    // - IEntityFieldDescription
    // - ICatalogCode
    // This separation provides flexibility in how catalogs are composed.
}
