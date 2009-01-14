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
using System.Drawing.Design;
using CCNetConfig.Core.Serialization;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
  [ReflectorName("workitemPublisher"), Plugin, MinimumVersion("1.3")]
  public class TfsWorkItemPublisher : PublisherTask, ICCNetDocumentation {
    public TfsWorkItemPublisher ( ) : base("workitemPublisher") {

    }

    /// <summary>
    /// Gets or sets the TFS server.
    /// </summary>
    /// <value>The TFS server.</value>
    [Description("The Team Foundation Server to publish to."), Category("Required"),
    DisplayName("(TfsServer)"), Required, ReflectorName("server"), DefaultValue(null)]
    public string TfsServer { get; set; }
    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    /// <value>The domain.</value>
    [Description ( "The domain for the TFS login credentials" ), DefaultValue ( null ),
    ReflectorName ( "domain" ), Category ( "Optional" )]
    public string Domain { get; set; }
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    [Description ( "The username for the TFS login credentials" ), DefaultValue ( null ),
    ReflectorName ( "username" ), Category ( "Optional" )]
    public string UserName { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [Description ( "The password for the TFS login credentials" ), DefaultValue ( null ),
    ReflectorName ( "password" ),
    TypeConverter ( typeof ( PasswordTypeConverter ) ), Category ( "Optional" )]
    public HiddenPassword Password { get; set; }
    /// <summary>
    /// Gets or sets the name of the TFS project.
    /// </summary>
    /// <value>The name of the TFS project.</value>
    [Description ( "The name of the project in TFS" ), Category ( "Required" ),
    DisplayName ( "(TfsProjectName)" ), Required, ReflectorName ( "project" ), DefaultValue ( null )]
    public string TfsProjectName { get; set; }
    /// <summary>
    /// Gets or sets the title prefix.
    /// </summary>
    /// <value>The title prefix.</value>
    [Description("A string to prepend to the TFS WorkItem title."), DefaultValue(null),
    ReflectorName ( "titleprefix" ), Category ( "Optional" )]
    public string TitlePrefix { get; set; }


    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    /// <returns></returns>
    public override PublisherTask Clone ( ) {
      TfsWorkItemPublisher pub = this.MemberwiseClone ( ) as TfsWorkItemPublisher;
      pub.Password = this.Password.Clone ( );
      return pub;
    }

    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public override void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<TfsWorkItemPublisher> ().Deserialize ( element, this );
      /*if ( string.Compare ( element.Name, this.TypeName, false ) != 0 )
        throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.Name, this.TypeName ) );

      this.Domain = string.Empty;
      this.UserName = string.Empty;
      this.Password = new HiddenPassword ( );
      this.TitlePrefix = string.Empty;

      this.TfsServer = Util.GetElementOrAttributeValue ( "server", element );
      this.TfsProjectName = Util.GetElementOrAttributeValue ( "project", element );

      string s = Util.GetElementOrAttributeValue ( "domain", element );
      if ( !string.IsNullOrEmpty ( s ) )
        this.Domain = s;

      s = Util.GetElementOrAttributeValue ( "username", element );
      if ( !string.IsNullOrEmpty ( s ) )
        this.UserName = s;

      s = Util.GetElementOrAttributeValue ( "password", element );
      if ( !string.IsNullOrEmpty ( s ) )
        this.Password.Password = s;
			*/
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public override System.Xml.XmlElement Serialize ( ) {
      return new Serializer<TfsWorkItemPublisher> ( ).Serialize ( this );
    }

    #region ICCNetDocumentation Members
    /// <summary>
    /// Gets the documentation URI.
    /// </summary>
    /// <value>The documentation URI.</value>
    [Browsable(false),EditorBrowsable(EditorBrowsableState.Never), ReflectorIgnore]
    public Uri DocumentationUri {
      get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/Print.aspx?title=TfsWorkItemPublisher" ); }
    }

    #endregion
  }
}
