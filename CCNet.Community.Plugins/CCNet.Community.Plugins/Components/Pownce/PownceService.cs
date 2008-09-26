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
using System.IO;
using System.Xml;

namespace CCNet.Community.Plugins.Components.Pownce {
	/// <summary>
	/// 
	/// </summary>
	public class PownceService {
		private const string DEFAULT_SEND_TO = "public";
		private const string APIURI = "http://api.pownce.com/";
		private const string APIVERSION  = "2.1";
		private const string APIURIFORMAT = "{0}{1}{2}?{3}";


		/// <summary>
		/// Initializes a new instance of the <see cref="PownceService"/> class.
		/// </summary>
		/// <param name="apiKey">The API key.</param>
		/// <param name="credentials">The credentials.</param>
		public PownceService ( string apiKey, ICredentials credentials ) {
			this.ApplicationKey = apiKey;
			this.QueryParams = new Dictionary<string, string> ();
			this.PostParams = new Dictionary<string, string> ();
			this.Headers = new Dictionary<string, string> ();
			if ( credentials == null )
				throw new ArgumentNullException ( "credentials" );
			this.Credentials = credentials;
			this.Timeout = 60 * 1000;
			this.DefaultSendTo = this.GetDefaultSendTo ();

		}

		#region Properties
		/// <summary>
		/// Gets or sets the default send to.
		/// </summary>
		/// <value>The default send to.</value>
		public string DefaultSendTo { get; private set; }
		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		public int Timeout { get; set; }

		/// <summary>
		/// Gets or sets the credentials.
		/// </summary>
		/// <value>The credentials.</value>
		public ICredentials Credentials { get; set; }

		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		public IWebProxy Proxy { get; set; }
		/// <summary>
		/// Gets or sets the post params.
		/// </summary>
		/// <value>The post params.</value>
		internal Dictionary<string, string> PostParams { get; set; }

		/// <summary>
		/// Gets or sets the query params.
		/// </summary>
		/// <value>The query params.</value>
		internal Dictionary<string, string> QueryParams { get; set; }

		/// <summary>
		/// Gets or sets the headers.
		/// </summary>
		/// <value>The headers.</value>
		internal Dictionary<string, string> Headers { get; set; }
		/// <summary>
		/// Gets or sets the application key.
		/// </summary>
		/// <value>The application key.</value>
		public string ApplicationKey { get; private set; }
		#endregion

		#region API Methods
		/// <summary>
		/// Posts the text.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public string PostText ( string message ) {
			return PostText ( message, this.DefaultSendTo );
		}

		/// <summary>
		/// Posts the text.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="to">To.</param>
		/// <returns></returns>
		public string PostText ( string message, string to ) {
			if ( string.IsNullOrEmpty ( message ) )
				throw new ArgumentNullException ( "message" );
			this.QueryParams.Add ( "app_key", this.ApplicationKey );
			this.PostParams.Add ( "note_to", string.IsNullOrEmpty ( to ) ? this.DefaultSendTo : to );
			this.PostParams.Add ( "note_body", message );
			return Request ( "/send/message.xml", WebRequestMethods.Http.Post );
		}

