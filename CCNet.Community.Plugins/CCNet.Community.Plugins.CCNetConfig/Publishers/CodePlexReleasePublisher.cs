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
	[Plugin, ReflectorName ( "codeplexRelease" ), MinimumVersion("1.2")]
	public class CodePlexReleasePublisher : PublisherTask{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodePlexReleasePublisher"/> class.
		/// </summary>
		public CodePlexReleasePublisher ()
			: base ( "codeplexRelease" ) {
			Password = new HiddenPassword ();
			Releases = new CloneableList<CodePlexReleaseItem> ();
		}

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
			if ( string.Compare ( element.Name, this.TypeName, false ) != 0 )
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
			}

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
