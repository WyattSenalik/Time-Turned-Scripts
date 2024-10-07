using UnityEngine;
using Helpers.Singletons;
using System.Collections.Generic;
// Original Authors - Eslis Vang


namespace Atma
{
	public class InteractionSingleton : SingletonMonoBehaviour<InteractionSingleton>
	{
		[SerializeField]
		private ToggleableDict m_toggleableDict = new ToggleableDict();
		[SerializeField]
		private ActivatorDict m_activatorDict = new ActivatorDict();

		public void AddToggleable(IToggleable t_toggleable, List<IActivator> t_activators)
        {
            if (m_toggleableDict.ContainsKey(t_toggleable)) { return; }
            m_toggleableDict.Add(t_toggleable, t_activators);
		}

		public void RemoveToggleable(IToggleable t_toggleable)
		{
			if (!m_toggleableDict.ContainsKey(t_toggleable)) { return; }
			m_toggleableDict.Remove(t_toggleable);
		}

        public void AddActivator(IActivator t_activator, List<IToggleable> t_toggleables)
        {
            if (m_activatorDict.ContainsKey(t_activator)) { return; }
            m_activatorDict.Add(t_activator, t_toggleables);
        }

        public void RemoveActivator(IActivator t_activator)
		{
			if (!m_activatorDict.ContainsKey(t_activator)) { return; }
			m_activatorDict.Remove(t_activator);
		}

		public void ClearDictionaries()
		{
			m_toggleableDict.Clear();
			m_activatorDict.Clear();
        }

        public void PrintDictionaries()
        {
			foreach (KeyValuePair<IToggleable, List<IActivator>> keyVal in m_toggleableDict)
            {
                Debug.Log($"\n----------------------------------------------");
                Debug.Log($"<color=green>{keyVal.Key} : {keyVal.Value.Count}</color>");
				foreach (object t_activator in keyVal.Value)
				{
					Debug.Log($"<color=red>{t_activator}</color>");
                }
            }
			foreach (KeyValuePair<IActivator, List<IToggleable>> keyVal in m_activatorDict)
            {
                Debug.Log($"\n----------------------------------------------");
                Debug.Log($"<color=yellow>{keyVal.Key} : {keyVal.Value.Count}</color>");
				foreach (object t_toggleable in keyVal.Value)
				{
                    Debug.Log($"<color=red>{t_toggleable}</color>");
                }
			}
        }
    }

}