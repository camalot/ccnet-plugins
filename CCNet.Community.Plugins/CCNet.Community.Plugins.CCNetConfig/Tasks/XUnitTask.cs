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
using CCNetConfig.Core.Serialization;
using System.Drawing.Design;
using CCNet.Community.Plugins.CCNetConfig.Common;

namespace CCNet.Community.Plugins.CCNetConfig.Tasks {
	/// <summary>
	/// Instruct CCNet to run the unit tests contained within a collection of assemblies using XUnit
	/// </summary>
	[ReflectorName("xunit"),Plugin]
	public class XUnitTask : PublisherTask, ICCNetDocumentation{
		/// <summary>
		/// The output types for xunit
		/// </summary>
		public enum XUnitOutputType {
			/// <summary>
			/// Xml style output
			/// </summary>
			Xml,
			/// <summary>
			/// NUnit stype output
			/// </summary>
			NUnit
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="XUnitTask"/> class.
		/// </summary>
		public XUnitTask () : base("xunit") {
			this.Assemblies = new CloneableList<Assembly> ();
		}

		/// <summary>
		/// Gets or sets the assemblies.
		/// </summary>
		/// <value>The assemblies.</value>
		[Description ( "A collection of assembly files " ),
		ReflectorName ( "assemblies" ), DefaultValue ( null ), Required,
		Category ( "Required" ), DisplayName ( "(Assemblies)" ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<Assembly> Assemblies { get; set; }

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		[Description ( "The number of seconds before the publisher times out" ),
		ReflectorName ( "timeout" ), Category ( "Optional" ), DefaultValue ( null ),
		Editor ( typeof ( NumericUpDownUIEditor ), typeof ( UITypeEditor ) ),
		MinimumValue ( 0 ), MaximumValue ( Int16.MaxValue )]
		public int? Timeout { get; set; }

		/// <summary>
		/// Gets or sets the executable.
		/// </summary>
		/// <value>The executable.</value>
		[Description ( "Path to the xunit.console.exe" ), DefaultValue ( null ),
		Category ( "Optional" ),
		ReflectorName ( "executable" ),
		Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
		OpenFileDialogTitle ( "Select xunit console" ),
		FileTypeFilter ( "XUnit Console|xunit.console.exe|All Files|*.*" )]
		public string Executable { get; set; }

		/// <summary>
		/// Gets or sets the output file.
		/// </summary>
		/// <value>The output file.</value>
		[Description ( "The type of output to write the results to " ), DefaultValue ( null ),
		Category ( "Optional" ), ReflectorName ( "outputfile" ),
		Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
		OpenFileDialogTitle ( "Choose output file name" ),
		FileTypeFilter ( "Xml Files|*.xml|All Files|*.*" )]
		public string OutputFile { get; set; }

		/// <summary>
		/// Gets or sets the configuration file.
		/// </summary>
		/// <value>The configuration file.</value>
		[Description ( "The path of the xunit configuration file to load" ), DefaultValue ( null ),
		Category ( "Optional" ), ReflectorName ( "configfile" ),
		Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
		OpenFileDialogTitle ( "Choose output file name" ),
		FileTypeFilter ( "Configuration Files|*.config;*.xunit|All Files|*.*" )]
		public string ConfigurationFile { get; set; }

		/// <summary>
		/// Gets or sets the type of the output.
		/// </summary>
		/// <value>The type of the output.</value>
		[Description("The type output type of the results"), DefaultValue(null),
		TypeConverter(typeof(DefaultableEnumTypeConverter)), 
		Editor(typeof(DefaultableEnumUIEditor),typeof(UITypeEditor)),
		ReflectorName ( "outputtype" ), Category ( "Optional" )]
		public XUnitOutputType? OutputType { get; set; }

		/// <summary>
		/// Gets or sets the shadow copy assemblies.
		/// </summary>
		/// <value>The shadow copy assemblies.</value>
		[Description ( "Indicates of assemblies should be shadow copied to temp directory" ),
		TypeConverter(typeof(DefaultableBooleanTypeConverter)),
		Editor(typeof(DefaultableBooleanUIEditor),typeof(UITypeEditor)),
		ReflectorName("shadowCopyAssemblies"), Category("Optional")]
		public bool? ShadowCopyAssemblies { get; set; }

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns></returns>
		public override PublisherTask Clone () {
			XUnitTask xut = this.MemberwiseClone () as XUnitTask;
			xut.Assemblies = this.Assemblies.Clone ();
			return xut;
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<XUnitTask> ().Deserialize ( element, this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<XUnitTask> ().Serialize ( this );
		}

		#region ICCNetDocumentation Members

		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/Print.aspx?title=XUnitTask" ); }
		}

		#endregion
	}
}
