using System;

namespace DarkNaku.Stat
{
    [Serializable]
    public class ModifierType : Enumeration
    {
        public static ModifierType Add = new(1, nameof(Add));
        public static ModifierType Percent = new(2, nameof(Percent));
        public static ModifierType Multiply = new(3, nameof(Multiply));

        public ModifierType(int id, string name) : base(id, name)
        {
        }
    }
}