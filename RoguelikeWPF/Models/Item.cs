using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeWPF.Models
{
    public abstract class Item
    {
        public string Name { get; }
        public string ImagePath { get; }
        protected Item(string name, string imagePath)
        {
            Name = name;
            ImagePath = imagePath;
        }
    }
}