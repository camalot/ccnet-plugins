using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector;
using CCNet.Community.Plugins.Components.XmlRpc;
using System.Net;
using CCNet.Community.Plugins.XmlRpc;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "metaweblog" )]
  public class MetaWeblogPublisher : ITask {
    [ReflectorProperty ( "url", Required = true )]
    public string MetaWeblogApiUrl { get; set; }
    [ReflectorProperty ( "username", Required = true )]
    public string Username { get; set; }
    [ReflectorProperty ( "password", Required = true )]
    public string Password { get; set; }
    [ReflectorProperty ( "titleformat" , Required = false) ]
    public string TitleFormat { get; set; }
    [ReflectorProperty ( "descriptionformat", Required = false )]
    public string DescriptionFormat { get; set; }
    [ReflectorProperty ( "continueOnFailure", Required = false )]
    public bool ContinueOnFailure { get; set; }
    [ReflectorArray ( "tags", Required = false )]
    public string[ ] Tags { get; set; }

    public MetaWeblogPublisher ( ) {
      this.ContinueOnFailure = false;
      this.DescriptionFormat = Properties.Settings.Default.MetaWeblogDefaultDescriptionFormat;
      this.TitleFormat = Properties.Settings.Default.MetaWeblogDefaultTitleFormat;
      this.Tags = new string[ 0 ];
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
          Log.Error ( ex );
          throw;
        } else {
          Log.Warning ( ex );
          return;
        }
      }

      try {
        Post post = new Post ( );
        post.categories = null;
        post.dateCreated = DateTime.Now;
        StringBuilder mods = new StringBuilder ( );
        foreach ( Modification mod in result.Modifications ) {
          mods.Append ( "<div class=\"Modification\"><cite>" );
          mods.Append ( mod.FileName );
          mods.Append ( " : " );
          mods.Append ( mod.UserName );
          mods.Append ( "</cite>" );
          mods.AppendFormat ( "<blockquote>{0}</blockquote>", mod.Comment );
          mods.Append ( "</div>" );
        }

        StringBuilder results = new StringBuilder ( );
        foreach ( ITaskResult item in result.TaskResults ) {
          results.Append ( "<div class=\"TaskResult\">" );
          results.AppendFormat ( "<blockquote cite=\"{1}\">{0}</blockquote>", item.Data, item.Succeeded ( ) );
          results.Append ( "</div>" );
        }

        StringBuilder tags = new StringBuilder ( );
        if ( this.Tags != null && this.Tags.Length > 0 ) {
          foreach ( string tag in this.Tags ) {
            tags.AppendFormat ( Properties.Settings.Default.MetaWeblogDefaultTagFormat, tag );
          }
        }

        post.description = string.Format ( this.DescriptionFormat, mods.ToString ( ), results, result.Label, result.LastChangeNumber, result.TotalIntegrationTime, result.Status, result.ProjectName, result.BuildCondition,tags.ToString() );
        post.title = string.Format ( this.TitleFormat, mods.ToString ( ), results, result.Label, result.LastChangeNumber, result.TotalIntegrationTime, result.Status, result.ProjectName, result.BuildCondition,tags.ToString() );

        client.newPost ( blogId, creds.UserName, creds.Password, post, true );
      } catch ( Exception ex ) {
        if ( !this.ContinueOnFailure ) {
          ThoughtWorks.CruiseControl.Core.Util.Log.Error ( ex );
          throw ex;
        } else {
          Log.Warning ( ex );
        }
      }
    }

    #endregion
  }
}
