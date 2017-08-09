using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire
{
    public class DiscardZone : Zone
    {

        public override bool CheckValidDrop(Card card, int position)
        {
            return false;
        }
    }
}
