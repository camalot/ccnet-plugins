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


// Parts of this code are from or were inspired by :
// Yedda Twitter C# Library (or more of an API wrapper) v0.1
// Written by Eran Sandler (eran AT yedda.com)
// http://devblog.yedda.com/index.php/twitter-c-library/
// Get more cool dev information and other stuff at the Yedda Dev Blog:
// http://devblog.yedda.com


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;

namespace CCNet.Community.Plugins.Components.Twitter {
  /// <summary>
  /// 
  /// </summary>
  public class TwitterService {
    /// <summary>
    /// 
    /// </summary>
    protected const string TwitterUrlFormat = "http://twitter.com/{0}/{1}.{2}";
    protected const int MaximumStatusLength = 140;
    /// <summary>
    /// The output formats supported by Twitter. Not all of them can be used with all of the functions.
    /// For more information about the output formats and the supported functions Check the 
    /// Twitter documentation at: http://groups.google.com/group/twitter-development-talk/web/api-documentation
    /// </summary>
    public enum OutputFormatType {
      JSON,
      XML,
      RSS,
      Atom
    }

    /// <summary>
    /// The various object types supported at Twitter.
    /// </summary>
    public enum ObjectType {
      Statuses,
      Account,
      Users
    }
    /// <summary>
    /// The various actions used at Twitter. Not all actions works on all object types.
    /// For more information about the actions types and the supported functions Check the 
    /// Twitter documentation at: http://groups.google.com/group/twitter-development-talk/web/api-documentation
    /// </summary>
    public enum ActionType {
      Public_Timeline,
      User_Timeline,
      Friends_Timeline,
      Friends,
      Followers,
      Update,
      Account_Settings,
      Featured,
      Show
    }

    /// <summary>
    /// Source is an additional parameters that will be used to fill the "From" field.
    /// Currently you must talk to the developers of Twitter at:
    /// http://groups.google.com/group/twitter-development-talk/
    /// Otherwise, Twitter will simply ignore this parameter and set the "From" field to "web".
    /// </summary>
    public string Source { get { return "CCNET"; } }
    /// <summary>
    /// Sets the name of the Twitter client.
    /// According to the Twitter Fan Wiki at http://twitter.pbwiki.com/API-Docs and supported by
    /// the Twitter developers, this will be used in the future (hopefully near) to set more information
    /// in Twitter about the client posting the information as well as future usage in a clients directory.
    /// </summary>
    public string Client { get { return Path.GetFileNameWithoutExtension ( typeof ( TwitterService ).Assembly.Location ); } }
    /// <summary>
    /// Sets the version of the Twitter client.
    /// According to the Twitter Fan Wiki at http://twitter.pbwiki.com/API-Docs and supported by
    /// the Twitter developers, this will be used in the future (hopefully near) to set more information
    /// in Twitter about the client posting the information as well as future usage in a clients directory.
    /// </summary>
    public Version Version { get { return typeof ( TwitterService ).Assembly.GetName ( ).Version; } }
    /// <summary>
    /// Sets the URL of the Twitter client.
    /// Must be in the XML format documented in the "Request Headers" section at:
    /// http://twitter.pbwiki.com/API-Docs.
    /// According to the Twitter Fan Wiki at http://twitter.pbwiki.com/API-Docs and supported by
    /// the Twitter developers, this will be used in the future (hopefully near) to set more information
    /// in Twitter about the client posting the information as well as future usage in a clients directory.		
    /// </summary>
    public Uri Url { get { return new Uri ( "http://codeplex.com/ccnetplugins" ); } }

    /// <summary>
    /// Gets or sets the proxy.
    /// </summary>
    /// <value>The proxy.</value>
    public IWebProxy Proxy { get; set; }
    /// <summary>
    /// Executes an HTTP GET command and retrives the information.		
    /// </summary>
    /// <param name="url">The URL to perform the GET operation</param>
    /// <param name="userName">The username to use with the request</param>
    /// <param name="password">The password to use with the request</param>
    /// <returns>The response of the request, or null if we got 404 or nothing.</returns>
    protected string ExecuteGetCommand ( string url, string userName, string password ) {
      using ( WebClient client = new WebClient ( ) ) {
        if ( !string.IsNullOrEmpty ( userName ) && !string.IsNullOrEmpty ( password ) ) {
          client.Credentials = new NetworkCredential ( userName, password );
        }

        if ( this.Proxy != null ) {
          client.Proxy = this.Proxy;
        }

        try {
          using ( Stream stream = client.OpenRead ( url ) ) {
            using ( StreamReader reader = new StreamReader ( stream ) ) {
              return reader.ReadToEnd ( );
            }
          }
        } catch ( WebException ex ) {
          //
          // Handle HTTP 404 errors gracefully and return a null string to indicate there is no content.
          //
          if ( ex.Response is HttpWebResponse ) {
            if ( ( ex.Response as HttpWebResponse ).StatusCode == HttpStatusCode.NotFound ) {
              return null;
            }
          }

          throw;
        }
      }
    }


