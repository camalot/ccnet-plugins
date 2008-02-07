using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNet.Community.Plugins.Tasks {
  public enum MbUnitReportTypes {
    Text,
    Html,
    Xml,
  }
  [ReflectorType ( "mbunit" )]
  public class MbUnitTask : ITask {

    private ProcessExecutor _processExecutor = null;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MbUnitTask"/> class.
    /// </summary>
    public MbUnitTask ( )
      : this ( new ProcessExecutor ( ) ) {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MbUnitTask"/> class.
    /// </summary>
    /// <param name="exec">The exec.</param>
    public MbUnitTask ( ProcessExecutor exec ) {
      this._processExecutor = exec;
      this.Timeout = 600;
      this.Assemblies = new string[0];
      this.Filters = new MbUnitFilters();
      this.OutputFile = "mbunit-results.xml";
      this.Executable = string.Empty;
      this.AssemblyPath = string.Empty;
      this.TransformFile = string.Empty;
    }

    [ReflectorProperty ( "timeout", Required = false )]
    public int Timeout { get; set; }

    /// <summary>
    /// Gets or sets the assemblies.
    /// </summary>
    /// <value>The assemblies.</value>
    [ReflectorArray ( "assemblies" )]
    public string[ ] Assemblies { get; set; }
    /// <summary>
    /// Gets or sets the executable.
    /// </summary>
    /// <value>The executable.</value>
    [ReflectorProperty ( "executable", Required = true )]
    public string Executable { get; set; }

    /// <summary>
    /// Gets or sets the assembly path.
    /// </summary>
    /// <value>The assembly path.</value>
    [ReflectorProperty ( "assemblypath", Required=false )]
    public string AssemblyPath { get; set; }

    /// <summary>
    /// Gets or sets the report.
    /// </summary>
    /// <value>The report.</value>
    [ReflectorProperty ( "outputfile", Required = false )]
    public string OutputFile { get; set; }

    /// <summary>
    /// Gets or sets the filters.
    /// </summary>
    /// <value>The filters.</value>
    [ReflectorProperty ( "filters", Required = false )]
    public MbUnitFilters Filters { get; set; }

    /// <summary>
    /// Gets or sets the transform file.
    /// </summary>
    /// <value>The transform file.</value>
    [ReflectorProperty ( "transformfile", Required = false )]
    public string TransformFile { get; set; }

    private ProcessInfo NewProcessInfo ( string outputFile, IIntegrationResult result ) {
      string str = new MbUnitArgument ( this, result ).ToString ( );
      Log.Debug ( string.Format ( "Running unit tests: {0} {1}", this.Executable, str ) );
      ProcessInfo info = new ProcessInfo ( this.Executable, str, result.WorkingDirectory );
      info.TimeOut = this.Timeout * 1000;
      return info;
    }


    #region ITask Members

    /// <summary>
    /// Runs the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    public void Run ( IIntegrationResult result ) {
      ListenerFile.WriteInfo ( result.ListenerFile, "Executing NUnit" );
      string outputFile = result.BaseFromArtifactsDirectory ( this.OutputFile );
      ProcessResult result2 = this._processExecutor.Execute ( this.NewProcessInfo ( outputFile, result ), ProcessMonitor.GetProcessMonitorByProject ( result.ProjectName ) );
      result.AddTaskResult ( new ProcessTaskResult ( result2 ) );
      if ( File.Exists ( outputFile ) ) {
        result.AddTaskResult ( new FileTaskResult ( outputFile ) );
      } else {
        Log.Warning ( string.Format ( "MbUnit test output file {0} was not created", outputFile ) );
      }
      ListenerFile.RemoveListenerFile ( result.ListenerFile );

    }
    #endregion
  }
}
