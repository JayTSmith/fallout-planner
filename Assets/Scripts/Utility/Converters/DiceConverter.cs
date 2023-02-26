using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceConverter : Newtonsoft.Json.JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Die) || objectType == typeof(int);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        int sides = (int) obj["sides"];

        return Die.makeDie(sides);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanWrite => false;
}
