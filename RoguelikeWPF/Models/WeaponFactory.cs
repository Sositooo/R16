using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RoguelikeWPF.Models
{
    public static class WeaponFactory
    {
        private static readonly Random _rnd = new Random();

        public static Weapon Create()
        {
            string[] names = { "Меч", "Топор", "Кинжал", "Посох", "Молот" };
            string name = names[_rnd.Next(names.Length)];
            int attack = _rnd.Next(13, 29);

            return new Weapon($"{name} +{attack}", attack);
        }
    }
}