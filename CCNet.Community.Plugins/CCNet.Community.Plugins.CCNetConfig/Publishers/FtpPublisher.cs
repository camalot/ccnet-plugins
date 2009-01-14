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
using CCNetConfig.Core.Enums;
using CCNet.Community.Plugins.CCNetConfig.Common;
using System.Drawing.Design;
using CCNet.Community.Plugins.CCNetConfig.Publishers.Ftp;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	/// <summary>
	/// Publishes specified files to an ftp server.
	/// </summary>
	[ReflectorName("ftpPublisher"), Plugin,
	Description ( "Publishes specified files to an ftp server." )]
	public class FtpPublisher : PublisherTask, ICCNetDocumentation {

		/// <summary>
		/// Initializes a new instance of the <see cref="FtpPublisher"/> class.
		/// </summary>
		public FtpPublisher () : base("ftpPublisher") {
			this.Password = new HiddenPassword ();
			this.Files = new CloneableList<FtpFile> ();
		}

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[Description ( "The ftp user account" ), ReflectorName ( "username" ),
		Category("Optional"), DefaultValue(null)]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[Description ( "The ftp account password" ), ReflectorName ( "password" ),
		Category ( "Optional" ), DefaultValue ( null ),TypeConverter ( typeof ( PasswordTypeConverter ) )]
		public HiddenPassword Password { get; set; }

		/// <summary>
		/// Gets or sets the continue on failure.
		/// </summary>
		/// <value>The continue on failure.</value>
		[Description ( "If true, the build will not fail if this publisher fails" ),
		ReflectorName ( "continueOnFailure" ), DefaultValue ( null ), Category ( "Required" ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		DisplayName("(Server)"),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public string Server { get; set; }

		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		[ReflectorName ( "port" ), DefaultValue ( null ),
		MinimumValue(0), MaximumValue(Int16.MaxValue),
		Description ( "The ftp port" ), Category ( "Optional" ),
		Editor ( typeof ( NumericUpDownUIEditor ), typeof ( UITypeEditor ) )]
		public int? Port { get; set; }

		/// <summary>
		/// Gets or sets the use passive.
		/// </summary>
		/// <value>The use passive.</value>
		[DefaultValue ( null ), ReflectorName ( "usePassive" ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		Description ( "If true, then passive mode will be used" ),
		Category ( "Optional" )]
		public bool? UsePassive { get; set; }

		/// <summary>
		/// Gets or sets the use SSL.
		/// </summary>
		/// <value>The use SSL.</value>
		[DefaultValue(null), ReflectorName("useSsl"),
		TypeConverter(typeof(DefaultableBooleanTypeConverter)),
		Editor(typeof(DefaultableBooleanUIEditor),typeof(UITypeEditor)),
		Description("If true, then an SSL connection will be used to connect to the ftp server"),
		Category("Optional")]
		public bool? UseSsl { get; set; }

		/// <summary>
		/// Gets or sets the working directory.
		/// </summary>
		/// <value>The working directory.</value>
		[Description ( "The path on the ftp server to upload the files to" ),
		ReflectorName("workingDirectory"), Category("Optional"),
		DefaultValue(null)]
		public string WorkingDirectory { get; set; }

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		[Description("The number of seconds before the publisher times out"),
		ReflectorName("timeout"),Category("Optional"),DefaultValue(null),
		Editor(typeof(NumericUpDownUIEditor),typeof(UITypeEditor)),
		MinimumValue(0),MaximumValue(Int16.MaxValue)]
		public int? Timeout { get; set; }

		/// <summary>
		/// Gets or sets the files.
		/// </summary>
		/// <value>The files.</value>
		[Description("A collection of files to upload"), ReflectorName("files"), DefaultValue(null),
		ReflectorArray("file"),Category("Required"),Required, TypeConverter(typeof(IListTypeConverter))]
		public CloneableList<FtpFile> Files { get; set; }

		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		[TypeConverter ( typeof ( ObjectOrNoneTypeConverter ) ), DefaultValue ( null ),
		NullOrObject,
		Editor ( typeof ( ObjectOrNoneUIEditor ), typeof ( UITypeEditor ) ), ReflectorName ( "proxy" ),
		Category ( "Optional" ), Description ( "Proxy information." )]
		public Proxy Proxy { get; set; }
		
		/// <summary>
		/// Gets or sets the continue on failure.
		/// </summary>
		/// <value>The continue on failure.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		Description ( "Indicates if the build process should continue even if this task fails" ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ), ReflectorName ( "continueOnFailure" )]
		public bool? ContinueOnFailure { get; set; }

		/// <summary>
		/// Gets or sets the build condition.
		/// </summary>
		/// <value>The build condition.</value>
		[Editor ( typeof ( DefaultableEnumUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableEnumTypeConverter ) ),
		ReflectorName ( "buildCondition" ),
		Description ( "The build condition in which the info should be added to the feed." ),
		DefaultValue ( null ), Category ( "Optional" )]
		public PublishBuildCondition? BuildCondition { get; set; }

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns></returns>
		public override PublisherTask Clone () {
			FtpPublisher fp = this.MemberwiseClone () as FtpPublisher;
			fp.Password = this.Password.Clone ();
			fp.Proxy = this.Proxy.Clone ();
			return fp;
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<FtpPublisher> ().Deserialize ( element, this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<FtpPublisher> ().Serialize ( this );
		}

		#region ICCNetDocumentation Members

		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://ccnetplugins.codeplex.com/Wiki/Print.aspx?title=FtpPublisher" ); }
		}

		#endregion
	}
}
