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
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Macros;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace CCNet.Community.Plugins.Common {
	/// <summary>
	/// 
	/// </summary>
	public abstract class BasePublisherTask : ITask, IContinueOnFailure, IMacroRunner{

		/// <summary>
		/// Initializes a new instance of the <see cref="BasePublisherTask"/> class.
		/// </summary>
		public BasePublisherTask () {
			this.MacroEngine = new MacroEngine ();
			this.ContinueOnFailure = false;
			this.BuildCondition = PublishBuildCondition.AllBuildConditions;
		}
		#region ITask Members

		/// <summary>
		/// Runs the specified result.
		/// </summary>
		/// <param name="result">The result.</param>
		public abstract void Run ( IIntegrationResult result );

		#endregion

		/// <summary>
		/// Gets or sets the build condition in which will execute this task/publisher.
		/// </summary>
		/// <value>The build condition.</value>
		[ReflectorProperty ( "buildCondition", Required = false )]
		public PublishBuildCondition BuildCondition { get; set; }

		#region ContinueOnFailure Members

		/// <summary>
		/// Gets or sets a value indicating whether [continue on error].
		/// </summary>
		/// <value><c>true</c> if [continue on error]; otherwise, <c>false</c>.</value>
		[ReflectorProperty ( "continueOnFailure", Required = false )]
		public bool ContinueOnFailure { get; set; }

		#endregion

		#region IMacroRunner Members

		/// <summary>
		/// Gets the macro engine.
		/// </summary>
		/// <value>The macro engine.</value>
		public MacroEngine MacroEngine {
			get;
			private set;
		}

		/// <summary>
		/// Gets the property string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="result">The result.</param>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public string GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
			string ret = this.MacroEngine.GetPropertyString<T> ( sender, result, input );
			if ( !typeof ( T ).IsAssignableFrom(this.GetType()) )
				ret = this.GetPropertyString<BasePublisherTask> ( this, result, ret );
			return ret;
		}

		#endregion
	}
}
