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
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CCNet.Community.Plugins.CodePlexApi;

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "codeplexRelease" )]
  public class CodePlexReleasePublisher : ITask {
    private List<ReleaseItem> releases = null;
    private ReleaseService releaseService = null;
    private string projectName = string.Empty;
    private string userName = string.Empty;
    private string password = string.Empty;

    private string _modificationComments = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="CodePlexReleasePublisher"/> class.
    /// </summary>
    public CodePlexReleasePublisher ( ) {
      releases = new List<ReleaseItem> ( );
    }

    #region reflector properties
    /// <summary>
    /// Sets the username.
    /// </summary>
    /// <value>The username.</value>
    [ReflectorProperty ( "username", Required = true )]
    public string Username {
      get { return this.userName; }
      set { this.userName = value; }
    }

    /// <summary>
    /// Sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorProperty ( "password", Required = true )]
    public string Password {
      get { return this.password; }
      set { this.password = value; }
    }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    /// <value>The name of the project.</value>
    [ReflectorProperty ( "projectName", Required = false )]
    public string ProjectName { get { return this.projectName; } set { this.projectName = value; } }

    /// <summary>
    /// Gets or sets the releases.
    /// </summary>
    /// <value>The releases.</value>
    [ReflectorArray ( "releases", Required = true )]
    public List<ReleaseItem> Releases { get { return this.releases; } set { this.releases = value; } }

    /// <summary>
    /// Gets the modification comments.
    /// </summary>
    /// <value>The modification comments.</value>
    public string ModificationComments {
      get { return _modificationComments; }
    }

#endregion
    #region ITask Members

    /// <summary>
    /// Runs the task.
    /// </summary>
    /// <param name="result">The result.</param>
    public void Run ( IIntegrationResult result ) {
      // only continue if the result was a success.
      if ( result.Status != ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success )
        return;

      // if the cert comes from microsoft, except it.
      System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate ( object sender,
          X509Certificate certificate, X509Chain chain,
          System.Net.Security.SslPolicyErrors sslPolicyErrors ) {
        if ( certificate.Issuer.ToLower ( ).Contains ( "microsoft" ) )
          return true;
        else
          return false;
      };

      // loop each release item
      foreach ( ReleaseItem item in this.releases ) {
        CodePlexReleaseTaskResult taskResult = new CodePlexReleaseTaskResult ( GetPropertyString<ReleaseItem> ( item, result, item.ReleaseName ) );
        try {
          if ( ( item.BuildCondition == PublishBuildCondition.ForceBuild &&
                result.BuildCondition != ThoughtWorks.CruiseControl.Remote.BuildCondition.ForceBuild ) ||
                item.BuildCondition == PublishBuildCondition.IfModificationExists &&
                result.BuildCondition != ThoughtWorks.CruiseControl.Remote.BuildCondition.IfModificationExists ) {
            ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( "Release Creation Skipped due to Build Condition not met." );
            continue;
          }

          this.releaseService = new ReleaseService ( );
          string tProjectName = string.IsNullOrEmpty ( this.ProjectName ) ? result.ProjectName.ToLower ( ).Trim ( ) : this.ProjectName;
          ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( string.Format ( "Creating release {1} for {0}",
            GetPropertyString<ReleaseItem> ( item, result, tProjectName ),
            GetPropertyString<ReleaseItem> ( item, result, item.ReleaseName ) ) );
          this.releaseService.Credentials = new NetworkCredential ( this.userName, this.password );

          string releaseName = string.Format("{0}{1}", 
            GetPropertyString<ReleaseItem> ( item, result, item.ReleaseName ),
            item.ReleaseType != ReleaseType.None ? string.Format(" {0}",item.ReleaseType.ToString() ) : string.Empty);

          int releaseId = this.releaseService.CreateRelease (
            GetPropertyString<ReleaseItem> ( item, result, tProjectName ).ToLower ( ).Trim ( ),
            releaseName.Trim(),
            GetPropertyString<ReleaseItem> ( item, result, item.Description ),
            item.ReleaseDate.ToShortDateString ( ),
            item.Status.ToString ( ),
            item.ShowToPublic,
            item.ShowOnHomePage,
            item.IsDefaultRelease,
            this.Username.Trim ( ),
            this.Password.Trim ( )
          );

          // set the release if in the task result.
          taskResult.ReleaseId = releaseId;
          if ( item.ReleaseType != ReleaseType.None )
            taskResult.ReleaseType = item.ReleaseType;

          //if ( item.Files == null || item.Files.Count == 0 )
          //  throw new ArgumentNullException ( "releaseFiles" );
          List<CodePlexApi.ReleaseFile> releaseFiles = new List<CodePlexApi.ReleaseFile> ( );

          // loop the release files
          foreach ( ReleaseFile releaseFile in item.Files ) {
            CodePlexApi.ReleaseFile trf = new CodePlexApi.ReleaseFile ( );
            trf.FileName = Path.GetFileName ( GetPropertyString<ReleaseFile> ( releaseFile, result, releaseFile.FileName ) );
            trf.Name = GetPropertyString<ReleaseFile> ( releaseFile, result, releaseFile.Name );
            trf.MimeType = GetPropertyString<ReleaseFile> ( releaseFile, result, releaseFile.MimeType );
            trf.FileType = releaseFile.FileType.ToString ( );
            // get the file data
            byte[ ] fileData = releaseFile.GetFileData ( result );
            if ( fileData == null || fileData.Length == 0 )
              throw new ArgumentException ( "Invalid File Data", string.Format ( "releaseFile ('{0}')", releaseFile.FileName ) );
            trf.FileData = fileData;
            releaseFiles.Add ( trf );
          }
          // upload the release files if there are any.
          if ( item.Files.Count > 0 ) {
            this.releaseService.UploadReleaseFiles (
              GetPropertyString<ReleaseItem> ( item, result, tProjectName ).ToLower ( ).Trim ( ),
              releaseName.Trim ( ),
              releaseFiles.ToArray ( ),
              this.Username.Trim ( ),
              this.Password.Trim ( )
            );
          }
        } catch ( Exception ex ) {
          // set the error
          taskResult.Exception = ex; 
        }
        // add the task result
        result.AddTaskResult ( taskResult );
        if ( taskResult.Exception != null )
          throw taskResult.Exception;
      }
    }
    #endregion

    /// <summary>
    /// Reads the modidication comments.
    /// </summary>
    /// <param name="result">The result.</param>
    private void ReadModidicationComments ( IIntegrationResult result ) {
      DateTime lastDate = DateTime.MinValue;
      StringBuilder descText = new StringBuilder ( );
      foreach ( Modification mod in result.Modifications ) {
        if ( lastDate.CompareTo ( mod.ModifiedTime ) != 0 ) {
          lastDate = mod.ModifiedTime;
          if ( !string.IsNullOrEmpty ( mod.Comment ) ) {
            descText.AppendLine ( mod.Comment );
          }
        }
      }
      this._modificationComments = descText.ToString ( );
    }


    /// <summary>
    /// Gets the property string.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="result">The result.</param>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    private string GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
      string ret = Util.GetCCNetPropertyString<CodePlexReleasePublisher> ( this, result, input );
      ret = Util.GetCCNetPropertyString<T> ( sender, result, ret );
      return ret;
    }
  }
}