		/// <summary>
		/// Posts the link.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public string PostLink ( Uri url ) {
			return PostLink ( string.Empty, url );
		}

		/// <summary>
		/// Posts the link.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public string PostLink ( string message, Uri url ) {
			return PostLink ( message, url, this.DefaultSendTo );
		}

		/// <summary>
		/// Posts the link.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="url">The URL.</param>
		/// <param name="to">To.</param>
		/// <returns></returns>
		public string PostLink ( string message, Uri url, string to ) {
			if ( url == null )
				throw new ArgumentNullException ( "url" );
			this.QueryParams.Add ( "app_key", this.ApplicationKey );
			this.PostParams.Add ( "note_to", string.IsNullOrEmpty ( to ) ? this.DefaultSendTo : to );
			this.PostParams.Add ( "url", url.ToString () );
			if ( !string.IsNullOrEmpty ( message ) )
				this.PostParams.Add ( "note_body", message );
			return Request ( "/send/link.xml", WebRequestMethods.Http.Post );
		}

		/// <summary>
		/// Posts the event.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		public string PostEvent ( string name, string location, DateTime date ) {
			return PostEvent ( string.Empty, name, location, date );
		}

		/// <summary>
		/// Posts the event.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		public string PostEvent ( string message, string name, string location, DateTime date ) {
			return PostEvent ( message, name, location, date, this.DefaultSendTo );
		}

		/// <summary>
		/// Posts the event.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="date">The date.</param>
		/// <param name="to">To.</param>
		/// <returns></returns>
		public string PostEvent ( string message, string name, string location, DateTime date, string to ) {
			if ( string.IsNullOrEmpty ( name ) )
				throw new ArgumentNullException ( "name" );
			if ( string.IsNullOrEmpty ( location ) )
				throw new ArgumentNullException ( "location" );
			if ( date.Date < DateTime.Today.Date ) {
				throw new ArgumentException ( "This event occurs in the past." );
			}
				this.QueryParams.Add ( "app_key", this.ApplicationKey );
			this.PostParams.Add ( "note_to", string.IsNullOrEmpty ( to ) ? this.DefaultSendTo : to );
			this.PostParams.Add ( "event_name", name );
			this.PostParams.Add ( "event_location", location );
			this.PostParams.Add ( "event_date", date.ToString ( "yyyy-MM-dd hh:mm" ) );
			if ( !string.IsNullOrEmpty ( message ) )
				this.PostParams.Add ( "note_body", message );
			return Request ( "/send/event.xml", WebRequestMethods.Http.Post );
		}

		/// <summary>
		/// Posts the file.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="file">The file.</param>
		/// <param name="to">To.</param>
		/// <param name="usePro">if set to <c>true</c> [use pro].</param>
		/// <returns></returns>
		public string PostFile ( string message, string file, string to, bool usePro ) {
			if ( string.IsNullOrEmpty ( file ) )
				throw new ArgumentNullException ( "file" );
			if ( !File.Exists ( file ) )
				throw new FileNotFoundException ( "The file was not found.", file );

			this.QueryParams.Add ( "app_key", this.ApplicationKey );

			this.PostParams.Add ( "note_to", string.IsNullOrEmpty ( to ) ? this.DefaultSendTo : to );
			if ( !string.IsNullOrEmpty ( message ) )
				this.PostParams.Add ( "note_body", message );

			FileInfo f = new FileInfo ( file );
			//this.Headers.Add ( "Content-Disposition", string.Format ( "filename=\"{1}\"", Path.GetFileNameWithoutExtension ( file ), f.Name ) );
			//this.PostParams.Add ( "media_file", Encoding.Default.GetString ( ReadFile ( f ) ) );
			//return Request ( string.Format ( "/send/file{0}.xml", usePro ? "_pro" : string.Empty ), WebRequestMethods.Http.Post, "multipart/form-data" );
			this.PostParams.Add ( "media_file", f.FullName );
			return Request ( string.Format ( "/send/file{0}.xml", usePro ? "_pro" : string.Empty ), f );
		}

		/// <summary>
		/// Posts the file.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="file">The file.</param>
		/// <param name="to">To.</param>
		/// <returns></returns>
		public string PostFile ( string message, string file, string to ) {
			return PostFile ( message, file, to, false );
		}


		/// <summary>
		/// Gets the send to list.
		/// </summary>
		/// <returns></returns>
		public List<SendToOption> GetSendToList () {
			this.QueryParams.Add ( "app_key", this.ApplicationKey );
			XmlDocument doc = new XmlDocument ();
			string resp = Request ( "/send/send_to.xml", WebRequestMethods.Http.Get );
			doc.LoadXml ( resp );
			List<SendToOption> options = new List<SendToOption> ();
			if ( doc.DocumentElement != null && string.Compare ( doc.DocumentElement.Name, "send_to", false ) == 0 ) {
				XmlNode selectedEle = doc.DocumentElement.SelectSingleNode ( "selected" );
				string selected = string.Empty;
				if ( selectedEle != null ) {
					selected = selectedEle.InnerText;
				}
				XmlNodeList optionsNodes = doc.DocumentElement.SelectNodes ( "options/*" );
				foreach ( XmlElement ele in optionsNodes ) {
					SendToOption sto = new SendToOption ( ele.Name, ele.InnerText, string.Compare ( selected, ele.InnerText, false ) == 0 );
					options.Add ( sto );
				}
				options.Add ( new SendToOption ( "all", "All", false ) );
			} else if ( doc.DocumentElement != null && string.Compare ( doc.DocumentElement.Name, "error", false ) == 0 ) {
				throw new Exception ( doc.DocumentElement.SelectSingleNode ( "message" ).InnerText );
			} else {
				throw new ApplicationException ( "Unable to get send to list." );
			}
			return options;
		}

		/// <summary>
		/// Requests the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public string Request ( string path, string method ) {
			return Request ( path, method, "application/x-www-form-urlencoded" );
		}

		public string Request ( string path, FileInfo file ) {
			StringBuilder respData = new StringBuilder ();

			try {
				string uri = string.Format ( APIURIFORMAT, APIURI, APIVERSION, path, ToQueryString ( this.QueryParams ) );
				HttpWebRequest req = HttpWebRequest.Create ( uri ) as HttpWebRequest;
				req.Timeout = this.Timeout;
				req.Method = WebRequestMethods.Http.Post;
				req.KeepAlive = false;
				if ( this.Proxy != null ) {
					req.Proxy = this.Proxy;
				}
				string postData = ToQueryString ( this.PostParams );
				req.Credentials = this.Credentials;

				foreach ( string skey in this.Headers.Keys ) {
					req.Headers.Add ( skey, this.Headers[ skey ] );
				}

				req.ContentType = new MimetypesFileTypeMap ().GetContentType ( file );
				req.ContentLength = postData.Length + file.Length;
				using ( StreamWriter sw = new StreamWriter ( req.GetRequestStream (), Encoding.Default ) ) {
					sw.Write ( postData );
					using ( Stream rs = req.GetRequestStream () ) {
						using ( FileStream fs = file.OpenRead () ) {
							BinaryCopier.Copy ( fs, rs );
						}
					}
				}

				HttpWebResponse resp = req.GetResponse () as HttpWebResponse;
				StreamReader reader = new StreamReader ( resp.GetResponseStream () );
				using ( resp ) {
					using ( reader ) {
						while ( !reader.EndOfStream ) {
							respData.AppendLine ( reader.ReadLine () );
						}
					}
				}
			} catch ( Exception e ) {
				throw new ApplicationException ( respData.ToString () + "::::::" + e.Message );
				throw;
			} finally {
				this.Headers.Clear ();
				this.QueryParams.Clear ();
				this.PostParams.Clear ();
			}

			return respData.ToString ();
		}

		/// <summary>
		/// Requests the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="method">The method.</param>
		/// <param name="contenttype">The contenttype.</param>
		/// <returns></returns>
		public string Request ( string path, string method, string contenttype ) {
			StringBuilder respData = new StringBuilder ();

			try {
				string uri = string.Format ( APIURIFORMAT, APIURI, APIVERSION, path, ToQueryString ( this.QueryParams ) );
				HttpWebRequest req = HttpWebRequest.Create ( uri ) as HttpWebRequest;
				req.Timeout = this.Timeout;
				req.Method = method;
				req.KeepAlive = false;
				if ( this.Proxy != null ) {
					req.Proxy = this.Proxy;
				}
				string postData = ToQueryString ( this.PostParams );
				req.Credentials = this.Credentials;

				foreach ( string skey in this.Headers.Keys ) {
					req.Headers.Add ( skey, this.Headers[ skey ] );
				}

				if ( string.Compare ( method, WebRequestMethods.Http.Post, true ) == 0 ) {
					req.ContentType = contenttype;
					req.ContentLength = postData.Length;
					using ( StreamWriter sw = new StreamWriter ( req.GetRequestStream (), Encoding.Default ) ) {
						sw.Write ( postData );
					}
				}

				HttpWebResponse resp = req.GetResponse () as HttpWebResponse;
				StreamReader reader = new StreamReader ( resp.GetResponseStream () );
				using ( resp ) {
					using ( reader ) {
						while ( !reader.EndOfStream ) {
							respData.AppendLine ( reader.ReadLine () );
						}
					}
				}
			} catch ( Exception e) {
				throw new ApplicationException ( respData.ToString () + "::::::" + e.Message );
				throw;
			} finally {
				this.Headers.Clear ();
				this.QueryParams.Clear ();
				this.PostParams.Clear ();
			}

			return respData.ToString ();
		}
		#endregion
		/// <summary>
		/// Encodes the string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		internal string EncodeString ( string text ) {
			return Convert.ToBase64String ( Encoding.UTF8.GetBytes ( text ) );
		}

		internal string EncodeFile ( FileInfo file ) {
			StringBuilder encoded = new StringBuilder ();
			using ( FileStream fs = file.OpenRead () ) {
				using ( MemoryStream ms = new MemoryStream () ) {
					int i = 0;
					byte[] buff = new byte[ 5 * 1024 ];
					while ( ( i = fs.Read ( buff, 0, buff.Length ) ) > 0 ) {
						ms.Write ( buff, 0, i );
					}
					ms.Position = 0;
					encoded.Append ( Convert.ToBase64String ( ms.ToArray () ) );
				}
			}
			return encoded.ToString ();
		}

		internal byte[] ReadFile ( FileInfo file ) {
			byte[] data = null;
			using ( FileStream fs = file.OpenRead () ) {
				using ( MemoryStream ms = new MemoryStream () ) {
					int i = 0;
					byte[] buff = new byte[ 5 * 1024 ];
					while ( ( i = fs.Read ( buff, 0, buff.Length ) ) > 0 ) {
						ms.Write ( buff, 0, i );
					}
					ms.Position = 0;
					data = ms.ToArray ();
				}
			}
			return data;
		}

		/// <summary>
		/// Converts a dictionary to a query string
		/// </summary>
		/// <param name="dict">The dict.</param>
		/// <returns></returns>
		internal string ToQueryString ( Dictionary<string, string> dict ) {
			if ( dict.Count == 0 )
				return string.Empty;
			string[] nvPair = new string[ dict.Count ];
			int i = 0;
			foreach ( KeyValuePair<string, string> e in dict )
				nvPair[ i++ ] = e.Key + "=" + System.Web.HttpUtility.UrlEncode ( e.Value );
			return string.Join ( "&", nvPair );
		}

		/// <summary>
		/// Gets the default send to value.
		/// </summary>
		/// <returns></returns>
		internal string GetDefaultSendTo () {
			try {
				List<SendToOption> options = this.GetSendToList ();
				foreach ( SendToOption sto in options ) {
					if ( sto.Selected )
						return sto.Name;
				}
			} catch { }
			return DEFAULT_SEND_TO;
		}
	}
}
