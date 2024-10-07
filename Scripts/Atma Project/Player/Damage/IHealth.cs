using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public interface IHealth : IMonoBehaviour
    {
        bool isDead { get; }

        /// <summary></summary>
        /// <returns>If the object causing damage is some consumable (like a bullet), if that consumable should be used up as a result of the damage being applied.</returns>
        bool TakeDamage(IDamageContext context);
    }
}