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
using System.Xml;
using System.IO;

namespace CCNet.Community.Plugins.Components.Macros {
  /// <summary>
  /// Performs an Xsl Transform on data in the IntegrationResult Task Results.
  /// </summary>
  public class XslTransform : IMacro {
    #region IMacro Members

    /// <summary>
    /// Executes the Macro.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="invoker">The invoker.</param>
    /// <param name="args">The args.</param>
    /// <returns></returns>
    public string Execute ( ThoughtWorks.CruiseControl.Core.IIntegrationResult result, IMacroRunner invoker, string args ) {
      if ( string.IsNullOrEmpty ( args ) )
        throw new ArgumentNullException ( "args" );
      if ( invoker == null ) {
        throw new ArgumentNullException ( "invoker" );
      }

      string[ ] arga = args.Split ( new char[ ] { ',' }, StringSplitOptions.RemoveEmptyEntries );
      if ( arga.Length < 2 ) {
        throw new ArgumentException ( "args must contain 2 arguments." );
      } else {
        string xpath = arga[ 0 ];
        string file = arga[ 1 ];
        StringBuilder outData = new StringBuilder ( );
        MemoryStream ms = new MemoryStream ( );
        using ( ms ) {
          XmlTextWriter writer = new XmlTextWriter ( ms, Encoding.UTF8 );
          using ( writer ) {
            XmlDocument tdoc = invoker.MacroEngine.CreateTaskResultXmlDocument ( result );
            XmlNode n = invoker.MacroEngine.GetXmlTaskResultNode ( result, xpath );
            if ( n == null )
              throw new ArgumentException ( "Xpath did not select any elements." );
            else if ( n.NodeType != XmlNodeType.Element )
              throw new ArgumentException ( "Xpath must select an element." );
            XmlDocument doc = new XmlDocument ( );
            doc.LoadXml ( n.OuterXml );
            System.Xml.Xsl.XslCompiledTransform trans = new System.Xml.Xsl.XslCompiledTransform();
            trans.Load ( file );

            trans.Transform ( doc, null, writer );
            ms.Position = 0;
            StreamReader reader = new StreamReader ( ms );
            while ( !reader.EndOfStream ) {
              outData.AppendLine ( reader.ReadLine ( ) );
            }

          }
        }
        return outData.ToString ( );
      }
    }

    #endregion
  }
}