    /// <summary>
    /// Executes an HTTP POST command and retrives the information.		
    /// This function will automatically include a "source" parameter if the "Source" property is set.
    /// </summary>
    /// <param name="url">The URL to perform the POST operation</param>
    /// <param name="userName">The username to use with the request</param>
    /// <param name="password">The password to use with the request</param>
    /// <param name="data">The data to post</param> 
    /// <returns>The response of the request, or null if we got 404 or nothing.</returns>
    protected string ExecutePostCommand ( string url, string userName, string password, string data ) {
      WebRequest request = WebRequest.Create ( url );
      if ( !string.IsNullOrEmpty ( userName ) && !string.IsNullOrEmpty ( password ) ) {
        request.Credentials = new NetworkCredential ( userName, password );
        request.ContentType = "application/x-www-form-urlencoded";
        request.Method = WebRequestMethods.Http.Post;
        if ( this.Proxy != null ) {
          request.Proxy = this.Proxy;
        }
        if ( !string.IsNullOrEmpty ( this.Client ) ) {
          request.Headers.Add ( "X-Twitter-Client", this.Client );
        }

        if ( this.Version != null ) {
          request.Headers.Add ( "X-Twitter-Version", this.Version.ToString ( ) );
        }

        if ( this.Url != null ) {
          request.Headers.Add ( "X-Twitter-URL", this.Url.ToString ( ) );
        }


        if ( !string.IsNullOrEmpty ( this.Source ) ) {
          data += "&source=" + HttpUtility.UrlEncode ( this.Source );
        }

        byte[ ] bytes = Encoding.UTF8.GetBytes ( data );

        request.ContentLength = bytes.Length;
        using ( Stream requestStream = request.GetRequestStream ( ) ) {
          requestStream.Write ( bytes, 0, bytes.Length );

          using ( WebResponse response = request.GetResponse ( ) ) {
            using ( StreamReader reader = new StreamReader ( response.GetResponseStream ( ) ) ) {
              return reader.ReadToEnd ( );
            }
          }
        }
      }

      return null;
    }

    #region Public_Timeline

