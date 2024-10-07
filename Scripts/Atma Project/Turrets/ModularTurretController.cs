using UnityEngine;

using Helpers.Physics.Custom2DInt;
using Helpers.Physics.Custom2DInt.NavAI;
// Original Author - Sam Smith
// Tweaked by Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Handles the core state of a modular turret
    /// </summary>
    public class ModularTurretController : MonoBehaviour
    {
        public Int2DTransform target { get; set; } = null;
        public int health { get; set; } = 1;
    }
}
