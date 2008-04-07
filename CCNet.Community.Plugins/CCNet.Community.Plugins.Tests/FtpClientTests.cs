using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CCNet.Community.Plugins.SourceControls;

namespace CCNet.Community.Plugins.Tests {
  [TestFixture]
  public class FtpClientTests {

    [Test]
    public void GetDirectories ( ) {
      FtpClient client = CreateFtpClient ( );
      List<string> dirs = client.GetDirectories ( );
      Console.WriteLine ( string.Join ( ",", dirs.ToArray ( ) ) );
      Assert.Greater ( dirs.Count, 0 );
    }

    [Test]
    public void GetFiles ( ) {
      FtpClient client = CreateFtpClient ( );
      List<string> files = client.GetFiles ( );
      Console.WriteLine ( string.Join ( ",", files.ToArray ( ) ) );
      Assert.Greater ( files.Count, 0 );
    }


    private FtpClient CreateFtpClient ( ) {
      FtpClient client = new FtpClient ( );
      client.FtpServer = "ftp.ccnetconfig.org";
      client.Path = "Sources/CCNetConfig";
      client.UsePassive = true;
      client.Username = "anonymous";
      client.Password = "ccnet@ccnetconfig.org";
      return client;
    }
  }
}
