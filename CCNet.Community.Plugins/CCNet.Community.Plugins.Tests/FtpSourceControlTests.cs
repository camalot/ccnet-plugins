using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CCNet.Community.Plugins.SourceControls;
using Exortech.NetReflector;

namespace CCNet.Community.Plugins.Tests {
  [TestFixture]
  public class FtpSourceControlTests {
    [Test]
    public void LoadWithUriNoPort ( ) {
        string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp://ftp.google.com/my/code/path</server>
</sourcecontrol>";
        FtpSourceControl task = new FtpSourceControl ( );
          NetReflector.Read ( xml, task ) ;
        Assert.AreEqual ( task.ToString ( ), "ftp://ftp.google.com/my/code/path/" );
    }

    [Test]
    public void LoadWithUriDefaultPort ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp://ftp.google.com:21/my/code/path</server>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.AreEqual ( task.ToString ( ), "ftp://ftp.google.com/my/code/path/" );
    }

    [Test]
    public void LoadWithUriNonDefaultPort ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp://ftp.google.com:2111/my/code/path</server>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.AreEqual ( task.ToString ( ), "ftp://ftp.google.com:2111/my/code/path/" );
    }

    [Test]
    public void LoadWithValuesNonDefaultPort ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp.google.com</server>
  <port>2111</port>
  <repositoryRoot>my/code/path</repositoryRoot>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.AreEqual ( task.ToString ( ), "ftp://ftp.google.com:2111/my/code/path/" );
    }

    [Test]
    public void LoadWithValuesNonDefaultPortSecured ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp.google.com</server>
  <port>2111</port>
  <repositoryRoot>my/code/path</repositoryRoot>
  <useSecuredFtp>true</useSecuredFtp>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.AreEqual ( task.ToString ( ), "sftp://ftp.google.com:2111/my/code/path/" );
    }

    [Test]
    public void GetSource ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp.ccnetconfig.org</server>
  <port>21</port>
  <repositoryRoot>/sources/CCNet.Community.Plugins</repositoryRoot>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      task.GetSource ( null );
    }
  }
}
