using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Settings
{
    public enum eResolution { Full, Small, Tiny, Huge }

    public static class ResolutionExtensions
    {
        public static Vector2Int GetVectorSize(this eResolution res)
        {
            switch (res)
            {
                case eResolution.Full: return new Vector2Int(1920, 1080);
                case eResolution.Small: return new Vector2Int(1280, 720);
                case eResolution.Tiny: return new Vector2Int(640, 360);
                case eResolution.Huge: return new Vector2Int(2560, 1440);
                default:
                    CustomDebug.UnhandledEnum(res, nameof(ResolutionExtensions.GetVectorSize));
                    return new Vector2Int(1920, 1080);
            }
        } 
    }
}