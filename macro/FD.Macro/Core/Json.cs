using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FD.Macro;

/// <summary>
/// JSON conventions shared by data loading, save/load and golden snapshots:
/// public fields, names = C# name with first letter lowered (TS names), nulls omitted
/// (TS undefined), NaN/Infinity written as strings, JsObj/JsNumObj as ordered objects.
/// </summary>
public static class Json
{
    public static readonly JsonSerializerOptions Options = Make(false);
    public static readonly JsonSerializerOptions Indented = Make(true);

    private static JsonSerializerOptions Make(bool indented)
    {
        var o = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNamingPolicy = new LowerFirst(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            WriteIndented = indented,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            MaxDepth = 256,
        };
        o.Converters.Add(new JsObjConverterFactory());
        return o;
    }

    public static string Serialize<T>(T value, bool indented = false) => JsonSerializer.Serialize(value, indented ? Indented : Options);
    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    public static T Deserialize<T>(Stream s) => JsonSerializer.Deserialize<T>(s, Options);

    /// <summary>C# <c>FooBar</c> → TS <c>fooBar</c> (only the first letter; <c>AA</c> → <c>aA</c>).</summary>
    public sealed class LowerFirst : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            string.IsNullOrEmpty(name) || !char.IsUpper(name[0]) ? name : char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}

/// <summary>(De)serializes <see cref="JsObj{V}"/> and <see cref="JsNumObj{V}"/> as JSON objects in JS key order.</summary>
public sealed class JsObjConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type t) =>
        t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(JsObj<>) || t.GetGenericTypeDefinition() == typeof(JsNumObj<>));

    public override JsonConverter CreateConverter(Type t, JsonSerializerOptions options)
    {
        var v = t.GetGenericArguments()[0];
        var conv = t.GetGenericTypeDefinition() == typeof(JsObj<>)
            ? typeof(JsObjConverter<>).MakeGenericType(v)
            : typeof(JsNumObjConverter<>).MakeGenericType(v);
        return (JsonConverter)Activator.CreateInstance(conv);
    }

    private sealed class JsObjConverter<V> : JsonConverter<JsObj<V>>
    {
        public override JsObj<V> Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
        {
            if (r.TokenType == JsonTokenType.Null) return null;
            if (r.TokenType != JsonTokenType.StartObject) throw new JsonException("object expected");
            var obj = new JsObj<V>();
            while (r.Read())
            {
                if (r.TokenType == JsonTokenType.EndObject) return obj;
                string k = r.GetString();
                r.Read();
                obj.Set(k, JsonSerializer.Deserialize<V>(ref r, o));
            }
            throw new JsonException("unterminated object");
        }

        public override void Write(Utf8JsonWriter w, JsObj<V> value, JsonSerializerOptions o)
        {
            w.WriteStartObject();
            foreach (var kv in value.Entries())
            {
                w.WritePropertyName(kv.Key);
                JsonSerializer.Serialize(w, kv.Value, o);
            }
            w.WriteEndObject();
        }
    }

    private sealed class JsNumObjConverter<V> : JsonConverter<JsNumObj<V>>
    {
        public override JsNumObj<V> Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
        {
            if (r.TokenType == JsonTokenType.Null) return null;
            if (r.TokenType != JsonTokenType.StartObject) throw new JsonException("object expected");
            var obj = new JsNumObj<V>();
            while (r.Read())
            {
                if (r.TokenType == JsonTokenType.EndObject) return obj;
                string k = r.GetString();
                r.Read();
                obj.O.Set(k, JsonSerializer.Deserialize<V>(ref r, o));
            }
            throw new JsonException("unterminated object");
        }

        public override void Write(Utf8JsonWriter w, JsNumObj<V> value, JsonSerializerOptions o)
        {
            w.WriteStartObject();
            foreach (var kv in value.O.Entries())
            {
                w.WritePropertyName(kv.Key);
                JsonSerializer.Serialize(w, kv.Value, o);
            }
            w.WriteEndObject();
        }
    }
}
