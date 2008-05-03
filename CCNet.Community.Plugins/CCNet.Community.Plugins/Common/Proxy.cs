using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using System.Net;

namespace CCNet.Community.Plugins.Common {
  [ReflectorType("proxy")]
  public class Proxy {
    [ReflectorProperty("useDefault",Required=false)]
    public bool UseDefault { get; set; }
    [ReflectorProperty ( "host", Required = false )]
    public string Host { get; set; }
    [ReflectorProperty ( "port", Required = false )]
    public int Port { get; set; }
    [ReflectorProperty("useDefaultCredentials",Required=false)]
    public bool UseDefaultCredentials { get; set; }
    [ReflectorProperty ( "username", Required = false )]
    public string UserName { get; set; }
    [ReflectorProperty ( "password", Required = false )]
    public string Password { get; set; }
    [ReflectorProperty("domain",Required=false)]
    public string Domain { get; set; }
    [ReflectorProperty("bypassOnLocal",Required=false)]
    public bool BypassOnLocal { get; set; }
    [ReflectorArray("bypassList",Required=false)]
    public string[] BypassList { get; set; }

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
