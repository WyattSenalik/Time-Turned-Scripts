using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace Helpers
{
    /// <summary>
    /// Collection of functions for setting on object active or inactive
    /// to be accessed by UnityEvents, AnimationEvents, and other function 
    /// serialization ways.
    /// </summary>
    public class ToggleGameObjectActive : MonoBehaviour
    {
        public void ToggleActive()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
        public void SetInactive()
        {
            gameObject.SetActive(false);
        }
    }
}
