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
 *    - CCNetConfig:
 *      - http://ccnetconfig.org
 *      - http://codeplex.com/ccnetconfig
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using CCNetConfig.Core;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using CCNetConfig.Core.Serialization;

namespace CCNet.Community.Plugins.CCNetConfig.Tasks.MBUnit {
	/// <summary>
	/// Specifies that only those tests of the type className will be run in this execution of MbUnit.Cons. 
	/// </summary>
	[ReflectorName("type")]
	public class MBUnitType : ICCNetObject, ICloneable {
		/// <summary>
		/// Initializes a new instance of the <see cref="MBUnitType"/> class.
		/// </summary>
		public MBUnitType () {

		}
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Description ( "The type to filter" ),
		ReflectorName ( "type" ), ReflectorNodeType ( ReflectorNodeTypes.Value ),
		Required, DisplayName ( "(Value)" ), Category ( "Required" )]
		public string Value { get; set; }

		#region ISerialize Members

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<MBUnitType> ().Deserialize ( element, this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public System.Xml.XmlElement Serialize () {
			return new Serializer<MBUnitType> ().Serialize ( this );
		}

		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public MBUnitType Clone () {
			return this.MemberwiseClone () as MBUnitType;
		}


		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		object ICloneable.Clone () {
			return this.Clone ();
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString () {
			return string.IsNullOrEmpty ( this.Value ) ? this.GetType ().Name : this.Value;
		}
	}
}
