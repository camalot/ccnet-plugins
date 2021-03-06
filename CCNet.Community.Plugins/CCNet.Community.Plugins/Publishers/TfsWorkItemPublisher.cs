﻿/*
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
using Exortech.NetReflector;
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Macros;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CCNet.Community.Plugins.Publishers {
  /// <summary>
  /// Publishes a failed build as a workitem to a tfs server
  /// </summary>
  [ReflectorType ( "workitemPublisher", Description = "Publishes results to a TFS WorkItem when build fails." )]
  public class TfsWorkItemPublisher : BasePublisherTask {

		public TfsWorkItemPublisher () {

		}
    /// <summary>
    /// Gets or sets the TFS server.
    /// </summary>
    /// <value>The TFS server.</value>
    [ReflectorProperty ( "server", Required = true )]
    public string TfsServer { get; set; }
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    [ReflectorProperty ( "username", Required = false )]
    public string UserName { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorProperty ( "password", Required = false )]
    public string Password { get; set; }
    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    /// <value>The domain.</value>
    [ReflectorProperty ( "domain", Required = false )]
    public string Domain { get; set; }
    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    /// <value>The name of the project.</value>
    [ReflectorProperty ( "project", Required = true )]
    public string ProjectName { get; set; }
    /// <summary>
    /// Gets or sets the title prefix.
    /// </summary>
    /// <value>The title prefix.</value>
    [ReflectorProperty ( "titleprefix", Required = false )]
    public string TitlePrefix { get; set; }
    #region ITask Members

    /// <summary>
    /// Runs the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    public override void Run ( IIntegrationResult result ) {
			// using a custom enum allows for supporting AllBuildConditions
			if ( this.BuildCondition != PublishBuildCondition.AllBuildConditions && string.Compare ( this.BuildCondition.ToString (), result.BuildCondition.ToString (), true ) != 0 ) {
				Log.Info ( "TfsWorkItemPublisher skipped due to build condition not met." );
				return;
			}
      if ( result.Failed ) {
        try {
          TfsServerConnection connection = new TfsServerConnection ( this, result );
          connection.Publish ( );
        } catch ( Exception ex) {
					if ( this.ContinueOnFailure ) {
						Log.Warning ( ex );
					} else {
						Log.Error ( ex );
						throw;
					}
				}
      }
    }

    #endregion
	}
}
