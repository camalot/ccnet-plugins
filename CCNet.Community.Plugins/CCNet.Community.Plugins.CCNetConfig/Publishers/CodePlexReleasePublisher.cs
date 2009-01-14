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
using CCNetConfig.Core.Serialization;
using System.Xml;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using CCNetConfig.Core.Enums;
using System.Drawing.Design;
using System.IO;
using CCNet.Community.Plugins.CCNetConfig.Common;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	/// <summary>
	/// Publishes a release to codeplex.
	/// </summary>
	[Plugin, ReflectorName ( "codeplexRelease" ), MinimumVersion("1.2")]
	public class CodePlexReleasePublisher : PublisherTask, ICCNetDocumentation {
		/// <summary>
		/// Initializes a new instance of the <see cref="CodePlexReleasePublisher"/> class.
		/// </summary>
		public CodePlexReleasePublisher ()
			: base ( "codeplexRelease" ) {
			Password = new HiddenPassword ();
			Releases = new CloneableList<CodePlexReleaseItem> ();
		}

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
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[Category ( "Required" ), DisplayName ( "(Username)" ), DefaultValue ( null ),
		ReflectorName("username"), Required,
		Description ( "The username used to log in to CodePlex. This username must have access to create a release for the project." )]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[Category ( "Required" ), DisplayName ( "(Password)" ), DefaultValue ( null ),
		ReflectorName("password"), Required,
		Description ( "The password used to log in to CodePlex." ), TypeConverter ( typeof ( PasswordTypeConverter ) )]
		public HiddenPassword Password { get; set; }

		/// <summary>
		/// Gets or sets the releases.
		/// </summary>
		/// <value>The releases.</value>
		[Category ( "Required" ), 
		DisplayName ( "(Releases)" ),
		DefaultValue ( null ),
		ReflectorName("releases"), 
		Required,
		Description ( "The releases to create. At least 1 release is required." ), 
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<CodePlexReleaseItem> Releases { get; set; }

		/// <summary>
		/// Gets or sets the name of the project.
		/// </summary>
		/// <value>The name of the project.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName ( "projectName" ),
		Description ( "The CodePlex Project name. This is the url name of the project. If left empty, the CCNet Project name will be used as all lowercase. This is case sensitive." )]
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		[TypeConverter ( typeof ( ObjectOrNoneTypeConverter ) ), DefaultValue ( null ),
		Editor ( typeof ( ObjectOrNoneUIEditor ), typeof ( UITypeEditor ) ), ReflectorName ( "proxy" ),
		Category ( "Optional" ), Description ( "Proxy information." )]
		public Proxy Proxy { get; set; }
		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<CodePlexReleasePublisher> ().Serialize ( this );
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<CodePlexReleasePublisher> ().Deserialize ( element, this );

			/*if ( string.Compare ( element.Name, this.TypeName, false ) != 0 )
				throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.Name, this.TypeName ) );

			Util.ResetObjectProperties<CodePlexReleasePublisher> ( this );

			string s = Util.GetElementOrAttributeValue ( "username", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.Username = s;

			s = Util.GetElementOrAttributeValue ( "password", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.Password.Password = s;

			s = Util.GetElementOrAttributeValue ( "projectName", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.ProjectName = s;

			XmlElement rele = element.SelectSingleNode ( "releases" ) as XmlElement;
			if ( rele != null ) {
				foreach ( XmlElement re in rele.SelectNodes ( "release" ) ) {
					CodePlexReleaseItem ri = new CodePlexReleaseItem ();
					ri.Deserialize ( re );
					this.Releases.Add ( ri );
				}
			}*/

		}

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns></returns>
		public override PublisherTask Clone () {
			CodePlexReleasePublisher cprp = this.MemberwiseClone () as CodePlexReleasePublisher;
			cprp.Password = this.Password.Clone ();
			cprp.Releases = this.Releases.Clone ();
			return cprp;
		}

		#region ICCNetDocumentation Members

		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://ccnetplugins.codeplex.com/Wiki/Print.aspx?title=CodePlexReleasePublisher&version=7&action=Print" ); }
		}

		#endregion


	}
}
