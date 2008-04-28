using System;
using System.Collections.Generic;
using System.Text;
using CCNetConfig.Core;
using CCNetConfig.Core.Components;
using System.ComponentModel;
using CCNetConfig.Core.Serialization;
using System.Drawing.Design;
using System.Xml;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
  [ReflectorName("metaweblog"), MinimumVersion("1.3"), Plugin]
  public class MetaWeblogPublisher : PublisherTask, ICCNetDocumentation {

    public MetaWeblogPublisher ( ) : base("metaweblog") {
      this.Password = new HiddenPassword ( );
      this.DescriptionFormat = string.Empty;
      this.UserName = string.Empty;
      this.TitleFormat = string.Empty;
      this.Tags = new CloneableList<MetaWeblogTag> ( );
    }
    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    /// <value>The URL.</value>
    [ReflectorName ( "url" ), Required, Category ( "Required" ), DisplayName ( "(Url)" ),
    DefaultValue ( null ), Description ( "The url to the MetaWeblog service" )]
    public Uri Url { get; set; }
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    [ReflectorName ( "username" ), Required, Category ( "Required" ), DisplayName ( "(UserName)" ),
    DefaultValue ( null ), Description ( "The username used to log in to the MetaWeblog service" )]
    public string UserName { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorName ( "password" ), Required, Category ( "Required" ), DisplayName ( "(Password)" ),
    TypeConverter ( typeof ( PasswordTypeConverter ) ), DefaultValue ( null ),
    Description ( "The password used to log in to the MetaWeblog service" )]
    public HiddenPassword Password { get; set; }
    /// <summary>
    /// Gets or sets the title format.
    /// </summary>
    /// <value>The title format.</value>
    [ReflectorName ( "titleformat" ), Category ( "Optional" ), DefaultValue ( null ),
    Description ( "The format string used to create the blog post title" )]
    public string TitleFormat { get; set; }
    /// <summary>
    /// Gets or sets the description format.
    /// </summary>
    /// <value>The description format.</value>
    [ReflectorName ( "descriptionformat" ), Category ( "Optional" ),
    Description ( "The format string used to create the blog post body" ),
    DefaultValue ( null ), Editor ( typeof ( MultilineStringUIEditor ), typeof ( UITypeEditor ) )]
    public string DescriptionFormat { get; set; }
    /// <summary>
    /// Gets or sets the continue on failure.
    /// </summary>
    /// <value>The continue on failure.</value>
    [ReflectorName ( "continueOnFailure" ), Category ( "Optional" ),
    Description ( "If true, and the post fails, it will not be reported as a failed build" ), DefaultValue ( null ),
    TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) ), Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) )]
    public bool? ContinueOnFailure { get; set; }
    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    /// <value>The tags.</value>
    [ReflectorName ( "tags" ), Description ( "A collection of Tags to indicate tags for the post" ),
    Category ( "Optional" ), Editor ( typeof ( CollectionEditor ), typeof ( UITypeEditor ) ),
    TypeConverter ( typeof ( IListTypeConverter ) )]
    public CloneableList<MetaWeblogTag> Tags { get; set; }
    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    /// <returns></returns>
    public override PublisherTask Clone ( ) {
      MetaWeblogPublisher pub = this.MemberwiseClone ( ) as MetaWeblogPublisher;
      pub.Password = this.Password.Clone ( );
      return pub;
    }

    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public override void Deserialize ( System.Xml.XmlElement element ) {
      if ( string.Compare ( element.Name, this.TypeName, false ) != 0 )
        throw new InvalidCastException ( string.Format ( "Unable to convert {0} to a {1}", element.Name, this.TypeName ) );

      this.UserName = Util.GetElementOrAttributeValue ( "username", element );
      this.Password = new HiddenPassword ( );
      this.Password.Password = Util.GetElementOrAttributeValue ( "password", element );
      this.Url = new Uri ( Util.GetElementOrAttributeValue ( "url", element ) );
      this.Tags.Clear ( );

      string s = Util.GetElementOrAttributeValue ( "titleformat", element );
      if ( !string.IsNullOrEmpty ( s ) )
        this.TitleFormat = s;

      s = Util.GetElementOrAttributeValue ( "descriptionformat", element );
      if ( !string.IsNullOrEmpty ( s ) )
        this.DescriptionFormat = s;

      s = Util.GetElementOrAttributeValue ( "continueOnFailure", element );
      if ( !string.IsNullOrEmpty ( s ) )
        this.ContinueOnFailure = string.Compare(bool.TrueString,s,true) == 0;

      XmlElement tagsNode = element.SelectSingleNode ( "tags" ) as XmlElement;
      foreach ( XmlElement tNode in tagsNode.SelectNodes ( "./*" ) ) {
        MetaWeblogTag mwt = new MetaWeblogTag();
        mwt.Deserialize(tNode);
        this.Tags.Add ( mwt );
      }
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public override System.Xml.XmlElement Serialize ( ) {
      return new Serializer<MetaWeblogPublisher> ( ).Serialize ( this );
    }

    #region ICCNetDocumentation Members
    /// <summary>
    /// Gets the documentation URI.
    /// </summary>
    /// <value>The documentation URI.</value>
    [ReflectorIgnore, Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never )]
    public Uri DocumentationUri {
      get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/View.aspx?title=MetaWeblogPublisher&referringTitle=Home" ); }
    }

    #endregion
  }
}
