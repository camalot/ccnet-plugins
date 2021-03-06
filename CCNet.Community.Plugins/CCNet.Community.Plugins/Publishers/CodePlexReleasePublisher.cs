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
using CCNet.Community.Plugins.Components.Macros;
using CCNet.Community.Plugins.Common;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CCNet.Community.Plugins.Publishers {
	/// <summary>
	/// Publishes the release on CodePlex using the CodePlex API
	/// </summary>
	[ReflectorType ( "codeplexRelease" )]
	public class CodePlexReleasePublisher : BasePublisherTask {
		/// <summary>
		/// Initializes a new instance of the <see cref="CodePlexReleasePublisher"/> class.
		/// </summary>
		public CodePlexReleasePublisher () {
			Releases = new List<ReleaseItem> ();
			this.Timeout = 60 * 3;
			this.ContinueOnFailure = false;
		}

		#region reflector properties

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		[ReflectorProperty("timeout",Required=false)]
		public int Timeout { get; set; }

		private ReleaseService ReleaseService { get; set; }
		/// <summary>
		/// Sets the username.
		/// </summary>
		/// <value>The username.</value>
		[ReflectorProperty ( "username", Required = true )]
		public string Username { get; set; }

		/// <summary>
		/// Sets the password.
		/// </summary>
		/// <value>The password.</value>
		[ReflectorProperty ( "password", Required = true )]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the name of the project.
		/// </summary>
		/// <value>The name of the project.</value>
		[ReflectorProperty ( "projectName", Required = false )]
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the releases.
		/// </summary>
		/// <value>The releases.</value>
		[ReflectorArray ( "releases", Required = true )]
		public List<ReleaseItem> Releases { get; set; }
		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		[ReflectorProperty ( "proxy", Required = false )]
		public Proxy Proxy { get; set; }
		/// <summary>
		/// Gets the modification comments.
		/// </summary>
		/// <value>The modification comments.</value>
		public string ModificationComments { get; private set; }

		#endregion
		#region ITask Members

		/// <summary>
		/// Runs the task.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <workitems>
		///		<workitem rel="3586">Update CodePlexReleasePublisher to support Dev Status</workitem>
		/// </workitems>
		public override void Run ( IIntegrationResult result ) {
			// only continue if the result was a success.
			if ( result.Status != ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success ) {
				Log.Info ( "CodePlexReleasePublisher skipped due to build status not met." );
				return;
			}

			// using a custom enum allows for supporting AllBuildConditions
			if ( this.BuildCondition != PublishBuildCondition.AllBuildConditions && string.Compare ( this.BuildCondition.ToString (), result.BuildCondition.ToString (), true ) != 0 ) {
				Log.Info ( "CodePlexReleasePublisher skipped due to build condition not met." );
				return;
			}

			// if the cert comes from microsoft, except it.
			System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate ( object sender,
					X509Certificate certificate, X509Chain chain,
					System.Net.Security.SslPolicyErrors sslPolicyErrors ) {
				if ( certificate.Issuer.ToLower ().Contains ( "microsoft" ) )
					return true;
				else
					return false;
			};

			this.ModificationComments = Util.GetModidicationCommentsString ( result );

			// loop each release item
			foreach ( ReleaseItem item in this.Releases ) {
				CodePlexReleaseTaskResult taskResult = new CodePlexReleaseTaskResult ( GetPropertyString<ReleaseItem> ( item, result, item.ReleaseName ) );
				try {
					if ( ( item.BuildCondition == PublishBuildCondition.ForceBuild &&
								result.BuildCondition != ThoughtWorks.CruiseControl.Remote.BuildCondition.ForceBuild ) ||
								item.BuildCondition == PublishBuildCondition.IfModificationExists &&
								result.BuildCondition != ThoughtWorks.CruiseControl.Remote.BuildCondition.IfModificationExists ) {
						ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( "Release Creation Skipped due to Build Condition not met." );
						continue;
					}

					this.ReleaseService = new ReleaseService ();
					this.ReleaseService.Timeout = this.Timeout * 1000;
					if ( this.Proxy != null )
						this.ReleaseService.Proxy = this.Proxy.CreateProxy ();
					string tProjectName = string.IsNullOrEmpty ( this.ProjectName ) ? result.ProjectName.ToLower ().Trim () : this.ProjectName;
					ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( string.Format ( "Creating release {1} for {0}",
						GetPropertyString<ReleaseItem> ( item, result, tProjectName ),
						GetPropertyString<ReleaseItem> ( item, result, item.ReleaseName ) ) );
					this.ReleaseService.Credentials = new NetworkCredential ( this.Username, this.Password );

					string releaseName = string.Format ( "{0}{1}",
						GetPropertyString<ReleaseItem> ( item, result, item.ReleaseName ),
						item.ReleaseType != ReleaseType.None ? string.Format ( " {0}", item.ReleaseType.ToString () ) : string.Empty );

					string status = item.Status.ToString ();
					if ( item.ReleaseType == ReleaseType.Alpha )
						status = ReleaseStatus.Alpha.ToString ();
					else if ( item.ReleaseType == ReleaseType.Beta || item.ReleaseType == ReleaseType.Nightly )
						status = ReleaseStatus.Beta.ToString ();
					else if ( item.ReleaseType == ReleaseType.Production )
						status = ReleaseStatus.Stable.ToString ();

					int releaseId = this.ReleaseService.CreateRelease (
						GetPropertyString<ReleaseItem> ( item, result, tProjectName ).ToLower ().Trim (),
						releaseName.Trim (),
						GetPropertyString<ReleaseItem> ( item, result, item.Description ),
						item.ReleaseDate.ToShortDateString (),
						status,
						item.ShowToPublic,
						item.ShowOnHomePage,
						item.IsDefaultRelease,
						this.Username.Trim (),
						this.Password.Trim ()
					);

					// set the release if in the task result.
					taskResult.ReleaseId = releaseId;
					if ( item.ReleaseType != ReleaseType.None )
						taskResult.ReleaseType = item.ReleaseType;

					//if ( item.Files == null || item.Files.Count == 0 )
					//  throw new ArgumentNullException ( "releaseFiles" );
					List<CodePlexApi.ReleaseFile> releaseFiles = new List<CodePlexApi.ReleaseFile> ();

					// loop the release files
					foreach ( ReleaseFile releaseFile in item.Files ) {
						CodePlexApi.ReleaseFile trf = new CodePlexApi.ReleaseFile ();
						trf.FileName = Path.GetFileName ( GetPropertyString<ReleaseFile> ( releaseFile, result, releaseFile.FileName ) );
						trf.Name = GetPropertyString<ReleaseFile> ( releaseFile, result, releaseFile.Name );
						trf.MimeType = GetPropertyString<ReleaseFile> ( releaseFile, result, releaseFile.MimeType );
						trf.FileType = releaseFile.FileType.ToString ();
						// get the file data
						byte[ ] fileData = releaseFile.GetFileData ( result );
						if ( fileData == null || fileData.Length == 0 )
							throw new ArgumentException ( "Invalid File Data", string.Format ( "releaseFile ('{0}')", releaseFile.FileName ) );
						trf.FileData = fileData;
						releaseFiles.Add ( trf );
					}
					// upload the release files if there are any.
					if ( item.Files.Count > 0 ) {
						this.ReleaseService.UploadReleaseFiles (
							GetPropertyString<ReleaseItem> ( item, result, tProjectName ).ToLower ().Trim (),
							releaseName.Trim (),
							releaseFiles.ToArray (),
							this.Username.Trim (),
							this.Password.Trim ()
						);
					}
				} catch ( Exception ex ) {
					// set the error
					taskResult.Exception = ex;
				}
				// add the task result
				result.AddTaskResult ( taskResult );
				if ( taskResult.Exception != null )
					if ( this.ContinueOnFailure ) {
						Log.Warning ( taskResult.Exception );
					} else {
						Log.Error ( taskResult.Exception );
						throw taskResult.Exception;
					}
			}
		#endregion
		}
	}
}
