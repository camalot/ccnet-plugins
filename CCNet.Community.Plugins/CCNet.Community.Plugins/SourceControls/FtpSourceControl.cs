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
using System.Net;
using System.Text.RegularExpressions;

namespace CCNet.Community.Plugins.SourceControls {
  [ReflectorType ( "ftp" )]
  public class FtpSourceControl : ISourceControl {
    private string _currentDirectory = string.Empty;
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
    [ReflectorProperty ( "useSecuredFtp", Required = false )]
    public bool UseSecuredFtp { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [use passive].
    /// </summary>
    /// <value><c>true</c> if [use passive]; otherwise, <c>false</c>.</value>
    [ReflectorProperty ( "usePassive", Required = false )]
    public bool UsePassive { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="FtpSourceControl"/> class.
    /// </summary>
    public FtpSourceControl ( ) {
      this.Port = 21;
      this.Username = string.Empty;
      this.Password = string.Empty;
      this.AutoGetSource = true;
      this.RepositoryRoot = "/";
      this.UseSecuredFtp = false;
      this.UsePassive = false;
    }

    public override string ToString ( ) {
      return new Uri ( string.Format ( "{4}{0}{1}{2}{3}{5}",
        this.FtpServer, this.Port != 21 ? ":" + this.Port : string.Empty,
        !this.RepositoryRoot.StartsWith("/") ? "/" : string.Empty, this.RepositoryRoot ,
        !this.FtpServer.StartsWith ( "ftp://" ) && !this.UseSecuredFtp ? "ftp://" : 
        !this.FtpServer.StartsWith ( "sftp://" ) && this.UseSecuredFtp  ? "sftp://" : 
        string.Empty,!this.RepositoryRoot.EndsWith("/") ? "/" : string.Empty) ).ToString ( );
    }

    /// <summary>
    /// Creates the modification.
    /// </summary>
    /// <param name="info">The info.</param>
    /// <returns></returns>
    private Modification CreateModification ( FileInfo info ) {
      Modification modification = new Modification ( );
      modification.FileName = info.Name;
      modification.ModifiedTime = info.LastWriteTime;
      modification.FolderName = info.DirectoryName;
      return modification;
    }

    #region ISourceControl Members

    public Modification[ ] GetModifications ( IIntegrationResult from, IIntegrationResult to ) {
      return null;
    }

    public void GetSource ( IIntegrationResult result ) {
      if ( this.AutoGetSource ) {
        FtpClient client = new FtpClient ( );
        client.FtpServer = this.FtpServer;
        client.Path = this.RepositoryRoot;
        client.Port = this.Port;
        client.UsePassive = this.UsePassive;
        client.Username = this.Username;
        client.Password = this.Password;
        
      }
    }

    private void DownloadFile ( string remotePath, IIntegrationResult result ) {
      /*FtpWebRequest req = this.CreateFtpRequest ( remotePath );
      //Specify we're downloading a file
      req.Method = WebRequestMethods.Ftp.DownloadFile;

      //initialize the Filestream we're using to create the downloaded file locally
      FileStream localfileStream = new FileStream ( Path.Combine ( result.WorkingDirectory, remotePath ), FileMode.Create, FileAccess.Write );

      //Get a reponse
      FtpWebResponse response = req.GetResponse ( ) as FtpWebResponse;
      Stream responseStream = response.GetResponseStream ( );
      using ( response ) {
        using ( responseStream ) {
          using ( localfileStream ) {
            //create the file
            byte[ ] buffer = new byte[ 1024 ];
            int bytesRead = responseStream.Read ( buffer, 0, 1024 );
            while ( bytesRead != 0 ) {
              localfileStream.Write ( buffer, 0, bytesRead );
              bytesRead = responseStream.Read ( buffer, 0, 1024 );
            }
          }
        }
      }*/
    }



    public void Initialize ( IProject project ) {
    }

    public void LabelSourceControl ( IIntegrationResult result ) {
    }

    public void Purge ( IProject project ) {
    }

    #endregion
  }
}
