using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class TimedSoundInstance : TimedRecorder
    {
        private const string TIMED_SOUND_INSTANCE_PREFAB_RESOURCE = "SoundInstance_PREFAB";

        public bool isLooping { get; private set; } = false;
        public float clipLength { get; private set; } = float.NaN;
        public uint playForwardEventId { get; private set; } = 0;
        public uint playBackwardEventId { get; private set; } = 0;
        public uint stopForwardEventId { get; private set; } = 0;
        public uint stopBackwardEventId { get; private set; } = 0;

        [SerializeField] private bool m_isDebugging = false;

        private PlaySpeedParamSyncer playSpeedSyncer
        {
            get
            {
                InitializeSoundSpeedSyncerIfNotInitialized();
                return m_playSpeedSyncer;
            }
        }
        private PlaySpeedParamSyncer m_playSpeedSyncer = null;

        private readonly List<TimeFrame> m_playTimeFrames = new List<TimeFrame>();
        private bool m_isPlayingForward = false;
        private bool m_isPlayingBackwards = false;


        private void Start()
        {
            InitializeSoundSpeedSyncerIfNotInitialized();
            timedObject.ForceSetTimeBounds(0.0f, furthestTime);
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // No need to do anything if recording
            if (isRecording) { return; }

            if (TryGetCurrentSoundStartTime(out float t_soundStartTime))
            {
                if (deltaTime > 0.0f)
                {
                    if (!m_isPlayingForward/*IsEventPlayingOnGameObject(playForwardEventId, gameObject)*/)
                    {
                        StartPlayingForward(t_soundStartTime);
                    }
                    else if (playSpeedSyncer.prevPitch == 0.0f)
                    {
                        UpdateForwardSeek(t_soundStartTime);
                    }
                }
                else if (deltaTime < 0.0f)
                {
                    if (!m_isPlayingBackwards/*IsEventPlayingOnGameObject(playBackwardEventId, gameObject)*/)
                    {
                        StartPlayingBackward(t_soundStartTime);
                    }
                    else if (playSpeedSyncer.prevPitch == 0.0f)
                    {
                        UpdateBackwardSeek(t_soundStartTime);
                    }
                }
                else
                {
                    StopPlayingBackward();
                    StopPlayingForward();
                }
            }
            else
            {
                StopPlayingBackward();
                StopPlayingForward();
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            // Remove all play times that are in the future.
            while (m_playTimeFrames.Count > 0 && m_playTimeFrames[^1].startTime > time)
            {
                m_playTimeFrames.RemoveAt(m_playTimeFrames.Count - 1);
            }

            // If we resume when a sound should be playing, play at that point
            if (m_playTimeFrames.Count > 0)
            {
                if (TryGetCurrentSoundStartTime(out float t_soundStartTime))
                {
                    StopPlayingBackward();
                    StopPlayingForward();
                    StartPlayingForward(t_soundStartTime);
                }
            }
        }


        public bool TryPlay(float soundStartTime)
        {
            if (IsAvailableForSound(soundStartTime))
            {
                if (!isRecording)
                {
                    #region Logs
                    //CustomDebug.LogWarningForComponent($"tried to play sound during manipulation.", this);
                    #endregion Logs
                    return false;
                }
                float t_endTime = isLooping ? float.PositiveInfinity : soundStartTime + clipLength;
                m_playTimeFrames.Add(new TimeFrame(soundStartTime, t_endTime));
                StartPlayingForward(soundStartTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Stop(float newStopTime)
        {
            bool t_frameWasShortened = false;
            for (int i = 0; i < m_playTimeFrames.Count; ++i)
            {
                TimeFrame t_frame = m_playTimeFrames[i];
                if (t_frame.ContainsTime(newStopTime))
                {
                    m_playTimeFrames[i] = new TimeFrame(t_frame.startTime, newStopTime);
                    t_frameWasShortened = true;
                }
            }
            if (t_frameWasShortened)
            {
                StopPlayingForward();
            }
        }
        /// <summary>
        /// If there does not exist a sound currently is after or overlaps the given time.
        /// </summary>
        public bool IsAvailableForSound(float newSoundStartTime)
        {
            // Assumes that all TimedSounds are stored in chronological order and that none have overlapping TimeFrames.
            if (m_playTimeFrames.Count <= 0)
            {
                return true;
            }
            TimeFrame t_furthestFrame = m_playTimeFrames[^1];
            if (newSoundStartTime <= t_furthestFrame.startTime)
            {
                // Time is before furthest start time, don't allow.
                return false;
            }
            if (t_furthestFrame.ContainsTime(newSoundStartTime))
            {
                // Time is during furthest sound, don't allow.
                return false;
            }
            // Time is after furthest sound, allow. 
            return true;
        }
        public float GetFurthestPlayTime()
        {
            if (m_playTimeFrames.Count <= 0)
            {
                return float.NaN;
            }
            return m_playTimeFrames[^1].startTime;
        }


        private bool TryGetCurrentSoundStartTime(out float soundStartTime)
        {
            for (int i = m_playTimeFrames.Count - 1; i >= 0; --i)
            {
                TimeFrame t_frame = m_playTimeFrames[i];
                soundStartTime = t_frame.startTime;
                if (t_frame.ContainsTime(curTime))
                {
                    return true;
                }
            }
            soundStartTime = float.PositiveInfinity;
            return false;
        }

        private void StartPlayingForward(float startTime)
        {
            #region Logs
            //CustomDebug.LogForComponent($"[{Time.frameCount}] Playing Forward ({playForwardEventId})", this, m_isDebugging);
            #endregion Logs

            AkSoundEngine.PostEvent(playForwardEventId, gameObject);
            m_isPlayingForward = true;
            UpdateForwardSeek(startTime);
        }
        private void UpdateForwardSeek(float startTime)
        {
            #region Logs
            //CustomDebug.LogForComponent($"[{Time.frameCount}] Updating Forward Seek for ({playForwardEventId}) that started at ({startTime})", this, m_isDebugging);
            #endregion Logs

            float t_seekPercent;
            if (isLooping)
            {
                t_seekPercent = ((curTime - startTime) % clipLength) / clipLength;
                AkSoundEngine.SeekOnEvent(playForwardEventId, gameObject, t_seekPercent);
                AkSoundEngine.RenderAudio();
            }
            else
            {
                t_seekPercent = (curTime - startTime) / clipLength;
                if (t_seekPercent < 1.0f && t_seekPercent >= 0.0f)
                {
                    AkSoundEngine.SeekOnEvent(playForwardEventId, gameObject, t_seekPercent);
                    AkSoundEngine.RenderAudio();
                }
                else
                {
                    StopPlayingForward();
                }
            }
        }
        private void StopPlayingForward()
        {
            if (m_isPlayingForward)
            {
                #region Logs
                //CustomDebug.LogForComponent($"[{Time.frameCount}] Stopping Play of ({playForwardEventId})", this, m_isDebugging);
                #endregion Logs
                AkSoundEngine.PostEvent(stopForwardEventId, gameObject);
                m_isPlayingForward = false;
            }
        }

        private void StartPlayingBackward(float startTime)
        {
            #region Logs
            //CustomDebug.LogForComponent($"[{Time.frameCount}] Playing Backward ({playBackwardEventId})", this, m_isDebugging);
            #endregion Logs

            AkSoundEngine.PostEvent(playBackwardEventId, gameObject);
            m_isPlayingBackwards = true;
            UpdateBackwardSeek(startTime);
        }
        private void UpdateBackwardSeek(float startTime)
        {
            #region Logs
            //CustomDebug.LogForComponent($"[{Time.frameCount}] Updating Backward Seek for ({playForwardEventId}) that started at ({startTime})", this, m_isDebugging);
            #endregion Logs

            float t_seekPercent;
            if (isLooping)
            {
                t_seekPercent = (clipLength - ((curTime - startTime) % clipLength)) / clipLength;
                AkSoundEngine.SeekOnEvent(playBackwardEventId, gameObject, t_seekPercent);
                AkSoundEngine.RenderAudio();
            }
            else
            {
                t_seekPercent = (clipLength - (curTime - startTime)) / clipLength;
                if (t_seekPercent < 1.0f && t_seekPercent >= 0.0f)
                {
                    AkSoundEngine.SeekOnEvent(playBackwardEventId, gameObject, t_seekPercent);
                    AkSoundEngine.RenderAudio();
                }
                else
                {
                    StopPlayingBackward();
                }
            }
        }
        private void StopPlayingBackward()
        {
            if (m_isPlayingBackwards)
            {
                #region Logs
                //CustomDebug.LogForComponent($"[{Time.frameCount}] Stopping Play of ({playBackwardEventId})", this, m_isDebugging);
                #endregion Logs
                AkSoundEngine.PostEvent(stopBackwardEventId, gameObject);
                m_isPlayingBackwards = false;
            }
        }

        private void InitializeSoundSpeedSyncerIfNotInitialized()
        {
            if (m_playSpeedSyncer == null)
            {
                m_playSpeedSyncer = PlaySpeedParamSyncer.instance;
                #region Asserts
                //CustomDebug.AssertSingletonIsNotNull(m_playSpeedSyncer, this);
                #endregion Asserts
            }
        }

        private void PlaySoundAfterFrame(float playTime)
        {
            StartCoroutine(PlaySoundAfterFrameCorout(playTime));
        }
        private IEnumerator PlaySoundAfterFrameCorout(float playTime)
        {
            yield return null;
            TryPlay(playTime);
        }


        private static GameObject instanceParentObj
        {
            get
            {
                if (s_instanceParentObj == null)
                {
                    s_instanceParentObj = new GameObject("SoundInstanceParent");
                }
                return s_instanceParentObj;
            }
        }
        private static GameObject s_instanceParentObj = null;
        public static TimedSoundInstance CreateInstance(float firstPlayTime, bool isLooping, float clipLength, uint playForwardEventId, uint playBackwardEventId, uint stopForwardEventId, uint stopBackwardEventId)
        {
            GameObject t_soundInstancePrefab = Resources.Load<GameObject>(TIMED_SOUND_INSTANCE_PREFAB_RESOURCE);
            GameObject t_spawnedObj = Instantiate(t_soundInstancePrefab, instanceParentObj.transform);
            t_spawnedObj.name = $"SoundInstance ({playForwardEventId}_{firstPlayTime})";
            TimedSoundInstance t_soundInstance = t_spawnedObj.GetComponent<TimedSoundInstance>();
            t_soundInstance.isLooping = isLooping;
            t_soundInstance.clipLength = clipLength;
            t_soundInstance.playForwardEventId = playForwardEventId;
            t_soundInstance.playBackwardEventId = playBackwardEventId;
            t_soundInstance.stopForwardEventId = stopForwardEventId;
            t_soundInstance.stopBackwardEventId = stopBackwardEventId;
            // Since the object just spawned, we need to wait 1 frame before actually playing the sound.
            t_soundInstance.PlaySoundAfterFrame(firstPlayTime);
            return t_soundInstance;
        }

        private readonly static uint[] s_playingIds = new uint[16];
        private static bool IsEventPlayingOnGameObject(uint eventId, GameObject go)
        {
            uint t_count = (uint)s_playingIds.Length;
            AkSoundEngine.GetPlayingIDsFromGameObject(go, ref t_count, s_playingIds);

            for (int i = 0; i < t_count; i++)
            {
                uint t_playingId = s_playingIds[i];
                uint t_curEventId = AkSoundEngine.GetEventIDFromPlayingID(t_playingId);

                if (t_curEventId == eventId) { return true; }
            }

            return false;
        }
    }
}