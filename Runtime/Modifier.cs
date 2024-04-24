using System;
using UnityEngine;

namespace DarkNaku.Stat
{
    [Serializable]
    public class Modifier
    {
        [SerializeField] private ModifierType _type;
        [SerializeField] private float _value;

        public ModifierType Type => _type;
        public float Value => _value;
        public object Source { get; set; }
        public string ID { get; set; }

        public Modifier(ModifierType type, float value, object source = null, string id = "")
        {
            _type = type;
            _value = value;
            Source = source;
            ID = id;
        }

        public override string ToString() => $"Type : {_type}, Value : {_value}, Source : {nameof(Source)}, ID : {ID}";
    }
}