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
using CCNetConfig.Core.Serialization;
using CCNetConfig.Core.Components;
using System.Drawing.Design;
using System.Xml;

namespace CCNet.Community.Plugins.CCNetConfig.Common {
	/// <summary>
	/// A common proxy object
	/// </summary>
	[ReflectorName("proxy")]
	public class Proxy : ICCNetDocumentation, ICCNetObject, ICloneable {

		/// <summary>
		/// Initializes a new instance of the <see cref="Proxy"/> class.
		/// </summary>
		public Proxy () {
			this.BypassList = new CloneableList<string> ();
		}

		/// <summary>
		/// Gets or sets the host.
		/// </summary>
		/// <value>The host.</value>
		[ReflectorName("host"), DefaultValue(null), DisplayName("(Host)"),
		Description ( "The proxy host name" ), Required, Category ( "Required" ),]
		public string Host { get; set; }
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[ReflectorName("username"), DefaultValue(null),
		Description ( "Proxy credential username" ), Category ( "Optional" ),]
		public string Username { get; set; }
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[ReflectorName("password"), DefaultValue(null),
		Description ( "Proxy credential password" ),
		TypeConverter ( typeof ( PasswordTypeConverter ) ), Category ( "Optional" ),]
		public HiddenPassword Password { get; set; }
		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		[ReflectorName("domain"), DefaultValue(null),
		Description ( "Proxy credential domain" ), Category ( "Optional" ),]
		public string Domain { get; set; }
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		[ReflectorName ( "port" ), DefaultValue ( null ),
		Description ( "The proxy port." ), Category ( "Optional" ),
		Editor(typeof(NumericUpDownUIEditor),typeof(UITypeEditor))]
		public int? Port { get; set; }
		/// <summary>
		/// Gets or sets the bypass list.
		/// </summary>
		/// <value>The bypass list.</value>
		[ReflectorName ( "bypassList" ), ReflectorArray ( "pattern" ), DefaultValue ( null ),
		Description ( "The proxy port." ), Category ( "Optional" ),
		TypeConverter(typeof(IListTypeConverter)),
		Editor ( typeof ( CollectionEditor ), typeof ( UITypeEditor ) )]
		public CloneableList<string> BypassList { get; set; }

		#region ISerialize Members

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void Deserialize ( System.Xml.XmlElement element ) {
			string typeName =  Util.GetReflectorNameAttributeValue ( this.GetType () );
			if ( string.Compare ( element.Name, typeName, false ) != 0 )
				throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.Name, typeName ) );
			Utils.ResetObjectProperties<Proxy> ( this );

			this.Host = Util.GetElementOrAttributeValue ( "host", element );
			string s = Util.GetElementOrAttributeValue ( "port", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				int t = -1;
				if ( int.TryParse ( s, out t ) ) {
					if ( t > 0 ) {
						this.Port = t;
					}
				}
			}

			s = Util.GetElementOrAttributeValue ( "username", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.Username = s;
			}

			s = Util.GetElementOrAttributeValue ( "password", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.Password.Password = s;
			}

			s = Util.GetElementOrAttributeValue ( "domain", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.Domain = s;
			}
			XmlElement bypass = element.SelectSingleNode("bypassList") as XmlElement;
			if ( bypass!= null ) {
				XmlNodeList nodes = element.SelectNodes ( "pattern" );
				foreach ( XmlElement ele in nodes ) {
					this.BypassList.Add ( ele.InnerText );
				}
			}
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public System.Xml.XmlElement Serialize () {
			return new Serializer<Proxy> ().Serialize ( this );
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public Proxy Clone () {
			Proxy p = this.MemberwiseClone () as Proxy;
			p.Password = this.Password.Clone ();
			p.BypassList = this.BypassList.Clone ();
			return p;
		}

		#endregion

		#region ICCNetDocumentation Members

		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/Print.aspx?title=Proxy" ); }
		}

		#endregion

		#region ICloneable Members

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
	}
}
