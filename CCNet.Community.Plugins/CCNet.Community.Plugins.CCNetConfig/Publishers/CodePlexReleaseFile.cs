using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using CCNetConfig.Core;
using CCNetConfig.Core.Serialization;
using System.IO;
using System.Drawing.Design;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	
		/// <summary>
		/// Represents a file that will be added to the release.
		/// </summary>
	[ReflectorName("releaseFile")]
	public class CodePlexReleaseFile : ICCNetObject, ICloneable {
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
		public CodePlexReleaseFile () {

		}

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		[Category ( "Required" ), DisplayName ( "(FileName)" ), DefaultValue ( null ),
		ReflectorName ( "fileName" ), Required,
		Description ( "The full path to the file." ), Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
		OpenFileDialogTitle ( "Select the file to upload" ),
		ReflectorNodeType(ReflectorNodeTypes.Attribute),
		FileTypeFilter ( "Compressed Files|*.zip;*.7z;*.rar;*.jar;*.z;*.tar;*.gzip|All Files|*.*" )]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the type of the file.
		/// </summary>
		/// <value>The type of the file.</value>
		[Category ( "Required" ), DefaultValue ( CodePlexReleaseFile.FileType.RuntimeBinary ),
		ReflectorName ( "fileType" ), Required,
		ReflectorNodeType ( ReflectorNodeTypes.Attribute ),
		DisplayName ( "(FileType)" ), Description ( "The type of file this is." )]
		public CodePlexReleaseFile.FileType ReleaseFileType { get; set; }

		/// <summary>
		/// Gets or sets the type of the MIME.
		/// </summary>
		/// <value>The type of the MIME.</value>
		[Category ( "Optional" ), DefaultValue ( "application/octet-stream" ),
		ReflectorName ( "mimeType" ),
		ReflectorNodeType ( ReflectorNodeTypes.Attribute ),
		Description ( "The MIME type associated with the file. If not specified, the default value is application/octet-stream." )]
		public string MimeType { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName ( "name" ), Required,
		ReflectorNodeType ( ReflectorNodeTypes.Attribute ),
		Description ( "The display name associated with the file. If not specified, the FileName will be displayed." )]
		public string Name { get; set; }
		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public CodePlexReleaseFile Clone () {
			CodePlexReleaseFile rf = this.MemberwiseClone () as CodePlexReleaseFile;
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
			return new Serializer<CodePlexReleaseFile> ().Serialize ( this );
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void Deserialize ( System.Xml.XmlElement element ) {
			Util.ResetObjectProperties<CodePlexReleaseFile> ( this );

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
