using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Stat
{
    public interface ICharacterStats
    {
        string Name { get; }
        IReadOnlyList<IStat> Stats { get; }
    }
    
    [System.Serializable]
    public class CharacterStats<T> : ICharacterStats
    {
        public string Name => _name ?? _parent.Name;
        public IReadOnlyList<IStat> Stats { get; }
        public IReadOnlyDictionary<T, Stat<T>> All => _stats;
        public UnityEvent<CharacterStats<T>, Stat<T>> OnChangeStat { get; } = new();

        public Stat<T> this[T key]
        {
            get
            {
                if (_stats.ContainsKey(key))
                {
                    return _stats[key];
                }
                else
                {
                    Debug.LogErrorFormat("[CharacterStats] Can't found stat - {0}", key);
                    return null;
                }
            }
        }
        
        private string _name;
        private CharacterStats<T> _parent;
        private Dictionary<T, Stat<T>> _stats = new();

        public CharacterStats(string name)
        {
            _name = name;
            
#if UNITY_EDITOR 
            StatMonitoring.Add(this);
#endif
        }
        
        public CharacterStats(string name, CharacterStats<T> parent) : this(name)
        {
            _parent = parent;

            foreach (var item in _parent.All)
            {
                var stat = new Stat<T>(item.Value);

                stat.OnChangeValue.AddListener(OnChangeValue);
                
                _stats.Add(item.Key, stat);
            }
        } 

        public bool Contains(T key) => _stats.ContainsKey(key);

        public void Add(T key, float initialValue)
        {
            if (_stats.ContainsKey(key))
            {
                Debug.LogErrorFormat("[CharacterStats] Add : Already added - {0}", key);
            }
            else
            {
                var stat = new Stat<T>(key, initialValue);

                stat.OnChangeValue.AddListener(OnChangeValue);

                _stats.Add(key, stat);
            }
        }

        public void AddModifier(T key, Modifier modifier)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key].AddModifier(modifier);
            }
            else
            {
                Debug.LogErrorFormat("[CharacterStats] AddModifier : Can't found stat - {0}", key);
            }
        }

        public void RemoveModifier(T key, Modifier modifier)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key].RemoveModifier(modifier);
            }
            else
            {
                Debug.LogErrorFormat("[CharacterStats] RemoveModifier : Can't found stat - {0}", key);
            }
        }
        
        public void RemoveModifierFromID(string id)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveModifiersFromID(id);
            }
        }
        
        public void RemoveModifierFromSource(object source)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveModifiersFromSource(source);
            }
        }

        public void Log(string title)
        {
            var info = $"[CharacterStats  -  {title}]";
            
            foreach (var stat in _stats.Values)
            {
                info += $"\n{stat.Key} = Value : {stat.Value}";
            }
            
            Debug.Log(info);
        }

        private void OnChangeValue(Stat<T> stat)
        {
            OnChangeStat.Invoke(this, stat);
        }
    }
}