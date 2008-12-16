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
				if ( !pi.CanWrite ) {
					continue;
				}
				DefaultValueAttribute dva = Util.GetCustomAttribute<DefaultValueAttribute> ( pi );
				// check if the object allows nulls
				TypeConverterAttribute tca = Util.GetCustomAttribute<TypeConverterAttribute> ( pi );
				bool allowsNull = tca != null && string.Compare ( tca.ConverterTypeName, typeof ( ObjectOrNoneTypeConverter ).FullName ) == 0;
				ConstructorInfo constructorInfo = pi.PropertyType.GetConstructor ( new Type[] { } );
				if ( dva != null ) {
					if ( pi.PropertyType.IsClass && !Util.IsNullable ( pi.PropertyType ) && !allowsNull && constructorInfo != null ) {
						try {
							object tobj = constructorInfo.Invoke ( new object[] { } );
							pi.SetValue ( obj, tobj, null );
						} catch ( Exception ex ) {
							throw;
						}
					} else {
						pi.SetValue ( obj, dva.Value, null );
					}
				} else {
					if ( pi.PropertyType.IsClass && !Util.IsNullable ( pi.PropertyType ) && !allowsNull && constructorInfo != null ) {
						try {
							object tobj = constructorInfo.Invoke ( new object[] { } );
							pi.SetValue ( obj, tobj, null );
						} catch ( Exception ex ) {
							throw;
						}
					} else {
						pi.SetValue ( obj, null, null );
					}
				}
			}
		}
	}
}
