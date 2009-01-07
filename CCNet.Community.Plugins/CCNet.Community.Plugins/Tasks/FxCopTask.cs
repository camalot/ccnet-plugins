using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using CCNet.Community.Plugins.Common;

namespace CCNet.Community.Plugins.Tasks {
	[ReflectorType ( "fxcop" )]
	public class FxCopTask : BasePublisherTask {
		public string AnalysisReport { get; set; }
		public bool AppluOutXsl { get; set; }
		public string ConsoleXml { get; set; }
		public bool IncludeSummaryReport { get; set; }
		public string OutputXsl { get; set; }
		public string PlatformDirectory { get; set; }
		public string Project { get; set; }
		public bool SaveResults { get; set; }
		public List<string> TypeList { get; set; }
		#region ITask Members

		/// <summary>
		/// Runs the specified result.
		/// </summary>
		/// <param name="result">The result.</param>
		public override void Run ( IIntegrationResult result ) {
			throw new NotImplementedException ();
		}

		#endregion
	}
}
