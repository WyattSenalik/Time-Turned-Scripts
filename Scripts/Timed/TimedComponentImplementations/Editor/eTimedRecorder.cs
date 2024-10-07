// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Supported TimedRecorders that have implementations for displaying there data using <see cref="TimedTimlineEditorWindow"/>. If more are added. <see cref="TimedRecorderEnumExtensions.GetCorrespondingGraphs"/> must also be edited to include the corrsponding implementation of <see cref="ITimedRecorderEditorGraphs"/>.
    /// </summary>
    public enum eTimedRecorder { Transform, Rigidbody2D, EnemyHealth, HoldPressurePlate, SpriteRenderer, Animator, CharacterMover, IntTransform, TimeCloneTransform }

    public static class TimedRecorderEnumExtensions
    {
        private static readonly TimedTransformEditorGraphs s_transformEditorGraphs = new TimedTransformEditorGraphs();
        private static readonly TimedRigidbody2DEditorGraphs s_rb2DEditorGraphs = new TimedRigidbody2DEditorGraphs();
        private static readonly EnemyHealthEditorGraphs s_enemyHealthEditorGraphs = new EnemyHealthEditorGraphs();
        private static readonly HoldPPEditorGraphs s_holdPPEditorGraphs = new HoldPPEditorGraphs();
        private static readonly TimedSpriteRendererEditorGraphs s_sprRendEditorGraphs = new TimedSpriteRendererEditorGraphs();
        private static readonly TimedAnimatorEditorGraphs s_timedAnimEditorGraphs = new TimedAnimatorEditorGraphs();
        private static readonly TimedCharacterMoverEditorGraphs s_timedCharMoverEditorGraphs = new TimedCharacterMoverEditorGraphs();
        private static readonly TimedIntTransformEditorGraphs s_timedIntEditorGraphs = new TimedIntTransformEditorGraphs();
        private static readonly TimeCloneTransformEditorGraphs s_timeCloneTransEditorGraphs = new TimeCloneTransformEditorGraphs();

        public static ITimedRecorderEditorGraphs GetCorrespondingGraphs(this eTimedRecorder timedRecorderType)
        {
            switch (timedRecorderType)
            {
                case eTimedRecorder.Transform:
                    return s_transformEditorGraphs;
                case eTimedRecorder.Rigidbody2D:
                    return s_rb2DEditorGraphs;
                case eTimedRecorder.EnemyHealth:
                    return s_enemyHealthEditorGraphs;
                case eTimedRecorder.HoldPressurePlate:
                    return s_holdPPEditorGraphs;
                case eTimedRecorder.SpriteRenderer:
                    return s_sprRendEditorGraphs;
                case eTimedRecorder.Animator:
                    return s_timedAnimEditorGraphs;
                case eTimedRecorder.CharacterMover:
                    return s_timedCharMoverEditorGraphs;
                case eTimedRecorder.IntTransform:
                    return s_timedIntEditorGraphs;
                case eTimedRecorder.TimeCloneTransform:
                    return s_timeCloneTransEditorGraphs;
                default:
                    CustomDebug.UnhandledEnum(timedRecorderType, nameof(TimedRecorderEnumExtensions));
                    return null;
            }
        }
    }
}