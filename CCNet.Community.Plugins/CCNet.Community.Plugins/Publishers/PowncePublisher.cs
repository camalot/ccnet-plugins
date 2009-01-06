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
using Exortech.NetReflector;
using CCNet.Community.Plugins.Common;
using CCNet.Community.Plugins.Components.Pownce;
using System.Net;
using System.Threading;

namespace CCNet.Community.Plugins.Publishers {
	/// <summary>
	/// 
	/// </summary>
	[ReflectorType ( "pownce" ), Obsolete ( "Pownce is closed", true )]
	public class PowncePublisher : ITask {
		/// <summary>
		/// The api key
		/// </summary>
		private const string APPKEY = "i43hw0i1231oq23o058h21lr0j505mf6";

		/// <summary>
		/// Initializes a new instance of the <see cref="PowncePublisher"/> class.
		/// </summary>
		public PowncePublisher () {
			this.Files = new List<PownceFile> ();
			this.Notes = new List<PownceNote> ();
			this.Links = new List<PownceLink> ();
			this.Events = new List<PownceEvent> ();
		}
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[ReflectorProperty ( "username", Required = true )]
		public string UserName { get; set; }
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[ReflectorProperty ( "password", Required = true )]
		public string Password { get; set; }
		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		[ReflectorProperty ( "proxy", Required = false )]
		public Proxy Proxy { get; set; }

		/// <summary>
		/// Gets or sets the notes.
		/// </summary>
		/// <value>The notes.</value>
		[ReflectorArray ( "notes", Required = false )]
		public List<PownceNote> Notes { get; set; }

		/// <summary>
		/// Gets or sets the links.
		/// </summary>
		/// <value>The notes.</value>
		[ReflectorArray ( "links", Required = false )]
		public List<PownceLink> Links { get; set; }

		/// <summary>
		/// Gets or sets the notes.
		/// </summary>
		/// <value>The notes.</value>
		[ReflectorArray ( "files", Required = false )]
		public List<PownceFile> Files { get; set; }

		/// <summary>
		/// Gets or sets the events.
		/// </summary>
		/// <value>The events.</value>
		[ReflectorArray ( "events", Required = false )]
		public List<PownceEvent> Events { get; set; }

		#region ITask Members

		/// <summary>
		/// Runs the specified result.
		/// </summary>
		/// <param name="result">The result.</param>
		public void Run ( IIntegrationResult result ) {
			PownceService service = new PownceService ( APPKEY, new NetworkCredential ( this.UserName, this.Password ) );
			PublishBuildStatus pbs = Util.GetBuildStatus ( result );
			PublishBuildCondition pbc = Util.GetBuildCondition ( result );
			foreach ( PownceNote pn in this.Notes ) {
				if ( ( pn.BuildStatus == pbs || pn.BuildStatus == PublishBuildStatus.Any ) &&
					( pn.BuildCondition == pbc || pn.BuildCondition == PublishBuildCondition.AllBuildConditions ) ) {
					service.PostText ( pn.Message, pn.RecipientListToString () );
				}
				// required by pownce
				Thread.Sleep ( 1000 );
			}

			foreach ( PownceLink pl in this.Links ) {
				if ( ( pl.BuildStatus == pbs || pl.BuildStatus == PublishBuildStatus.Any ) &&
					( pl.BuildCondition == pbc || pl.BuildCondition == PublishBuildCondition.AllBuildConditions ) ) {
					service.PostLink ( pl.Message, new Uri ( pl.Url ), pl.RecipientListToString () );
				}
				// required by pownce
				Thread.Sleep ( 1000 );
			}

			foreach ( PownceEvent pe in this.Events ) {
				if ( ( pe.BuildStatus == pbs || pe.BuildStatus == PublishBuildStatus.Any ) &&
					( pe.BuildCondition == pbc || pe.BuildCondition == PublishBuildCondition.AllBuildConditions ) ) {
					service.PostEvent ( pe.Message, pe.Name, pe.Location, pe.Date, pe.RecipientListToString () );
				}
				// required by pownce
				Thread.Sleep ( 1000 );
			}

			foreach ( PownceFile pf in this.Files ) {
				if ( ( pf.BuildStatus == pbs || pf.BuildStatus == PublishBuildStatus.Any ) &&
					( pf.BuildCondition == pbc || pf.BuildCondition == PublishBuildCondition.AllBuildConditions ) ) {
					service.PostFile ( pf.Message, pf.FilePath, pf.RecipientListToString () );
				}
				// required by pownce
				Thread.Sleep ( 1000 );
			}
		}



		#endregion
	}
}
