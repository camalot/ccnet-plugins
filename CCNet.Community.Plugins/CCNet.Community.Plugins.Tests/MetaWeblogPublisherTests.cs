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
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CCNet.Community.Plugins.Publishers;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using System.Threading;

namespace CCNet.Community.Plugins.Tests {
  public class MetaWeblogPublisherTests {
    [Fact]
    public void PublishBuildTest ( ) {
      
      string xml = @"<metaweblog>
  <xmlrpcurl>http://yourdomain.com/services/metablogapi.aspx</xmlrpcurl>
  <username>someUser</username>
  <password>p@ssVV0rd!</password>
  <titleformat>[Test]{6} {2}: {5}</titleformat>
  <descriptionformat><![CDATA[[Test]<br /><h4>{6} {2}: {5}</h4><h5>Status</h5>{7}<br /><h5>Modifications</h5>{0}<br /><h5>Results</h5>{1}<br /><h5>Last Changeset Number</h5>{3}<br /><h5>Total Integration Time</h5>{4}<br />]]></descriptionformat>
  <continueOnFailure>false</continueOnFailure>
</metaweblog>";

      MetaWeblogPublisher publisher = NetReflector.Read ( xml ) as MetaWeblogPublisher;

      Assert.True ( string.Compare ( publisher.Username, "someUser", false ) == 0 );

      return;
      // remove the return line and change the xmlrpcurl, username and password to fully test this. 
      // I have tested it and changed those values to protect the server and account.
      IntegrationResult result = new IntegrationResult ( "test", "c:\\source", "c:\\builds", new ThoughtWorks.CruiseControl.Remote.IntegrationRequest ( ThoughtWorks.CruiseControl.Remote.BuildCondition.ForceBuild, "foo" ), new IntegrationSummary ( ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success, "0.1.2.3", "0.1.2.2", DateTime.Now.Subtract ( new TimeSpan ( 0, 5, 30 ) ) ) );
      result.MarkStartTime ( );
      result.Modifications = new Modification[ 2 ];
      result.Modifications[ 0 ] = new Modification ( );
      result.Modifications[ 0 ].UserName = "someUser";
      result.Modifications[ 0 ].Comment = "fooy fooy booy hooy gluey";
      result.Modifications[ 0 ].FileName = "foo.cs";

      result.Modifications[ 1 ] = new Modification ( );
      result.Modifications[ 1 ].UserName = "someUser";
      result.Modifications[ 1 ].Comment = "fooy fooy booy hooy gluey";
      result.Modifications[ 1 ].FileName = "bar.cs";
      result.Label = "0.1.2.3";
      result.LastIntegrationStatus = ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success;
      result.LastSuccessfulIntegrationLabel = "0.1.2.2";
      Thread.Sleep ( 1500 );
      result.MarkEndTime ( );
      result.AddTaskResult ( "blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah" );
      result.AddTaskResult ( "dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah dah" );
      result.AddTaskResult ( "rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah rah" );
      publisher.Run ( result );
    }

  }
}
