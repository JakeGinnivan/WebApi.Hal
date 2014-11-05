﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal.JsonConverters
{
    public class EmbeddedResourceConverter : JsonConverter
    {
        const string StreamingContextEmbeddedConverterToken = "hal+json";
        const StreamingContextStates StreamingContextResourceConverterState = StreamingContextStates.Clone;
        public static bool IsEmbeddedResourceConverterContext(StreamingContext context)
        {
            return context.Context is string &&
                   (string)context.Context == StreamingContextEmbeddedConverterToken &&
                   context.State == StreamingContextResourceConverterState;
        }

        private static StreamingContext GetResourceConverterContext()
        {
            return new StreamingContext(StreamingContextResourceConverterState, StreamingContextEmbeddedConverterToken);
        } 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resourceList = (IList<EmbeddedResource>)value;
            if (resourceList.Count == 0) return;
            var saveContext = serializer.Context;
            serializer.Context = GetResourceConverterContext();
            
            
            writer.WriteStartObject();

            foreach (var rel in resourceList)
            {
                writer.WritePropertyName(NormalizeRel(rel.Resources[0]));
                if (rel.IsSourceAnArray)
                    writer.WriteStartArray();
                foreach (var res in rel.Resources)
                    serializer.Serialize(writer, res);
                if (rel.IsSourceAnArray)
                    writer.WriteEndArray();
            }
            writer.WriteEndObject();
            serializer.Context = saveContext;
        }

        private static string NormalizeRel(IResource res)
        {
            if (!string.IsNullOrEmpty(res.Rel)) return res.Rel;
            return "unknownRel-" + res.GetType().Name;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<EmbeddedResource>).IsAssignableFrom(objectType);
        }
    }
}
