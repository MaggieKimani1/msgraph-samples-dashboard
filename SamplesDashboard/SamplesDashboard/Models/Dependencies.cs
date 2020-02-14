﻿// ------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.
// ------------------------------------------------------------------------------

namespace SamplesDashboard
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Welcome
    {
        [JsonProperty("data")]
        public Data data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("organization")]
        public Organization organization { get; set; }
    }

    public partial class Organization
    {
        [JsonProperty("repository")]
        public Repository repository { get; set; }
    }

    public partial class Repository
    {
        [JsonProperty("dependencyGraphManifests")]
        public DependencyGraphManifests dependencyGraphManifests { get; set; }
    }

    public partial class DependencyGraphManifests
    {
        [JsonProperty("nodes")]
        public List<DependencyGraphManifestsNode> nodes { get; set; }
    }

    public partial class DependencyGraphManifestsNode
    {
        [JsonProperty("filename")]
        public string filename { get; set; }

        [JsonProperty("dependencies")]
        public Dependencies dependencies { get; set; }
    }

    public partial class Dependencies
    {
        [JsonProperty("nodes")]
        public List<DependenciesNode> nodes { get; set; }

        [JsonProperty("totalCount")]
        public long totalCount { get; set; }
    }

    public partial class DependenciesNode
    {
        [JsonProperty("packageManager")]
        public string packageManager { get; set; }

        [JsonProperty("packageName")]
        public string packageName { get; set; }

        [JsonProperty("requirements")]
        public string requirements { get; set; }
    }

    public enum PackageManager { Nuget };

    public partial class Welcome
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                PackageManagerConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class PackageManagerConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PackageManager) || t == typeof(PackageManager?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "NUGET")
            {
                return PackageManager.Nuget;
            }
            throw new Exception("Cannot unmarshal type PackageManager");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (PackageManager)untypedValue;
            if (value == PackageManager.Nuget)
            {
                serializer.Serialize(writer, "NUGET");
                return;
            }
            throw new Exception("Cannot marshal type PackageManager");
        }

        public static readonly PackageManagerConverter Singleton = new PackageManagerConverter();
    }
}