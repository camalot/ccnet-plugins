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
using System.IO;
using System.Text.RegularExpressions;

namespace CCNet.Community.Plugins.Components.Ftp {

	/// <summary>
	/// A wrapper of the <see cref="System.Net.FtpWebRequest"/> and <see cref="System.Net.FtpWebResponse"/> classes
	/// to simplify ftp requests
	/// </summary>
	public class FtpWebRequest : WebRequest {
		/// <summary>
		/// Standard FTP
		/// </summary>
		public const string UriSchemeFtp = "ftp";
		/// <summary>
		/// Ftp over SSH ( not supported )
		/// </summary>
		public const string UriSchemeSshFtp = "sftp";
		/// <summary>
		/// Ftp over SSL ( supported )
		/// </summary>
		public const string UriSchemeFtpSsl = "ftps";
		/// <summary>
		/// Initializes a new instance of the <see cref="FtpWebRequest"/> class.
		/// </summary>
		public FtpWebRequest () {
			EnableSsl = false;
			UsePassive = false;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use passive mode.
		/// </summary>
		/// <value><c>true</c> if [use passive]; otherwise, <c>false</c>.</value>
		public bool UsePassive { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [enable SSL].
		/// </summary>
		/// <value><c>true</c> if [enable SSL]; otherwise, <c>false</c>.</value>
		public bool EnableSsl { get; set; }

		/// <summary>
		/// Lists the directory.
		/// </summary>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <returns></returns>
		public List<FtpSystemInfo> ListDirectory ( Uri ftpUrl ) {
			List<FtpSystemInfo> items = new List<FtpSystemInfo> ();
			FtpWebResponse data = Request ( ftpUrl, System.Net.WebRequestMethods.Ftp.ListDirectoryDetails );
			//throw new Exception ( data.Data );
			Regex regex = new Regex ( Properties.Resources.FtpDirectoryListRegexPattern,
				RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline );
			Match match = regex.Match ( data.Data );
			while ( match.Success ) {
				string perm = match.Groups[ 0 ].Value;
				bool isDirectory = perm.StartsWith ( "d" );
				string[] perms = new string[ 3 ];
				// owner
				perms[ 0 ] = new String ( new char[] { perm[ 1 ], perm[ 2 ], perm[ 3 ] } );
				// group
				perms[ 1 ] = new String ( new char[] { perm[ 4 ], perm[ 5 ], perm[ 6 ] } );
				// public
				perms[ 2 ] = new String ( new char[] { perm[ 7 ], perm[ 8 ], perm[ 9 ] } );
				FtpSystemInfoPermission owner = FtpSystemInfoPermission.FromString ( perms[ 0 ] );
				FtpSystemInfoPermission group = FtpSystemInfoPermission.FromString ( perms[ 1 ] );
				FtpSystemInfoPermission pub = FtpSystemInfoPermission.FromString ( perms[ 2 ] );

				FtpSystemInfo fsi = null;
				DateTime lastMod = GetLastModificationDateTime ( match.Groups[ 7 ].Value.Trim () );
				if ( isDirectory ) {
					fsi = new FtpDirectoryInfo ( match.Groups[ 8 ].Value.Trim (), 0, owner, group, pub, lastMod );
					fsi.Url = ftpUrl;
				} else {
					long size = 0;
					long.TryParse ( match.Groups[ 6 ].Value, out size );
					fsi = new FtpFileInfo ( match.Groups[ 8 ].Value.Trim (), size, owner, group, pub, lastMod );
					fsi.Url = ftpUrl;
				}

				items.Add ( fsi );
				match = match.NextMatch ();
			}
			return items;
		}

		private DateTime GetLastModificationDateTime ( string dt ) {
			return DateTime.Parse ( dt );
		}

		/// <summary>
		/// Downloads the file.
		/// </summary>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <param name="localFile">The local file.</param>
		public void DownloadFile ( Uri ftpUrl, string localFile ) {
			try {
				FileInfo local = new FileInfo ( localFile );
				FtpWebResponse data = Request ( ftpUrl, System.Net.WebRequestMethods.Ftp.DownloadFile );
				FileStream fs = new FileStream ( local.FullName, FileMode.Create, FileAccess.Write, FileShare.Read );
				using ( fs ) {
					fs.Write ( data.ByteData, 0, data.ByteData.Length );
					fs.Flush ();
				}
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Makes the directory.
		/// </summary>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <param name="newDirectory">The new directory.</param>
		/// <returns></returns>
		public bool MakeDirectory ( Uri ftpUrl, string newDirectory ) {
			Uri newUrl = new Uri ( ftpUrl.ToString () + "/" + newDirectory );
			try {
				FtpWebResponse resp = Request ( newUrl, System.Net.WebRequestMethods.Ftp.MakeDirectory );
			} catch ( System.Net.WebException ) {
				throw;
			}
			return true;
		}

		/// <summary>
		/// Removes the directory.
		/// </summary>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <returns></returns>
		public bool RemoveDirectory ( Uri ftpUrl ) {
			try {
				FtpWebResponse resp = Request ( ftpUrl, System.Net.WebRequestMethods.Ftp.RemoveDirectory );
			} catch ( System.Net.WebException ) {
				throw;
			}
			return true;
		}

		/// <summary>
		/// Deletes the file.
		/// </summary>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <returns></returns>
		public bool DeleteFile ( Uri ftpUrl ) {
			try {
				FtpWebResponse resp = Request ( ftpUrl, System.Net.WebRequestMethods.Ftp.DeleteFile );
			} catch ( System.Net.WebException ) {
				throw;
			}
			return true;
		}

		/// <summary>
		/// Uploads the file.
		/// </summary>
		/// <param name="localFile">The local file.</param>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <returns></returns>
		public bool UploadFile ( string localFile, Uri ftpUrl ) {
			try {
				UploadFile ( new FileInfo ( localFile ), ftpUrl );
			} catch {
				throw;
			}
			return true;
		}

		/// <summary>
		/// Uploads the file.
		/// </summary>
		/// <param name="localFile">The local file.</param>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <returns></returns>
		public bool UploadFile ( FileInfo localFile, Uri ftpUrl ) {
			try {
				using ( Stream s = localFile.OpenRead () ) {
					try {
						UploadFile ( s, ftpUrl );
					} catch {
						throw;
					}
				}
			} catch {
				throw;
			}
			return true;
		}

		public bool UploadFile ( Stream fileStream, Uri ftpUrl ) {
			try {
				Write ( ftpUrl, System.Net.WebRequestMethods.Ftp.UploadFile, fileStream );
			} catch {
				throw;
			}
			return true;
		}

		public FtpWebResponse Write ( Uri ftpUrl, string method, Stream stream ) {
			CheckNotSshFtp ( ftpUrl );

			System.Net.FtpWebRequest req = System.Net.FtpWebRequest.Create ( ftpUrl ) as System.Net.FtpWebRequest;

			if ( this.Credentials != null ) {
				req.Credentials = this.Credentials;
			} else {
				req.Credentials = new System.Net.NetworkCredential ( "anonymous", "user@" + ftpUrl.Host );
			}

			req.UsePassive = this.UsePassive;
			req.Timeout = this.Timeout;
			req.Method = method;
			req.EnableSsl = this.EnableSsl || string.Compare ( ftpUrl.Scheme, FtpWebRequest.UriSchemeFtpSsl ) == 0 ||
				string.Compare ( ftpUrl.Scheme, FtpWebRequest.UriSchemeSshFtp ) == 0;
			req.KeepAlive = false;

			if ( stream != null ) {
				using ( Stream reqStream = req.GetRequestStream () ) {
					using ( stream ) {
						int i = 0;
						byte[] buffer = new byte[ 2048 ];
						while ( ( i = stream.Read ( buffer, 0, buffer.Length ) ) > 0 ) {
							reqStream.Write ( buffer, 0, i );
						}
					}
				}
			}

			System.Net.FtpWebResponse resp = req.GetResponse () as System.Net.FtpWebResponse;
			StreamReader sr = new StreamReader ( resp.GetResponseStream () );
			System.Net.FtpStatusCode code = resp.StatusCode;
			string desc = resp.StatusDescription;
			byte[] respBuffer = null;

			using ( resp ) {
				Stream strm = resp.GetResponseStream ();
				using ( strm ) {
					MemoryStream ms = new MemoryStream ();
					using ( ms ) {
						byte[ ] buff = new byte[ 1024 ];
						int i = 0;
						while ( ( i = strm.Read ( buff, 0, buff.Length ) ) > 0 ) {
							ms.Write ( buff, 0, i );
							ms.Flush ();
						}
						ms.Position = 0;
						respBuffer = ms.ToArray ();
					}
				}
			}

			FtpWebResponse fwr = new FtpWebResponse ( respBuffer );
			fwr.StatusCode = code;
			fwr.StatusDescription = desc;
			return fwr;
		}

		/// <summary>
		/// Requests the specified FTP URL.
		/// </summary>
		/// <param name="ftpUrl">The FTP URL.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public FtpWebResponse Request ( Uri ftpUrl, string method ) {
			return Write ( ftpUrl, method, null );
		}

		private void CheckNotSshFtp ( Uri ftpUrl ) {
			if ( string.Compare ( ftpUrl.Scheme, FtpWebRequest.UriSchemeSshFtp ) == 0 )
				throw new NotSupportedException ( Properties.Resources.SshFtpNotSupportedMessage );
		}
	}
}
