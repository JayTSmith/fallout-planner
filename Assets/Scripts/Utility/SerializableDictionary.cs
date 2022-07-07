using System;
using System.Collections.Generic;
using UnityEngine;

namespace TTRPGSimulator.Utilites
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<TKey> _keys = new List<TKey>();
        public List<TValue> _values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < Math.Min(_keys.Count, _values.Count); i++)
            {
                Add(_keys[i], _values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }
    }
}