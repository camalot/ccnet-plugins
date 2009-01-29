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
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;
using System.IO;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Text.RegularExpressions;
using CCNet.Community.Plugins.Components.Macros;
using CCNet.Community.Plugins.Components;
using CCNet.Community.Plugins.Common;
using ThoughtWorks.CruiseControl.Core.Util;


namespace CCNet.Community.Plugins.Publishers {
	/// <summary>
	/// An Rss Builds publisher for CruiseControl.NET
	/// </summary>
	/// <example>
	/// <code language="xml" title="RssBuildsPublisher Example" htmlDecode="true">
	/// <![CDATA[<rssBuilds encoding="utf-8" fileName="$(ProjectName)Nightly" 
	///       buildCondition="IfModificationExists"
	///       addEnclosure="true">
	/// <!-- Collection of namespaces added to the feed -->
	/// <rssExtensions>
	///   <namespace prefix="slash" namespaceURI="http://purl.org/rss/1.0/modules/slash/" />
	/// </rssExtensions>
	/// <!-- the feed image -->
	/// <feedImage url="http://mydomain.com/images/logo.png" title="$(ProjectName)" link="$(ProjectUrl)" />
	/// <maxHistory>25</maxHistory>
	/// <channelUrl>$(ProjectUrl)</channelUrl>
	/// <itemUrl>http://mydomain.com/builds/$(Label)/$(ProjectName).$(Label).zip</itemUrl>
	/// <enclosureUrl>http://mydomain.com/builds/$(Label)/$(ProjectName).$(Label).zip</enclosureUrl>
	/// <feedTitle>Nightly Builds for {0}</feedTitle>
	/// <feedDescription>Build Report for $(ProjectName)</feedDescription>
	/// <itemTitle>$(ProjectName) $(Label)</itemTitle>
	/// <feedElements>
	///   <rssElement name="webmaster" value="my.email@address.com" />
	/// </feedElements>
	/// <itemElements>
	///   <rssElement prefix="dc" name="creator" value="Ryan" />
	/// </itemElements>
	/// <categories>
	///   <category name="NightlyBuilds" />
	///   <category name="CC.NET" />
	///   <category name="Publisher" />
	///   <category name="$(ProjectName)" />
	///   <category name="$(ProjectName) $(Label)" />
	/// </categories>
	/// <descriptionHeader>&lt;p&gt;</descriptionHeader>
	/// <descriptionFooter>&lt;/p&gt;
	/// &lt;p&gt;&lt;a href="http://mydomain.com/builds/$(Label)/$(ProjectName).$(Label).zip"&gt;$(ProjectName) $(Label) Binaries&lt;/a&gt;&lt;br /&gt;
	/// &lt;a href="http://mydomain.com/builds/$(Label)/$(ProjectName).$(Label).src.zip"&gt;$(ProjectName) $(Label) Source&lt;/a&gt;&lt;br /&gt;
	/// &lt;a href="http://mydomain.com/builds/$(Label)/$(ProjectName).$(Label).msi.zip"&gt;$(ProjectName) $(Label) Installer&lt;/a&gt;&lt;br /&gt;
	/// &lt;/p&gt;</descriptionFooter>
	/// </rssBuilds>]]>
	/// </code>
	/// </example>
	[ReflectorType ( "rssBuilds" )]
	public class RssBuildsPublisher : BasePublisherTask {
		#region Private Members
		/// <summary>
		/// The max number of items to store in the main feed.
		/// </summary>
		private int _maxHistory = 25;

		private string _encoding = string.Empty;
		/// <summary>
		/// The filename of the Xml Document without the extension.
		/// </summary>
		private string _fileName = string.Empty;
		/// <summary>
		/// the Rss Feed XmlDocument
		/// </summary>
		private XmlDocument rssDoc = null;

		List<Namespace> _namespaces = null;

		/// <summary>
		/// namespace manager used for added elements to the feed.
		/// </summary>
		private XmlNamespaceManager namespaceManager = null;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="RssBuildsPublisher"/> class.
		/// </summary>
		public RssBuildsPublisher () {
			this._maxHistory = 25;
			this.OutputPath = string.Empty;
			this.Encoding = "UTF-8";
			this._fileName = "rss";
			this._namespaces = new List<Namespace> ();
			this.FeedImage = new FeedImage ();
			this.FeedElements = new List<RssElement> ();
			this.ItemElements = new List<RssElement> ();
			this.Categories = new List<Category> ();
			this.PingItems = new List<PingElement> ();
			this.ItemTitle = "${Project} ${Label}";
			this.ItemUrl = "${ProjectUrl}";
			// need to add some default namespaces.
			AddDefaultNamespaces ();
		}
		#endregion

