using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Stat
{
    public interface ICharacterStats : IDisposable
    {
        string Name { get; }
        IReadOnlyList<IStat> Stats { get; }
    }
    
    [System.Serializable]
    public class CharacterStats<T> : ICharacterStats
    {
        public string Name { get;}
        public IReadOnlyList<IStat> Stats => _stats.Values.Cast<IStat>().ToList();
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
        
        public CharacterStats<T> Parent { get; protected set; }

        protected Dictionary<T, Stat<T>> _stats = new();

        public CharacterStats()
        {
            CharacterStatsManager.Add(this);
        }

        public CharacterStats(string name)
        {
            Name = name;
            
            CharacterStatsManager.Add(this);
        }
        
        public CharacterStats(string name, CharacterStats<T> parent)
        {
            Name = name;
            Parent = parent;

            foreach (var item in Parent.All)
            {
                var stat = new Stat<T>(item.Value);

                stat.OnChangeValue.AddListener(OnChangeValue);
                
                _stats.Add(item.Key, stat);
            }

            CharacterStatsManager.Add(this);
        } 

        public void Dispose()
        {
            CharacterStatsManager.Remove(this);
        }

        public bool Contains(T key) => _stats.ContainsKey(key);

        public bool AddStat(T key, float initialValue)
        {
            if (_stats.ContainsKey(key))
            {
                Debug.LogWarningFormat("[CharacterStats] AddStat : Already added - {0}", key);
                return false;
            }
            else
            {
                var stat = new Stat<T>(key, initialValue);

                stat.OnChangeValue.AddListener(OnChangeValue);

                _stats.Add(key, stat);

                return true;
            }
        }

        public bool AddModifier(T key, Modifier modifier)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key].Add(modifier);
                return true;
            }
            else
            {
                Debug.LogWarningFormat("[CharacterStats] AddModifier : Can't found stat - {0}", key);
                return false;
            }
        }

        public void RemoveModifier(T key, Modifier modifier)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key].Remove(modifier);
            }
            else
            {
                Debug.LogErrorFormat("[CharacterStats] RemoveModifier : Can't found stat - {0}", key);
            }
        }
        
        public void RemoveModifierByID(string id)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveByID(id);
            }
        }
        
        public void RemoveModifierBySource(object source)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveBySource(source);
            }
        }

        private void OnChangeValue(Stat<T> stat)
        {
            OnChangeStat.Invoke(this, stat);
        }
    }
}