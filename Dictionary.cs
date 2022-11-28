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
            { ActionEnum.NoAction, "No action" },
            { ActionEnum.DrawPoints, "Draw points" },
            { ActionEnum.MoveMouse, "Move with mouse" },
            { ActionEnum.MoveKeyboard, "Move with text field" },
        };
    }
}
