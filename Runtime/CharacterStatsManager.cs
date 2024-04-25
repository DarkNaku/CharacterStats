using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Stat
{
    public static class CharacterStatsManager
    {
        public static IReadOnlyCollection<ICharacterStats> All => _characterStats;
        
        private static readonly HashSet<ICharacterStats> _characterStats = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _characterStats.Clear();

            Application.quitting += OnQuit;
        }
        
        private static void OnQuit()
        {
            _characterStats.Clear();

            Application.quitting -= OnQuit;
        }

        public static void Add(ICharacterStats character)
        {
            if (_characterStats.Contains(character) == false)
            {
                _characterStats.Add(character);
            }
        }

        public static void Remove(ICharacterStats character)
        {
            if (_characterStats.Contains(character))
            {
                _characterStats.Remove(character);
            }
        }

        public static void ExecuteForeach<T>(System.Action<CharacterStats<T>> action)
        {
            foreach (var stats in _characterStats)
            {
                if (stats is CharacterStats<T> characterStats)
                {
                    action.Invoke(characterStats);
                }
            }
        }

        public static IReadOnlyCollection<CharacterStats<T>> GetCharacterStatsAll<T>()
        {
            var result = new HashSet<CharacterStats<T>>();

            ExecuteForeach<T>(characterStats => result.Add(characterStats));

            return result;
        }

    }
}