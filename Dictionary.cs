using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paint_project
{
    class Dictionary
    {
        public readonly static Dictionary<ActionEnum, string> ActionTypesDictionary = new()
        {
            { ActionEnum.NoAction, "Brak" },
            { ActionEnum.DrawPoints, "Rysuj punkty" },
            { ActionEnum.MoveMouse, "Przesuwanie myszą" },
            { ActionEnum.MoveKeyboard, "Przesuwanie \npolem tekstowym" },
        };
    }
}
