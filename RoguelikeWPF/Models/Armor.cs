using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeWPF.Models
{
    // Armor.cs
    public class Armor
    {
        public string Name { get; }
        public int Defense { get; }
        public Armor(string name, int defense) { Name = name; Defense = defense; }
    }
}
