using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class QuitGame : MonoBehaviour
    {
        public void Quit()
        {
            if (Application.isEditor)
            {
                //CustomDebug.Log("Quit", true);
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
