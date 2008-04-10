using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "workitemPublisher", Description = "Publishes results to a TFS WorkItem when build fails." )]
  public class TfsWorkItemPublisher : ITask {
    [ReflectorProperty ( "server", Required = true )]
    public string TfsServer { get; set; }
    [ReflectorProperty ( "username", Required = false )]
    public string UserName { get; set; }
    [ReflectorProperty ( "password", Required = false )]
    public string Password { get; set; }
    [ReflectorProperty ( "domain", Required = false )]
    public string Domain { get; set; }
    [ReflectorProperty ( "project", Required = true )]
    public string ProjectName { get; set; }
    [ReflectorProperty ( "titleprefix", Required = false )]
    public string TitlePrefix { get; set; }
    #region ITask Members

    public void Run ( IIntegrationResult result ) {
      if ( result.Failed ) {
        string title = string.Format ( "{0}{1} Failed @ {2}", this.TitlePrefix, result.ProjectName, DateTime.Now.ToString ( "g" ) );
        StringBuilder results = new StringBuilder ( );

        foreach ( ITaskResult itr in result.TaskResults ) {
          if ( itr.Failed() )
            results.AppendLine ( itr.Data );
        }

        string body = string.Format ( "{0}", results.ToString ( ) );
      }
    }

    #endregion
  }
}
