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
using System.Reflection;
using System.IO;

namespace CCNet.Community.Plugins.Components.Macros {
  /// <summary>
  /// Static class for executing macros
  /// </summary>
  public class MacroEngine {

    /// <summary>
    /// Initializes the <see cref="Macros"/> class.
    /// </summary>
    public MacroEngine () {
      Macros = new Dictionary<string, IMacro> ();
      LoadMacrosIntoAppDomain ();
    }

    public Dictionary<string,IMacro> Macros { get; private set; }

    /// <summary>
    /// Executes the macro.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="invoker">The invoker.</param>
    /// <param name="macroName">Name of the macro.</param>
    /// <param name="args">The args.</param>
    /// <returns>results of the macros execution</returns>
    public string Execute ( IIntegrationResult result, IMacroRunner invoker, string macroName, string args ) {
      if ( Macros.ContainsKey ( macroName ) )
        return Macros[ macroName ].Execute ( result, invoker, args );
      else
        return string.Format ( Properties.Resources.MacroNotFoundMessage, macroName );
    }

    /// <summary>
    /// Loads the macros into app domain.
    /// </summary>
    private void LoadMacrosIntoAppDomain () {
      DirectoryInfo path = new DirectoryInfo ( Path.GetDirectoryName ( this.GetType ().Assembly.Location ) );
      LoadMacroFromAssembly ( this.GetType ().Assembly );
      foreach ( FileInfo file in path.GetFiles ( Properties.Resources.MacroDllPattern ) ) {
        Assembly asm = Assembly.LoadFrom ( file.FullName );
        LoadMacroFromAssembly ( asm );
      }
    }

    /// <summary>
    /// Loads the macro from assembly.
    /// </summary>
    /// <param name="asm">The asm.</param>
    private void LoadMacroFromAssembly ( Assembly asm ) {
      foreach ( Type t in asm.GetTypes () ) {
        if ( t.GetInterface ( typeof ( IMacro ).FullName, true ) != null ) {
          if ( !Macros.ContainsKey ( t.Name ) )
            Macros.Add ( t.Name, asm.CreateInstance ( t.FullName, true ) as IMacro );
          else
            ThoughtWorks.CruiseControl.Core.Util.Log.Warning ( string.Format ( Properties.Resources.MacroLoadedMessage, t.Name ) );
        }
      }
    }

  }
}
