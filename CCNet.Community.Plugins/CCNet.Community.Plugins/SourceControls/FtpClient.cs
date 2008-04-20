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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CCNet.Community.Plugins.SourceControls {
  public class FtpClient : WebClient {
    public FtpClient ( ) {
      this.Port = 21;
    }

    /// <summary>
    /// Gets or sets the FTP server.
    /// </summary>
    /// <value>The FTP server.</value>
    public string FtpServer { get; set; }
    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>The port.</value>
    public int Port { get; set; }
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>The username.</value>
    public string Username { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use passive].
    /// </summary>
    /// <value><c>true</c> if [use passive]; otherwise, <c>false</c>.</value>
    public bool UsePassive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable SSL].
    /// </summary>
    /// <value><c>true</c> if [enable SSL]; otherwise, <c>false</c>.</value>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>The path.</value>
    public string Path { get; set; }

    /// <summary>
    /// Gets the FTP URI.
    /// </summary>
    /// <returns></returns>
    private Uri GetFtpUri ( ) {
      StringBuilder sbUri = new StringBuilder ( );
      Regex regex = new Regex ( "^ftps?://([^:|/]+)(.*?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline );
      string server = this.FtpServer;
      string path = this.Path;
      if ( regex.IsMatch ( this.FtpServer ) ) {
        server = regex.Replace ( this.FtpServer, "$1" );
        path = regex.Replace ( this.FtpServer, "$2" );
      }
      int port = this.Port;
      regex = new Regex ( @"^:(\d{1,})(.*?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline );
      if ( regex.IsMatch ( path ) ) {
        port = int.Parse ( regex.Replace ( path, "$1" ) );
        path = regex.Replace ( path, "$2" );
      }

      if ( string.IsNullOrEmpty ( path ) || this.Path.Length > 1 ) {
        path = this.Path;
      }

      if ( !path.StartsWith ( System.IO.Path.AltDirectorySeparatorChar.ToString ( ) ) ) {
        path = System.IO.Path.AltDirectorySeparatorChar + path;
      }

      if ( !path.EndsWith ( System.IO.Path.AltDirectorySeparatorChar.ToString ( ) ) ) {
        path += System.IO.Path.AltDirectorySeparatorChar;
      }

      sbUri.AppendFormat ( "ftp{0}://", this.EnableSsl ? "s" : string.Empty );
      sbUri.Append ( server );
      sbUri.Append ( ":" );
      sbUri.Append ( port );

      sbUri.Append ( path );
      Console.WriteLine ( sbUri.ToString ( ) );
      return new Uri ( sbUri.ToString ( ) );
    }

    /// <summary>
    /// Creates the credentials.
    /// </summary>
    /// <returns></returns>
    private ICredentials CreateCredentials ( ) {
      if ( !string.IsNullOrEmpty ( this.Username ) ) {
        return new NetworkCredential ( this.Username, this.Password );
      } else {
        return new NetworkCredential ( "anonymous", "ftp@ccnetconfig.org" );
      }
    }

    /// <summary>
    /// Creates the FTP request.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns></returns>
    private FtpWebRequest CreateFtpRequest ( Uri address ) {
      FtpWebRequest req = FtpWebRequest.Create ( address ) as FtpWebRequest;
      req.Credentials = this.CreateCredentials ( );
      req.UsePassive = this.UsePassive;
      req.EnableSsl = this.EnableSsl;
      return req;

    }

    /// <summary>
    /// Gets the directories.
    /// </summary>
    /// <returns></returns>
    public List<string> GetDirectories ( ) {
      List<string> directories = new List<string> ( );
      FtpWebRequest req = this.CreateFtpRequest ( this.GetFtpUri() );
      req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
      FtpWebResponse response = req.GetResponse ( ) as FtpWebResponse;
      Stream responseStream = response.GetResponseStream ( );
      StreamReader reader = new StreamReader ( responseStream );
      string data = reader.ReadToEnd ( );
      Regex regex = new Regex ( @"^.*?(?:\<dir\>\s+(?<dir>.*?))$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline );
      Match match = regex.Match ( data );
      while ( match.Success ) {
        string dir = match.Groups["dir"].Value;
        if ( !directories.Contains ( dir ) ) {
          directories.Add ( dir.Trim() );
        }
        match = match.NextMatch ( );
      }
      return directories;
    }

    public List<string> GetFiles ( ) {
      List<string> files = new List<string> ( );
      FtpWebRequest req = this.CreateFtpRequest ( this.GetFtpUri ( ) );
      req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
      FtpWebResponse response = req.GetResponse ( ) as FtpWebResponse;
      Stream responseStream = response.GetResponseStream ( );
      StreamReader reader = new StreamReader ( responseStream );
      string data = reader.ReadToEnd ( );
      Regex regex = new Regex ( @"^.*?(?:[AP]M\s+\d{1,}\s+)(?<file>.*?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline );
      Match match = regex.Match ( data );
      while ( match.Success ) {
        string file = match.Groups[ "file" ].Value;
        if ( !files.Contains ( file ) ) {
          files.Add ( file.Trim ( ) );
        }
        match = match.NextMatch ( );
      }
      return files;
    }

    protected override WebRequest GetWebRequest ( Uri address ) {
      return this.CreateFtpRequest(address);
    }

    protected override WebResponse GetWebResponse ( WebRequest request ) {
      return  request.GetResponse ( ) as FtpWebResponse;
    }

    protected override WebResponse GetWebResponse ( WebRequest request, IAsyncResult result ) {
      return base.GetWebResponse ( request, result ) as FtpWebResponse;
    }
  }
}
