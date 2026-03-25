using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeWPF.Models
{
    // Weapon.cs
    public class Weapon
    {
        public string Name { get; }
        public int Attack { get; }
        public Weapon(string name, int attack) { Name = name; Attack = attack; }
    }
}
