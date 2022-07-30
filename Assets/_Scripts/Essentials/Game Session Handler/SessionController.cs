using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    namespace GameSession
    {
        public class SessionController : Singleton<SessionController>
        {

            #region Private Attributes

            private long startTime;
            private List<GameState> gamePausedList;

            #endregion

            #region Public Properties

            public long StartTime
            {
                get { return startTime; }
            }

            public long CurrenTimeSincePlayed
            {
                get { return GetCurrentTimeInSeconds(); }
            }

            public float GameFPS
            {
                get { return GetCurrentFPS(); }
            }

            #endregion

            #region Unity Methods

            private void Start()
            {
                startTime = GetCurrentTimeInSeconds();
            }

            private void OnApplicationFocus(bool focus)
            {
                NotifyGameStateChanged(!focus);
            }

            #endregion

            #region Private Methods

            private long GetCurrentTimeInSeconds()
            {
                var newTime = new DateTimeOffset(DateTime.UtcNow);
                return newTime.ToUnixTimeSeconds();
            }

            private void NotifyGameStateChanged(bool focus)
            {
                gamePausedList = FindObjectsOfType<GameState>().ToList();
                foreach(GameState gamePaused in gamePausedList)
                {
                    gamePaused.GameStateChanged(focus);
                }
            }

            private float GetCurrentFPS()
            {
                return Time.frameCount / Time.time;
            }

            #endregion

        }

        public abstract class GameState : MonoBehaviour
        {
            public abstract void GameStateChanged(bool isPaused);
        }
    }
}