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
using Exortech.NetReflector;
using CCNet.Community.Plugins.Publishers;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
	public class PowncePublisherTests : IntegrationResultTestObject {
		string username = "ccnetplugins";
		string password = "CodePlex1";
		[Fact]
		public void Create() {
			
			string xml = @"<pownce>
	<username>" + username + @"</username>
	<password>" + password + @"</password>
	<notes>
		<note>
			<message>Test Note</message>
		</note>
		<note>
			<message>Another Test Note</message>
		</note>
	</notes>
	<links>
		<link>
			<message>New Search Engine!</message>
			<url>http://google.com</url>
		</link>
	</links>
	<events>
		<event>
			<date>5/21/2008 16:00</date>
			<location>Great Wide Open</location>
			<name>New Event</name>
		</event>
	</events>
</pownce>";

			PowncePublisher publisher = NetReflector.Read ( xml ) as PowncePublisher;
			Assert.True ( publisher.Notes.Count == 2 );
			Assert.True ( publisher.Links.Count == 1 );
			Assert.True ( publisher.Events.Count == 1 );
			Assert.True ( publisher.Files.Count == 0);
			Assert.True ( publisher.Notes[0].BuildCondition == PublishBuildCondition.AllBuildConditions );
			Assert.True ( publisher.Notes[0].BuildStatus == PublishBuildStatus.Any );
			Assert.True ( publisher.Links[0].BuildCondition == PublishBuildCondition.AllBuildConditions );
			Assert.True ( publisher.Links[0].BuildStatus == PublishBuildStatus.Any );
			Assert.True ( publisher.Events[0].BuildCondition == PublishBuildCondition.AllBuildConditions );
			Assert.True ( publisher.Events[0].BuildStatus == PublishBuildStatus.Any );
			Assert.Equal<string> ( username, publisher.UserName );
			publisher.Run ( Result );
		}
	}
}
