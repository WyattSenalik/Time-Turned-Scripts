using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Original Authors - Jack Dekko, Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Getting, Setting popup obj and details
    /// </summary>
    public sealed class PopupInstance
    {
        public GameObject popupObj { get; private set; } = null;
        public PopupDetails details { get; private set; } = null;


        public PopupInstance(GameObject popupObj, PopupDetails details) 
        {
            this.popupObj = popupObj;
            this.details = details;
        }

    }
}
