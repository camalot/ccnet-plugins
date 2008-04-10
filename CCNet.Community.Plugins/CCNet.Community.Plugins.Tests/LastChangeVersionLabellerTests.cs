using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CCNet.Community.Plugins.Labellers;
using Exortech.NetReflector;

namespace CCNet.Community.Plugins.Tests {
  [TestFixture]
  public class LastChangeVersionLabellerTests {
    [Test]
    public void LoadDefaults ( ) {
      try {
        string xml = @"<lastChangeVersionLabeller>
  <major>1</major>
  <minor>0</minor>
</lastChangeVersionLabeller>";
        LastChangeVersionLabeller lcvl = NetReflector.Read ( xml ) as LastChangeVersionLabeller;
      } catch {
        Assert.Fail ( "Required fields where supplied, should not have errored." );
      }
    }
  }
}
