using System;
using System.Collections.Generic;
using System.Text;
using CCNet.Community.Plugins.Publishers;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
	public class CodeplexReleasePublisherTests : IntegrationResultTestObject {
		private string xml = string.Empty;
		public CodeplexReleasePublisherTests () {
			xml = @"<codeplexRelease projectName=""ccnetpluginstest"">
        <username>ccnetplugins_test</username>
        <password>p@ssw0rd1</password>
        <releases>
          <release releaseName=""${Label}"">
            <description>${ProjectName} ${Label}

${ModificationComments}</description>
            <releaseFiles>
              <releaseFile fileName=""b:\redist\${ProjectName}\debug\${Label}\${ProjectName}.${Label}.zip"" fileType=""RuntimeBinary"" mimeType=""application/x-zip"" name=""${ProjectName}.${Label}"" />
              <releaseFile fileName=""b:\redist\${ProjectName}\debug\${Label}\${ProjectName}.${Label}.src.zip"" fileType=""SourceCode"" mimeType=""application/x-zip"" name=""${ProjectName}.${Label}.src"" />
            </releaseFiles>
            <buildCondition>IfModificationExists</buildCondition>
            <releaseStatus>Released</releaseStatus>
            <releaseType>Beta</releaseType>
            <isDefaultRelease>True</isDefaultRelease>
            <showToPublic>True</showToPublic>
            <showOnHomePage>True</showOnHomePage>
          </release>
				</releases>
      </codeplexRelease>";
		}
		
		[Fact]
		public void CreatePublisherTest () {
			CodePlexReleasePublisher task = new CodePlexReleasePublisher ();
			NetReflector.Read ( xml, task );

			Assert.Equal<int> ( 1, task.Releases.Count );
			Assert.Equal<string> ( "${Label}", task.Releases[ 0 ].ReleaseName );

			Assert.Equal<string> ( "1.2.3.4", task.MacroEngine.GetPropertyString<CodePlexReleasePublisher> ( task, this.Result, "${Label}" ) );
		}

		[Fact]
		public void RunTaskTest () {


			CodePlexReleasePublisher task = new CodePlexReleasePublisher ();
			NetReflector.Read ( xml, task );
			IntegrationResult result = this.Result;
			Assert.DoesNotThrow ( delegate {
				task.Run ( result );
			} );

		}
	}
}
