using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector2 ToVector2(this Vector2Int vector)
        {
            return new Vector2(vector.x, vector.y);
        } 
    }
}
