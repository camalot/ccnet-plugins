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
using System.ComponentModel;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNet.Community.Plugins.Tasks {
 /* public enum MbUnitReportTypes {
    Text,
    Html,
    Xml,
  }*/

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

    /// <summary>
    /// Gets or sets the timeout.
    /// </summary>
    /// <value>The timeout.</value>
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

    /// <summary>
    /// News the process info.
    /// </summary>
    /// <param name="outputFile">The output file.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
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
      string outputFile = result.BaseFromArtifactsDirectory ( this.OutputFile );
      ProcessResult result2 = this._processExecutor.Execute ( this.NewProcessInfo ( outputFile, result ), ProcessMonitor.GetProcessMonitorByProject ( result.ProjectName ) );
      result.AddTaskResult ( new ProcessTaskResult ( result2 ) );
      if ( File.Exists ( outputFile ) ) {
        result.AddTaskResult ( new FileTaskResult ( outputFile ) );
      } else {
        Log.Warning ( string.Format ( "MbUnit test output file {0} was not created", outputFile ) );
      }
    }
    #endregion
  }
}
