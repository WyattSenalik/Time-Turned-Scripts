using System;
using System.Collections.Generic;
using Helpers.GenericDictionary;

namespace Atma
{
    [Serializable]
    public sealed class ToggleableDict : GenericDictionary<IToggleable, List<IActivator>> { }
}