using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;

namespace CCNet.Community.Plugins.Tasks {
  /// <summary>
  /// 
  /// </summary>
  public class MbUnitArgument {
    public MbUnitArgument ( MbUnitTask mbunit, IIntegrationResult result ) {
      if ( ( mbunit.Assemblies == null ) || ( mbunit.Assemblies.Length == 0 ) ) {
        throw new CruiseControlException ( "No unit test assemblies are specified. Please use the <assemblies> element to specify the test assemblies to run." );
      }
      this.MbUnit = mbunit;
      this.IntegrationResult = result;
    }

    public MbUnitTask MbUnit { get; set; }
    public IIntegrationResult IntegrationResult { get; set; }

    public override string ToString ( ) {

      ProcessArgumentBuilder builder = new ProcessArgumentBuilder ( );
      builder.AddArgument ( "/rt", ":", "xml" );
      builder.AddArgument ( "/v" );

      if ( !string.IsNullOrEmpty ( MbUnit.TransformFile ) ) {
        builder.AddArgument ( "/tr", ":", MbUnit.TransformFile );
      }

      if ( MbUnit.Filters.Author != null && MbUnit.Filters.Author.Length > 0 ) {
        builder.AddArgument ( "/fa", ":", string.Join ( ",", MbUnit.Filters.Author ) );
      }

      if ( MbUnit.Filters.Categories != null && MbUnit.Filters.Categories.Length > 0 ) {
        builder.AddArgument ( "/fc", ":", string.Join ( ",", MbUnit.Filters.Categories ) );
      }

      if ( MbUnit.Filters.ExcludeCategories != null && MbUnit.Filters.ExcludeCategories.Length > 0 ) {
        builder.AddArgument ( "/ec", ":", string.Join ( ",", MbUnit.Filters.ExcludeCategories ) );
      }

      if ( MbUnit.Filters.Type != null && MbUnit.Filters.Type.Length > 0 ) {
        builder.AddArgument ( "/ft", ":", string.Join ( ",", MbUnit.Filters.Type ) );
      }

      if ( MbUnit.Filters.Namespaces != null && MbUnit.Filters.Namespaces.Length > 0 ) {
        builder.AddArgument ( "/fn", ":", string.Join ( ",", MbUnit.Filters.Namespaces ) );
      }

      foreach ( string assembly in MbUnit.Assemblies ) {
        builder.AddArgument ( assembly );
      }

      if ( !string.IsNullOrEmpty ( MbUnit.AssemblyPath ) ) {
        builder.AddArgument ( "/ap", ":", MbUnit.AssemblyPath );
      }

      builder.AddArgument ( "/rf", ":", Path.GetDirectoryName ( IntegrationResult.BaseFromArtifactsDirectory ( MbUnit.OutputFile ) ) );
      builder.AddArgument ( "/rnf", ":", Path.GetFileName ( MbUnit.OutputFile ) );

      return builder.ToString ( );
    }
  }
}
