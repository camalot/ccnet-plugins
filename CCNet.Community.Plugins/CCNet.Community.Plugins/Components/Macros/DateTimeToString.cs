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

namespace CCNet.Community.Plugins.Components.Macros {
  /// <summary>
  /// Converts a DateTime to the specifiec string format
  /// </summary>
  /// <example>
  /// <para><c>@{DateTimeToString($(EndTime),'yyyy-MM-ddTHH:mm:ss.fffffzzz')}</c><br />
  /// This will format the date like "2007-03-12T17:41:57.953125-05:00"</para>
  /// You can also just supply the format and <c>DateTime.Now.ToString('format')</c> will be returned.
  /// </example>
  public class DateTimeToString : IMacro {
    #region IMacro Members

    /// <summary>
    /// Executes the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="args">The args.</param>
    /// <returns>Date formated string.</returns>
    public string Execute ( ThoughtWorks.CruiseControl.Core.IIntegrationResult result, string args ) {
      return Execute ( result, null, args );
    }

    #endregion

    #region IMacro Members


    /// <summary>
    /// Executes the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="invoker">The invoker.</param>
    /// <param name="args">The args.</param>
    /// <returns></returns>
    public string Execute ( ThoughtWorks.CruiseControl.Core.IIntegrationResult result, IMacroRunner invoker, string args ) {
      string[ ] arga = args.Split ( new char[ ] { ',' }, StringSplitOptions.RemoveEmptyEntries );
      if ( arga.Length == 1 )
        return DateTime.Now.ToString ( args );
      else if ( arga.Length == 2 )
        return DateTime.Parse ( arga[ 0 ] ).ToString ( arga[ 1 ] );
      else
        return "MacroFormatException: This macro does not support the supplied parmeters.";
    }

    #endregion
  }
}
