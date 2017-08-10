using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire {
    //interfaccia per l'implementazione dei comanti che contende l'esecuzione, l'undo e una nvariabile di monitoraggio dell'esecuzione in corso
    public interface ICommand
    {

        bool Running { get; set; }

        IEnumerator Execute();

        IEnumerator Undo();
    }

}
