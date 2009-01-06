﻿using System;
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
	public class MetaWeblogPublisher : ITask, IMacroRunner {
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
    /// Gets or sets a value indicating whether [continue on failure].
    /// </summary>
    /// <value><c>true</c> if [continue on failure]; otherwise, <c>false</c>.</value>
    [ReflectorProperty ( "continueOnFailure", Required = false )]
    public bool ContinueOnFailure { get; set; }
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

    public MetaWeblogPublisher ( ) {
      this.ContinueOnFailure = false;
      this.DescriptionFormat = Properties.Settings.Default.MetaWeblogDefaultDescriptionFormat;
      this.TitleFormat = Properties.Settings.Default.MetaWeblogDefaultTitleFormat;
      this.Tags = new string[ 0 ];
			this.MacroEngine = new MacroEngine ();
    }

    #region ITask Members

    public void Run ( IIntegrationResult result ) {
      MetaWeblogClient client = new MetaWeblogClient ( );
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

		#region IMacroRunner Members

		/// <summary>
		/// Gets the macro engine.
		/// </summary>
		/// <value>The macro engine.</value>
		public MacroEngine MacroEngine { get; private set; }

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
			string ret = this.MacroEngine.GetPropertyString<MetaWeblogPublisher> ( this, result, input );
			ret = this.GetPropertyString<T> ( sender, result, ret );
			return ret;
		}
		#endregion

	}
}
