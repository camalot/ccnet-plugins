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
using Microsoft.TeamFoundation.Client;
using System.Net;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CCNet.Community.Plugins.Publishers {
  /// <summary>
  /// 
  /// </summary>
  public class TfsServerConnection {

    /// <summary>
    /// Initializes a new instance of the <see cref="TfsServerConnection"/> class.
    /// </summary>
    /// <param name="tfsworkitem">The tfsworkitem.</param>
    /// <param name="result">The result.</param>
    public TfsServerConnection ( TfsWorkItemPublisher tfsworkitem, ThoughtWorks.CruiseControl.Core.IIntegrationResult result ) {
      this.TfsWorkItem = tfsworkitem;
      this.Result = result;
    }

    /// <summary>
    /// Gets or sets the TFS work item.
    /// </summary>
    /// <value>The TFS work item.</value>
    public TfsWorkItemPublisher TfsWorkItem { get; set; }
    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    /// <value>The result.</value>
    public ThoughtWorks.CruiseControl.Core.IIntegrationResult Result { get; set; }

    /// <summary>
    /// Publishes this instance.
    /// </summary>
    public void Publish ( ) {
      TeamFoundationServer tfs = this.CreateServer ( );
      WorkItemStore store = tfs.GetService ( typeof ( WorkItemStore ) ) as WorkItemStore;

      int projectId = this.GetProjectIdByName ( store, this.TfsWorkItem.ProjectName );
      if ( projectId > -1 ) {
        Project project = store.Projects.GetById ( projectId );

        WorkItem wi = new WorkItem ( project.WorkItemTypes[ 0 ] );
        wi.Title = string.Format ( "{0}Build Failed for {1}", this.TfsWorkItem.TitlePrefix, this.Result.Label );

        StringBuilder results = new StringBuilder ( );

        foreach ( ThoughtWorks.CruiseControl.Core.ITaskResult itr in this.Result.TaskResults ) {
          if ( itr.Failed ( ) )
            results.AppendLine ( itr.Data );
        }

        wi.Description = results.ToString ( );

        wi.Save ( );
        tfs.Dispose ( );
      } else {
        throw new ThoughtWorks.CruiseControl.Core.CruiseControlException ( string.Format ( "Unable to find project {0} on Team Foundation Server", this.TfsWorkItem.ProjectName ) );
      }
        
    }

    /// <summary>
    /// Creates the server.
    /// </summary>
    /// <returns></returns>
    private TeamFoundationServer CreateServer ( ) {
      ICredentials creds = null;
      if ( !string.IsNullOrEmpty(this.TfsWorkItem.UserName) ) {
        creds= new NetworkCredential(this.TfsWorkItem.UserName,this.TfsWorkItem.Password,this.TfsWorkItem.Domain);
      }
      TeamFoundationServer tfs = new TeamFoundationServer ( this.TfsWorkItem.TfsServer, creds );
      tfs.EnsureAuthenticated ( );
      return tfs;
    }

    /// <summary>
    /// Gets the name of the project id by.
    /// </summary>
    /// <param name="store">The store.</param>
    /// <param name="projectName">Name of the project.</param>
    /// <returns></returns>
    private int GetProjectIdByName ( WorkItemStore store, string projectName ) {
      foreach ( Project proj in store.Projects ) {
        if ( string.Compare ( proj.Name, projectName ) == 0 )
          return proj.Id;
      }
      return -1;
    }
  }
}
