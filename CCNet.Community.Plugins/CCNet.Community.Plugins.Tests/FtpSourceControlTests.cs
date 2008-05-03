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
using CCNet.Community.Plugins.SourceControls;
using Exortech.NetReflector;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
  public class FtpSourceControlTests {
    [Fact]
    public void LoadWithUriNoPort ( ) {
        string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp://ftp.google.com/my/code/path</server>
</sourcecontrol>";
        FtpSourceControl task = new FtpSourceControl ( );
          NetReflector.Read ( xml, task ) ;
        Assert.Equal<String> ( task.ToString ( ), "ftp://ftp.google.com/my/code/path/" );
    }

    [Fact]
    public void LoadWithUriDefaultPort ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp://ftp.google.com:21/my/code/path</server>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.Equal<String> ( task.ToString ( ), "ftp://ftp.google.com/my/code/path/" );
    }

    [Fact]
    public void LoadWithUriNonDefaultPort ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp://ftp.google.com:2111/my/code/path</server>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.Equal<String> ( task.ToString ( ), "ftp://ftp.google.com:2111/my/code/path/" );
    }

    [Fact]
    public void LoadWithValuesNonDefaultPort ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp.google.com</server>
  <port>2111</port>
  <repositoryRoot>my/code/path</repositoryRoot>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.Equal<String> ( task.ToString ( ), "ftp://ftp.google.com:2111/my/code/path/" );
    }

    [Fact]
    public void LoadWithValuesNonDefaultPortSecured ( ) {
      string xml = @"<sourcecontrol type=""ftp"">
	<server>ftp.google.com</server>
  <port>2111</port>
  <repositoryRoot>my/code/path</repositoryRoot>
  <useSsl>true</useSsl>
</sourcecontrol>";
      FtpSourceControl task = new FtpSourceControl ( );
      NetReflector.Read ( xml, task );
      Assert.Equal<String> ( task.ToString ( ), "ftps://ftp.google.com:2111/my/code/path/" );
    }

    [Fact]
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
