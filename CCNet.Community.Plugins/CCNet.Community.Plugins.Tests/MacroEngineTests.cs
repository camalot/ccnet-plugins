using System;
using System.Collections.Generic;
using System.Text;
using CCNet.Community.Plugins.Components.Macros;
using ThoughtWorks.CruiseControl.Core;
using Xunit;
using System.IO;

namespace CCNet.Community.Plugins.Tests {
  public class MacroRunner : IMacroRunner {
    public MacroRunner ( ) {
      this.MacroEngine = new MacroEngine ( );
    }

    #region IMacroRunner Members

    public MacroEngine MacroEngine {
      get;
      private set;
    }

		public string GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
			return input;
		}

		#endregion
    public string MyProperty { get { return "This is My Property"; } }
	}
  public class MacroEngineTests : IntegrationResultTestObject {
    public MacroEngineTests ( )
      : base ( ) {
    }
    [Fact]
    public void GetResultPropertyValue ( ) {
      MacroRunner runner = new MacroRunner ( );
      string ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "${ArtifactDirectory}" );
      Assert.Equal<string> ( @"d:\redist\ccnetplugins\", ad );
      ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "${CCNetLabel}" );
      Assert.Equal<string> ( @"1.2.3.4", ad );

			ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "${CCNetLabel}:${CCNetLabel}" );
			Assert.Equal<string> ( @"1.2.3.4:1.2.3.4", ad );

    }

    [Fact]
    public void GetObjectPropertyValue ( ) {
      MacroRunner runner = new MacroRunner ( );
      string ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "${MyProperty}" );
      Assert.Equal<string> ( @"This is My Property", ad );
    }

    [Fact]
    public void NoPropertyFound ( ) {
      MacroRunner runner = new MacroRunner ( );
      string ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "${NoProperty}" );
      Assert.Equal<string> ( @"${NoProperty}", ad );
    }

    [Fact]
    public void GetXPathValue ( ) {
      MacroRunner runner = new MacroRunner ( );
      string ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "!{twitter/id}" );
      Assert.Equal<string> ( "799201794", ad );
      ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "!{twitter/id/!}" );
      Assert.Equal<string> ( "!{twitter/id/!}", ad );

      // node not found
      ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "!{CodePlexRelease/@id}" );
      Assert.Equal<string> ( "!{CodePlexRelease/@id}", ad );

      ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "!{CodePlexRelease/@Id}" );
      Assert.Equal<string> ( "12932", ad );
      ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "!{CodePlexRelease/@Name}" );
      Assert.Equal<string> ( "1.2.3.4", ad );
      ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "!{CodePlexRelease/@Type}" );
      Assert.Equal<string> ( "Beta", ad );
    }

    [Fact]
    public void CheckMacroValues ( ) {
      MacroRunner runner = new MacroRunner ( );
      string ad = runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "@{BuildConditionToConfiguration()}" );
      Assert.Equal<string> ( @"Debug", ad );
      ad = runner.MacroEngine.Execute ( Result, runner, "BuildConditionToConfiguration", string.Empty );
      Assert.Equal<string> ( @"Debug", ad );
      Result.BuildCondition = ThoughtWorks.CruiseControl.Remote.BuildCondition.ForceBuild;
      ad = new BuildConditionToConfiguration ( ).Execute ( Result, string.Empty );
      Assert.Equal<string> ( @"Release", ad );

      ad = new BuildConditionToReleaseType ( ).Execute ( Result, string.Empty );
      Assert.Equal<string> ( "Production", ad );
      ad = runner.MacroEngine.Execute ( Result, runner, "BuildConditionToReleaseType", string.Empty );
      Assert.Equal<string> ( "Production", ad );

      ad = new DateTimeToString ( ).Execute ( Result, "MM/dd/yyyy" );
      Assert.Equal<string> ( DateTime.Now.ToString ( "MM/dd/yyyy" ), ad );
      ad = new DateTimeToString ( ).Execute ( Result, "04/01/1977 02:12:00,MM/dd/yyyy" );
      Assert.Equal<string> ( "04/01/1977", ad );
      ad = new DateTimeToString ( ).Execute ( Result, "04/01/1977 02:12:00,MM/dd/yyyy,foo" );
      Assert.Equal<string> ( "MacroFormatException: This macro does not support the supplied parmeters.", ad );
      //2,724
      ad = new GetFileSize ( ).Execute ( Result, @"d:\Projects\CCNetPlugins\trunk\CCNet.Community.Plugins\License.txt" );
      Assert.Equal<string> ( "2724", ad );
      Assert.Throws<ArgumentNullException> ( new Assert.ThrowsDelegate ( delegate ( ) {
        new GetFileSize ( ).Execute ( Result, string.Empty );
      } ) );

      ad = new GetFileSize ( ).Execute ( Result, @"c:\my.file" );
      Assert.Equal<string> ( @"MacroException: File 'c:\my.file' was not found.", ad );

      Assert.DoesNotThrow ( new Assert.ThrowsDelegate ( delegate ( ) {
        new XslTransform ( ).Execute ( Result, runner, "CodePlexRelease,d:\\Projects\\CCNetPlugins\\trunk\\CCNet.Community.Plugins\\CCNet.Community.Plugins.Tests\\Resources\\xsltest.xslt" );
        runner.MacroEngine.GetPropertyString<IMacroRunner> ( runner, Result, "@{XslTransform(CodePlexRelease,D:\\Projects\\CCNetPlugins\\trunk\\CCNet.Community.Plugins\\CCNet.Community.Plugins.Tests\\Resources\\xsltest.xslt)}" );
      } ) );

      
    }

  }
}
