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
