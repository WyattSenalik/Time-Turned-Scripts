using UnityEngine;

namespace Atma
{
    public static class WwiseBankStartupSystem
    {
        private static bool s_isLoaded = false;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            if (s_isLoaded) { return; }

            GameObject t_bankObj = Object.Instantiate(Resources.Load("WwiseBank")) as GameObject;
            AkSoundEngine.RegisterGameObj(t_bankObj, "BankHolder");

            s_isLoaded = true;
        }

    }
}