using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using CCNetConfig.Core;
using CCNetConfig.Core.Serialization;
using CCNetConfig.Core.Enums;
using System.Xml;
using System.Drawing.Design;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	/// <summary>
	/// Represents a new release that is going to be created.
	/// </summary>
	[ReflectorName ( "release" )]
	public class CodePlexReleaseItem : ICCNetObject, ICloneable {
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
		public CodePlexReleaseItem () {
			this.Files = new CloneableList<CodePlexReleaseFile> ();
		}

		/// <summary>
		/// Gets or sets the name of the release.
		/// </summary>
		/// <value>The name of the release.</value>
		[Category ( "Required" ), DefaultValue ( null ), DisplayName ( "(ReleaseName)" ),
		ReflectorName ( "releaseName" ), Required,
		ReflectorNodeType(ReflectorNodeTypes.Attribute),
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
		ReflectorName ( "buildCondition" ),
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
		public CloneableList<CodePlexReleaseFile> Files { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>The status.</value>
		[Category ( "Optional" ), DefaultValue ( null ), Editor ( typeof ( DefaultableEnumUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableEnumTypeConverter ) ),
		ReflectorName ( "releaseStatus" ),
		Description ( "The status of the release. The default is 'Planned'." )]
		public ReleaseStatus? Status { get; set; }

		/// <summary>
		/// Gets or sets the release date.
		/// </summary>
		/// <value>The release date.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName ( "releaseDate" ), FormatProvider ( "M/d/yyyy" ),
		Description ( "If the release status is 'Released', this is the date the release will be or was released." ),
		Editor ( typeof ( DatePickerUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DateTimeConverter ) )]
		public DateTime? ReleaseDate { get; set; }

		/// <summary>
		/// Gets or sets the is default release.
		/// </summary>
		/// <value>The is default release.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName ( "isDefaultRelease" ),
		Description ( "If true, this release will be set as the default release. The default is true." ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? IsDefaultRelease { get; set; }

		/// <summary>
		/// Gets or sets the show on home page.
		/// </summary>
		/// <value>The show on home page.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName ( "showOnHomePage" ),
		Description ( "If true, this release will be shown on the home page. The default is true." ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? ShowOnHomePage { get; set; }

		/// <summary>
		/// Gets or sets the show to public.
		/// </summary>
		/// <value>The show to public.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName ( "showToPublic" ),
		Description ( "If true, this release will be shown to the public. The default is true." ),
		Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
		public bool? ShowToPublic { get; set; }

		/// <summary>
		/// Gets or sets the name of the release type.
		/// </summary>
		/// <value>The name of the release type.</value>
		[Category ( "Optional" ), DefaultValue ( null ), DisplayName ( "ReleaseType" ),
		ReflectorName ( "releaseType" ),
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
		public CodePlexReleaseItem Clone () {
			CodePlexReleaseItem ri = this.MemberwiseClone () as CodePlexReleaseItem;
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
			return new Serializer<CodePlexReleaseItem> ().Serialize ( this );
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void Deserialize ( System.Xml.XmlElement element ) {
			Util.ResetObjectProperties<CodePlexReleaseItem> ( this );

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
					CodePlexReleaseFile rf = new CodePlexReleaseFile ();
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
}
