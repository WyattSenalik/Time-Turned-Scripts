// Original Authors - Wyatt Senalik

namespace Atma
{
    public interface IEnemyDeathHandler
    {
        bool isDead { get; }
        float deathTime { get; }

        void HandleDeath(float deathTime);
        void RevertDeath();
    }
}
