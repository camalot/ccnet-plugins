using System;
using System.Collections.Generic;
using System.Text;
using CCNetConfig.Core;
using CCNetConfig.Core.Serialization;
using CCNetConfig.Core.Components;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
  [ReflectorName("tag")]
  public class MetaWeblogTag : ICCNetObject, ISerialize, ICloneable {
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    [ReflectorNodeType(ReflectorNodeTypes.Value),ReflectorName("tagValue")]
    public string Value { get; set; }
    #region ISerialize Members

    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public void Deserialize ( System.Xml.XmlElement element ) {
      this.Value = Util.GetElementOrAttributeValue ( "tag", element );
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public System.Xml.XmlElement Serialize ( ) {
      return new Serializer<MetaWeblogTag> ( ).Serialize ( this );
    }

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone ( ) {
      return this.MemberwiseClone ( ) as MetaWeblogTag;
    }

    #endregion

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString ( ) {
      return string.IsNullOrEmpty ( this.Value ) ? "[Empty]" : this.Value;
    }
  }
}
