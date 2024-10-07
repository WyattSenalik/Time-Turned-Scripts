using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Interface for <see cref="Behaviour"/> if a new Behaviour script is created
    /// with its own interface and you want to be able to do Behaviour things from that
    /// interface.
    /// </summary>
    public interface IBehaviour : IComponent
    {
        /// <summary>
        /// Enabled Behaviours are Updated, disabled Behaviours are not.
        /// 
        /// This is shown as the small checkbox in the inspector of the behaviour.
        /// </summary>
        public bool enabled { get; set; }
        /// <summary>
        /// Has the Behaviour had active and enabled called?
        /// 
        /// A GameObject can be active or not active.Similarly, a script can
        /// be enabled or disabled.If a GameObject is active and has an enabled
        /// script then isActiveAndEnabled will return true. Otherwise false is
        /// returned.
        /// </summary>
        public bool isActiveAndEnabled { get; }
    }
}
