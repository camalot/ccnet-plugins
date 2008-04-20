/*
 * http://www.codeplex.com/ccnetplugins/
 * 
 * Microsoft Public License (Ms-PL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not 
 * accept the license, do not use the software.
 * 
 * 1. Definitions
 * The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. 
 * copyright law.
 * 
 * A "contribution" is the original software, or any additions or changes to the software.
 * 
 * A "contributor" is any person that distributes its contribution under this license.
 * 
 * "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 * 
 * 2. Grant of Rights
 * 
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
 * each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
 * prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 * 
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each 
 * contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
 * sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the 
 * contribution in the software.
 * 
 * 3. Conditions and Limitations
 * 
 * (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 * 
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your 
 * patent license from such contributor to the software ends automatically.
 * 
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices 
 * that are present in the software.
 * 
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a 
 * complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code 
 * form, you may only do so under a license that complies with this license.
 * 
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees 
 * or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent 
 * permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular 
 * purpose and non-infringement.
 */
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
