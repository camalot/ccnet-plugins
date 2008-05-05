using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;

namespace CCNet.Community.Plugins.Tests {
  public abstract class IntegrationResultTestObject {
    public IntegrationResult Result { get; set; }
    public IntegrationResultTestObject ( ) {
      Result = new IntegrationResult ( "test", @"d:\source\ccnetplugins\", @"d:\redist\ccnetplugins\",
        new ThoughtWorks.CruiseControl.Remote.IntegrationRequest (
          ThoughtWorks.CruiseControl.Remote.BuildCondition.IfModificationExists, "Test" ),
          new IntegrationSummary ( ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success,
            "1.2.3.4", "1.2.3.3", DateTime.Now ) );
      Result.IntegrationProperties.Add ( "ProjectStatisticsFile", "foo.xml" );
      Result.Modifications = new Modification[ 2 ];
      Result.Modifications[ 0 ] = new Modification ( );
      Result.Modifications[ 0 ].Comment = "Test Comment";
      Result.Modifications[ 0 ].EmailAddress = "codeplex@codeplex.com";
      Result.Modifications[ 0 ].FileName = "foo.file";
      Result.Modifications[ 0 ].FolderName = "foo";
      Result.Modifications[ 0 ].ModifiedTime = DateTime.Now;
      Result.Modifications[ 0 ].UserName = "codeplex";

      Result.Modifications[ 1 ] = new Modification ( );
      Result.Modifications[ 1 ].Comment = "Test Comment 2";
      Result.Modifications[ 1 ].EmailAddress = "codeplex@codeplex.com";
      Result.Modifications[ 1 ].FileName = "foo2.file";
      Result.Modifications[ 1 ].FolderName = "foo2";
      Result.Modifications[ 1 ].ModifiedTime = DateTime.Now;
      Result.Modifications[ 1 ].UserName = "codeplex";
      Result.TaskResults.Add ( new ThoughtWorks.CruiseControl.Core.Tasks.DataTaskResult ( @"<twitter>
  <created_at>Tue Apr 29 02:32:19 +0000 2008</created_at>
  <id>799201794</id>
  <text>ccnetplugins Build Fixed: Build 1.0.421.10385. See http://codeplex.com/ccnetplugins</text>
  <source>web</source>
  <truncated>false</truncated>
  <in_reply_to_status_id>
  </in_reply_to_status_id>
  <in_reply_to_user_id>
  </in_reply_to_user_id>
  <user>
    <id>14520429</id>
    <name>ccnetplugins</name>
    <screen_name>ccnetplugins</screen_name>
    <location>Chicago</location>
    <description>
    </description>
    <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/53282374/cH_s2KX0DrkOt1xS3e9Kig___normal.png</profile_image_url>
    <url>http://codeplex.com/ccnetplugins</url>
    <protected>false</protected>
    <followers_count>0</followers_count>
  </user>
</twitter>" ) );

      Result.TaskResults.Add ( new ThoughtWorks.CruiseControl.Core.Tasks.DataTaskResult ( "<CodePlexRelease Id=\"12932\" Name=\"1.2.3.4\" Type=\"Beta\" />" ) );
    }

  }
}
