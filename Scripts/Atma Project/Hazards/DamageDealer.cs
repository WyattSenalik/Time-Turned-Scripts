using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Simple class that contains a function to hurt the given game object.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Tag] private string[] m_tagsToDamage = new string[0];


        /// <summary>
        /// Tries to deal damage to the given object. If the object does not have one of the listed tags, does nothing. If the object has one of hte listed tags but has no <see cref="IHealth"/> in its ancestors, throws an error.
        /// </summary>
        /// <returns>If the thing that dealt damage was consumable (like a bullet), this value is whether or not that consumable should be used up or not.</returns>
        public bool DealDamage(GameObject objToHurt, float damageTime, out bool hasTagToDmg)
        {
            #region Logs
            //CustomDebug.LogForComponent($"Trying to deal damage to {objToHurt}", this, IS_DEBUGGING);
            #endregion Logs
            hasTagToDmg = HasTagToDamage(objToHurt);
            if (hasTagToDmg)
            {
                IHealth t_playerHp = objToHurt.GetComponentInParent<IHealth>();
                #region Asserts
                //CustomDebug.AssertIComponentInParentOnOtherIsNotNull(t_playerHp, objToHurt, this);
                #endregion Asserts
                #region Logs
                //CustomDebug.LogForComponent($"Calling TakeDamage", this, IS_DEBUGGING);
                #endregion Logs
                return t_playerHp.TakeDamage(new DamageContext(damageTime, gameObject));
            }
            return true;
        }
        /// <summary>
        /// Tries to deal damage to the given object. If the object does not have one of the listed tags, does nothing. If the object has one of hte listed tags but has no <see cref="IHealth"/> in its ancestors, throws an error.
        /// </summary>
        /// <returns>If the thing that dealt damage was consumable (like a bullet), this value is whether or not that consumable should be used up or not.</returns>
        public bool DealDamage(GameObject objToHurt, float damageTime) => DealDamage(objToHurt, damageTime, out _);


        private bool HasTagToDamage(GameObject objToHurt)
        {
            foreach (string t_tag in m_tagsToDamage)
            {
                if (objToHurt.CompareTag(t_tag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}