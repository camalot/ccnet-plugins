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
using Exortech.NetReflector;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using CCNet.Community.Plugins.Components.Macros;

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "releaseFile" )]
  public class ReleaseFile : IMacroRunner {
    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseFile"/> class.
    /// </summary>
    public ReleaseFile ( ) {
      this.MacroEngine = new MacroEngine ( );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseFile"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileType">Type of the file.</param>
    public ReleaseFile ( string name, string mimeType, string fileName, ReleaseFileType fileType ) {
      this.Name = name;
      this.MimeType = mimeType;
      this.FileName = fileName;
      this.FileType = fileType;
    }
    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    [ReflectorProperty ( "fileName", Required = true )]
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the type of the file.
    /// </summary>
    /// <value>The type of the file.</value>
    [ReflectorProperty ( "fileType", Required = true )]
    public ReleaseFileType FileType { get; set; }

    /// <summary>
    /// Gets or sets the Mime Type.
    /// </summary>
    /// <value>The MimeType.</value>
    [ReflectorProperty ( "mimeType", Required = false )]
    public string MimeType { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [ReflectorProperty ( "name", Required = false )]
    public string Name { get; set; }
    /// <summary>
    /// Gets the file data.
    /// </summary>
    /// <returns></returns>
    public byte[ ] GetFileData ( IIntegrationResult result ) {
      FileInfo file = new FileInfo ( this.MacroEngine.GetPropertyString<ReleaseFile> ( this, result, this.FileName ) );
      if ( file.Exists ) {
        FileStream tfs = new FileStream ( file.FullName, FileMode.Open, FileAccess.Read );
        byte[ ] fullBuffer = new byte[ file.Length ];
        using ( tfs ) {
          int read = 0;
          if ( ( read = tfs.Read ( fullBuffer, 0, fullBuffer.Length ) ) > 0 )
            return fullBuffer;
          else
            throw new FileLoadException ( "Unable to read file." );
        }
      } else
        throw new FileNotFoundException ( string.Format ( Properties.Resources.FileNotFoundMessage, file.FullName ) );
    }

    #region IMacroRunner Members

    public MacroEngine MacroEngine {
      get;
      private set;
    }

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
			string ret = this.GetPropertyString<ReleaseFile> ( this, result, input );
			ret = this.GetPropertyString<T> ( sender, result, ret );
			return ret;
		}

    #endregion
  }
}
