using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CCNetConfig.Core;
using System.ComponentModel;
using CCNetConfig.Core.Components;

namespace CCNet.Community.Plugins.CCNetConfig {
	/// <summary>
	/// 
	/// </summary>
	static class Utils {
		/// <summary>
		/// Resets the object properties.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		public static void ResetObjectProperties<T> ( T obj ) {
			PropertyInfo[] props = obj.GetType ().GetProperties ( BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty );
			foreach ( PropertyInfo pi in props ) {
				DefaultValueAttribute dva = Util.GetCustomAttribute<DefaultValueAttribute> ( pi );
				// check if the object allows nulls
				TypeConverterAttribute tca = Util.GetCustomAttribute<TypeConverterAttribute> ( pi );
				bool allowsNull = tca != null && string.Compare ( tca.ConverterTypeName, typeof ( ObjectOrNoneTypeConverter ).FullName ) == 0;

		
				if ( dva != null ) {
					if ( pi.PropertyType.IsClass && ( !pi.PropertyType.IsValueType || !pi.PropertyType.IsPrimitive || Util.IsNullable ( pi.PropertyType ) ) && !allowsNull ) {
						object tobj = pi.PropertyType.TypeInitializer.Invoke ( null );
						pi.SetValue ( obj, tobj, null );
					} else {
						pi.SetValue ( obj, dva.Value, null );
					}
				} else {
					if ( pi.PropertyType.IsClass && ( !pi.PropertyType.IsValueType || !pi.PropertyType.IsPrimitive || Util.IsNullable ( pi.PropertyType ) ) && !allowsNull ) {
						object tobj = pi.PropertyType.TypeInitializer.Invoke ( null );
						pi.SetValue ( obj, tobj, null );
					} else {
						pi.SetValue ( obj, null, null );
					}
				}
			}
		}
	}
}
