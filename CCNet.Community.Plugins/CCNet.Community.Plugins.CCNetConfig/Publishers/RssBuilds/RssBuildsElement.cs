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
using System.ComponentModel.Design;
using System.Drawing.Design;
using CCNetConfig.Core.Serialization;
using System.Xml;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	/// <summary>
	/// Represents an XmlElement that is added to the XmlDocument.
	/// </summary>
	public class RssBuildsElement : ICCNetObject, ICloneable {

		/// <summary>
		/// Initializes a new instance of the <see cref="RssBuildsElement"/> class.
		/// </summary>
		public RssBuildsElement () {

		}

		/// <summary>
		/// Gets or sets the prefix.
		/// </summary>
		/// <value>The prefix.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName("prefix"), ReflectorNodeType(ReflectorNodeTypes.Attribute),
		Description ( "The prefix to use for the element. This prefix MUST exist in the namespaces defined in the rss extensions" )]
		public string Prefix { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Category ( "Required" ), DisplayName ( "(Name)" ), DefaultValue ( null ), ReflectorName("name"),
		Description ( "The name of the element." ), Required, ReflectorNodeType(ReflectorNodeTypes.Attribute)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Category ( "Optional" ), DefaultValue ( null ), ReflectorName("value"),
		Description ( "The value of the element." ), ReflectorNodeType(ReflectorNodeTypes.Attribute),
		Editor ( typeof ( MultilineStringEditor ), typeof ( UITypeEditor ) )]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the child elements.
		/// </summary>
		/// <value>The child elements.</value>
		[Category ( "Optional" ), DefaultValue ( null ),
		ReflectorName("childElements"),
		Description ( "Child nodes to add to this element." ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<RssBuildsElement> ChildElements { get; set; }

		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		[Category ( "Optional" ), DefaultValue ( null ), ReflectorName("attributes"),
		Description ( "Attributes to add to this element." ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<RssBuildsElementAttribute> Attributes { get; set; }

		#region ISerialize Members

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void Deserialize ( System.Xml.XmlElement element ) {
			if ( string.Compare ( element.Name, "rssElement", false ) != 0 )
				throw new InvalidCastException ( string.Format ( "Unable to convert {0} to an {1}", element.Name, this.GetType ().Name ) );

			Util.ResetObjectProperties<RssBuildsElement> ( this );
			
			string s = Util.GetElementOrAttributeValue ( "name", element );
			this.Name = s;

			XmlNodeList subNodes = element.SelectNodes ( string.Format ( "childElements/{0}", element.Name ) );
			foreach ( XmlElement ele in subNodes ) {
				RssBuildsElement rsse = new RssBuildsElement ();
				rsse.Deserialize ( ele );
				this.ChildElements.Add ( rsse );
			}

			if ( this.ChildElements.Count == 0 ) {
				s = Util.GetElementOrAttributeValue ( "value", element );
				this.Value = s;
			}

			s = Util.GetElementOrAttributeValue ( "prefix", element );
			if ( !string.IsNullOrEmpty ( s ) )
				this.Prefix = s;

			subNodes = element.SelectNodes ( "attributes/attribute" );
			foreach ( XmlElement ele in subNodes ) {
				RssBuildsElementAttribute rea = new RssBuildsElementAttribute ();
				rea.Deserialize ( ele );
				this.Attributes.Add ( rea );
			}
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public System.Xml.XmlElement Serialize () {
			return new Serializer<RssBuildsElement> ().Serialize ( this );
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone () {
			RssBuildsElement re = this.MemberwiseClone () as RssBuildsElement;
			re.ChildElements = this.ChildElements.Clone ();
			re.Attributes = this.Attributes.Clone ();
			return re;
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

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString () {
			return this.Name;
		}
	}
}
