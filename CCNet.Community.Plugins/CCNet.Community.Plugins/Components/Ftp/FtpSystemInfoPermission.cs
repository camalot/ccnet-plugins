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
  public class FtpSystemInfoPermission {
    /// <summary>
    /// Initializes a new instance of the <see cref="FtpSystemInfoPermission"/> class.
    /// </summary>
    public FtpSystemInfoPermission ( ) {

    }
    /// <summary>
    /// 
    /// </summary>
    public static string NO_PERMISSION = "-";
    /// <summary>
    /// 
    /// </summary>
    public static string READ = "r";
    /// <summary>
    /// 
    /// </summary>
    public static string WRITE = "w";
    /// <summary>
    /// 
    /// </summary>
    public static string EXECUTE = "x";

    /// <summary>
    /// Gets or sets a value indicating whether this instance can execute.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
    /// </value>
    public bool CanExecute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can write.
    /// </summary>
    /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
    public bool CanWrite { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can read.
    /// </summary>
    /// <value><c>true</c> if this instance can read; otherwise, <c>false</c>.</value>
    public bool CanRead { get; set; }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString ( ) {
      //rwx
      StringBuilder sb = new StringBuilder ( 3 );
      sb.Append ( this.CanRead ? FtpSystemInfoPermission.READ : FtpSystemInfoPermission.NO_PERMISSION );
      sb.Append ( this.CanWrite ? FtpSystemInfoPermission.WRITE : FtpSystemInfoPermission.NO_PERMISSION );
      sb.AppendFormat ( this.CanExecute ? FtpSystemInfoPermission.EXECUTE : FtpSystemInfoPermission.NO_PERMISSION );
      return sb.ToString ( );
    }

    /// <summary>
    /// converts the permission string to a <see cref="FtpSystemInfoPermission"/>
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    public static FtpSystemInfoPermission FromString ( string s ) {
      if ( string.IsNullOrEmpty ( s ) ) {
        throw new ArgumentNullException ( "s", "Must define permision string" );
      }
      
      if ( s.Length > 3 ) {
        throw new ArgumentException ( "Only permission for one group/user can be parsed." );
      }

      FtpSystemInfoPermission perm = new FtpSystemInfoPermission ( );
      perm.CanRead = string.Compare ( FtpSystemInfoPermission.READ, s[ 0 ].ToString ( ), false ) == 0;
      perm.CanWrite = string.Compare ( FtpSystemInfoPermission.WRITE, s[ 1 ].ToString ( ), false ) == 0;
      perm.CanExecute = string.Compare ( FtpSystemInfoPermission.EXECUTE, s[ 2 ].ToString ( ), false ) == 0;
      return perm;
    }

  }
}
