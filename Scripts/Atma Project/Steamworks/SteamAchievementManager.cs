using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Helpers.Singletons;
using Steamworks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
// Original Authors - Sam Smith

namespace Atma
{
    public sealed class SteamAchievementManager : DynamicSingletonMonoBehaviourPersistant<SteamAchievementManager>
    {
        public const bool ACHIEVEMENTS_ENABLED = DeveloperConstants.IS_STEAM_BUILD && !DeveloperConstants.IS_DEMO_BUILD && !DeveloperConstants.FORCE_DISABLE_ACHIEVEMENTS;

        private static readonly Dictionary<AchievementId, string> m_achievementsSteamKey = new Dictionary<AchievementId, string>() {
            { AchievementId.GET_STOPWATCH, "GET_STOPWATCH_ACHIEVEMENT" },
            { AchievementId.MEET_INEPTUS, "MEET_INEPTUS_ACHIEVEMENT" },
            { AchievementId.LEAVE_DIRIGO, "LEAVE_DIRIGO_ACHIEVEMENT" },
            { AchievementId.GOOSE_CHASE, "GOOSE_CHASE_ACHIEVEMENT" },
            { AchievementId.FIRST_SHOT, "FIRST_SHOT_ACHIEVEMENT" },
            { AchievementId.FIRST_LEAP, "FIRST_LEAP_ACHIEVEMENT" },
            { AchievementId.REACH_ISLAND, "REACH_ISLAND_ACHIEVEMENT" },
            { AchievementId.DEFEAT_EVIL, "DEFEAT_EVIL_ACHIEVEMENT" },
            { AchievementId.TIME_LORD, "TIME_LORD_ACHIEVEMENT" },
            { AchievementId.THANK_YOU, "THANK_YOU_ACHIEVEMENT" },
            { AchievementId.LAST_SECOND, "LAST_SECOND_ACHIEVEMENT" },
            { AchievementId.OUT_OF_TIME, "OUT_OF_TIME_ACHIEVEMENT" },
            { AchievementId.GRANDFATHER_PARADOX, "GRANDFATHER_PARADOX_ACHIEVEMENT" },
            { AchievementId.SHOW_OFF, "SHOW_OFF_ACHIEVEMENT" },
            { AchievementId.DEAD_SHOT, "DEAD_SHOT_ACHIEVEMENT" },
            { AchievementId.FAVORITE_SHOW, "FAVORITE_SHOW_ACHIEVEMENT" }
        };

        private readonly Dictionary<AchievementId, bool> m_achievementCompletion = new Dictionary<AchievementId, bool>();

        protected override void OnSingletonCreated() {
            if ( ACHIEVEMENTS_ENABLED && SteamManager.Initialized ) {
                foreach ( KeyValuePair<AchievementId, string> t_achievement in m_achievementsSteamKey ) {
                    // Get whether the user has each achievement and cache that value
                    Steamworks.SteamUserStats.GetAchievement( t_achievement.Value, out bool t_achieved );
                    m_achievementCompletion.Add( t_achievement.Key, t_achieved );
                }
            }
        }

        public void GrantAchievement(AchievementId achievement)
        {
            //string t_completedString = "";
            //if (m_achievementCompletion.ContainsKey(achievement))
            //{
            //    t_completedString = $" Completed?{!m_achievementCompletion[achievement]}.";
            //}
            //Debug.Log($"Trying to grant achievenemnt: {achievement}. ACHIEVEMENTS_ENABLED?{ACHIEVEMENTS_ENABLED}. IS_STEAM_BUILD?{DeveloperConstants.IS_STEAM_BUILD}. IS_DEMO_BUILD?{DeveloperConstants.IS_DEMO_BUILD}. FORCE_DISABLE_ACHIEVEMENTS{DeveloperConstants.FORCE_DISABLE_ACHIEVEMENTS}. Contains?{m_achievementCompletion.ContainsKey(achievement)}.{t_completedString}");

            // Achivements are not enabled, just return.
            if (!ACHIEVEMENTS_ENABLED) { return; }

            // Steam manager not initialized, wait for it to be before continuing.
            if (!SteamManager.Initialized)
            {
                StartCoroutine(GrantAchivementCorout(achievement));
                return;
            }
            else
            {
                // Initialized, go straight to next part
                GrantAchivementPart2(achievement);
            }
        }
        private IEnumerator GrantAchivementCorout(AchievementId achievement)
        {
            float t_beginTime = Time.time;
            // Wait until SteamManager initialized or until the player has waited 5 seconds.
            yield return new WaitUntil(() =>
            {
                return SteamManager.Initialized || Time.time - t_beginTime > 5.0f;
            });
            float t_waitTime = Time.time - t_beginTime;
            if (t_waitTime > 5.0f)
            {
                CustomDebug.LogError($"SteamManager failed to initialize");
            }
            else
            {
                GrantAchivementPart2(achievement);
            }
        }
        private void GrantAchivementPart2(AchievementId achievement)
        {
            bool t_doesCompletionContainsAchivement = false;
            if (m_achievementCompletion.TryGetValue(achievement, out bool t_isCompleted))
            {
                t_doesCompletionContainsAchivement = true;
                if (t_isCompleted)
                {
                    // Already completed, no need to set it as achieved again.
                    return;
                }
            }

            if (m_achievementsSteamKey.TryGetValue(achievement, out string t_achivementKey))
            {
                SteamUserStats.SetAchievement(t_achivementKey);
                SteamUserStats.GetAchievement(t_achivementKey, out bool t_achieved);
                if (t_doesCompletionContainsAchivement)
                {
                    m_achievementCompletion[achievement] = t_achieved;
                }
                //Debug.Log($"Granted achievement: {m_achievementsSteamKey[achievement]}");
            }
            else
            {
                // Failed to get value, something is wrong.
                CustomDebug.LogError($"Failed to give achivement {achievement}. Was not in SteamKey dictionary");
            }
        }


        private void Update()
        {
            if (DeveloperConstants.ALLOW_RESET_STEAM_ACHIVEMENTS)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (!SteamManager.Initialized)
                    {
                        Debug.Log("Steam Manager not initialized");
                    }
                    else
                    {
                        foreach (string t_steamKey in m_achievementsSteamKey.Values)
                        {
                            SteamUserStats.ClearAchievement(t_steamKey);
                        }
                        Debug.Log("Achievements reset");
                    }
                }
            }
        }
    }
}