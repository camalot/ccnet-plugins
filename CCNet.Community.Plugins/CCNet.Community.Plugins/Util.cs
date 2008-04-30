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
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core.Util;
using CCNet.Community.Plugins.Components.Macros;

namespace CCNet.Community.Plugins {
  public static class Util {
    /// <summary>
    /// Converts an objects tostring to lowercase
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns></returns>
    public static string ToLowerString ( Object o ) {
      return o.ToString ( ).ToLower ( );
    }

    /// <summary>
    /// Gets the CCNet property string.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="result">The result.</param>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public static string GetCCNetPropertyString<T> ( T sender, IIntegrationResult result, string input ) {
      Regex xpathFinder = new Regex ( Properties.Resources.XPathPattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace );
      Match m1 = xpathFinder.Match ( input );
      string ret = input;
      if ( m1.Success ) {
        ret = m1.Result ( "$1" );
      }

      Regex propFinder = new Regex ( Properties.Resources.PropertyPatern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace );
      Match m = propFinder.Match ( ret );
      while ( m.Success ) {
        string propName = m.Result ( "$1" );
        if ( ContainsCCNetPropertyName<T> ( sender, result, propName ) )
          ret = ret.Replace ( string.Format ( "$({0})", propName ), GetCCNetPropertyValue<T> ( sender, result, propName ) );
        m = m.NextMatch ( );
      }

      try {
        XmlNode tNode = GetXmlTaskResultNode ( result, ret );
        if ( tNode != null )
          return tNode.InnerText;
      } catch ( Exception ex ) {
        Log.Warning ( ex.ToString ( ) );
      }

      if ( typeof ( T ).GetInterface ( typeof ( IMacroRunner ).FullName ) != null ) {
        IMacroRunner runner = sender as IMacroRunner;
        propFinder = new Regex ( Properties.Resources.MacroPattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace );
        m = propFinder.Match ( ret );
        while ( m.Success ) {
          string propName = m.Result ( "$1" );
          string arg = m.Result ( "$2" );
          ThoughtWorks.CruiseControl.Core.Util.Log.Debug ( string.Format ( "Found Macro {0}", m.Value ) );
          ret = ret.Replace ( m.Value, runner.MacroEngine.Execute ( result, runner, propName, GetCCNetPropertyString<T> (sender, result, arg ) ) );
          m = m.NextMatch ( );
        }
      }
      return ret;
    }

    /// <summary>
    /// Creates the task result XML document.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    internal static XmlDocument CreateTaskResultXmlDocument ( IIntegrationResult result ) {
      XmlDocument doc = new XmlDocument ( );
      doc.AppendChild ( doc.CreateElement ( "CCNetResults" ) );
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
    internal static XmlNode GetXmlTaskResultNode ( IIntegrationResult result, string xpath ) {
      XmlDocument doc = CreateTaskResultXmlDocument ( result );
      XmlNode node = doc.SelectSingleNode ( xpath );
      if ( node != null )
        return node;
      else
        return null;
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
    internal static bool ContainsCCNetPropertyName<T> ( T sender, IIntegrationResult result, string propName ) {
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
    internal static string GetCCNetPropertyValue<T> ( T sender, IIntegrationResult result, string propName ) {
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
  }

}
