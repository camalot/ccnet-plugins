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
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using CCNet.Community.Plugins.Components.Macros;
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Ftp;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CCNet.Community.Plugins.Publishers {
	/// <summary>
	/// An Ftp publisher.
	/// </summary>
	[ReflectorType ( "ftp" )]
	public class FtpPublisher : ITask, IMacroRunner {

		/// <summary>
		/// Initializes a new instance of the <see cref="FtpPublisher"/> class.
		/// </summary>
		public FtpPublisher () {
			this.MacroEngine = new MacroEngine ();
			this.Files = new List<string> ();
		}

		/// <summary>
		/// Gets or sets the files.
		/// </summary>
		/// <value>The files.</value>
		[ReflectorArray("files")]
		public List<string> Files { get; set; }

		/// <summary>
		/// Gets or sets the root FTP path.
		/// </summary>
		/// <value>The root FTP path.</value>
		private string RootFtpPath { get; set; }

		/// <summary>
		/// Gets or sets the repository root.
		/// </summary>
		/// <value>The repository root.</value>
		[ReflectorProperty ( "workingDirectory", Required = false )]
		public string WorkingDirectory { get; set; }

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
		[ReflectorProperty("proxy", Required=false)]
		public Proxy Proxy { get; set; }

		/// <summary>
		/// Gets the FTP URL.
		/// </summary>
		/// <value>The FTP URL.</value>
		public Uri FtpUrl { get { return new Uri ( this.ToString () ); } }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString () {
			return new Uri ( string.Format ( "{4}{0}{1}{2}{3}{5}",
				this.FtpServer, this.Port != 21 ? ":" + this.Port : string.Empty,
				!this.WorkingDirectory.StartsWith ( "/" ) ? "/" : string.Empty, this.WorkingDirectory,
				!this.FtpServer.StartsWith ( "ftp://" ) && !this.UseSsl ? "ftp://" :
				!this.FtpServer.StartsWith ( "ftps://" ) && this.UseSsl ? "ftps://" :
				string.Empty, !this.WorkingDirectory.EndsWith ( "/" ) ? "/" : string.Empty ) ).ToString ();
		}

		/// <summary>
		/// Creates the credentials.
		/// </summary>
		/// <returns></returns>
		private System.Net.NetworkCredential CreateCredentials () {
			if ( !string.IsNullOrEmpty ( this.Username ) ) {
				return new System.Net.NetworkCredential ( this.Username, this.Password );
			} else {
				return null;
			}
		}

		/// <summary>
		/// Creates the FTP web request.
		/// </summary>
		/// <returns></returns>
		private FtpWebRequest CreateFtpWebRequest () {
			FtpWebRequest req = new FtpWebRequest ();
			if ( this.Proxy != null )
				req.Proxy = this.Proxy.CreateProxy ();
			req.UsePassive = this.UsePassive;
			req.EnableSsl = this.UseSsl;
			System.Net.NetworkCredential creds = CreateCredentials ();
			if ( creds != null ) {
				req.Credentials = creds;
			}
			//req.Timeout = this.Timeout;
			return req;
		}

		#region ITask Members

		/// <summary>
		/// Runs the specified result.
		/// </summary>
		/// <param name="result">The result.</param>
		public void Run ( IIntegrationResult result ) {
			if ( result.Succeeded ) {
				FtpWebRequest req = this.CreateFtpWebRequest ();
				foreach ( string s in this.Files ) {
					FileInfo fi = new FileInfo ( s );
					if ( fi.Exists ) {
						try {
							req.UploadFile ( fi, this.FtpUrl );
						} catch ( Exception ex ) {
							Log.Error ( ex );
						}
					} else {
						Log.Warning ( string.Format ( "The file {0} was not found. File skipped.", fi.FullName ) );
					}
				}
			} else {
				Log.Debug ( "Build failed, Ftp process skipped" );
			}
		}

		#endregion

		#region IMacroRunner Members

		/// <summary>
		/// Gets the macro engine.
		/// </summary>
		/// <value>The macro engine.</value>
		public MacroEngine MacroEngine { get; private set; }

		/// <summary>
		/// Gets the property string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="result">The result.</param>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public string GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
			string ret = this.GetPropertyString<FtpPublisher> ( this, result, input );
			ret = this.GetPropertyString<T> ( sender, result, ret );
			return ret;
		}

		#endregion
	}
}
