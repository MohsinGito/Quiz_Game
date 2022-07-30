using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.UIMenu;
using System;

namespace UnityCore
{
    namespace Scene
    {
        public class SceneController : Singleton<SceneController>
        {

            #region Main Attributes

            public bool debug;
            public delegate void SceneLoadDelegate(SceneType _scene);

            private bool m_SceneIsLoading;
            private float screenLoadProgress;
            private SceneType m_TargetScene;
            private ScreenType m_LoadingPage;
            private ScreenController m_Menu;
            private SceneLoadDelegate m_SceneLoadDelegate;

            #endregion

            #region Public Properties

            private string currentSceneName
            {
                get { return SceneManager.GetActiveScene().name; }
            }

            public float LoadProgress
            {
                get { return screenLoadProgress; }
            }

            private ScreenController menu
            {
                get
                {
                    if (m_Menu == null)
                    {
                        m_Menu = ScreenController.Instance;
                    }
                    if (m_Menu == null)
                    {
                        LogWarning("You are trying to access the PageController, but no instance was found.");
                    }
                    return m_Menu;
                }
            }

            #endregion

            #region Unity Functions

            private void Start()
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            private void OnDisable()
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }

            #endregion

            #region Public Functions

            public void Load(SceneType _scene, SceneLoadDelegate _sceneLoadDelegate = null, bool _reload = false, ScreenType _loadingPage = ScreenType.None)
            {
                if (_loadingPage != ScreenType.None)
                    return;

                if (!SceneCanBeLoaded(_scene, _reload))
                    return;

                m_SceneIsLoading = true;
                m_TargetScene = _scene;
                m_LoadingPage = _loadingPage;
                m_SceneLoadDelegate = _sceneLoadDelegate;
                StartCoroutine("LoadScene");
            }

            #endregion

            #region Private Functions

            private async void OnSceneLoaded(UnityEngine.SceneManagement.Scene _scene, LoadSceneMode _mode)
            {
                if (m_TargetScene == SceneType.None)
                {
                    return;
                }

                SceneType _sceneType;
                if (!Enum.TryParse<SceneType>(_scene.name, out _sceneType))
                    return;

                if (m_TargetScene != _sceneType)
                {
                    return;
                }

                if (m_SceneLoadDelegate != null)
                {
                    try
                    {
                        m_SceneLoadDelegate(_sceneType);
                    }
                    catch (Exception)
                    {
                        LogWarning("Unable to respond with sceneLoadDelegate after scene [" + _sceneType + "] loaded.");
                    }
                }

                if (m_LoadingPage != ScreenType.None)
                {
                    await Task.Delay(1000);
                    menu.TurnPageOff(m_LoadingPage);
                }

                m_SceneIsLoading = false;
            }

            private IEnumerator LoadScene()
            {
                if (m_LoadingPage != ScreenType.None)
                {
                    menu.TurnPageOn(m_LoadingPage);
                    while (!menu.PageIsOn(m_LoadingPage))
                    {
                        yield return null;
                    }
                }

                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_TargetScene.ToString());
                while (!asyncLoad.isDone)
                {
                    screenLoadProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                    Log("Scene Loading : " + screenLoadProgress + "%");
                    yield return null;
                }
            }

            private bool SceneCanBeLoaded(SceneType _scene, bool _reload)
            {
                string _targetSceneName = _scene.ToString();
                if (currentSceneName == _targetSceneName && !_reload)
                {
                    LogWarning("You are trying to load a scene [" + _scene + "] which is already active.");
                    return false;
                }
                else if (_targetSceneName == string.Empty)
                {
                    LogWarning("The scene you are trying to load [" + _scene + "] is not valid.");
                    return false;
                }
                else if (m_SceneIsLoading)
                {
                    LogWarning("Unable to load scene [" + _scene + "]. Another scene [" + m_TargetScene + "] is already loading.");
                    return false;
                }

                return true;
            }

            private void Log(string _msg)
            {
                if (!debug)
                    return;

                Debug.Log("[Scene Controller]: " + _msg);
            }

            private void LogWarning(string _msg)
            {
                if (!debug)
                    return;

                Debug.LogWarning("[Scene Controller]: " + _msg);
            }
            #endregion
        }
    }
}