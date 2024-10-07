using System;
using System.Collections.Generic;
using Helpers.GenericDictionary;

namespace Atma
{
    [Serializable]
    public sealed class ActivatorDict : GenericDictionary<IActivator, List<IToggleable>> { }
}