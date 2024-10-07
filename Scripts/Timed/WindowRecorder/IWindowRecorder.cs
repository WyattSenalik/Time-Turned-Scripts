using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public interface IWindowRecorder<TData> : ITimedRecorder
    {
        IReadOnlyWindowCollection<TData> windowCollection { get; }
    }
}