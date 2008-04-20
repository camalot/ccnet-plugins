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

namespace CCNet.Community.Plugins.Publishers {
  [ReflectorType ( "workitemPublisher", Description = "Publishes results to a TFS WorkItem when build fails." )]
  public class TfsWorkItemPublisher : ITask {
    [ReflectorProperty ( "server", Required = true )]
    public string TfsServer { get; set; }
    [ReflectorProperty ( "username", Required = false )]
    public string UserName { get; set; }
    [ReflectorProperty ( "password", Required = false )]
    public string Password { get; set; }
    [ReflectorProperty ( "domain", Required = false )]
    public string Domain { get; set; }
    [ReflectorProperty ( "project", Required = true )]
    public string ProjectName { get; set; }
    [ReflectorProperty ( "titleprefix", Required = false )]
    public string TitlePrefix { get; set; }

    #region ITask Members

    public void Run ( IIntegrationResult result ) {
      if ( result.Failed ) {
        string title = string.Format ( "{0}{1} Failed @ {2}", this.TitlePrefix, result.ProjectName, DateTime.Now.ToString ( "g" ) );
        StringBuilder results = new StringBuilder ( );

        foreach ( ITaskResult itr in result.TaskResults ) {
          if ( itr.Failed() )
            results.AppendLine ( itr.Data );
        }

        string body = string.Format ( "{0}", results.ToString ( ) );
      }
    }

    #endregion
  }
}
