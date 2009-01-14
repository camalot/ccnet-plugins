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
		ReflectorNodeType ( ReflectorNodeTypes.Attribute ),
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
			new Serializer<CodePlexReleaseItem> ().Deserialize ( element, this );
			/*Util.ResetObjectProperties<CodePlexReleaseItem> ( this );

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
		*/
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
