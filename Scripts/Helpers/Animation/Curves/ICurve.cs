// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    public interface ICurve
    {
        BetterCurve.eTimeChoice timing { get; }
        float timeScale { get; }
        float timeDuration { get; }

        float GetEndTime();
    }
}