using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    [DisallowMultipleComponent]
    public sealed class TimedGameObjectActivate : MonoBehaviour
    {

        public void SetActiveForTime(float timeToRemainActive)
        {
            gameObject.SetActive(true);

            CancelInvoke();
            Invoke(nameof(TurnOff), timeToRemainActive);
        }


        private void TurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}