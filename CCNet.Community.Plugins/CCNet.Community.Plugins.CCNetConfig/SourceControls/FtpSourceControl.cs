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
 * 
 * 
 *    - CCNetConfig:
 *      - http://ccnetconfig.org
 *      - http://codeplex.com/ccnetconfig
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using CCNetConfig.Core;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using CCNetConfig.Core.Serialization;
using CCNet.Community.Plugins.CCNetConfig.Common;
using System.Drawing.Design;
using System.Xml;

namespace CCNet.Community.Plugins.CCNetConfig.SourceControls {
	/// <summary>
	/// This is an Ftp Source control. Allowing the source to be retrieved from an ftp server.
	/// </summary>
	[Plugin, MinimumVersion ( "1.2" )]
	public class FtpSourceControl : SourceControl, ICCNetDocumentation {
		/// <summary>
		/// Initializes a new instance of the <see cref="FtpSourceControl"/> class.
		/// </summary>
		public FtpSourceControl ()
			: base ( "ftp" ) {
			this.Proxy = null;
		}

		/// <summary>
		/// Gets or sets the server.
		/// </summary>
		/// <value>The server.</value>
		[Required, ReflectorName ( "server" ), DefaultValue ( null ), Category ( "Required" ),
		Description ( "The ftp Uri or just the host name." ), DisplayName ( "(Server)" )]
		public string Server { get; set; }
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		[ReflectorName ( "port" ), DefaultValue ( null ),
		Description ( "The ftp port." ), Category ( "Optional" ),
		Editor ( typeof ( NumericUpDownUIEditor ), typeof ( UITypeEditor ) )]
		public int? Port { get; set; }
		/// <summary>
		/// Gets or sets the repository root.
		/// </summary>
		/// <value>The repository root.</value>
		[ReflectorName ( "repositoryRoot" ), Category ( "Optional" ), DefaultValue ( null ),
		Description ( "The ftp path to start at." )]
		public string RepositoryRoot { get; set; }
		/// <summary>
		/// Gets or sets the use passive mode.
		/// </summary>
		/// <value>The use passive mode.</value>
		[Description ( "Use passive mode when connecting to ftp server." ),
		ReflectorName ( "usePassive" ), DefaultValue ( null ), Category ( "Optional" ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? UsePassiveMode { get; set; }
		/// <summary>
		/// Gets or sets the clean source.
		/// </summary>
		/// <value>The clean source.</value>
		[Description ( "If true, delete all files from the working directory before getting the source from the ftp server." ),
		ReflectorName ( "cleanSource" ), DefaultValue ( null ), Category ( "Optional" ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? CleanSource { get; set; }
		/// <summary>
		/// Gets or sets the use SSL.
		/// </summary>
		/// <value>The use SSL.</value>
		[Description ( "If true, use ftps (FTP over SSL) when connecting. SFTP ( FTP over SSH is not supported )." ),
		ReflectorName ( "useSsl" ), DefaultValue ( null ), Category ( "Optional" ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? UseSsl { get; set; }
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[ReflectorName ( "username" ), DefaultValue ( null ), Category ( "Optional" ),
		Description ( "The username to log in to the ftp server." )]
		public string Username { get; set; }
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[ReflectorName ( "password" ), DefaultValue ( null ), Category ( "Optional" ),
		Description ( "The password for logging in to the ftp server." ),
		TypeConverter ( typeof ( PasswordTypeConverter ) )]
		public HiddenPassword Password { get; set; }
		/// <summary>
		/// Gets or sets the auto get source.
		/// </summary>
		/// <value>The auto get source.</value>
		[Description ( "If true, the source will be downloaded automatically." ),
		ReflectorName ( "autoGetSource" ), DefaultValue ( null ), Category ( "Optional" ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? AutoGetSource { get; set; }
		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		[TypeConverter ( typeof ( ObjectOrNoneTypeConverter ) ), DefaultValue ( null ),
		Editor ( typeof ( ObjectOrNoneUIEditor ), typeof ( UITypeEditor ) ), ReflectorName ( "proxy" ),
		Category ( "Optional" ), Description ( "Proxy information." )]
		public Proxy Proxy { get; set; }

		/// <summary>
		/// Creates a copy of the source control object
		/// </summary>
		/// <returns></returns>
		public override SourceControl Clone () {
			FtpSourceControl fsc = this.MemberwiseClone () as FtpSourceControl;
			fsc.Proxy = this.Proxy.Clone ();
			fsc.Password = this.Password.Clone ();
			return fsc;
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			if ( string.Compare ( element.GetAttribute ( "type" ), this.TypeName, false ) != 0 )
				throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.GetAttribute ( "type" ), this.TypeName ) );

			Utils.ResetObjectProperties<FtpSourceControl> ( this );

			this.Server = Util.GetElementOrAttributeValue ( "server", element );

			string s = Util.GetElementOrAttributeValue ( "port", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				int t = -1;
				if ( int.TryParse ( s, out t ) ) {
					if ( t > 0 ) {
						this.Port = t;
					}
				}
			}

			s = Util.GetElementOrAttributeValue ( "repositoryRoot", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.RepositoryRoot = s;
			}

			s = Util.GetElementOrAttributeValue ( "usePassive", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.UsePassiveMode = string.Compare(bool.TrueString,s,true) == 0;
			}

			s = Util.GetElementOrAttributeValue ( "cleanSource", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.CleanSource = string.Compare ( bool.TrueString, s, true ) == 0;
			}

			s = Util.GetElementOrAttributeValue ( "useSsl", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.UseSsl = string.Compare ( bool.TrueString, s, true ) == 0;
			}

			s = Util.GetElementOrAttributeValue ( "autoGetSource", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.AutoGetSource = string.Compare ( bool.TrueString, s, true ) == 0;
			}

			s = Util.GetElementOrAttributeValue ( "username", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.Username = s;
			}

			s = Util.GetElementOrAttributeValue ( "password", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.Password.Password = s;
			}

			XmlElement proxyElement = element.SelectSingleNode ( "proxy" ) as XmlElement;
			if ( proxyElement != null ) {
				this.Proxy = new Proxy ();
				this.Proxy.Deserialize ( proxyElement );
			}

		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<FtpSourceControl> ().Serialize ( this );
		}

		#region ICCNetDocumentation Members

		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/Print.aspx?title=FtpSourceControl" ); }
		}

		#endregion
	}
}
