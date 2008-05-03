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
using CCNet.Community.Plugins.Tasks;
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
  public class MbUnitTasksTests {
    [Fact]
    public void LoadWithNoAssemblies ( ) {
      try {
        string xml = @"<mbunit>
	<executable>d:\temp\mbunit-console.exe</executable>
	<outputfile>c:\testfile.xml</outputfile>
  <filters>
    <filterCategories>
      <category>Bar</category>
    </filterCategories>
    <excludeCategories>
      <category>Foo</category>
    </excludeCategories>
    <authors>
      <author>Ryan</author>
    </authors>
    <types>
      <type>Foo.Bar.Blarg</type>
    </types>
    <namespaces>
      <namespace>Foo.Bar</namespace>
    </namespaces>
  </filters>
  <assemblypath>c:\assemblies</assemblypath>
  <transformfile>c:\trans.xsl</transformfile>
	<timeout>50</timeout>
</mbunit>";
        MbUnitTask task = NetReflector.Read ( xml ) as MbUnitTask;
        Assert.True (false, "Expected an exception." );
      }  catch ( Exception ex ) {

      }
    }

    [Fact]
    public void LoadWithSingleAssemblyMbUnitPath ( ) {
      string xml = @"<mbunit>
	<executable>d:\temp\mbunit-console.exe</executable>
	<assemblies>
		<assembly>foo.dll</assembly>
	</assemblies>
	<outputfile>c:\testfile.xml</outputfile>
  <filters>
    <filterCategories>
      <category name=""Bar"" />
    </filterCategories>
    <excludeCategories>
      <category name=""Foo"" />
    </excludeCategories>
    <authors>
      <author>Ryan</author>
    </authors>
    <types>
      <type>Foo.Bar.Blarg</type>
    </types>
    <namespaces>
      <ns>Foo.Bar</ns>
    </namespaces>
  </filters>
  <assemblypath>c:\assemblies</assemblypath>
  <transformfile>c:\trans.xsl</transformfile>
	<timeout>50</timeout>
</mbunit>";
      MbUnitTask task = NetReflector.Read ( xml ) as MbUnitTask;
      Assert.Equal<String> ( @"d:\temp\mbunit-console.exe", task.Executable );
      Assert.Equal ( 1, task.Assemblies.Length );
      Assert.Equal<String> ( "foo.dll", task.Assemblies[ 0 ] );
      Assert.Equal<String> ( "Bar", task.Filters.Categories[ 0 ].Name );
      Assert.Equal<String> ( task.Filters.Categories[ 0 ].ToString(), task.Filters.Categories[ 0 ].Name );
      Assert.Equal<String> ( "Foo", task.Filters.ExcludeCategories[ 0 ].Name );
      Assert.Equal<String> ( task.Filters.ExcludeCategories[ 0 ].ToString(), task.Filters.ExcludeCategories[ 0 ].Name );
      Assert.Equal<String> ( "Ryan", task.Filters.Author[ 0 ] );
      Assert.Equal<String> ( "Foo.Bar.Blarg", task.Filters.Type[ 0 ] );
      Assert.Equal<String> ( "Foo.Bar", task.Filters.Namespaces[ 0 ] );
      Assert.Equal<String> ( @"c:\testfile.xml", task.OutputFile );
      Assert.Equal<String> ( @"c:\assemblies", task.AssemblyPath );
      Assert.Equal<String> ( @"c:\trans.xsl", task.TransformFile );
      Assert.Equal ( 50, task.Timeout );

      Console.WriteLine ( new MbUnitArgument ( task, new IntegrationResult ( ) ) );
    }

    [Fact]
    public void LoadWithMultipleAssemblies ( ) {
      string xml = @"<mbunit>
							 <executable>d:\temp\mbunit-console.exe</executable>
				             <assemblies>
			                     <assembly>foo.dll</assembly>
								 <assembly>bar.dll</assembly>
							</assemblies>
						 </mbunit>";

      MbUnitTask task = NetReflector.Read ( xml ) as MbUnitTask;
      AssertEqualArrays ( new string[ ] { "foo.dll", "bar.dll" }, task.Assemblies );
    }

    [Fact]
    public void HandleMbUnitTaskFailure ( ) {
      /*CreateProcessExecutorMock ( NUnitTask.DefaultPath );
      ExpectToExecuteAndReturnWithMonitor ( SuccessfulProcessResult ( ), new ProcessMonitor ( ) );
      IIntegrationResult result = IntegrationResult ( );
      result.ArtifactDirectory = Path.GetTempPath ( );

      task = new NUnitTask ( ( ProcessExecutor ) mockProcessExecutor.MockInstance );
      task.Assemblies = new string[ ] { "foo.dll" };
      task.Run ( result );

      Assert.AreEqual ( 1, result.TaskResults.Count );
      Assert.AreEqual ( ProcessResultOutput, result.TaskOutput );
      Verify ( );*/
    }

    public static void AssertEqualArrays ( Array expected, Array actual ) {
      Assert.Equal ( actual.Length, expected.Length );

      for ( int i = 0; i < expected.Length; i++ ) {
        Assert.Equal ( expected.GetValue ( i ), actual.GetValue ( i ) );
      }
    }
  }
}
