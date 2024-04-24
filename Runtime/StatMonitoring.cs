using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Stat
{
    public static class StatMonitoring
    {
        public static IReadOnlyCollection<ICharacterStats> Characters => _characters;
        
        private static readonly HashSet<ICharacterStats> _characters = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _characters.Clear();

            Application.quitting += OnQuit;
        }
        
        private static void OnQuit()
        {
            _characters.Clear();

            Application.quitting -= OnQuit;
        }

        public static void Add(ICharacterStats character)
        {
            if (_characters.Contains(character) == false)
            {
                _characters.Add(character);
            }
        }

        public static void Remove(ICharacterStats character)
        {
            if (_characters.Contains(character))
            {
                _characters.Remove(character);
            }
        }
    }
}