		#region Public Reflector Properties
		/// <summary>
		/// Gets or sets the output path.
		/// </summary>
		/// <value>The output path.</value>
		[ReflectorProperty ( "outputPath", Required = false )]
		public string OutputPath { get; set; }
		/// <summary>
		/// Gets or sets the RSS extensions.
		/// </summary>
		/// <value>The RSS extensions.</value>
		[ReflectorArray ( "rssExtensions", Required = false )]
		public List<Namespace> RssExtensions {
			get { return this._namespaces; }
			set {
				this._namespaces.Clear ();
				AddDefaultNamespaces ();
				this._namespaces.AddRange ( value );
			}
		}

		/// <summary>
		/// Gets or sets the ping items.
		/// </summary>
		/// <value>The ping items.</value>
		[ReflectorArray ( "pingItems", Required = false )]
		public List<PingElement> PingItems { get; set; }
		/// <summary>
		/// Gets or sets the feed image.
		/// </summary>
		/// <value>The feed image.</value>
		[ReflectorProperty ( "feedImage", Required = false )]
		public FeedImage FeedImage { get; set; }
		/// <summary>
		/// Gets or sets the max history.
		/// </summary>
		/// <value>The max history.</value>
		[ReflectorProperty ( "maxHistory", Required = false )]
		public int MaxHistory {
			get { return this._maxHistory; }
			set { this._maxHistory = value > 0 ? value : 25; }
		}
		/// <summary>
		/// Gets or sets the channel URL format.
		/// </summary>
		/// <value>The channel URL format.</value>
		[ReflectorProperty ( "channelUrl", Required = false )]
		public string ChannelUrl { get; set; }
		/// <summary>
		/// Gets or sets the encoding.
		/// </summary>
		/// <value>The encoding.</value>
		[ReflectorProperty ( "encoding", Required = false )]
		public string Encoding {
			get { return this._encoding; }
			set {
				Encoding encode = System.Text.Encoding.GetEncoding ( value );
				if ( encode == null )
					this._encoding = "UTF-8";
				else
					this._encoding = value;
			}
		}
		/// <summary>
		/// Gets or sets the URL format.
		/// </summary>
		/// <value>The URL format.</value>
		[ReflectorProperty ( "itemUrl", Required = false )]
		public string ItemUrl { get; set; }
		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		[ReflectorProperty ( "fileName", Required = false )]
		public string FileName {
			get { return this._fileName; }
			set { this._fileName = string.IsNullOrEmpty ( value ) ? "rss" : value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether [add enclosure].
		/// </summary>
		/// <value><c>true</c> if [add enclosure]; otherwise, <c>false</c>.</value>
		[ReflectorProperty ( "addEnclosure", Required = false )]
		public bool AddEnclosure { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [add enclosure].
		/// </summary>
		/// <value><c>true</c> if [add enclosure]; otherwise, <c>false</c>.</value>
		[ReflectorProperty ( "enclosureUrl", Required = false )]
		public string EnclosureUrl { get; set; }
		/// <summary>
		/// Gets or sets the feed title.
		/// </summary>
		/// <value>The feed title.</value>
		[ReflectorProperty ( "feedTitle", Required = false )]
		public string FeedTitle { get; set; }
		/// <summary>
		/// Gets or sets the item title format.
		/// </summary>
		/// <value>The item title format.</value>
		[ReflectorProperty ( "itemTitle", Required = false )]
		public string ItemTitle { get; set; }
		/// <summary>
		/// Gets or sets the feed description format.
		/// </summary>
		/// <value>The feed description format.</value>
		[ReflectorProperty ( "feedDescription", Required = false )]
		public string FeedDescription { get; set; }
		/// <summary>
		/// Gets or sets the feed elements.
		/// </summary>
		/// <value>The feed elements.</value>
		[ReflectorProperty ( "feedElements", Required = false )]
		public List<RssElement> FeedElements { get; set; }
		/// <summary>
		/// Gets or sets the item elements.
		/// </summary>
		/// <value>The item elements.</value>
		[ReflectorProperty ( "itemElements", Required = false )]
		public List<RssElement> ItemElements { get; set; }
		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>The categories.</value>
		[ReflectorCollection ( "categories", Required = false )]
		public List<Category> Categories { get; set; }
		/// <summary>
		/// Gets or sets the description header.
		/// </summary>
		/// <value>The description header.</value>
		[ReflectorProperty ( "descriptionHeader", Required = false )]
		public string DescriptionHeader { get; set; }
		/// <summary>
		/// Gets or sets the description footer.
		/// </summary>
		/// <value>The description footer.</value>
		[ReflectorProperty ( "descriptionFooter", Required = false )]
		public string DescriptionFooter { get; set; }

		/// <summary>
		/// Gets the modification comments.
		/// </summary>
		/// <value>The modification comments.</value>
		public string ModificationComments {
			get;
			private set;
		}
		#endregion

		#region ITask Members

		/// <summary>
		/// Runs the task.
		/// </summary>
		/// <param name="result">The result.</param>
		public override void Run ( IIntegrationResult result ) {
			try {
				Log.Debug ( "Running RssBuildsPublisher" );
				string outputPath = Path.Combine ( result.ArtifactDirectory, this.GetPropertyString<RssBuildsPublisher> ( this, result, this.OutputPath ) );

				if ( result.Status != IntegrationStatus.Success )
					return;

				// using a custom enum allows for supporting AllBuildConditions
				if ( this.BuildCondition != PublishBuildCondition.AllBuildConditions && string.Compare ( this.BuildCondition.ToString (), result.BuildCondition.ToString (), true ) != 0 ) {
					Log.Info ( "RssBuildsPublisher skipped due to build condition not met." );
					return;
				}
				this.ModificationComments = Util.GetModidicationCommentsString ( result );

				string fileNameFormat = "{0}.xml";
				string fileName = string.Format ( fileNameFormat, this.GetPropertyString<IMacroRunner> ( this, result, this.FileName ) );
				FileInfo rssFile = new FileInfo ( Path.Combine ( outputPath, fileName ) );
				rssDoc = new XmlDocument ();
				if ( !rssFile.Exists )
					rssDoc = BuildBaseFeedDocument ( result, rssFile );
				else {
					rssDoc.Load ( rssFile.FullName );
					namespaceManager = CreateXmlNamespaceManager ( rssDoc );
					XmlElement chan = rssDoc.DocumentElement.SelectSingleNode ( "channel" ) as XmlElement;
					UpdateGeneratorTag ( chan );
					if ( chan != null )
						AddBuildItemsToChannel ( result, chan );
				}
				ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( string.Format ( "Saving {0}", rssFile.FullName ) );
				rssDoc.Save ( rssFile.FullName );

				ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( "Pinging Defined Urls" );
				foreach ( PingElement pe in this.PingItems ) {
					pe.PingUrl = this.GetPropertyString<IMacroRunner> ( this, result, pe.PingUrl );
					pe.FeedName = this.GetPropertyString<IMacroRunner> ( this, result, pe.FeedName );
					pe.FeedUrl = this.GetPropertyString<IMacroRunner> ( this, result, pe.FeedUrl );
					pe.Send ();
				}
			} catch ( Exception ex ) {
				if ( this.ContinueOnFailure ) {
					Log.Warning ( ex );
				} else {
					Log.Error ( ex );
					throw;
				}
			}
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// Builds the base feed document.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="rssFile">The RSS file.</param>
		/// <returns></returns>
		private XmlDocument BuildBaseFeedDocument ( IIntegrationResult result, FileInfo rssFile ) {
			XmlDocument doc = BuildEmptyFeedDocument ( result, rssFile );
			XmlElement chan = CreateChannel ( result, doc );
			UpdateGeneratorTag ( chan );
			doc.DocumentElement.AppendChild ( chan );
			return doc;
		}

		/// <summary>
		/// Builds the empty feed document.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private XmlDocument BuildEmptyFeedDocument ( IIntegrationResult result, FileInfo file ) {
			XmlDocument doc = new XmlDocument ();
			doc.AppendChild ( doc.CreateXmlDeclaration ( "1.0", this.Encoding, string.Empty ) );
			doc.AppendChild ( doc.CreateElement ( "rss" ) );

			foreach ( Namespace ns in RssExtensions ) {
				XmlAttribute attr = doc.CreateAttribute ( "xmlns", ns.Prefix, "http://www.w3.org/2000/xmlns/" );
				attr.Value = ns.NamespaceUri.ToString ();
				doc.DocumentElement.SetAttributeNode ( attr );
			}

			doc.DocumentElement.SetAttribute ( "version", "2.0" );
			namespaceManager = CreateXmlNamespaceManager ( doc );

			return doc;
		}

		/// <summary>
		/// Creates the channel.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="doc">The doc.</param>
		/// <returns></returns>
		private XmlElement CreateChannel ( IIntegrationResult result, XmlDocument doc ) {
			XmlElement channel = CreateEmptyChannel ( result, doc );
			AddBuildItemsToChannel ( result, channel );
			return channel;
		}

		/// <summary>
		/// Creates the empty channel.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="doc">The doc.</param>
		/// <returns></returns>
		private XmlElement CreateEmptyChannel ( IIntegrationResult result, XmlDocument doc ) {
			XmlElement channel = doc.CreateElement ( "channel" );
			XmlElement ele = doc.CreateElement ( "title" );
			ele.InnerText = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, FeedTitle ), result.ProjectName, result.Status, result.BuildCondition );
			channel.AppendChild ( ele );

			if ( !string.IsNullOrEmpty ( result.ProjectUrl ) ) {
				ele = doc.CreateElement ( "link" );
				ele.InnerText = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, this.ChannelUrl ), result.ProjectName, result.Label );
				channel.AppendChild ( ele );
			}

