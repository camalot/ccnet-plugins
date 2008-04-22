using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;
using CCNet.Community.Plugins.Components.XmlRpc;
using System.Net;
using CCNet.Community.Plugins.XmlRpc;

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "metaweblog" )]
  public class MetaWeblogPublisher : ITask {
    [ReflectorProperty ( "xmlrpcurl", Required = true )]
    public string MetaWeblogApiUrl { get; set; }
    [ReflectorProperty ( "username", Required = true )]
    public string Username { get; set; }
    [ReflectorProperty ( "password", Required = true )]
    public string Password { get; set; }
    //[ReflectorArray("categories")]
    //public string[] Categories { get; set; }
    [ReflectorProperty ( "titleformat" )]
    public string TitleFormat { get; set; }
    [ReflectorProperty ( "descriptionformat" )]
    public string DescriptionFormat { get; set; }
    [ReflectorProperty ( "continueOnFailure" )]
    public bool ContinueOnFailure { get; set; }

    public MetaWeblogPublisher ( ) {
      this.ContinueOnFailure = false;
      this.DescriptionFormat = "<h4>{6} {2}: {5}</h4><h5>Status</h5>{7}<br /><h5>Modifications</h5>{0}<br /><h5>Results</h5>{1}<br /><h5>Last Changeset Number</h5>{3}<br /><h5>Total Integration Time</h5>{4}<br />";
      this.TitleFormat = "{6} {2}: {5}";
    }
    #region ITask Members

    public void Run ( IIntegrationResult result ) {
      MetaWeblogClient client = new MetaWeblogClient ( );
      NetworkCredential creds = new NetworkCredential ( this.Username, this.Password );
      client.Url = this.MetaWeblogApiUrl;
      client.Credentials = creds;
      client.KeepAlive = false;
      string blogId = string.Empty;
      try {
        BlogInfo[ ] blogs = client.getUsersBlogs ( string.Empty, creds.UserName, creds.Password );
        if ( blogs.Length > 0 ) {
          blogId = blogs[ 0 ].blogid;
        } else {
          throw new ArgumentOutOfRangeException ( "blogId", "Unable to get blog id for user." );
        }
      } catch ( Exception ex ) {
        if ( !this.ContinueOnFailure ) {
          ThoughtWorks.CruiseControl.Core.Util.Log.Error ( ex );
          throw;
        } else {
          return;
        }
      }
      
      try {
        Post post = new Post ( );
        post.categories = null;
        post.dateCreated = DateTime.Now;
        StringBuilder mods = new StringBuilder ( );
        mods.Append ( "<dl>" );
        foreach ( Modification mod in result.Modifications ) {
          mods.Append ( "<dt>" );
          mods.Append ( mod.FileName );
          mods.Append ( " : " );
          mods.Append ( mod.UserName );
          mods.Append ( "</dt>" );
          mods.Append ( "<dd>" );
          mods.Append ( "<br />" );
          mods.AppendFormat ( "<blockquote>{0}</blockquote>", mod.Comment );
          mods.Append ( "</dd>" );
        }
        mods.Append ( "</dl>" );

        StringBuilder results = new StringBuilder ( );
        foreach ( ITaskResult item in result.TaskResults ) {
          results.AppendFormat ( "<blockquote cite=\"{1}\">{0}</blockquote>",item.Data,item.Succeeded() );
          results.Append ( "<br />" );
        }

        post.description = string.Format ( this.DescriptionFormat, mods.ToString ( ), results, result.Label, result.LastChangeNumber, result.TotalIntegrationTime, result.Status, result.ProjectName, result.BuildCondition );
        post.title = string.Format ( this.TitleFormat, mods.ToString ( ), results, result.Label, result.LastChangeNumber, result.TotalIntegrationTime, result.Status, result.ProjectName, result.BuildCondition );

        client.newPost ( blogId, creds.UserName, creds.Password, post, true );
      } catch ( Exception ex ) {
        if ( !this.ContinueOnFailure ) {
          ThoughtWorks.CruiseControl.Core.Util.Log.Error ( ex );
          throw ex;
        }
      }
    }

    #endregion
  }
}
