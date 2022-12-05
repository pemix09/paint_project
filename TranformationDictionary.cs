using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paint_project
{
    static class TranformationDictionary
    {
        public readonly static Dictionary<TransformationActions, string> ActionTypesDictionary = new()
        {
            { TransformationActions.NoAction, "Brak" },
            { TransformationActions.Draw, "Rysowanie" },
            { TransformationActions.MouseMove, "Przesuwanie myszą" },
            { TransformationActions.MoveKeyboard, "Przesuwanie polami\n tekstowymi" },
            { TransformationActions.Scale, "Skalowanie" },
            { TransformationActions.Rotate, "Obrót" },
        };
    }
}
