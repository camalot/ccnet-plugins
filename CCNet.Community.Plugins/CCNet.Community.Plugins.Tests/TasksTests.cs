using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CCNet.Community.Plugins.Tasks;
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;

namespace CCNet.Community.Plugins.Tests {
  [TestFixture]
  public class MbUnitTasksTests {
    [Test]
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
        Assert.Fail ( "Expected an exception." );
      }  catch ( Exception ex ) {

      }
    }

    [Test]
    public void LoadWithSingleAssemblyMbUnitPath ( ) {
      string xml = @"<mbunit>
	<executable>d:\temp\mbunit-console.exe</executable>
	<assemblies>
		<assembly>foo.dll</assembly>
	</assemblies>
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
      Assert.AreEqual ( @"d:\temp\mbunit-console.exe", task.Executable );
      Assert.AreEqual ( 1, task.Assemblies.Length );
      Assert.AreEqual ( "foo.dll", task.Assemblies[ 0 ] );
      Assert.AreEqual ( "Bar", task.Filters.Categories[ 0 ] );
      Assert.AreEqual ( "Foo", task.Filters.ExcludeCategories[ 0 ] );
      Assert.AreEqual ( "Ryan", task.Filters.Author[ 0 ] );
      Assert.AreEqual ( "Foo.Bar.Blarg", task.Filters.Type[ 0 ] );
      Assert.AreEqual ( "Foo.Bar", task.Filters.Namespaces[ 0 ] );
      Assert.AreEqual ( @"c:\testfile.xml", task.OutputFile );
      Assert.AreEqual ( @"c:\assemblies", task.AssemblyPath );
      Assert.AreEqual ( @"c:\trans.xsl", task.TransformFile );
      Assert.AreEqual ( 50, task.Timeout );

      Console.WriteLine ( new MbUnitArgument ( task, new IntegrationResult ( ) ) );
    }

    [Test]
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

    [Test]
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
      Assert.AreEqual ( actual.Length, expected.Length, "Arrays should have same length" );

      for ( int i = 0; i < expected.Length; i++ ) {
        Assert.AreEqual ( expected.GetValue ( i ), actual.GetValue ( i ), "Comparing array index " + i );
      }
    }
  }
}
