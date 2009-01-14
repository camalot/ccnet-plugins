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
using CCNet.Community.Plugins.CCNetConfig.Tasks.MBUnit;
using System.Drawing.Design;
using CCNet.Community.Plugins.CCNetConfig.Common;

namespace CCNet.Community.Plugins.CCNetConfig.Tasks {
	/// <summary>
	/// enables you to instruct CCNet to run the unit tests contained within a collection of assemblies.
	/// The results of the unit tests will be automatically included in the CCNet build results. This
	/// can be useful if you have some unit tests that you want to run as part of the integration process,
	/// but you don't need as part of your developer build process.
	/// </summary>
	[ReflectorName("mbunit"),Plugin]
	public class MBUnitTask : PublisherTask, ICCNetDocumentation {
		/// <summary>
		/// Initializes a new instance of the <see cref="MBUnitTask"/> class.
		/// </summary>
		public MBUnitTask ()
			: base ( "mbunit" ) {
			this.Assemblies = new CloneableList<Assembly> ();
		}

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
		/// Gets or sets the assemblies.
		/// </summary>
		/// <value>The assemblies.</value>
		[Description ( "A collection of assembly files " ),
		ReflectorName ( "assemblies" ), DefaultValue ( null ), Required, 
		Category ( "Required" ), DisplayName ( "(Assemblies)" ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<Assembly> Assemblies { get; set; }

		/// <summary>
		/// Gets or sets the executable.
		/// </summary>
		/// <value>The executable.</value>
		[Description ( "Path to the mbunit-console.exe" ), DefaultValue ( null ),
		Required, DisplayName ( "(Executable)" ), Category ( "Required" ),
		ReflectorName ( "executable" ),
		Editor(typeof(OpenFileDialogUIEditor),typeof(UITypeEditor)),
		OpenFileDialogTitle("Select mbunit-console"),
		FileTypeFilter("MBUnit Console|mbunit-console.exe|All Files|*.*")]
		public string Executable { get; set; }

		/// <summary>
		/// Gets or sets the output file.
		/// </summary>
		/// <value>The output file.</value>
		[Description ( "The file that MbUnit will write the test results to" ), DefaultValue ( null ),
		Category ( "Optional" ), ReflectorName ( "outputfile" ),
		Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
		OpenFileDialogTitle ( "Choose output file name" ),
		FileTypeFilter ( "Xml Files|*.xml|All Files|*.*" )]
		public string OutputFile { get; set; }

		/// <summary>
		/// Gets or sets the assembly path.
		/// </summary>
		/// <value>The assembly path.</value>
		[Description ( "Used to locate the MbUnit dlls needed to run the tests in your assembly if they aren't in the directory you're calling the runner from" ), DefaultValue ( null ),
		Category ( "Optional" ), BrowseForFolderDescription("Select Assembly Path"),
		ReflectorName ( "assemblypath" ),
		Editor(typeof(BrowseForFolderUIEditor), typeof(UITypeEditor))]
		public string AssemblyPath { get; set; }

		[Description ( "Specifies the location of a XSLT stylesheet to be applied to the report once it has been generated." ), DefaultValue ( null ),
		Category ( "Optional" ), ReflectorName ( "transformfile" ),
		Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ),
		OpenFileDialogTitle ( "Select transform file" ),
		FileTypeFilter ( "Xslt Files|*.xslt;*.xsl|All Files|*.*" )]
		public string TransformFile { get; set; }

		/// <summary>
		/// Gets or sets the filters.
		/// </summary>
		/// <value>The filters.</value>
		[Description ( "Specifies that only those tests that meet the filters defined will be run in this execution of MbUnit.Cons. " ),
		ReflectorName ( "filters" ), DefaultValue ( null ),
		Category ( "Optional" ), NullOrObject,
		Editor ( typeof ( ObjectOrNoneUIEditor ),typeof(UITypeEditor) ),
		TypeConverter(typeof(ObjectOrNoneTypeConverter))]
		public MBUnitFilters Filters { get; set; }

		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns></returns>
		public override PublisherTask Clone () {
			MBUnitTask mbut = this.MemberwiseClone () as MBUnitTask;
			mbut.Assemblies = this.Assemblies.Clone ();
			mbut.Filters = this.Filters.Clone ();
			return mbut;
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<MBUnitTask> ().Deserialize ( element, this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<MBUnitTask> ().Serialize ( this );
		}

		#region ICCNetDocumentation Members
		/// <summary>
		/// Gets the documentation URI.
		/// </summary>
		/// <value>The documentation URI.</value>
		[Browsable ( false ), EditorBrowsable ( EditorBrowsableState.Never ), ReflectorIgnore]
		public Uri DocumentationUri {
			get { return new Uri ( "http://www.codeplex.com/ccnetplugins/Wiki/Print.aspx?title=MbUnitTask" ); }
		}

		#endregion
	}
}
