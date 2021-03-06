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

namespace CCNet.Community.Plugins.Components.Ftp {
  /// <summary>
  /// 
  /// </summary>
  public class FtpSystemInfo {

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpSystemInfo"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="size">The size.</param>
    /// <param name="owner">The owner.</param>
    /// <param name="group">The group.</param>
    /// <param name="publicUsers">The public users.</param>
    /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
    /// <param name="lastModified">The last modified.</param>
    internal FtpSystemInfo ( string name, long size, FtpSystemInfoPermission owner, 
      FtpSystemInfoPermission group, FtpSystemInfoPermission publicUsers, bool isDirectory, DateTime lastModified ) {
      this.Name = name;
      this.Size = size;
      this.Group = group;
      this.Public = publicUsers;
      this.Owner = owner;
      this.IsDirectory = isDirectory;
      this.LastModified = lastModified;
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public long Size {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets the public Ftp System Info Permission.
    /// </summary>
    /// <value>The public.</value>
    public FtpSystemInfoPermission Public {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets the group Ftp System Info Permission.
    /// </summary>
    /// <value>The group.</value>
    public FtpSystemInfoPermission Group {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets the owner Ftp System Info Permission.
    /// </summary>
    /// <value>The owner.</value>
    public FtpSystemInfoPermission Owner {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is directory.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is directory; otherwise, <c>false</c>.
    /// </value>
    public bool IsDirectory {
      get { return !this.IsFile; }
      protected set { this.IsFile = !value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is file.
    /// </summary>
    /// <value><c>true</c> if this instance is file; otherwise, <c>false</c>.</value>
    public bool IsFile {
      get;
      protected set;
    }

    /// <summary>
    /// Gets or sets the last modified.
    /// </summary>
    /// <value>The last modified.</value>
    public DateTime LastModified { get; protected set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    /// <value>The URL.</value>
    public Uri Url { get; set; }

    /// <summary>
    /// Gets or sets the directory.
    /// </summary>
    /// <value>The directory.</value>
    public string Directory {
      get {
        return this.Url != null ? this.Url.AbsolutePath.Substring ( 0, this.Url.AbsolutePath.LastIndexOf ( "/" ) ) : "/";
      }
    }

  }
}