    /// <summary>
    /// Gets the public timeline.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetPublicTimeline ( OutputFormatType format ) {
      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
        Util.ToLowerString ( ActionType.Public_Timeline ), Util.ToLowerString ( format ) );
      return ExecuteGetCommand ( url, null, null );
    }

    /// <summary>
    /// Gets the public timeline as json.
    /// </summary>
    /// <returns></returns>
    public string GetPublicTimelineAsJson ( ) {
      return GetPublicTimeline ( OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the public timeline as XML.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public XmlDocument GetPublicTimelineAsXml ( OutputFormatType format ) {
      if ( format == OutputFormatType.JSON ) {
        throw new ArgumentException ( "GetPublicTimelineAsXml supports only XML based formats (XML, RSS, Atom)", "format" );
      }

      string output = GetPublicTimeline ( format );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    /// <summary>
    /// Gets the public timeline as XML.
    /// </summary>
    /// <returns></returns>
    public XmlDocument GetPublicTimelineAsXml ( ) {
      return GetPublicTimelineAsXml ( OutputFormatType.XML );
    }

    /// <summary>
    /// Gets the public timeline as RSS.
    /// </summary>
    /// <returns></returns>
    public XmlDocument GetPublicTimelineAsRss ( ) {
      return GetPublicTimelineAsXml ( OutputFormatType.RSS );
    }

    /// <summary>
    /// Gets the public timeline as atom.
    /// </summary>
    /// <returns></returns>
    public XmlDocument GetPublicTimelineAsAtom ( ) {
      return GetPublicTimelineAsXml ( OutputFormatType.Atom );
    }

    #endregion

    #region User_Timeline

    /// <summary>
    /// Gets the user timeline.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetUserTimeline ( string userName, string password, string IDorScreenName, OutputFormatType format ) {
      string url = null;
      if ( string.IsNullOrEmpty ( IDorScreenName ) ) {
        url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
          Util.ToLowerString ( ActionType.User_Timeline ), Util.ToLowerString ( format ) );
      } else {
        url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
          Util.ToLowerString ( ActionType.User_Timeline ) + "/" + IDorScreenName, Util.ToLowerString ( format ) );
      }

      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Gets the user timeline.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetUserTimeline ( string userName, string password, OutputFormatType format ) {
      return GetUserTimeline ( userName, password, null, format );
    }

    /// <summary>
    /// Gets the user timeline as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public string GetUserTimelineAsJson ( string userName, string password ) {
      return GetUserTimeline ( userName, password, OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the user timeline as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public string GetUserTimelineAsJson ( string userName, string password, string IDorScreenName ) {
      return GetUserTimeline ( userName, password, IDorScreenName, OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the user timeline as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsXml ( string userName, string password, string IDorScreenName,
      OutputFormatType format ) {
      if ( format == OutputFormatType.JSON ) {
        throw new ArgumentException ( "GetUserTimelineAsXml supports only XML based formats (XML, RSS, Atom)", "format" );
      }

      string output = GetUserTimeline ( userName, password, IDorScreenName, format );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    /// <summary>
    /// Gets the user timeline as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsXml ( string userName, string password, OutputFormatType format ) {
      return GetUserTimelineAsXml ( userName, password, null, format );
    }

    /// <summary>
    /// Gets the user timeline as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsXml ( string userName, string password, string IDorScreenName ) {
      return GetUserTimelineAsXml ( userName, password, IDorScreenName, OutputFormatType.XML );
    }

    /// <summary>
    /// Gets the user timeline as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsXml ( string userName, string password ) {
      return GetUserTimelineAsXml ( userName, password, null );
    }

    /// <summary>
    /// Gets the user timeline as RSS.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsRss ( string userName, string password, string IDorScreenName ) {
      return GetUserTimelineAsXml ( userName, password, IDorScreenName, OutputFormatType.RSS );
    }

    /// <summary>
    /// Gets the user timeline as RSS.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsRss ( string userName, string password ) {
      return GetUserTimelineAsXml ( userName, password, OutputFormatType.RSS );
    }

    /// <summary>
    /// Gets the user timeline as atom.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsAtom ( string userName, string password, string IDorScreenName ) {
      return GetUserTimelineAsXml ( userName, password, IDorScreenName, OutputFormatType.Atom );
    }

    /// <summary>
    /// Gets the user timeline as atom.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetUserTimelineAsAtom ( string userName, string password ) {
      return GetUserTimelineAsXml ( userName, password, OutputFormatType.Atom );
    }
    #endregion

    #region Friends_Timeline
    /// <summary>
    /// Gets the friends timeline.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetFriendsTimeline ( string userName, string password, OutputFormatType format ) {
      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
        Util.ToLowerString ( ActionType.Friends_Timeline ), Util.ToLowerString ( format ) );

      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Gets the friends timeline as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public string GetFriendsTimelineAsJson ( string userName, string password ) {
      return GetFriendsTimeline ( userName, password, OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the friends timeline as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public XmlDocument GetFriendsTimelineAsXml ( string userName, string password, OutputFormatType format ) {
      if ( format == OutputFormatType.JSON ) {
        throw new ArgumentException ( "GetFriendsTimelineAsXML supports only XML based formats (XML, RSS, Atom)", "format" );
      }

      string output = GetFriendsTimeline ( userName, password, format );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    /// <summary>
    /// Gets the friends timeline as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetFriendsTimelineAsXml ( string userName, string password ) {
      return GetFriendsTimelineAsXml ( userName, password, OutputFormatType.XML );
    }

    /// <summary>
    /// Gets the friends timeline as RSS.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetFriendsTimelineAsRSS ( string userName, string password ) {
      return GetFriendsTimelineAsXml ( userName, password, OutputFormatType.RSS );
    }

    /// <summary>
    /// Gets the friends timeline as atom.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetFriendsTimelineAsAtom ( string userName, string password ) {
      return GetFriendsTimelineAsXml ( userName, password, OutputFormatType.Atom );
    }

    #endregion

    #region Friends

    /// <summary>
    /// Gets the friends.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetFriends ( string userName, string password, OutputFormatType format ) {
      if ( format != OutputFormatType.JSON && format != OutputFormatType.XML ) {
        throw new ArgumentException ( "GetFriends support only XML and JSON output format", "format" );
      }

      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
        Util.ToLowerString ( ActionType.Friends ), Util.ToLowerString ( format ) );
      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Gets the friends.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetFriends ( string userName, string password, string IDorScreenName, OutputFormatType format ) {
      if ( format != OutputFormatType.JSON && format != OutputFormatType.XML ) {
        throw new ArgumentException ( "GetFriends support only XML and JSON output format", "format" );
      }

      string url = null;
      if ( string.IsNullOrEmpty ( IDorScreenName ) ) {
        url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
          Util.ToLowerString ( ActionType.Friends ), Util.ToLowerString ( format ) );
      } else {
        url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
          Util.ToLowerString ( ActionType.Friends ) + "/" + IDorScreenName, Util.ToLowerString ( format ) );
      }

      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Gets the friends as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public string GetFriendsAsJson ( string userName, string password, string IDorScreenName ) {
      return GetFriends ( userName, password, IDorScreenName, OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the friends as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public string GetFriendsAsJson ( string userName, string password ) {
      return GetFriendsAsJson ( userName, password, null );
    }

    /// <summary>
    /// Gets the friends as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public XmlDocument GetFriendsAsXml ( string userName, string password, string IDorScreenName ) {
      string output = GetFriends ( userName, password, IDorScreenName, OutputFormatType.XML );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    /// <summary>
    /// Gets the friends as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetFriendsAsXml ( string userName, string password ) {
      return GetFriendsAsXml ( userName, password, null );
    }

    #endregion

    #region Followers

    /// <summary>
    /// Gets the followers.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetFollowers ( string userName, string password, OutputFormatType format ) {
      if ( format != OutputFormatType.JSON && format != OutputFormatType.XML ) {
        throw new ArgumentException ( "GetFollowers supports only XML and JSON output format", "format" );
      }

      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
        Util.ToLowerString ( ActionType.Followers ), Util.ToLowerString ( format ) );
      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Gets the followers as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public string GetFollowersAsJson ( string userName, string password ) {
      return GetFollowers ( userName, password, OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the followers as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetFollowersAsXml ( string userName, string password ) {
      string output = GetFollowers ( userName, password, OutputFormatType.XML );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    #endregion

    #region Update

    /// <summary>
    /// Updates the specified user name with the specified status string.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="status">The status.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string Update ( string userName, string password, string status, OutputFormatType format ) {
      if ( format != OutputFormatType.JSON && format != OutputFormatType.XML ) {
        throw new ArgumentException ( "Update support only XML and JSON output format", "format" );
      }

      if ( status.Length > MaximumStatusLength ) {
        throw new ArgumentException ( string.Format ( "The maximum length of the status is {0}", MaximumStatusLength ), "status" );
      }

      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
        Util.ToLowerString ( ActionType.Update ), Util.ToLowerString ( format ) );
      string data = string.Format ( "status={0}", HttpUtility.UrlEncode ( status ) );

      return ExecutePostCommand ( url, userName, password, data );
    }

    /// <summary>
    /// Updates as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public string UpdateAsJson ( string userName, string password, string text ) {
      return Update ( userName, password, text, OutputFormatType.JSON );
    }

    /// <summary>
    /// Updates as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public XmlDocument UpdateAsXml ( string userName, string password, string text ) {
      string output = Update ( userName, password, text, OutputFormatType.XML );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    #endregion

    #region Featured

    /// <summary>
    /// Gets the featured.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string GetFeatured ( string userName, string password, OutputFormatType format ) {
      if ( format != OutputFormatType.JSON && format != OutputFormatType.XML ) {
        throw new ArgumentException ( "GetFeatured supports only XML and JSON output format", "format" );
      }

      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Statuses ),
        Util.ToLowerString ( ActionType.Featured ), Util.ToLowerString ( format ) );
      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Gets the featured as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public string GetFeaturedAsJson ( string userName, string password ) {
      return GetFeatured ( userName, password, OutputFormatType.JSON );
    }

    /// <summary>
    /// Gets the featured as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public XmlDocument GetFeaturedAsXml ( string userName, string password ) {
      string output = GetFeatured ( userName, password, OutputFormatType.XML );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    #endregion

    #region Show

    /// <summary>
    /// Shows the specified user name.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the Id or screen.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public string Show ( string userName, string password, string IDorScreenName, OutputFormatType format ) {
      if ( format != OutputFormatType.JSON && format != OutputFormatType.XML ) {
        throw new ArgumentException ( "Show supports only XML and JSON output format", "format" );
      }

      string url = string.Format ( TwitterUrlFormat, Util.ToLowerString ( ObjectType.Users ),
        Util.ToLowerString ( ActionType.Show ) + "/" + IDorScreenName, Util.ToLowerString ( format ) );
      return ExecuteGetCommand ( url, userName, password );
    }

    /// <summary>
    /// Shows as json.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public string ShowAsJson ( string userName, string password, string IDorScreenName ) {
      return Show ( userName, password, IDorScreenName, OutputFormatType.JSON );
    }

    /// <summary>
    /// Shows as XML.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="IDorScreenName">Name of the I dor screen.</param>
    /// <returns></returns>
    public XmlDocument ShowAsXml ( string userName, string password, string IDorScreenName ) {
      string output = Show ( userName, password, IDorScreenName, OutputFormatType.XML );
      if ( !string.IsNullOrEmpty ( output ) ) {
        XmlDocument xmlDocument = new XmlDocument ( );
        xmlDocument.LoadXml ( output );

        return xmlDocument;
      }

      return null;
    }

    #endregion
  }
}
