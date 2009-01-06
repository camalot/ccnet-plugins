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
using System.Text.RegularExpressions;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

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
      LoadedAssemblies = new List<string> ( );
      LoadMacrosIntoAppDomain ();
    }

    /// <summary>
    /// Gets or sets the macros.
    /// </summary>
    /// <value>The macros.</value>
    public Dictionary<string,IMacro> Macros { get; private set; }
    private List<string> LoadedAssemblies { get; set; }
    /// <summary>
    /// Gets the property string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="result">The result.</param>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public string GetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
      Regex xpathFinder = new Regex ( Properties.Resources.XPathPattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace );
      Match m1 = xpathFinder.Match ( input );
      string ret = input;
      if ( m1.Success ) {
        // get the xpath string.
        ret = m1.Result ( "$1" );
        try {
          XmlNode tNode = GetXmlTaskResultNode ( result, ret );
          if ( tNode != null )
            return tNode.InnerText;
          else {
            throw new Exception ( "No node found for xpath (" + ret + ")" );
          }
        } catch ( Exception ex ) {
          // log the error as a waring and continue
          Log.Warning ( ex.ToString ( ) );
          return "!{" + ret + "}";
        }
      }

      Regex propFinder = new Regex ( Properties.Resources.PropertyPatern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace );
      Match m = propFinder.Match ( ret );
      while ( m.Success ) {
        string propName = m.Result ( "$1" );
        if ( ContainsPropertyName<T> ( sender, result, propName ) )
          ret = ret.Replace ( m.Value, GetPropertyValue<T> ( sender, result, propName ) );
        m = m.NextMatch ( );
      }

      
      if ( sender.GetType().GetInterface ( typeof ( IMacroRunner ).FullName ) != null ) {
        IMacroRunner runner = sender as IMacroRunner;
        propFinder = new Regex ( Properties.Resources.MacroPattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace );
        m = propFinder.Match ( ret );
        while ( m.Success ) {
          string propName = m.Result ( "$1" );
          string arg = m.Result ( "$2" );
          ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( string.Format ( "Found Macro {0}", m.Value ) );
          ret = ret.Replace ( m.Value, runner.MacroEngine.Execute ( result, runner, propName, GetPropertyString<T> ( sender, result, arg ) ) );
          m = m.NextMatch ( );
        }
      }

			//ret = GetPropertyString<IIntegrationResult> ( result, result, ret );
      return ret;
    }

    /// <summary>
    /// Contains the name of the CCnet property.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="result">The result.</param>
    /// <param name="propName">Name of the prop.</param>
    /// <returns>
    /// 	<see langword="true"/> if [contains CC net property name] [the specified sender]; otherwise, <see langword="false"/>.
    /// </returns>
    internal bool ContainsPropertyName<T> ( T sender, IIntegrationResult result, string propName ) {
      foreach ( string s in result.IntegrationProperties.Keys )
        if ( string.Compare ( s, propName, true ) == 0 )
          return true;

      // check this objects properties
      BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance;
      PropertyInfo pi = null;
      if ( sender != null ) {
        pi = sender.GetType ( ).GetProperty ( propName, flags );
        if ( pi != null )
          return true;
      }

      pi = result.GetType ( ).GetProperty ( propName, flags );
      if ( pi != null )
        return true;
      return false;
    }

    /// <summary>
    /// Gets the CCnet property value.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="result">The result.</param>
    /// <param name="propName">Name of the prop.</param>
    /// <returns></returns>
    internal string GetPropertyValue<T> ( T sender, IIntegrationResult result, string propName ) {
      foreach ( string s in result.IntegrationProperties.Keys )
        if ( string.Compare ( s, propName, true ) == 0 )
          return result.IntegrationProperties[ s ] as string;

      BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance;
      PropertyInfo pi = null;
      if ( sender != null ) {
        pi = sender.GetType ( ).GetProperty ( propName, flags );
        if ( pi != null )
          return pi.GetValue ( sender, null ).ToString ( );
      }

      pi = result.GetType ( ).GetProperty ( propName, flags );
      return pi == null ? string.Empty : pi.GetValue ( result, null ) as String;
    }

    /// <summary>
    /// Creates the task result XML document.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public XmlDocument CreateTaskResultXmlDocument ( IIntegrationResult result ) {
      XmlDocument doc = new XmlDocument ( );
      doc.AppendChild ( doc.CreateElement ( "IntegrationResults" ) );
      foreach ( ITaskResult tr in result.TaskResults ) {
        XmlDocument tdoc = new XmlDocument ( );
        XmlDocumentFragment frag = tdoc.CreateDocumentFragment ( );
        frag.InnerXml = tr.Data;
        doc.DocumentElement.AppendChild ( doc.ImportNode ( frag, true ) );
      }
      return doc;
    }

    /// <summary>
    /// Gets the XML task result node.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="xpath">The xpath.</param>
    /// <returns></returns>
    public XmlNode GetXmlTaskResultNode ( IIntegrationResult result, string xpath ) {
      XmlDocument doc = CreateTaskResultXmlDocument ( result );
      XmlNode node = doc.DocumentElement.SelectSingleNode ( xpath );
      if ( node != null )
        return node;
      else
        return null;
    }

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
      LoadedAssemblies.Clear ( );
      DirectoryInfo path = new DirectoryInfo ( Path.GetDirectoryName ( this.GetType ().Assembly.Location ) );
      LoadMacrosFromAssembly ( this.GetType ().Assembly );
      foreach ( FileInfo file in path.GetFiles ( Properties.Resources.MacroDllPattern ) ) {
        if ( !IsAssemblyAlreadyLoaded ( file.FullName ) ) {
          Assembly asm = Assembly.LoadFrom ( file.FullName );
          LoadMacrosFromAssembly ( asm );
        }
      }
    }

    /// <summary>
    /// Checks if an assembly file is already loaded in the app domain
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool IsAssemblyAlreadyLoaded ( string fileName ) {
      // check the list first
      if ( LoadedAssemblies.Contains ( fileName ) )
        return true;

      AppDomain ad = AppDomain.CurrentDomain;
      Assembly[ ] assemblies = ad.GetAssemblies ( );
      foreach ( Assembly asm in assemblies ) {
        if ( string.Compare ( fileName, asm.Location, true ) == 0 )
          return true;
      }
      // add the new assembly to the list.
      LoadedAssemblies.Add ( fileName );
      return false;
    }

    /// <summary>
    /// Loads the macros from assembly.
    /// </summary>
    /// <param name="asm">The asm.</param>
    private void LoadMacrosFromAssembly ( Assembly asm ) {
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
