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
using CCNet.Community.Plugins.Publishers;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using Exortech.NetReflector;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
  internal class TestTaskResult : ITaskResult{
    private string _data = string.Empty;
    public TestTaskResult ( string data ) {
      this._data = data;
    }
    #region ITaskResult Members

    public string Data {
      get { return this._data; }
    }

    public bool Failed ( ) {
      return true;
    }

    public bool Succeeded ( ) {
      return false;
    }

    #endregion
  }


  public class TfsWorkItemPublisherTests {
    [Fact]
    public void PublishFailedBuildTest ( ) {
      // the user name and password have been removed so this test will fail.
      return;
      try {
        TfsWorkItemPublisher tfsp = new TfsWorkItemPublisher ( );
        tfsp.Domain = "snd";
        tfsp.UserName = "_cp";
        tfsp.Password = "";
        tfsp.TfsServer = "https://tfs05.codeplex.com";
        tfsp.ProjectName = "ccnetplugins";
        tfsp.TitlePrefix = "[Test]";

        IntegrationResult result = new IntegrationResult (
          "ccnetplugins", "./", "./", new IntegrationRequest ( BuildCondition.IfModificationExists, "Test" ),
            new IntegrationSummary ( IntegrationStatus.Success, "0.0.0.2", "0.0.0.1", DateTime.Now ) );

        for ( int i = 0; i < 10; i++ ) {
          if ( i % 3 == 0 ) {
            result.AddTaskResult ( new DataTaskResult ( "Successfull result" ) );
          } else {
            TestTaskResult ttr = new TestTaskResult ( "Test " + i );
            result.AddTaskResult ( ttr );
          }
        }

        TfsServerConnection conn = new TfsServerConnection ( tfsp, result );
        conn.Publish ( );
      } catch ( Exception ex ) {
        Assert.True (false, ex.Message );
      }
    }

    [Fact]
    public void TfsWorkItemCreationTest ( ) {
      string xml = @"<workitemPublisher>
							 <server>https://tfs05.codeplex.com</server>
				       <domain>snd</domain>
               <username>foo_cp</username>
               <password>foo</password>
               <project>ccnetplugins</project>
               <titleprefix>[Test]</titleprefix>
						 </workitemPublisher>";

      TfsWorkItemPublisher task = NetReflector.Read ( xml ) as TfsWorkItemPublisher;
      Assert.Equal<String> ( "https://tfs05.codeplex.com", task.TfsServer );
      Assert.Equal<String> ( "snd", task.Domain );
      Assert.Equal<String> ( "foo_cp", task.UserName );
      Assert.Equal<String> ( "foo", task.Password );
      Assert.Equal<String> ( "ccnetplugins", task.ProjectName );
      Assert.Equal<String> ( "[Test]", task.TitlePrefix );
    }

  }
}
