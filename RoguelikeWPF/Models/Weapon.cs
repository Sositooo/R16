using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeWPF.Models
{
    public class Weapon
    {
        public string Name { get; }
        public int Attack { get; }
        public string ImagePath { get; } = "/Assets/weapon.png";

        public Weapon(string name, int attack)
        {
            Name = name;
            Attack = attack;
        }

        public Weapon(string name, int attack, string imagePath)
        {
            Name = name;
            Attack = attack;
            ImagePath = imagePath;
        }
    }
}