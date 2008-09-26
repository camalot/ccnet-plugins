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
 */
using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNet.Community.Plugins.Components.Twitter;
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Macros;

namespace CCNet.Community.Plugins.Publishers {
  /// <summary>
  /// A twitter publisher. 
  /// </summary>
  [ReflectorType("twit")]
  public class TwitterPublisher : ITask, IMacroRunner {
    public TwitterPublisher ( ) {
      this.ContinueOnFailure = false;
			this.MacroEngine = new MacroEngine ();
    }
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    [ReflectorProperty("username")]
    public string UserName { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorProperty ( "password" )]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [continue on failure].
    /// </summary>
    /// <value><c>true</c> if [continue on failure]; otherwise, <c>false</c>.</value>
    [ReflectorProperty("continueOnFailure",Required=false)]
    public bool ContinueOnFailure { get; set; }

    /// <summary>
    /// Gets or sets the project URL, used if you don't want to use the default url.
    /// </summary>
    /// <value>The project URL.</value>
    [ReflectorProperty("projectUrl",Required=false)]
    public string ProjectUrl { get; set; }
    /// <summary>
    /// Gets or sets the proxy.
    /// </summary>
    /// <value>The proxy.</value>
    [ReflectorProperty("proxy",Required=false)]
    public Proxy Proxy { get; set; }

    #region ITask Members

    public void Run ( IIntegrationResult result ) {
      try {
        TwitterService twitter = new TwitterService ( );
        if ( this.Proxy != null )
          twitter.Proxy = this.Proxy.CreateProxy ( );
        System.Xml.XmlDocument resultXml = twitter.UpdateAsXml ( this.UserName, this.Password, CreateStatus ( result ) );
        Log.Info ( "Integration results published to twitter" );
        result.AddTaskResult ( "<twitter>" + resultXml.DocumentElement.InnerXml + "</twitter>" );
      } catch ( Exception ex ) {
        if ( !ContinueOnFailure ) {
          Log.Error ( ex );
          throw;
        } else {
          Log.Warning ( ex );
        }
      }
    }

    #endregion

    /// <summary>
    /// Creates the status.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    private string CreateStatus ( IIntegrationResult result ) {
      if ( result.Status == IntegrationStatus.Success ) {
        if ( result.LastIntegrationStatus != result.Status ) {
          return String.Format ( "{0} Build Fixed: Build {1}. See {2}", result.ProjectName, result.Label, ProjectUrl ?? result.ProjectUrl  );
        } else {
          return String.Format ( "{0} Build Successful: Build {1}. See {2}", result.ProjectName, result.Label, ProjectUrl ?? result.ProjectUrl );
        }
      } else {
        return String.Format ( "{0} Build Failed. See {1}", result.ProjectName, ProjectUrl ?? result.ProjectUrl );
      }
    }

		#region IMacroRunner Members

		public MacroEngine MacroEngine {get;set;}

		/// <summary>
		/// Gets the property string.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="result">The result.</param>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		string IMacroRunner.GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
			return this.GetPropertyString<T> ( sender, result, input );
		}

		/// <summary>
		/// Gets the property string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="result">The result.</param>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		private string GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
			string ret = this.GetPropertyString<TwitterPublisher> ( this, result, input );
			ret = this.GetPropertyString<T> ( sender, result, ret );
			return ret;
		}

		#endregion
	}
}
