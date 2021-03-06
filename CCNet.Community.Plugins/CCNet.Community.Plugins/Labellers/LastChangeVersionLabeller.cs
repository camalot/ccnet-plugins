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
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Label;

namespace CCNet.Community.Plugins.Labellers {
  /// <summary>
  /// 
  /// </summary>
  [ReflectorType ( "lastChangeVersionLabeller" )]
  public class LastChangeVersionLabeller : ILabeller, ITask {
    /// <summary>
    /// Initializes a new instance of the <see cref="LastChangeVersionLabeller"/> class.
    /// </summary>
    public LastChangeVersionLabeller ( ) {
      this.Major = 1;
      this.Minor = 0;
      this.IncrementOnFailure = false;
      this.Separator = ".";
    }
    /// <summary>
    /// Gets or sets the major.
    /// </summary>
    /// <value>The major.</value>
    [ReflectorProperty ( "major", Required = true )]
    public int Major { get; set; }
    /// <summary>
    /// Gets or sets the minor.
    /// </summary>
    /// <value>The minor.</value>
    [ReflectorProperty ( "minor", Required = true )]
    public int Minor { get; set; }
    /// <summary>
    /// Gets or sets the separator.
    /// </summary>
    /// <value>The separator.</value>
    [ReflectorProperty ( "separator", Required = false )]
    public string Separator { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [increment on failed].
    /// </summary>
    /// <value><c>true</c> if [increment on failed]; otherwise, <c>false</c>.</value>
    [ReflectorProperty ( "incrementOnFailure", Required = false )]
    public bool IncrementOnFailure { get; set; }

    #region ILabeller Members

    /// <summary>
    /// Generates the specified integration result.
    /// </summary>
    /// <param name="integrationResult">The integration result.</param>
    /// <returns></returns>
    public string Generate ( IIntegrationResult integrationResult ) {
      IntegrationSummary lastIntegration = integrationResult.LastIntegration;
      if ( ( lastIntegration.Label == null ) || lastIntegration.IsInitial ( ) ) {
        return GetVersionLabel ( 0 );
      }

      if ( ( lastIntegration.Status != IntegrationStatus.Success ) && !this.IncrementOnFailure ) {
        return lastIntegration.Label;
      }
      int lastChangeNumber = integrationResult.LastChangeNumber;
      return GetVersionLabel ( lastChangeNumber );
    }

    #endregion

    /// <summary>
    /// Gets the version label.
    /// </summary>
    /// <param name="lastChange">The last change.</param>
    /// <returns></returns>
    private string GetVersionLabel ( int lastChange ) {
      return string.Format ( "{1}{0}{2}{0}{4}{0}{3}", this.Separator, this.Major, this.Minor, this.CalculateFractionalPartOfDay ( ), lastChange );
    }

    /// <summary>
    /// calculate the value for the revision. The calculation is the same that is used in the MSBuild Community Tasks
    /// </summary>
    /// <returns></returns>
    private int CalculateFractionalPartOfDay ( ) {
      float factor = ( float ) ( UInt16.MaxValue - 1 ) / ( 24 * 60 * 60 );
      return ( int ) ( DateTime.Now.TimeOfDay.TotalSeconds * factor );
    }

    /// <summary>
    /// Paddeds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    private string PaddedValue ( int value ) {
      return string.Format ( "{0}{1}", value < 10 ? "0" : string.Empty, value );
    }

    #region ITask Members

    /// <summary>
    /// Runs the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    public void Run ( IIntegrationResult result ) {
      result.Label = this.Generate ( result );
    }

    #endregion
  }
}