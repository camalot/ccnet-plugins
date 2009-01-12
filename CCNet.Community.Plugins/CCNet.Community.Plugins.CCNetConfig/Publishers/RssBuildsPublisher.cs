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
using CCNetConfig.Core.Components;
using CCNetConfig.Core;
using System.ComponentModel;
using CCNetConfig.Core.Serialization;
using System.Drawing.Design;
using CCNetConfig.Core.Enums;
using CCNet.Community.Plugins.CCNetConfig.Common;
using System.Xml;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	/// <summary>
	/// A publisher that takes successful builds and generates a rolling rss feed.
	/// </summary>
	[Plugin, MinimumVersion ( "1.2.1" ), ReflectorName ( "rssBuilds" )]
	public class RssBuildsPublisher : PublisherTask, ICCNetDocumentation {
		private string _encoding = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="RssBuildsPublisher"/> class.
		/// </summary>
		public RssBuildsPublisher ()
			: base ( "rssBuilds" ) {
			this.Categories = new CloneableList<Category> ();
			this.FeedElements = new CloneableList<RssBuildsElement> ();
			this.ItemElements = new CloneableList<RssBuildsElement> ();
			this.Namespaces = new CloneableList<RssBuildsNamespace> ();
			this.PingItems = new CloneableList<RssBuildsPingItem> ();
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
		/// Gets or sets the output path.
		/// </summary>
		/// <value>The output path.</value>
		[BrowseForFolderDescription ( "Select output path." ), Editor ( typeof ( BrowseForFolderUIEditor ), typeof ( UITypeEditor ) ),
		DefaultValue ( null ), Category ( "Optional" ),
		ReflectorName ( "outputPath" ),
		Description ( "The path to output the rss file. This can be an absolute path or a relative path based on the artifact directory" )]
		public string OutputPath { get; set; }

		/// <summary>
		/// Gets or sets the encoding.
		/// </summary>
		/// <value>The encoding.</value>
		[Description ( "The encoding used in the rss document. The default is \"UTF-8\"" ), 
		DefaultValue ( null ), Category ( "Optional" ),
		ReflectorName("encoding")]
		public string Encoding {
			get { return this._encoding; }
			set {
				if ( string.IsNullOrEmpty ( value ) ) {
					this._encoding = value;
				} else {
					Encoding enc = System.Text.Encoding.GetEncoding ( value );
					if ( enc != null )
						this._encoding = enc.WebName;
					else
						throw new ArgumentException ( "Encoding not found." );
				}
			}
		}

		/// <summary>
		/// Gets or sets the channel URL.
		/// </summary>
		/// <value>The channel URL.</value>
		[Description ( "The format string used to create the url for the channel." ),
		ReflectorName("channelUrl"),
		DefaultValue ( null ), Category ( "Optional" )]
		public string ChannelUrl { get; set; }

		/// <summary>
		/// The max number of items to keep in the main feed, older items will be moved to the
		/// history feed. The default is 25
		/// </summary>
		/// <value>The max history.</value>
		[Description ( "The max number of items to keep in the main feed, older items will be moved to the history feed. The default is 25" ),
		DefaultValue ( null ), Category ( "Optional" ), ReflectorName("maxHistory"),
		Editor(typeof(NumericUpDownUIEditor),typeof(UITypeEditor)), MinimumValue(5), MaximumValue(100)]
		public int? MaxHistory { get; set; }

		/// <summary>
		/// The format string used to create the url for each item.
		/// </summary>
		/// <value>The item URL.</value>
		[Description ( "The format string used to create the url for each item." ),
		ReflectorName("itemUrl"),
		DefaultValue ( null ), Category ( "Optional" )]
		public string ItemUrl { get; set; }

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		[Description ( "The file name (without the extension) used for the default feed. The history feed will be the same with '.history' appended." ), DefaultValue ( null ), 
		ReflectorName("fileName"), Category ( "Optional" )]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the enclosure URL.
		/// </summary>
		/// <value>The enclosure URL.</value>
		[Description ( "This is the format string used to generate the enclosure url." ),
		ReflectorName("enclosureUrl"), DefaultValue ( null ), Category ( "Optional" )]
		public string EnclosureUrl { get; set; }

		[Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ), 
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) ),
		ReflectorName("addEnclosure"),
		Description ( "If true, then an enclosure will be added to the feed using the EnclosureUrl" ), 
		DefaultValue ( null ), Category ( "Optional" )]
		public bool? AddEnclosure { get; set; }

		/// <summary>
		/// Gets or sets the feed elements.
		/// </summary>
		/// <value>The feed elements.</value>
		[Category ( "Optional" ), Description ( "A collection of elements that are added to the feed channel." ), 
		ReflectorName("feedElements"), DefaultValue ( null ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<RssBuildsElement> FeedElements { get; set; }

		/// <summary>
		/// Gets or sets the item elements.
		/// </summary>
		/// <value>The item elements.</value>
		[Category ( "Optional" ), Description ( "A collection of elements that are added to the feed items." ), 
		ReflectorName("itemElements"),DefaultValue ( null ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<RssBuildsElement> ItemElements { get; set; }

		/// <summary>
		/// Gets or sets the ping items.
		/// </summary>
		/// <value>The ping items.</value>
		[Category ( "Optional" ), Description ( "A collection of hosts to notify when the feed is updated." ), 
		ReflectorName("pingItems"), DefaultValue ( null ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<RssBuildsPingItem> PingItems { get; set; }

		/// <summary>
		/// Gets or sets the namespaces.
		/// </summary>
		/// <value>The namespaces.</value>
		[Category ( "Optional" ), Description ( "A collection of namespaces that are added to the feed." ), 
		ReflectorName("rssExtensions"), DefaultValue ( null ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<RssBuildsNamespace> Namespaces { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>The categories.</value>
		[Category ( "Optional" ), Description ( "A collection of categories that are added to each item." ), 
		DefaultValue ( null ), ReflectorName("categories"),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<Category> Categories { get; set; }

		/// <summary>
		/// Gets or sets the feed image.
		/// </summary>
		/// <value>The feed image.</value>
		[Category ( "Optional" ), Description ( "Represents the image that is associated with the feed.\nTo not use a feed image set all of the value to empty." ), 
		DefaultValue ( null ), Editor(typeof(NullValueEditorObject), typeof(UITypeEditor)),
		NullOrObject, ReflectorName("feedImage"),
		TypeConverter ( typeof ( ExpandableObjectConverter ) ), DisplayName ( "FeedImage" )]
		public RssBuildsFeedImage FeedImage { get; set; }

		/// <summary>
		/// Gets or sets the description footer.
		/// </summary>
		/// <value>The description footer.</value>
		[Category ( "Optional" ), ReflectorName("descriptionFooter"),
		Editor ( typeof ( MultilineStringUIEditor ), typeof ( UITypeEditor ) ),
		Description ( "String that is used to build the item description. This is added to the description after the modification comments." )]
		public string DescriptionFooter { get; set; }

		/// <summary>
		/// Gets or sets the description header.
		/// </summary>
		/// <value>The description header.</value>
		[Category ( "Optional" ), ReflectorName("descriptionHeader"),
		Editor ( typeof ( MultilineStringUIEditor ), typeof ( UITypeEditor ) ),
		Description ( "String that is used to build the item description. This is added to the description before the modification comments." )]
		public string DescriptionHeader { get; set; }

		/// <summary>
		/// Gets or sets the feed title.
		/// </summary>
		/// <value>The feed title.</value>
		[Category ( "Optional" ), ReflectorName("feedTitle"),
		Description ( "Format use to create the feed title." )]
		public string FeedTitle { get; set; }

		/// <summary>
		/// Gets or sets the feed description.
		/// </summary>
		/// <value>The feed description.</value>
		[Category ( "Optional" ), ReflectorName("feedDescription"),
		Description ( "Format use to create the feed description." )]
		public string FeedDescription { get; set; }

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns></returns>
		public override PublisherTask Clone () {
			RssBuildsPublisher rbp = this.MemberwiseClone () as RssBuildsPublisher;
			rbp.FeedImage = this.FeedImage.Clone ();
			rbp.Categories = this.Categories.Clone ();
			rbp.FeedElements = this.FeedElements.Clone ();
			rbp.ItemElements = this.ItemElements.Clone ();
			rbp.Namespaces = this.Namespaces.Clone ();
			rbp.PingItems = this.PingItems.Clone ();
			return rbp;
		}

		public override void Deserialize ( System.Xml.XmlElement element ) {
			if ( string.Compare ( element.Name, this.TypeName, false ) != 0 )
				throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.Name, this.TypeName ) );

			Util.ResetObjectProperties<RssBuildsPublisher> ( this );

			string s = Util.GetElementOrAttributeValue ( "outputPath", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.OutputPath = s;

			s = Util.GetElementOrAttributeValue ( "fileName", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.FileName = s;

			s = Util.GetElementOrAttributeValue ( "feedTitle", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.FeedTitle = s;

			s = Util.GetElementOrAttributeValue ( "feedDescription", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.FeedDescription = s;

			s = Util.GetElementOrAttributeValue ( "channelUrl", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.ChannelUrl = s;

			s = Util.GetElementOrAttributeValue ( "descriptionHeader", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.DescriptionHeader = s;

			s = Util.GetElementOrAttributeValue ( "descriptionFooter", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.DescriptionFooter = s;

			s = Util.GetElementOrAttributeValue ( "addEnclosure", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.AddEnclosure = string.Compare ( s, bool.TrueString, true ) == 0;


			s = Util.GetElementOrAttributeValue ( "buildCondition", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.BuildCondition = (PublishBuildCondition)Enum.Parse ( typeof ( PublishBuildCondition ), s, true );

			s = Util.GetElementOrAttributeValue ( "enclosureUrl", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.EnclosureUrl = s;

			s = Util.GetElementOrAttributeValue ( "encoding", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.Encoding = s;

			s = Util.GetElementOrAttributeValue ( "maxHistory", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				int val = 25;
				int.TryParse ( s, out val );
				this.MaxHistory = val;
			}

			s = Util.GetElementOrAttributeValue ( "itemUrl", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.ItemUrl = s;

			XmlElement cele = element.SelectSingleNode ( "categories" ) as XmlElement;
			if ( cele != null ) {
				foreach ( XmlElement re in cele.SelectNodes ( "category" ) ) {
					Category c = new Category ();
					c.Deserialize ( re );
					this.Categories.Add ( c );
				}
			}

			XmlElement fele = element.SelectSingleNode ( "feedElements" ) as XmlElement;
			if ( fele != null ) {
				foreach ( XmlElement re in fele.SelectNodes ( "rssElement" ) ) {
					RssBuildsElement f = new RssBuildsElement ();
					f.Deserialize ( re );
					this.FeedElements.Add ( f );
				}
			}

			XmlElement iele = element.SelectSingleNode ( "itemElements" ) as XmlElement;
			if ( iele != null ) {
				foreach ( XmlElement re in iele.SelectNodes ( "rssElement" ) ) {
					RssBuildsElement i = new RssBuildsElement ();
					i.Deserialize ( re );
					this.ItemElements.Add ( i );
				}
			}

			XmlElement pele = element.SelectSingleNode ( "pingItems" ) as XmlElement;
			if ( pele != null ) {
				foreach ( XmlElement pe in pele.SelectNodes ( "pingItem" ) ) {
					RssBuildsPingItem pi = new RssBuildsPingItem ();
					pi.Deserialize ( pe );
					this.PingItems.Add ( pi );
				}
			}

			XmlElement nele = element.SelectSingleNode ( "rssExtensions" ) as XmlElement;
			if ( nele != null ) {
				foreach ( XmlElement ns in nele.SelectNodes ( "namespace" ) ) {
					RssBuildsNamespace n = new RssBuildsNamespace ();
					n.Deserialize ( ns );
					this.Namespaces.Add ( n );
				}
			}

			XmlElement fiEle = element.SelectSingleNode ( "feedImage" ) as XmlElement;
			if ( fiEle != null )
				this.FeedImage.Deserialize ( fiEle );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<RssBuildsPublisher> ().Serialize ( this );
		}

		#region ICCNetDocumentation Members
		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://ccnetplugins.codeplex.com/Wiki/Print.aspx?title=RssBuildsPublisher&version=8&action=Print" ); }
		}

		#endregion
	}
}
