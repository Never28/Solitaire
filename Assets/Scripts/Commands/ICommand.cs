using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    public interface ICommand
    {

        bool Running { get; set; }

        IEnumerator Execute();

        IEnumerator Undo();
    }

}
