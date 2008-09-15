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
using CCNet.Community.Plugins.Components.Pownce;
using System.Net;
using Xunit;

namespace CCNet.Community.Plugins.Tests {
	public class PownceServiceTests {
		// README!
		// To run these tests, change the network credentials to a valid pownce username and password
		// then remove the Skip = "No user name specified." from the tests.
		// also remove the 'return' statement in the constructor

		public PownceServiceTests() {
			return;
			this.Service = new PownceService ( "i43hw0i1231oq23o058h21lr0j505mf6",
				new NetworkCredential ( "SET_ME_TO_A_POWNCE_USER_ACCOUNT", "AND_THE_PASSWORD" ) );
		}
		public PownceService Service { get; set; }
		[Fact(Skip="No user name specified.")]
		public void PostNote() {
			Assert.DoesNotThrow ( new Assert.ThrowsDelegate ( delegate () {
				Service.PostText ( "testing post" );
			} ) );
		}
		[Fact ( Skip = "No user name specified." )]
		public void PostLink() {
			Assert.DoesNotThrow ( new Assert.ThrowsDelegate ( delegate () {
				Service.PostLink ( new Uri ( "http://codeplex.com/ccnetplugins" ) );
			} ) );
		}
		[Fact ( Skip = "No user name specified." )]
		public void PostEvent() {
			Assert.DoesNotThrow ( new Assert.ThrowsDelegate ( delegate () {
				string s = Service.PostEvent ( "Test Event", "Cloud 9", DateTime.Now.AddDays ( 1 ) );
			} ) );
		}

		/// <summary>
		/// Gets the send to list.
		/// </summary>
		[Fact ( Skip = "No user name specified." )]
		public void GetSendToList() {
			Assert.True ( Service.GetSendToList ( ).Count > 0 );
		}
	}
}
