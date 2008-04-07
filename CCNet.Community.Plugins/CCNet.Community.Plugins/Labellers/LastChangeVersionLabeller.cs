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
      return string.Format ( "{1}{0}{2}{0}{4}{0}{3}", this.Separator, this.Major, this.Minor, this.GetYearMonthDay ( ), lastChange );
    }

    /// <summary>
    /// Gets the month day.
    /// </summary>
    /// <returns></returns>
    private int GetYearMonthDay ( ) {
      DateTime time = DateTime.Now;
      string longYear = time.Year.ToString ( );
      int shortYear = int.Parse ( longYear.Substring ( longYear.Length - 2 ) );
      string timeString = string.Format ( "{2}{0}{1}", PaddedValue ( time.Month ), PaddedValue ( time.Day ), PaddedValue ( shortYear ) );
      return Convert.ToInt32 ( timeString );
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