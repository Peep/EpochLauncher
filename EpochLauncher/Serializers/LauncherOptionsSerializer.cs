using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpochLauncher.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EpochLauncher.Serializers
{
	public class LauncherOptionsSerializer
		: JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var config = value as LauncherOptions;
			writer.WriteStartObject();
			writer.WritePropertyName("ArmaPath");
			serializer.Serialize(writer, config.ArmaPath);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var obj = JObject.Load(reader);
			var config = new LauncherOptions();
			JToken value;

			if (obj.TryGetValue("ArmaPath", out value))
			{
				config.ArmaPath = value.Value<string>();
			}

			return config;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (LauncherOptions).IsAssignableFrom(objectType);
		}
	}
}
