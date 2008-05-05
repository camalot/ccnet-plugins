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
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;
using System.IO;
using System.Text.RegularExpressions;
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Ftp;

namespace CCNet.Community.Plugins.SourceControls {
  /// <summary>
  /// An FTP server source control provider
  /// </summary>
  [ReflectorType ( "ftp" )]
  public class FtpSourceControl : ISourceControl {

    public string RootFtpPath { get; private set; }
    /// <summary>
    /// Gets or sets the FTP server.
    /// </summary>
    /// <value>The FTP server.</value>
    [ReflectorProperty ( "server", Required = true )]
    public string FtpServer { get; set; }
    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>The port.</value>
    [ReflectorProperty ( "port", Required = false )]
    public int Port { get; set; }
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>The username.</value>
    [ReflectorProperty ( "username", Required = false )]
    public string Username { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorProperty ( "password", Required = false )]
    public string Password { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [auto get source].
    /// </summary>
    /// <value><c>true</c> if [auto get source]; otherwise, <c>false</c>.</value>
    [ReflectorProperty ( "autoGetSource", Required = false )]
    public bool AutoGetSource { get; set; }
    /// <summary>
    /// Gets or sets the repository root.
    /// </summary>
    /// <value>The repository root.</value>
    [ReflectorProperty ( "repositoryRoot", Required = false )]
    public string RepositoryRoot { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [use secured FTP].
    /// </summary>
    /// <value><c>true</c> if [use secured FTP]; otherwise, <c>false</c>.</value>
    [ReflectorProperty ( "useSsl", Required = false )]
    public bool UseSsl { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [use passive].
    /// </summary>
    /// <value><c>true</c> if [use passive]; otherwise, <c>false</c>.</value>
    [ReflectorProperty ( "usePassive", Required = false )]
    public bool UsePassive { get; set; }

    /// <summary>
    /// Gets or sets the timeout.
    /// </summary>
    /// <value>The timeout.</value>
    [ReflectorProperty ( "timeout", Required = false )]
    public int Timeout { get; set; }
    /// <summary>
    /// Gets or sets the proxy.
    /// </summary>
    /// <value>The proxy.</value>
    [ReflectorProperty ( "proxy", Required = false )]
    public Proxy Proxy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to clean the working directory before getting the source.
    /// </summary>
    /// <value><c>true</c> if should clean the working directory; otherwise, <c>false</c>.</value>
    [ReflectorProperty("cleanSource",Required=false)]
    public bool CleanSource { get; set; }

    /// <summary>
    /// Gets the FTP URL.
    /// </summary>
    /// <value>The FTP URL.</value>
    public Uri FtpUrl { get { return new Uri ( this.ToString ( ) ); } }

    /// <summary>
    /// Gets or sets the modifications.
    /// </summary>
    /// <value>The modifications.</value>
    public List<Modification> Modifications { get; private set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="FtpSourceControl"/> class.
    /// </summary>
    public FtpSourceControl ( ) {
      this.Port = 21;
      this.Username = string.Empty;
      this.Password = string.Empty;
      this.AutoGetSource = true;
      this.RepositoryRoot = "/";
      this.UseSsl = false;
      this.UsePassive = false;
      this.Timeout = 60000;
      this.CleanSource = false;
      this.Modifications = new List<Modification> ( );
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString ( ) {
      return new Uri ( string.Format ( "{4}{0}{1}{2}{3}{5}",
        this.FtpServer, this.Port != 21 ? ":" + this.Port : string.Empty,
        !this.RepositoryRoot.StartsWith ( "/" ) ? "/" : string.Empty, this.RepositoryRoot,
        !this.FtpServer.StartsWith ( "ftp://" ) && !this.UseSsl ? "ftp://" :
        !this.FtpServer.StartsWith ( "ftps://" ) && this.UseSsl ? "ftps://" :
        string.Empty, !this.RepositoryRoot.EndsWith ( "/" ) ? "/" : string.Empty ) ).ToString ( );
    }

    /// <summary>
    /// Creates the modification.
    /// </summary>
    /// <param name="info">The info.</param>
    /// <returns></returns>
    private Modification CreateModification ( FtpFileInfo info ) {
      Modification modification = new Modification ( );
      modification.FileName = info.Name;
      modification.ModifiedTime = info.LastModified;
      modification.Url = info.Url.ToString ( );
      modification.FolderName = info.Directory;
      return modification;
    }

    #region ISourceControl Members

    /// <summary>
    /// Gets the modifications.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <returns></returns>
    public Modification[ ] GetModifications ( IIntegrationResult from, IIntegrationResult to ) {
      return this.Modifications.ToArray ( );
    }

    /// <summary>
    /// Gets the source.
    /// </summary>
    /// <param name="result">The result.</param>
    public void GetSource ( IIntegrationResult result ) {
      if ( this.AutoGetSource ) {
        Uri ftpUrl = new Uri ( this.ToString ( ) );
        this.RootFtpPath = ftpUrl.AbsolutePath;
        DownloadFilesFromFtpServer ( result );
      }
    }

    /// <summary>
    /// Initializes the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    public void Initialize ( IProject project ) {
    }

    /// <summary>
    /// Labels the source control.
    /// </summary>
    /// <param name="result">The result.</param>
    public void LabelSourceControl ( IIntegrationResult result ) {
    }

    /// <summary>
    /// Purges the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    public void Purge ( IProject project ) {
    }

    #endregion

    private System.Net.NetworkCredential CreateCredentials ( ) {
      if ( !string.IsNullOrEmpty ( this.Username ) ) {
        return new System.Net.NetworkCredential ( this.Username, this.Password );
      } else {
        return null;
      }
    }

    private void CleanWorkingDirectory ( IIntegrationResult result ) {
      if ( this.CleanSource ) {
        Directory.Delete ( result.WorkingDirectory, true );
      }
    }

    /// <summary>
    /// Creates the FTP web request.
    /// </summary>
    /// <returns></returns>
    private FtpWebRequest CreateFtpWebRequest ( ) {
      FtpWebRequest req = new FtpWebRequest ( );
      if ( this.Proxy != null )
        req.Proxy = this.Proxy.CreateProxy ( );
      req.UsePassive = this.UsePassive;
      req.EnableSsl = this.UseSsl;
      System.Net.NetworkCredential creds = CreateCredentials ( );
      if ( creds != null ) {
        req.Credentials = creds;
      }
      //req.Timeout = this.Timeout;
      return req;
    }

    private void DownloadFilesFromFtpServer ( IIntegrationResult result ) {
      DownloadFilesRecursive ( this.FtpUrl, result );
    }

    private bool IsFileChanged ( FtpFileInfo file, DateTime date ) {
      return file.LastModified > date;
    }

    private void DownloadFilesRecursive ( Uri ftpUrl, IIntegrationResult result ) {
      FtpWebRequest req = this.CreateFtpWebRequest ( );
      List<FtpSystemInfo> siList = req.ListDirectory ( ftpUrl );
      foreach ( FtpSystemInfo fsi in siList ) {
        if ( fsi.IsFile ) {
          try {
            int start = ftpUrl.ToString ( ).IndexOf ( this.RootFtpPath ) + this.RootFtpPath.Length;
            string path = ftpUrl.ToString ( ).Substring ( start );
            if ( string.IsNullOrEmpty ( path ) )
              path = ".\\";
            if ( !path.EndsWith ( "\\" ) || !path.EndsWith ( "/" ) )
              path = path + "\\";
            DirectoryInfo saveDir = new DirectoryInfo ( result.BaseFromWorkingDirectory ( path ) );
            if ( !saveDir.Exists )
              saveDir.Create ( );
            FileInfo localFile = new FileInfo ( Path.Combine ( saveDir.FullName, fsi.Name ) );
            req.DownloadFile ( new Uri ( ftpUrl.ToString ( ) + fsi.Name ), localFile.FullName );
            if ( IsFileChanged ( fsi as FtpFileInfo, result.StartTime ) ) {
              this.Modifications.Add ( this.CreateModification ( fsi as FtpFileInfo ) );
            }
          } catch ( Exception ex ) {
            throw new Exception ( "\n" + ftpUrl.ToString ( ) + fsi.Name, ex );
          }
        } else {
          DownloadFilesRecursive ( new Uri ( ftpUrl.ToString ( ) + fsi.Name + "/" ), result );
        }
      }
    }
  }
}
