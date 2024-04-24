using System;
using UnityEngine;
using Object = System.Object;

namespace DarkNaku.Stat
{
    [Serializable]
    public abstract class Enumeration : Object, IComparable
    {
        [SerializeField] private string _name;
        [SerializeField] private int _id;

        public int ID => _id;
        public string Name => _name;

        protected Enumeration(int id, string name) => (_id, _name) = (id, name);

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var valueMatches = ID.Equals(otherValue.ID);
            var typeMatches = GetType().Equals(obj.GetType());

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => ID.GetHashCode();

        public int CompareTo(object other) => ID.CompareTo(((Enumeration)other).ID);
    }
}