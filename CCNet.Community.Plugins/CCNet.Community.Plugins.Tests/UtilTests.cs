using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
  public class UtilTests : IntegrationResultTestObject {

    [Fact]
    public void GetModificationComments ( ) {
      string s = Util.GetModidicationCommentsString ( this.Result );
      Assert.True ( s.Length > 0 );
    }

    [Fact]
    public void LowerString ( ) {
      Assert.True ( string.Compare ( "foo", Util.ToLowerString ( "FOO" ), false ) == 0 );
    }
    
  }
}
