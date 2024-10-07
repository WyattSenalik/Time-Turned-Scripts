using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2Int RoundToVector2Int(this Vector2 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }
    }
}
