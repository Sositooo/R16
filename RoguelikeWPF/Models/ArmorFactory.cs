using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeWPF.Models
{
    public static class ArmorFactory
    {
        private static readonly Random _rnd = new Random();

        public static Armor Create()
        {
            string[] names = { "Кожаная броня", "Кольчуга", "Латы", "Роба мага" };
            string name = names[_rnd.Next(names.Length)];
            int defense = _rnd.Next(6, 22);

            return new Armor(name, defense);   // ← только 2 параметра!
        }
    }
}