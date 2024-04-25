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
        IReadOnlyList<Modifier> GetModifiers(ModifierType modifierType);
    }
    
    public class Stat<T> : IStat
    {
        public float InitialValue => _initialValue;

        public float BaseValue 
        { 
            get => _parent?.Value ?? _baseValue;
            set
            {
                if (_parent == null)
                {
                    _baseValue = value;
                }
                else
                {
                    Debug.LogError("[Stat] Can't set base value to child stat.");
                }
            }
        }

        public float Value
        {
            get
            {
                if (_isDirty || _lastBaseValue != BaseValue)
                {
                    _lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    _isDirty = false;
                }

                return _value;
            }
        }

        public string Name => Key.ToString();

        public T Key => (_parent == null) ? _key : _parent.Key;

        public UnityEvent<Stat<T>> OnChangeValue { get; } = new();

        private float _initialValue;
        private float _baseValue;
        private Stat<T> _parent;

        private T _key;
        private bool _isDirty = true;
        private float _lastBaseValue;
        private float _value;

        private readonly Dictionary<ModifierType, List<Modifier>> _modifiers;

        private Stat()
        {
            _modifiers = new Dictionary<ModifierType, List<Modifier>>
            {
                { ModifierType.Add, new List<Modifier>() },
                { ModifierType.Percent, new List<Modifier>() },
                { ModifierType.Multiply, new List<Modifier>() }
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
                _modifiers.Add(modifier.Type, new List<Modifier>());
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
                if (modifiers.RemoveAll(modifier => modifier.ID == id) > 0)
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
                if (modifiers.RemoveAll(modifier => modifier.Source == source) > 0)
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
        
        public IReadOnlyList<Modifier> GetModifiers(ModifierType modifierType)
        {
            if (_modifiers.TryGetValue(modifierType, out var modifiers))
            {
                return modifiers;
            }

            return Enumerable.Empty<Modifier>().ToList();
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;

            finalValue = CalculateAdd(finalValue);
            finalValue = CalculatePercent(finalValue);
            finalValue = CalculateMultiply(finalValue);

            return finalValue;
        }

        protected float CalculateAdd(float baseValue)
        {
            var modifiers = _modifiers[ModifierType.Add];

            foreach (var modifier in modifiers)
            {
                baseValue += modifier.Value;
            }

            return baseValue;
        }

        protected float CalculatePercent(float baseValue)
        {
            var modifiers = _modifiers[ModifierType.Percent];

            var percentAddSum = 0f;

            foreach (var modifier in modifiers)
            {
                percentAddSum += modifier.Value;
            }

            return baseValue * (1f + percentAddSum);
        }

        protected float CalculateMultiply(float baseValue)
        {
            var modifiers = _modifiers[ModifierType.Multiply];

            foreach (var modifier in modifiers)
            {
                baseValue *= (1f + modifier.Value);
            }

            return baseValue;
        }
    }
}