using UnityEngine;

using Helpers.Transforms;
using Timed.TimedComponentImplementations;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Time Clone Descendant Copier Class.
    /// Plays the transform for a descendant of the time clone.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimeCloneDescendantTransform : TimeCloneDescendantSnapshotCopier<TimedTransform, TransformDataSnapshot, TransformData>
    {
        protected override void ApplyData(TransformDataSnapshot snapshot)
        {
            snapshot.data.ApplyLocal(transform);
        }
    }
}