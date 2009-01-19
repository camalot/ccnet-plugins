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
using ThoughtWorks.CruiseControl.Core;

namespace CCNet.Community.Plugins.Publishers {
  public class CodePlexReleaseTaskResult : ITaskResult {
    private int releaseId = -1;
    private string releaseName = string.Empty;
    private Exception execption = null;
    private ReleaseType? releaseType = null;
    /// <summary>
    /// Initializes a new instance of the <see cref="CodePlexReleaseTaskResult"/> class.
    /// </summary>
    /// <param name="releaseName">Name of the release.</param>
    public CodePlexReleaseTaskResult ( string releaseName) {
      this.releaseName = releaseName;
    }

    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    /// <value>The exception.</value>
    public Exception Exception { get { return this.execption; } set { this.execption = value; } }
    /// <summary>
    /// Gets or sets the release id.
    /// </summary>
    /// <value>The release id.</value>
    public int ReleaseId { get { return this.releaseId; } set { this.releaseId = value; } }

    /// <summary>
    /// Gets or sets the type of the release.
    /// </summary>
    /// <value>The type of the release.</value>
    public ReleaseType? ReleaseType { get { return this.releaseType; } set { this.releaseType = value; } }

    #region ITaskResult Members

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <value>The data.</value>
    public string Data {
      get {
        if ( Succeeded() )
          return string.Format ( "<CodePlexRelease Id=\"{0}\" Name=\"{1}\" Type=\"{2}\" />", this.ReleaseId, this.releaseName, this.ReleaseType.HasValue ? this.ReleaseType.Value.ToString ( ) : string.Empty ); 
        else
          return string.Format ( "<CodePlexRelease Name=\"{0}\">{1}</CodePlexRelease>", this.releaseName, this.Exception.ToString ( ) );
      }
    }

    /// <summary>
    /// Returns if this task failed
    /// </summary>
    /// <returns></returns>
    public bool Failed ( ) {
      return Exception != null || ReleaseId <= 0;
    }

    /// <summary>
		///  Returns if this task Succeeded
    /// </summary>
    /// <returns></returns>
    public bool Succeeded ( ) {
      return Exception == null;
    }

    #endregion
  }
}
