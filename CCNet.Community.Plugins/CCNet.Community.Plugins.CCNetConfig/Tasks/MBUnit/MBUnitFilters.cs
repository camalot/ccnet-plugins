﻿/*
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
using CCNetConfig.Core.Serialization;
using CCNetConfig.Core.Components;
using CCNet.Community.Plugins.CCNetConfig.Common;
using System.ComponentModel;

namespace CCNet.Community.Plugins.CCNetConfig.Tasks.MBUnit {
	[ReflectorName("filters")]
	public class MBUnitFilters : ICCNetObject, ICloneable {
		/// <summary>
		/// Initializes a new instance of the <see cref="MBUnitFilters"/> class.
		/// </summary>
		public MBUnitFilters () {
			this.FilterCategories = new CloneableList<Category> ();
			this.ExludeCategories = new CloneableList<Category> ();
			this.Authors = new CloneableList<MBUnitAuthor> ();
			this.Types = new CloneableList<MBUnitType> ();
			this.Namespaces = new CloneableList<MBUnitNamespace> ();
		}

		/// <summary>
		/// Gets or sets the filter categories.
		/// </summary>
		/// <value>The filter categories.</value>
		[Description ( "Specifies that only those test fixtures decorated with the FixtureCategory attribute and categoryName will be run in this execution of MbUnit.Cons." ),
		ReflectorName ( "filterCategories" ), DefaultValue ( null ),
		ReflectorArray ( "category" ), Category ( "Optional" ), 
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<Category> FilterCategories { get; set; }

		/// <summary>
		/// Gets or sets the exlude categories.
		/// </summary>
		/// <value>The exlude categories.</value>
		[Description ( "Specifies that those test fixtures decorated with the FixtureCategory attribute and categoryName will not be run in this execution of MbUnit.Cons." ),
		ReflectorName ( "excludeCategories" ), DefaultValue ( null ),
		ReflectorArray ( "category" ), Category ( "Optional" ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<Category> ExludeCategories { get; set; }

		/// <summary>
		/// Gets or sets the authors.
		/// </summary>
		/// <value>The authors.</value>
		[Description ( "Specifies that only those test fixtures decorated with the Author attribute and authorName will be run in this execution of MbUnit.Cons" ),
		ReflectorName ( "authors" ), DefaultValue ( null ),
		ReflectorArray ( "author" ), Category ( "Optional" ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<MBUnitAuthor> Authors { get; set; }

		/// <summary>
		/// Gets or sets the types.
		/// </summary>
		/// <value>The types.</value>
		[Description ( "Specifies that only those tests of the type className will be run in this execution of MbUnit.Cons." ),
		ReflectorName ( "types" ), DefaultValue ( null ),
		ReflectorArray ( "type" ), Category ( "Optional" ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<MBUnitType> Types { get; set; }

		/// <summary>
		/// Gets or sets the namespaces.
		/// </summary>
		/// <value>The namespaces.</value>
		[Description ( "Specifies that only those tests in the named namespace will be run in this execution of MbUnit.Cons. " ),
		ReflectorName ( "namespaces" ), DefaultValue ( null ),
		ReflectorArray ( "ns" ), Category ( "Optional" ),
		TypeConverter ( typeof ( IListTypeConverter ) )]
		public CloneableList<MBUnitNamespace> Namespaces { get; set; }

		#region ISerialize Members

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public void Deserialize ( System.Xml.XmlElement element ) {
			new Serializer<MBUnitFilters> ().Deserialize ( element, this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public System.Xml.XmlElement Serialize () {
			return new Serializer<MBUnitFilters> ().Serialize ( this );
		}

		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public MBUnitFilters Clone () {
			MBUnitFilters f = this.MemberwiseClone () as MBUnitFilters;
			f.Authors = this.Authors.Clone ();
			f.ExludeCategories = this.ExludeCategories.Clone ();
			f.FilterCategories = this.FilterCategories.Clone ();
			f.Namespaces = this.Namespaces.Clone ();
			f.Types = this.Types.Clone ();
			return f;
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

		public override string ToString () {
			return this.GetType().Name;
		}
	}
}
