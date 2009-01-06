using System;
using System.Collections.Generic;
using System.Text;
using CCNet.Community.Plugins.Publishers;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
	public class CodeplexReleasePublisherTests {
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

			Assert.Equal<string> ( "0.0.0.1", task.MacroEngine.GetPropertyString<CodePlexReleasePublisher> ( task, CreateResultObject (), "${Label}" ) );
		}

		[Fact]
		public void RunTaskTest () {


			CodePlexReleasePublisher task = new CodePlexReleasePublisher ();
			NetReflector.Read ( xml, task );
			IntegrationResult result = CreateResultObject ();
			Assert.DoesNotThrow ( delegate {
				task.Run ( result );
			} );

		}

		private IntegrationResult CreateResultObject () {
			IntegrationResult result = new IntegrationResult ( "ccnetpluginstest", @"d:\source\ccnetpluginstest\", @"d:\redist\ccnetpluginstest\",
				new ThoughtWorks.CruiseControl.Remote.IntegrationRequest (
					ThoughtWorks.CruiseControl.Remote.BuildCondition.IfModificationExists, "Test" ),
					new IntegrationSummary ( ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success,
						"0.0.0.1", "0.0.0.0", DateTime.Now ) );

			result.Status = ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success;
			result.ProjectName = "ccnetpluginstest";
			result.Label = "0.0.0.1";

			result.IntegrationProperties.Add ( "ProjectStatisticsFile", "foo.xml" );
			result.Modifications = new Modification[ 2 ];
			result.Modifications[ 0 ] = new Modification ();
			result.Modifications[ 0 ].Comment = "Test Comment";
			result.Modifications[ 0 ].EmailAddress = "codeplex@codeplex.com";
			result.Modifications[ 0 ].FileName = "foo.file";
			result.Modifications[ 0 ].FolderName = "foo";
			result.Modifications[ 0 ].ModifiedTime = DateTime.Now;
			result.Modifications[ 0 ].UserName = "codeplex";

			result.Modifications[ 1 ] = new Modification ();
			result.Modifications[ 1 ].Comment = "Test Comment 2";
			result.Modifications[ 1 ].EmailAddress = "codeplex@codeplex.com";
			result.Modifications[ 1 ].FileName = "foo2.file";
			result.Modifications[ 1 ].FolderName = "foo2";
			result.Modifications[ 1 ].ModifiedTime = DateTime.Now;
			result.Modifications[ 1 ].UserName = "codeplex";

			return result;
		}
	}
}
