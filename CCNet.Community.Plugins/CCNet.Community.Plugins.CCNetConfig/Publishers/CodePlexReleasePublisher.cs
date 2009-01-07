using System;
using System.Collections.Generic;
using System.Text;
using CCNetConfig.Core;
using CCNetConfig.Core.Serialization;

namespace CCNet.Community.Plugins.CCNetConfig.Publishers {
	public class CodePlexReleasePublisher : PublisherTask{
		/// <summary>
		/// Creates a copy of this object.
		/// </summary>
		/// <returns></returns>
		public override PublisherTask Clone () {
			CodePlexReleasePublisher cprp = this.MemberwiseClone () as CodePlexReleasePublisher;

			return cprp;
		}

		/// <summary>
		/// Deserializes the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		public override void Deserialize ( System.Xml.XmlElement element ) {
			Utils.ResetObjectProperties<CodePlexReleasePublisher> ( this );
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns></returns>
		public override System.Xml.XmlElement Serialize () {
			return new Serializer<CodePlexReleasePublisher> ().Serialize ( this );
		}
	}
}
