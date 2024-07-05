using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Stat
{
    public interface IStat
    {
        float InitialValue { get; }
        float BaseValue { get; set; }
        float Value { get; }    
        string Name { get; }
        IReadOnlyCollection<Modifier> GetModifiers(ModifierType modifierType);
    }
    
    public class Stat<T> : IStat
    {
        public float InitialValue => _initialValue;

        public float BaseValue 
        { 
            get => _baseValue + (_parent == null ? 0 : _parent.ValueWithoutPost);
            set
            {
                _baseValue = value;
            }
        }

        public float Value
        {
            get
            {
                if (_isDirty || _lastBaseValue != BaseValue) UpdateValues();

                return _value;
            }
        }

        public string Name => Key.ToString();

        public T Key => (_parent == null) ? _key : _parent.Key;

        public UnityEvent<Stat<T>> OnChangeValue { get; } = new();

        private float ValueWithoutPost
        {
            get
            {
                if (_isDirty || _lastBaseValue != BaseValue) UpdateValues();

                return _valueWithoutPost;
            }
        }

        private float _initialValue;
        private float _baseValue;
        private Stat<T> _parent;

        private T _key;
        private bool _isDirty = true;
        private float _lastBaseValue;
        private float _value;
        private float _valueWithoutPost;

        private readonly Dictionary<ModifierType, HashSet<Modifier>> _modifiers;

        private Stat()
        {
            _modifiers = new Dictionary<ModifierType, HashSet<Modifier>>
            {
                { ModifierType.Add, new HashSet<Modifier>() },
                { ModifierType.Percent, new HashSet<Modifier>() },
                { ModifierType.Multiply, new HashSet<Modifier>() }
            };
        }

        public Stat(T key, float initialValue) : this()
        {
            _initialValue = initialValue;
            _baseValue = _initialValue;
            _key = key;
        }
        
        public Stat(Stat<T> parent) : this()
        {
            _initialValue = 0f;
            _baseValue = _initialValue;
            _parent = parent;
            
            _parent.OnChangeValue.AddListener((stat) =>
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            });
        }

        public Stat(Stat<T> parent, float initialValue) : this()
        {
            _initialValue = initialValue;
            _baseValue = _initialValue;
            _parent = parent;
            
            _parent.OnChangeValue.AddListener((stat) =>
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            });
        }

        public void Add(Modifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.Type) == false)
            {
                _modifiers.Add(modifier.Type, new HashSet<Modifier>());
            }

            _modifiers[modifier.Type].Add(modifier);
            _isDirty = true;
            OnChangeValue.Invoke(this);
        }

        public void Remove(Modifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.Type))
            {
                if (_modifiers[modifier.Type].Remove(modifier))
                {
                    _isDirty = true;
                    OnChangeValue.Invoke(this);
                }
            }
        }

        public void RemoveByID(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            var removed = false;
            
            foreach (var modifiers in _modifiers.Values)
            {
                if (modifiers.RemoveWhere(modifier => modifier.ID == id) > 0)
                {
                    removed = true;
                }
            }

            if (removed)
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            }
        }

        public void RemoveBySource(object source)
        {
            if (source == null) return;

            var removed = false;

            foreach (var modifiers in _modifiers.Values)
            {
                if (modifiers.RemoveWhere(modifier => modifier.Source == source) > 0)
                {
                    removed = true;
                }
            }

            if (removed)
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            }
        }
        
        public IReadOnlyCollection<Modifier> GetModifiers(ModifierType modifierType)
        {
            if (_modifiers.TryGetValue(modifierType, out var modifiers))
            {
                return modifiers;
            }

            return Enumerable.Empty<Modifier>().ToList();
        }

        protected virtual float CalculateValue(bool withPostModifier)
        {
            float value = BaseValue;

            value = CalculateAdd(value, withPostModifier);
            value = CalculatePercent(value, withPostModifier);
            value = CalculateMultiply(value, withPostModifier);

            return value;
        }

        protected float CalculateAdd(float baseValue, bool withPostModifier)
        {
            var modifiers = _modifiers[ModifierType.Add];

            foreach (var modifier in modifiers)
            {
                if (modifier.IsPost) continue;

                baseValue += modifier.Value;
            }

            if (withPostModifier)
            {
                var postModifiers = GetPostModifiers(ModifierType.Add);

                foreach (var modifier in postModifiers)
                {
                    baseValue += modifier.Value;
                }
            }

            return baseValue;
        }

        protected float CalculatePercent(float baseValue, bool withPostModifier)
        {
            var modifiers = _modifiers[ModifierType.Percent];

            var percentAddSum = 0f;

            foreach (var modifier in modifiers)
            {
                if (modifier.IsPost) continue;

                percentAddSum += modifier.Value;
            }

            if (withPostModifier)
            {
                var postModifiers = GetPostModifiers(ModifierType.Percent);

                foreach (var modifier in postModifiers)
                {
                    percentAddSum += modifier.Value;
                }
            }

            return baseValue * (1f + percentAddSum);
        }

        protected float CalculateMultiply(float baseValue, bool withPostModifier)
        {
            var modifiers = _modifiers[ModifierType.Multiply];

            foreach (var modifier in modifiers)
            {
                if (modifier.IsPost) continue;

                baseValue *= (1f + modifier.Value);
            }

            if (withPostModifier)
            {
                var postModifiers = GetPostModifiers(ModifierType.Multiply);

                foreach (var modifier in postModifiers)
                {
                    baseValue *= (1f + modifier.Value);
                }
            }

            return baseValue;
        }

        private void UpdateValues()
        {
            _lastBaseValue = BaseValue;
            _value = CalculateValue(true);
            _valueWithoutPost = CalculateValue(false);
            _isDirty = false;
        }

        private List<Modifier> GetPostModifiers(ModifierType modifierType)
        {
            var postModifiers = new List<Modifier>();

            if (_parent != null)
            {
                postModifiers.AddRange(_parent.GetPostModifiers(modifierType));
            }

            if (_modifiers.TryGetValue(modifierType, out var modifiers))
            {
                postModifiers.AddRange(modifiers.Where(modifier => modifier.IsPost));
            }

            return postModifiers;
        }
    }
}