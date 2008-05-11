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
using System.Net;
using Exortech.NetReflector;
using System.IO;
using System.Xml;

namespace CCNet.Community.Plugins.Publishers {
  /// <summary>
  /// Represents the hosts to ping and provides a way to do that.
  /// </summary>
  [ReflectorType ( "pingItem" )]
  public class PingElement {
    private string _pingUrl = string.Empty;
    private string _feedUrl = string.Empty;
    private string _feedName = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingElement"/> class.
    /// </summary>
    public PingElement () {

    }

    /// <summary>
    /// Gets or sets the ping host.
    /// </summary>
    /// <value>The ping host.</value>
    [ReflectorProperty ( "pingUrl", Required = true )]
    public string PingUrl { get { return this._pingUrl; } set { this._pingUrl = value; } }

    /// <summary>
    /// Gets or sets the feed URL.
    /// </summary>
    /// <value>The feed URL.</value>
    [ReflectorProperty ( "feedUrl", Required = true )]
    public string FeedUrl { get { return this._feedUrl; } set { this._feedUrl = value; } }

    /// <summary>
    /// Gets or sets the name of the feed.
    /// </summary>
    /// <value>The name of the feed.</value>
    [ReflectorProperty ( "feedName", Required = true )]
    public string FeedName { get { return this._feedName; } set { this._feedName = value; } }

    /// <summary>
    /// Sends the ping request.
    /// </summary>
    public void Send () {
      string result = string.Empty;
      try {
        HttpWebRequest request = HttpWebRequest.Create ( this.PingUrl ) as HttpWebRequest;
        request.UserAgent = string.Format ( "{0} version {1} - http://codeplex.com/ccnetplugins", this.GetType ().Assembly.GetName ().Name, this.GetType ().Assembly.GetName ().Version.ToString () );
        request.Method = "POST";
        request.ContentType = "text/xml";
        XmlTextWriter xmlPing = new XmlTextWriter ( request.GetRequestStream (), Encoding.UTF8 );
        using ( xmlPing ) {
          xmlPing.WriteStartDocument ();
          xmlPing.WriteStartElement ( "methodCall" );
          xmlPing.WriteElementString ( "methodName", "weblogUpdates.ping" );
          xmlPing.WriteStartElement ( "params" );
          xmlPing.WriteStartElement ( "param" );
          xmlPing.WriteElementString ( "value", FeedName );
          xmlPing.WriteEndElement ();
          xmlPing.WriteStartElement ( "param" );
          xmlPing.WriteElementString ( "value", FeedUrl );
          xmlPing.WriteEndElement ();
          xmlPing.WriteEndElement ();
          xmlPing.WriteEndElement ();
        }

        HttpWebResponse response = request.GetResponse () as HttpWebResponse;
        using ( response ) {
          StreamReader streamPingResponse = new StreamReader ( response.GetResponseStream () );
          using ( streamPingResponse ) {
            result = streamPingResponse.ReadToEnd ();
          }
        }
      } catch ( Exception ex ) {
        result = ex.Message;
      }

      ThoughtWorks.CruiseControl.Core.Util.Log.Info ( string.Format ( "Ping ('{0}') response: {1}", this.PingUrl, result ) );
    }

    
  }
}
