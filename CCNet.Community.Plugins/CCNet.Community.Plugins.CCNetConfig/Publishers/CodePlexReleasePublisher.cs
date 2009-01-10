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
			Releases = new CloneableList<ReleaseItem> ();
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
		[Category ( "Required" ), DisplayName ( "(Releases)" ), DefaultValue ( null ),
		ReflectorName("releases"), Required,
		ReflectorArray("release"),
		Description ( "The releases to create. At least 1 release is required." ), TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<ReleaseItem> Releases { get; set; }

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
					ReleaseItem ri = new ReleaseItem ();
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
			get { return new Uri ( "http://codeplex.com/cprp/" ); }
		}

		#endregion



		/// <summary>
		/// Represents a new release that is going to be created.
		/// </summary>
		[ReflectorName("release")]
		public class ReleaseItem : ICCNetObject, ICloneable, ISerialize {
			/// <summary>
			/// The status of the release.
			/// </summary>
			public enum ReleaseStatus {
				/// <summary>
				/// The release is a planned release.
				/// </summary>
				Planned,
				/// <summary>
				/// The release is released (stable?)
				/// </summary>
				Released,
				/// <summary>
				/// The release is in the planning stages
				/// </summary>
				Planning,
				/// <summary>
				/// The release is an alpha
				/// </summary>
				Alpha,
				/// <summary>
				/// The release is a beta
				/// </summary>
				Beta,
				/// <summary>
				/// The release is a stable release
				/// </summary>
				Stable,
			}

			/// <summary>
			/// The type of release
			/// </summary>
			public enum ReleaseType {
				/// <summary>
				/// An alpha release
				/// </summary>
				Alpha,
				/// <summary>
				/// A beta release
				/// </summary>
				Beta,
				/// <summary>
				/// A nightly release
				/// </summary>
				Nightly,
				/// <summary>
				/// A production release
				/// </summary>
				Production
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ReleaseItem"/> class.
			/// </summary>
			public ReleaseItem () {
				this.Files = new CloneableList<ReleaseFile> ();
			}

			/// <summary>
			/// Gets or sets the name of the release.
			/// </summary>
			/// <value>The name of the release.</value>
			[Category ( "Required" ), DefaultValue ( null ), DisplayName ( "(ReleaseName)" ),
			ReflectorName("releaseName"), Required,
			Description ( "The name of the release. This MUST be a unique name." )]
			public string ReleaseName { get; set; }

			/// <summary>
			/// Gets or sets the description.
			/// </summary>
			/// <value>The description.</value>
			[Category ( "Required" ), DefaultValue ( null ), DisplayName ( "(Description)" ),
			Description ( "A description of the release." ), MaximumStringLength ( 4000 ),
			ReflectorName ( "description" ), Required,
			Editor ( typeof ( MultilineStringUIEditor ), typeof ( UITypeEditor ) )]
			public string Description { get; set; }

			/// <summary>
			/// Gets or sets the build condition.
			/// </summary>
			/// <value>The build condition.</value>
			[Category ( "Optional" ), DefaultValue ( null ), Editor ( typeof ( DefaultableEnumUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DefaultableEnumTypeConverter ) ),
			ReflectorName("buildCondition"),
			Description ( "The build condition in which a publish should take place." )]
			public PublishBuildCondition? BuildCondition { get; set; }

			/// <summary>
			/// Gets or sets the files.
			/// </summary>
			/// <value>The files.</value>
			[Category ( "Optional" ), DefaultValue ( null ),
			Description ( "The files to add to the release." ),
			ReflectorName ( "releaseFiles" ), ReflectorArray ( "releaseFile" ),
			TypeConverter ( typeof ( IListTypeConverter ) )]
			public CloneableList<ReleaseFile> Files { get; set; }

			/// <summary>
			/// Gets or sets the status.
			/// </summary>
			/// <value>The status.</value>
			[Category ( "Optional" ), DefaultValue ( null ), Editor ( typeof ( DefaultableEnumUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DefaultableEnumTypeConverter ) ),
			ReflectorName("releaseStatus"),
			Description ( "The status of the release. The default is 'Planned'." )]
			public ReleaseStatus? Status { get; set; }

			/// <summary>
			/// Gets or sets the release date.
			/// </summary>
			/// <value>The release date.</value>
			[Category ( "Optional" ), DefaultValue ( null ),
			ReflectorName("releaseDate"), FormatProvider("M/d/yyyy"),
			Description ( "If the release status is 'Released', this is the date the release will be or was released." ),
			Editor ( typeof ( DatePickerUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DateTimeConverter ) )]
			public DateTime? ReleaseDate { get; set; }

			/// <summary>
			/// Gets or sets the is default release.
			/// </summary>
			/// <value>The is default release.</value>
			[Category ( "Optional" ), DefaultValue ( null ),
			ReflectorName("isDefaultRelease"),
			Description ( "If true, this release will be set as the default release. The default is true." ),
			Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
			public bool? IsDefaultRelease { get; set; }

			/// <summary>
			/// Gets or sets the show on home page.
			/// </summary>
			/// <value>The show on home page.</value>
			[Category ( "Optional" ), DefaultValue ( null ),
			ReflectorName("showOnHomePage"),
			Description ( "If true, this release will be shown on the home page. The default is true." ),
			Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
			public bool? ShowOnHomePage { get; set; }

			/// <summary>
			/// Gets or sets the show to public.
			/// </summary>
			/// <value>The show to public.</value>
			[Category ( "Optional" ), DefaultValue ( null ),
			ReflectorName("showToPublic"),
			Description ( "If true, this release will be shown to the public. The default is true." ),
			Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
			public bool? ShowToPublic { get; set; }

			/// <summary>
			/// Gets or sets the name of the release type.
			/// </summary>
			/// <value>The name of the release type.</value>
			[Category ( "Optional" ), DefaultValue ( null ), DisplayName ( "ReleaseType" ),
			ReflectorName("releaseType"),
			Description ( "The type of the release. This was added to supply additional information in the release name." ),
			Editor ( typeof ( DefaultableEnumUIEditor ), typeof ( UITypeEditor ) ),
			TypeConverter ( typeof ( DefaultableEnumTypeConverter ) )]
			public ReleaseType? ReleaseTypeName { get; set; }
			#region ICloneable Members

			/// <summary>
			/// Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			/// A new object that is a copy of this instance.
			/// </returns>
			public ReleaseItem Clone () {
				ReleaseItem ri = this.MemberwiseClone () as ReleaseItem;
				ri.ReleaseDate = this.ReleaseDate;
				ri.Status = this.Status;
				ri.Files = this.Files;
				ri.ReleaseTypeName = this.ReleaseTypeName;
				ri.BuildCondition = this.BuildCondition;
				return ri;
			}


			/// <summary>
			/// Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			/// A new object that is a copy of this instance.
			/// </returns>
			object ICloneable.Clone () {
				return this.Clone ();
			}

			#endregion

			#region ISerialize Members

			/// <summary>
			/// Serializes this instance.
			/// </summary>
			/// <returns></returns>
			public System.Xml.XmlElement Serialize () {
				return new Serializer<ReleaseItem> ().Serialize ( this );
			}

			/// <summary>
			/// Deserializes the specified element.
			/// </summary>
			/// <param name="element">The element.</param>
			public void Deserialize ( System.Xml.XmlElement element ) {
				Util.ResetObjectProperties<ReleaseItem> ( this );

				string s = Util.GetElementOrAttributeValue ( "releaseName", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.ReleaseName = s;

				s = Util.GetElementOrAttributeValue ( "description", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.Description = s;

				s = Util.GetElementOrAttributeValue ( "buildCondition", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.BuildCondition = (PublishBuildCondition)Enum.Parse ( typeof ( PublishBuildCondition ), s, true );

				XmlElement rele = element.SelectSingleNode ( "releaseFiles" ) as XmlElement;
				if ( rele != null ) {
					foreach ( XmlElement re in rele.SelectNodes ( "releaseFile" ) ) {
						ReleaseFile rf = new ReleaseFile ();
						rf.Deserialize ( re );
						this.Files.Add ( rf );
					}
				}

				s = Util.GetElementOrAttributeValue ( "releaseStatus", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.Status = (ReleaseStatus)Enum.Parse ( typeof ( ReleaseStatus ), s, true );

				s = Util.GetElementOrAttributeValue ( "releaseType", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.ReleaseTypeName = (ReleaseType)Enum.Parse ( typeof ( ReleaseType ), s, true );

				s = Util.GetElementOrAttributeValue ( "releaseDate", element );
				if ( !string.IsNullOrEmpty ( s ) ) {
					DateTime date = DateTime.Now;
					if ( DateTime.TryParse ( s, out date ) )
						this.ReleaseDate = date;
				}

				s = Util.GetElementOrAttributeValue ( "isDefaultRelease", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.IsDefaultRelease = string.Compare ( s, bool.TrueString, true ) == 0;

				s = Util.GetElementOrAttributeValue ( "showOnHomePage", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.ShowOnHomePage = string.Compare ( s, bool.TrueString, true ) == 0;

				s = Util.GetElementOrAttributeValue ( "showToPublic", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.ShowToPublic = string.Compare ( s, bool.TrueString, true ) == 0;
			}

			#endregion

			/// <summary>
			/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </returns>
			public override string ToString () {
				return string.IsNullOrEmpty ( this.ReleaseName ) ? this.GetType ().Name : this.ReleaseName;
			}
		}

		/// <summary>
		/// Represents a file that will be added to the release.
		/// </summary>
		public class ReleaseFile : ICCNetObject, ICloneable, ISerialize {
			/// <summary>
			/// The types of files that can be uploaded.
			/// </summary>
			public enum FileType {
				/// <summary>
				/// A binary file
				/// </summary>
				RuntimeBinary,
				/// <summary>
				/// Source code file
				/// </summary>
				SourceCode,
				/// <summary>
				/// Documentation file
				/// </summary>
				Documentation,
				/// <summary>
				/// Example file
				/// </summary>
				Example,
			}
			/// <summary>
			/// Initializes a new instance of the <see cref="ReleaseFile"/> class.
			/// </summary>
			public ReleaseFile () {

			}

			/// <summary>
			/// Gets or sets the name of the file.
			/// </summary>
			/// <value>The name of the file.</value>
			[Category ( "Required" ), DisplayName ( "(FileName)" ), DefaultValue ( null ),
			ReflectorName ( "fileName" ), Required,
			Description ( "The full path to the file." ), Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
			OpenFileDialogTitle ( "Select the file to upload" ),
			FileTypeFilter ( "Compressed Files|*.zip;*.7z;*.rar;*.jar;*.z;*.tar;*.gzip|All Files|*.*" )]
			public string FileName { get; set; }

			/// <summary>
			/// Gets or sets the type of the file.
			/// </summary>
			/// <value>The type of the file.</value>
			[Category ( "Required" ), DefaultValue ( CodePlexReleasePublisher.ReleaseFile.FileType.RuntimeBinary ),
			ReflectorName ( "fileType" ), Required,
			DisplayName ( "(FileType)" ), Description ( "The type of file this is." )]
			public ReleaseFile.FileType ReleaseFileType { get; set; }

			/// <summary>
			/// Gets or sets the type of the MIME.
			/// </summary>
			/// <value>The type of the MIME.</value>
			[Category ( "Optional" ), DefaultValue ( "application/octet-stream" ),
			ReflectorName ( "mimeType" ),
			Description ( "The MIME type associated with the file. If not specified, the default value is application/octet-stream." )]
			public string MimeType { get; set; }

			/// <summary>
			/// Gets or sets the name.
			/// </summary>
			/// <value>The name.</value>
			[Category ( "Optional" ), DefaultValue ( null ),
			ReflectorName ( "name" ), Required,
			Description ( "The display name associated with the file. If not specified, the FileName will be displayed." )]
			public string Name { get; set; }
			#region ICloneable Members

			/// <summary>
			/// Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			/// A new object that is a copy of this instance.
			/// </returns>
			public ReleaseFile Clone () {
				ReleaseFile rf = this.MemberwiseClone () as ReleaseFile;
				rf.ReleaseFileType = this.ReleaseFileType;
				return rf;
			}


			/// <summary>
			/// Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			/// A new object that is a copy of this instance.
			/// </returns>
			object ICloneable.Clone () {
				return this.Clone ();
			}

			#endregion

			#region ISerialize Members

			/// <summary>
			/// Serializes this instance.
			/// </summary>
			/// <returns></returns>
			public System.Xml.XmlElement Serialize () {
				return new Serializer<ReleaseFile> ().Serialize ( this );
			}

			/// <summary>
			/// Deserializes the specified element.
			/// </summary>
			/// <param name="element">The element.</param>
			public void Deserialize ( System.Xml.XmlElement element ) {
				Util.ResetObjectProperties<ReleaseFile> ( this );

				string s = Util.GetElementOrAttributeValue ( "fileName", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.FileName = s;

				s = Util.GetElementOrAttributeValue ( "fileType", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.ReleaseFileType = (FileType)Enum.Parse ( typeof ( FileType ), s, true );

				s = Util.GetElementOrAttributeValue ( "mimeType", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.MimeType = s;

				s = Util.GetElementOrAttributeValue ( "name", element );
				if ( !string.IsNullOrEmpty ( s ) )
					this.Name = s;
			}

			#endregion

			/// <summary>
			/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </returns>
			public override string ToString () {
				return string.IsNullOrEmpty ( this.Name ) ? Path.GetFileName ( this.FileName ) : this.Name;
			}
		}
	}
}
