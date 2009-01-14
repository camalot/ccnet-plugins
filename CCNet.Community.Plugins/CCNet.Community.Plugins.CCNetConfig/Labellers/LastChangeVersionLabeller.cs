using System;
using System.Collections.Generic;
using System.Text;
using CCNetConfig.Core;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using CCNetConfig.Core.Serialization;
using System.Drawing.Design;

namespace CCNet.Community.Plugins.CCNetConfig.Labellers {
	/// <summary>
	/// The LastChangeVersionLabeller works a lot like the LastChangeLabeller, except it also uses the date to build the label.
	/// </summary>
	[Plugin,
	ReflectorName ( "lastChangeVersionLabeller" ),
	Description ( "The LastChangeVersionLabeller works a lot like the LastChangeLabeller, except it also uses the date to build the label." )]
	public class LastChangeVersionLabeller : Labeller, ICCNetDocumentation {

		/// <summary>
		/// Initializes a new instance of the <see cref="LastChangeVersionLabeller"/> class.
		/// </summary>
		public LastChangeVersionLabeller ()
			: base ( "lastChangeVersionLabeller" ) {
		}

		/// <summary>
		/// Gets or sets the major version number.
		/// </summary>
		/// <value>The major.</value>
		[Description ( "Indicates the major version number" ), Category ( "Required" ),
		DisplayName ( "(Major)" ), Required, ReflectorName ( "major" ),
		Editor ( typeof ( NumericUpDownUIEditor ), typeof ( UITypeEditor ) ),
		MinimumValue ( 0 ), MaximumValue ( int.MaxValue ), DefaultValue ( null )]
		public int Major { get; set; }
		/// <summary>
		/// Gets or sets the minor version number.
		/// </summary>
		/// <value>The minor.</value>
		[Description ( "Indicates the minor version number" ), Category ( "Required" ),
		DisplayName ( "(Minor)" ), Required, ReflectorName ( "major" ),
		Editor ( typeof ( NumericUpDownUIEditor ), typeof ( UITypeEditor ) ),
		MinimumValue ( 0 ), MaximumValue ( int.MaxValue ), DefaultValue ( null )]
		public int Minor { get; set; }
		/// <summary>
		/// Gets or sets the separator.
		/// </summary>
		/// <value>The separator.</value>
		[Description ( "The string to separate each section of the version" ), Category ( "Optional" ),
		ReflectorName ( "separator" ), DefaultValue ( null )]
		public string Separator { get; set; }
		/// <summary>
		/// Gets or sets the increment on failure.
		/// </summary>
		/// <value>The increment on failure.</value>
		[Description ( "If the label should increment even if the last build was unsuccessful" ),
		Category ( "Optional" ), Editor ( typeof ( DefaultableBooleanUIEditor ), typeof ( UITypeEditor ) ),
		TypeConverter ( typeof ( DefaultableBooleanTypeConverter ) ),
		ReflectorName ( "incrementOnFailure" ), DefaultValue ( null )]
		public bool? IncrementOnFailure { get; set; }

		/// <summary>
		/// Creates a copy of this object
		/// </summary>
		/// <returns></returns>
		public override Labeller Clone () {
			return this.MemberwiseClone () as LastChangeVersionLabeller;
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<LastChangeVersionLabeller> ().Deserialize ( element, this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<LastChangeVersionLabeller> ().Serialize ( this );
		}

		#region ICCNetDocumentation Members
		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://ccnetplugins.codeplex.com/Wiki/Print.aspx?title=LastChangeVersionLabeller" ); }
		}

		#endregion
	}
}
