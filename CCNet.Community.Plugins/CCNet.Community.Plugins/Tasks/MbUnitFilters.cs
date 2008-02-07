using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;

namespace CCNet.Community.Plugins.Tasks {
  /// <summary>
  /// 
  /// </summary>
  [ReflectorType("filters")]
  public class MbUnitFilters {
    /// <summary>
    /// Initializes a new instance of the <see cref="MbUnitFilters"/> class.
    /// </summary>
    public MbUnitFilters ( ) {
      this.Categories = new string[ 0 ];
      this.ExcludeCategories = new string[ 0 ];
      this.Author = new string[0];
      this.Type = new string[0];
      this.Namespaces = new string[0];
    }
    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    /// <value>The categories.</value>
    [ReflectorArray ( "filterCategories" )]
    public string[ ] Categories { get; set; }
    /// <summary>
    /// Gets or sets the exclude categories.
    /// </summary>
    /// <value>The exclude categories.</value>
    [ReflectorArray ( "excludeCategories" )]
    public string[ ] ExcludeCategories { get; set; }
    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    /// <value>The author.</value>
    [ReflectorArray ( "authors" )]
    public string[ ] Author { get; set; }
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    [ReflectorArray("types")]
    public string[] Type { get; set; }
    /// <summary>
    /// Gets or sets the namespaces.
    /// </summary>
    /// <value>The namespaces.</value>
    [ReflectorArray("namespaces")]
    public string[] Namespaces { get; set; }
  }
}
