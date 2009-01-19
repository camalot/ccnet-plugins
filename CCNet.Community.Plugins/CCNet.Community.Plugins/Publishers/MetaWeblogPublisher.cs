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
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Macros;

namespace CCNet.Community.Plugins.Publishers {
  /// <summary>
  /// Publishes the build results to a blog supporting the MetaWeblog API
  /// </summary>
  [ReflectorType ( "metaweblog" )]
	public class MetaWeblogPublisher : BasePublisherTask {

		public MetaWeblogPublisher () {
			this.Timeout = 30;
		}
    /// <summary>
    /// Gets or sets the meta weblog API URL.
    /// </summary>
    /// <value>The meta weblog API URL.</value>
    [ReflectorProperty ( "url", Required = true )]
    public string MetaWeblogApiUrl { get; set; }
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>The username.</value>
    [ReflectorProperty ( "username", Required = true )]
    public string Username { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorProperty ( "password", Required = true )]
    public string Password { get; set; }
    /// <summary>
    /// Gets or sets the title format.
    /// </summary>
    /// <value>The title format.</value>
    [ReflectorProperty ( "titleformat" , Required = false) ]
    public string TitleFormat { get; set; }
    /// <summary>
    /// Gets or sets the description format.
    /// </summary>
    /// <value>The description format.</value>
    [ReflectorProperty ( "descriptionformat", Required = false )]
    public string DescriptionFormat { get; set; }
    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    /// <value>The tags.</value>
    [ReflectorArray ( "tags", Required = false )]
    public string[ ] Tags { get; set; }
    /// <summary>
    /// Gets or sets the proxy.
    /// </summary>
    /// <value>The proxy.</value>
    [ReflectorProperty("proxy",Required=false)]
    public Proxy Proxy { get; set; }

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		[ReflectorProperty ( "timeout", Required = false )]
		public int Timeout { get; set; }


    public MetaWeblogPublisher ( ) {
      this.DescriptionFormat = Properties.Settings.Default.MetaWeblogDefaultDescriptionFormat;
      this.TitleFormat = Properties.Settings.Default.MetaWeblogDefaultTitleFormat;
      this.Tags = new string[ 0 ];
    }

    #region ITask Members

    public override void Run ( IIntegrationResult result ) {
			// using a custom enum allows for supporting AllBuildConditions
			if ( this.BuildCondition != PublishBuildCondition.AllBuildConditions && string.Compare ( this.BuildCondition.ToString (), result.BuildCondition.ToString (), true ) != 0 ) {
				Log.Info ( "MetaWeblogPublisher skipped due to build condition not met." );
				return;
			}
      MetaWeblogClient client = new MetaWeblogClient ( );
			client.Timeout = this.Timeout * 1000;
			NetworkCredential creds = new NetworkCredential ( this.GetPropertyString<IMacroRunner> ( this, result, this.Username ), this.GetPropertyString<IMacroRunner> ( this, result, this.Password ) );
			client.Url = this.GetPropertyString<IMacroRunner> ( this, result, this.MetaWeblogApiUrl );
      client.Credentials = creds;
      client.KeepAlive = false;

      if ( this.Proxy != null )
        client.Proxy = this.Proxy.CreateProxy ( );

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
            tags.AppendFormat ( Properties.Settings.Default.MetaWeblogDefaultTagFormat, this.GetPropertyString<IMacroRunner> ( this, result, tag ) );
          }
        }

				post.description = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, this.DescriptionFormat ), mods.ToString (), results, result.Label, result.LastChangeNumber, result.TotalIntegrationTime, result.Status, result.ProjectName, result.BuildCondition, this.GetPropertyString<IMacroRunner> ( this, result, tags.ToString () ) );
				post.title = string.Format ( this.GetPropertyString<IMacroRunner> ( this, result, this.TitleFormat ), mods.ToString (), results, result.Label, result.LastChangeNumber, result.TotalIntegrationTime, result.Status, result.ProjectName, result.BuildCondition, this.GetPropertyString<IMacroRunner> ( this, result, tags.ToString () ) );

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
