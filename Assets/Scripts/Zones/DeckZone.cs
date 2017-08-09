using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitaire {
    public class DeckZone : Zone, IPointerClickHandler
    {

        public override bool CheckValidDrop(Card card, int position) {
            return false;
        }

        #region Click Event
        public void OnPointerClick(PointerEventData eventData)
        {

        }
        #endregion
    }

}