			ele = doc.CreateElement ( "description" );
			ele.InnerText = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, FeedDescription ), result.ProjectName, result.Status, result.BuildCondition );
			channel.AppendChild ( ele );


			if ( FeedImage != null && !string.IsNullOrEmpty ( FeedImage.Image ) ) {
				ele = doc.CreateElement ( "image" );
				XmlElement tele = doc.CreateElement ( "link" );
				tele.InnerText = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, this.FeedImage.Link ), result.ProjectName );
				ele.AppendChild ( tele );

				tele = doc.CreateElement ( "url" );
				tele.InnerText = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, this.FeedImage.Image ), result.ProjectName );
				ele.AppendChild ( tele );

				tele = doc.CreateElement ( "title" );
				tele.InnerText = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, this.FeedImage.Title ), result.ProjectName );
				ele.AppendChild ( tele );

				channel.AppendChild ( ele );
			}

			AddCustomElements ( result, this.FeedElements, channel );

			return channel;
		}

		/// <summary>
		/// Adds the custom elements.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="elements">The elements.</param>
		/// <param name="parent">The parent.</param>
		private void AddCustomElements ( IIntegrationResult result, List<RssElement> elements, XmlElement parent ) {
			foreach ( RssElement element in elements ) {
				if ( !string.IsNullOrEmpty ( element.Prefix ) ) {
					if ( string.IsNullOrEmpty ( namespaceManager.LookupNamespace ( element.Prefix ) ) )
						continue;
				}

				XmlElement ele = parent.OwnerDocument.CreateElement ( element.Prefix, element.Name, namespaceManager.LookupNamespace ( element.Prefix ) );

				foreach ( RssElementAttribute attribute in element.Attributes )
					ele.SetAttribute ( attribute.Name, string.IsNullOrEmpty ( namespaceManager.LookupNamespace ( attribute.Prefix ) ) ? string.Empty :
						namespaceManager.LookupNamespace ( attribute.Prefix ), this.GetPropertyString<IMacroRunner> ( this, result, attribute.Value ) );

				if ( string.IsNullOrEmpty ( element.Value ) && element.ChildElements.Count > 0 )
					AddCustomElements ( result, element.ChildElements, ele );
				else {
					if ( !element.IsCData )
						ele.InnerText = this.GetPropertyString<IMacroRunner> ( this, result, element.Value );
					else
						ele.AppendChild ( parent.OwnerDocument.CreateCDataSection ( this.GetPropertyString<IMacroRunner> ( this, result, element.Value ) ) );
				}


				parent.AppendChild ( ele );
			}
		}

		/// <summary>
		/// Adds the build items to channel.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="channel">The channel.</param>
		private void AddBuildItemsToChannel ( IIntegrationResult result, XmlElement channel ) {
			XmlDocument doc = channel.OwnerDocument;
			FileInfo historyFile = new FileInfo ( Path.Combine ( Path.Combine ( result.ArtifactDirectory, this.GetPropertyString<RssBuildsPublisher> ( this, result, this.OutputPath ) ),
				string.Format ( "{0}.history.xml",
				this.GetPropertyString<IMacroRunner> ( this, result, this.FileName ) ) ) );
			XmlDocument docHistory = new XmlDocument ();

			if ( !historyFile.Exists ) {
				docHistory = BuildEmptyFeedDocument ( result, historyFile );
				docHistory.DocumentElement.AppendChild ( CreateEmptyChannel ( result, docHistory ) );
			} else
				docHistory.Load ( historyFile.FullName );
			XmlElement histChannel = docHistory.DocumentElement.SelectSingleNode ( "channel" ) as XmlElement;

			XmlNodeList oldItems = channel.SelectNodes ( "//item" );
			if ( oldItems.Count > this.MaxHistory ) {
				for ( int x = 0; x < oldItems.Count - this.MaxHistory; x++ ) {
					histChannel.AppendChild ( docHistory.ImportNode ( oldItems[ x ], true ) );
					oldItems[ x ].ParentNode.RemoveChild ( oldItems[ x ] );
				}
			}

			docHistory.Save ( historyFile.FullName );

			XmlElement itemElement = doc.CreateElement ( "item" );

			channel.AppendChild ( itemElement );

			XmlElement ele = doc.CreateElement ( "title" );
			ele.InnerText = this.GetPropertyString<IMacroRunner> ( this, result, ItemTitle );
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "guid" );
			ele.SetAttribute ( "isPermaLink", "true" );
			ele.InnerText = this.GetPropertyString<IMacroRunner> ( this, result, this.ItemUrl);
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "link" );
			ele.InnerText = this.GetPropertyString<IMacroRunner> ( this, result, this.ItemUrl );
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "pubDate" );
			ele.InnerText = result.EndTime.ToString ( "r" );
			itemElement.AppendChild ( ele );

			if ( AddEnclosure ) {
				ele = doc.CreateElement ( "enclosure" );
				ele.SetAttribute ( "url",this.GetPropertyString<IMacroRunner> ( this, result, this.EnclosureUrl ) );
				itemElement.AppendChild ( ele );
			}

			ele = doc.CreateElement ( "ci", "BuildCondition", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = Enum.GetName ( typeof ( BuildCondition ), result.BuildCondition );
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "IntegrationRequest", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.IntegrationRequest.ToString ();
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "IntegrationProperties", namespaceManager.LookupNamespace ( "ci" ) );
			foreach ( string s in result.IntegrationProperties.Keys ) {
				string val = result.IntegrationProperties[ s ] as string;
				XmlElement tele = doc.CreateElement ( "ci", "Property", namespaceManager.LookupNamespace ( "ci" ) );
				tele.SetAttribute ( "key", s );
				tele.InnerText = val;
				if ( !string.IsNullOrEmpty ( val ) )
					ele.AppendChild ( tele );
			}
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "Label", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.Label;
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "LastChangeNumber", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.LastChangeNumber.ToString ();
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "LastIntegrationStatus", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.LastIntegrationStatus.ToString ();
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "LastModificationDate", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.LastModificationDate.ToString ( "r" );
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "LastSuccessfulIntegrationLabel", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.LastSuccessfulIntegrationLabel;
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "ProjectName", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.ProjectName;
			itemElement.AppendChild ( ele );

			if ( !string.IsNullOrEmpty ( result.ProjectUrl ) ) {
				ele = doc.CreateElement ( "ci", "ProjectUrl", namespaceManager.LookupNamespace ( "ci" ) );
				ele.InnerText = result.ProjectUrl;
				itemElement.AppendChild ( ele );
			}

			ele = doc.CreateElement ( "ci", "Status", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.Status.ToString ();
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "TotalIntegrationTime", namespaceManager.LookupNamespace ( "ci" ) );
			ele.InnerText = result.TotalIntegrationTime.ToString ();
			itemElement.AppendChild ( ele );

			ele = doc.CreateElement ( "ci", "Modifications", namespaceManager.LookupNamespace ( "ci" ) );
			foreach ( Modification mod in result.Modifications ) {
				XmlElement tmod = doc.CreateElement ( "ci", "Modification", namespaceManager.LookupNamespace ( "ci" ) );
				XmlElement tele = doc.CreateElement ( "ci", "ChangeNumber", namespaceManager.LookupNamespace ( "ci" ) );
				tele.InnerText = mod.ChangeNumber.ToString ();
				tmod.AppendChild ( tele );

				if ( !string.IsNullOrEmpty ( mod.Comment ) ) {
					tele = doc.CreateElement ( "ci", "Comment", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.Comment;
					tmod.AppendChild ( tele );
				}

				if ( !string.IsNullOrEmpty ( mod.EmailAddress ) ) {
					tele = doc.CreateElement ( "ci", "ChangeNumber", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.EmailAddress;
					tmod.AppendChild ( tele );
				}

				if ( !string.IsNullOrEmpty ( mod.FileName ) ) {
					tele = doc.CreateElement ( "ci", "FileName", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.FileName;
					tmod.AppendChild ( tele );
				}

				if ( !string.IsNullOrEmpty ( mod.FolderName ) ) {
					tele = doc.CreateElement ( "ci", "FolderName", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.FolderName;
					tmod.AppendChild ( tele );
				}

				tele = doc.CreateElement ( "ci", "ModifiedTime", namespaceManager.LookupNamespace ( "ci" ) );
				tele.InnerText = mod.ModifiedTime.ToString ( "r" );
				tmod.AppendChild ( tele );

				if ( !string.IsNullOrEmpty ( mod.Type ) ) {
					tele = doc.CreateElement ( "ci", "Type", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.Type;
					tmod.AppendChild ( tele );
				}

				if ( !string.IsNullOrEmpty ( mod.Url ) ) {
					tele = doc.CreateElement ( "ci", "Url", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.Url;
					tmod.AppendChild ( tele );
				}

				if ( !string.IsNullOrEmpty ( mod.UserName ) ) {
					tele = doc.CreateElement ( "ci", "UserName", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.UserName;
					tmod.AppendChild ( tele );
				}

				if ( !string.IsNullOrEmpty ( mod.Version ) ) {
					tele = doc.CreateElement ( "ci", "Version", namespaceManager.LookupNamespace ( "ci" ) );
					tele.InnerText = mod.Version;
					tmod.AppendChild ( tele );
				}

				ele.AppendChild ( tmod );
			}
			itemElement.AppendChild ( ele );

			// add description.
			ele = doc.CreateElement ( "description" );
			XmlCDataSection cdata = doc.CreateCDataSection ( string.Format ( "{0}{1}{2}",
				this.GetPropertyString<IMacroRunner> ( this, result, this.DescriptionHeader ),
				this.ModificationComments,
				this.GetPropertyString<IMacroRunner> ( this, result, this.DescriptionFooter ) ) );
			ele.AppendChild ( cdata );
			itemElement.AppendChild ( ele );

			// add categories.
			foreach ( Category cat in this.Categories ) {
				ele = doc.CreateElement ( "category" );
				ele.InnerText = this.GetPropertyString<IMacroRunner> ( this, result, cat.Name );
				itemElement.AppendChild ( ele );
			}

			AddCustomElements ( result, this.ItemElements, itemElement );

		}

		/// <summary>
		/// Updates the generator tag.
		/// </summary>
		/// <param name="channel">The channel.</param>
		private void UpdateGeneratorTag ( XmlElement channel ) {
			XmlElement ele = channel.SelectSingleNode ( "generator" ) as XmlElement;
			if ( ele == null ) {
				ele = channel.OwnerDocument.CreateElement ( "generator" );
				channel.AppendChild ( ele );
			}
			ele.InnerText = string.Format ( Properties.Resources.GeneratorString, this.GetType ().Assembly.GetName ().Version.ToString () );
		}


		/// <summary>
		/// Creates the XML namespace manager.
		/// </summary>
		/// <param name="doc">The doc.</param>
		/// <returns></returns>
		private XmlNamespaceManager CreateXmlNamespaceManager ( XmlDocument doc ) {
			XmlNamespaceManager nsmgr = new XmlNamespaceManager ( doc.NameTable );
			foreach ( XmlAttribute attr in doc.SelectSingleNode ( "/*" ).Attributes )
				if ( string.Compare ( attr.Prefix, "xmlns" ) == 0 )
					nsmgr.AddNamespace ( attr.LocalName, attr.Value );
			return nsmgr;
		}

		/// <summary>
		/// Adds the default namespaces.
		/// </summary>
		private void AddDefaultNamespaces () {
			this.RssExtensions.Add ( new Namespace ( "dc", "http://purl.org/dc/elements/1.1/" ) );
			this.RssExtensions.Add ( new Namespace ( "ci", "http://ccnetconfig.org/2007/CCNetInfo" ) );
		}
		#endregion


	}
}
