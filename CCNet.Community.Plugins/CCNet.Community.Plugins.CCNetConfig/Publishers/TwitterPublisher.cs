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
using CCNet.Community.Plugins.CCNetConfig.Common;
using System.Xml;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
  [ReflectorName ( "twit" ), Plugin, MinimumVersion ( "1.3" )]
  public class TwitterPublisher : PublisherTask, ICCNetDocumentation {
    /// <summary>
    /// Initializes a new instance of the <see cref="TwitterPublisher"/> class.
    /// </summary>
    public TwitterPublisher ( )
      : base ( "twit" ) {
			this.Password = new HiddenPassword ();
    }

		/// <summary>
		/// Gets or sets the continue on failure.
		/// </summary>
		/// <value>The continue on failure.</value>
    [Description ( "If true, the build will not fail if this publisher fails" ),
    ReflectorName ( "continueOnFailure" ), DefaultValue ( null ), Category ( "Optional" ),
    Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
    TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) )]
    public bool? ContinueOnFailure { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
    [Description ( "The twitter user account" ),
    ReflectorName ( "username" ), DefaultValue ( null ),
    DisplayName ( "(UserName)" ), Category ( "Required" ),
    Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) )]
    public string UserName { get; set; }
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
    [Description ( "The twitter user account" ), Required,
    DisplayName ( "(Password)" ), Category ( "Required" ),
    ReflectorName ( "password" ), DefaultValue ( null ),
    TypeConverter ( typeof ( PasswordTypeConverter ) )]
		public HiddenPassword Password { get; set; }
		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		[TypeConverter ( typeof ( ObjectOrNoneTypeConverter ) ), DefaultValue ( null ),
		Editor ( typeof ( ObjectOrNoneUIEditor ), typeof ( UITypeEditor ) ), ReflectorName ( "proxy" ),
		Category ( "Optional" ), Description ( "Proxy information." )]
		public Proxy Proxy { get; set; }

    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    /// <returns></returns>
    public override PublisherTask Clone ( ) {
      return this.MemberwiseClone ( ) as TwitterPublisher;
    }

    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public override void Deserialize ( System.Xml.XmlElement element ) {
      if ( string.Compare ( element.Name, this.TypeName, false ) != 0 )
        throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.Name, this.TypeName ) );

			Utils.ResetObjectProperties<TwitterPublisher> ( this );

      this.UserName = Util.GetElementOrAttributeValue ( "username", element );
      this.Password.Password = Util.GetElementOrAttributeValue ( "password", element );

			string s = Util.GetElementOrAttributeValue ( "continueOnFailure", element );
			if ( !string.IsNullOrEmpty ( s ) ) {
				this.ContinueOnFailure = string.Compare ( bool.TrueString, s, true ) == 0;
			}

			XmlElement proxyElement = element.SelectSingleNode ( "proxy" ) as XmlElement;
			if ( proxyElement != null ) {
				this.Proxy = new Proxy ();
				this.Proxy.Deserialize ( proxyElement );
			}
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public override System.Xml.XmlElement Serialize ( ) {
      return new Serializer<TwitterPublisher> ( ).Serialize ( this );
    }

    #region ICCNetDocumentation Members

    [Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
    public Uri DocumentationUri {
      get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/Print.aspx?title=TwitterPublisher&referringTitle=Home" ); }
    }

    #endregion
  }
}
