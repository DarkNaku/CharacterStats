using System;
using UnityEngine;

namespace DarkNaku.Stat
{
    [Serializable]
    public class Modifier
    {
        [SerializeField] private ModifierType _type;
        [SerializeField] private float _value;
        [SerializeField] private string _id;
        [SerializeField] private bool _isPost;

        public ModifierType Type => _type;
        public float Value => _value;
        public string ID => _id;
        public bool IsPost => _isPost;
        public object Source { get; set; }

        public Modifier(ModifierType type, float value)
        {
            _type = type;
            _value = value;
        }

        public Modifier SetID(string id) {
            _id = id;

            return this;
        }

        public Modifier SetPost(bool isPost)
        {
            _isPost = isPost;

            return this;
        }

        public override string ToString() => $"Type : {_type}, Value : {_value}, Source : {nameof(Source)}, ID : {ID}, IsPost : {IsPost}";
    }
}