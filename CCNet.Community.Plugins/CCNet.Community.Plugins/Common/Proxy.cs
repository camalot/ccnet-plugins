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
using Exortech.NetReflector;
using System.Net;

namespace CCNet.Community.Plugins.Common {
  /// <summary>
  /// A proxy object that will create a <see cref="System.Net.WebProxy"/>
  /// </summary>
  [ReflectorType("proxy")]
  public class Proxy {
    public Proxy ( ) {
      this.Port = 8080;
      this.UseDefault = false;
      this.UseDefaultCredentials = false;
      this.BypassOnLocal = true;
      this.BypassList = new string[ 0 ];
    }
    /// <summary>
    /// Gets or sets a value indicating whether use the default proxy.
    /// </summary>
    /// <value><c>true</c> if use the default proxy; otherwise, <c>false</c>.</value>
    [ReflectorProperty("useDefault",Required=false)]
    public bool UseDefault { get; set; }
    /// <summary>
    /// Gets or sets the host.
    /// </summary>
    /// <value>The host.</value>
    [ReflectorProperty ( "host", Required = false )]
    public string Host { get; set; }
    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>The port.</value>
    [ReflectorProperty ( "port", Required = false )]
    public int Port { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to use default credentials.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if use default credentials; otherwise, <c>false</c>.
    /// </value>
    [ReflectorProperty("useDefaultCredentials",Required=false)]
    public bool UseDefaultCredentials { get; set; }
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    [ReflectorProperty ( "username", Required = false )]
    public string UserName { get; set; }
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [ReflectorProperty ( "password", Required = false )]
    public string Password { get; set; }
    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    /// <value>The domain.</value>
    [ReflectorProperty("domain",Required=false)]
    public string Domain { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [bypass on local].
    /// </summary>
    /// <value><c>true</c> if [bypass on local]; otherwise, <c>false</c>.</value>
    [ReflectorProperty("bypassOnLocal",Required=false)]
    public bool BypassOnLocal { get; set; }
    /// <summary>
    /// Gets or sets the bypass list.
    /// </summary>
    /// <value>The bypass list.</value>
    [ReflectorArray("bypassList",Required=false)]
    public string[] BypassList { get; set; }

    /// <summary>
    /// Creates the proxy.
    /// </summary>
    /// <returns></returns>
    public WebProxy CreateProxy ( ) {
      if ( UseDefault ) {
        return WebProxy.GetDefaultProxy ( );
      } else {
        WebProxy proxy = new WebProxy ( this.Host, this.Port );
        proxy.BypassProxyOnLocal = this.BypassOnLocal;
        proxy.BypassList = this.BypassList;
        proxy.UseDefaultCredentials = this.UseDefaultCredentials;
        if ( !UseDefaultCredentials && !string.IsNullOrEmpty ( UserName ) ) {
          proxy.Credentials = new NetworkCredential ( this.UserName, this.Password, this.Domain );
        }
        return proxy;
      }
        
    }
  }
}
