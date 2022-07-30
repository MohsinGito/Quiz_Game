using System.Collections;
using UnityEngine;

namespace Utilities
{
    namespace UIMenu
    {
        public class ScreenController : Singleton<ScreenController>
        {

            #region Public Attributes

            public bool debug;
            public ScreenType entryPage;
            public UIScreen[] pages;
            public Hashtable m_Pages;

            #endregion

            #region Unity Functions

            private void Awake()
            {
                m_Pages = new Hashtable();

                RegisterAllPages();
                if (entryPage != ScreenType.None)
                {
                    TurnPageOn(entryPage);
                }
            }

            #endregion

            #region Public Functions  

            public void TurnPageOn(ScreenType type)
            {
                if (type == ScreenType.None)
                    return;

                if (!PageExist(type))
                {
                    LogWarning("Page [" + type + "] Has Not Been Registered");
                    return;
                }

                UIScreen page = GetPage(type);
                page.gameObject.SetActive(true);
                page.DisplayScreen(true, this);
            }

            public void TurnPageOff(ScreenType off, ScreenType on = ScreenType.None, bool waitForExit = false)
            {
                if (off == ScreenType.None)
                    return;
                if (!PageExist(off))
                {
                    LogWarning("Page [" + off + "] Has Not Been Registered");
                    return;
                }

                UIScreen offPage = GetPage(off);
                if (offPage.gameObject.activeSelf)
                {
                    offPage.DisplayScreen(false, this);
                }

                if (on != ScreenType.None)
                {
                    UIScreen onPage = GetPage(on);
                    if (waitForExit)
                    {
                        StopCoroutine("WaitForPageExit");
                        StartCoroutine(WaitForPageExit(onPage, offPage));
                    }
                    else
                    {
                        TurnPageOn(on);
                    }
                }
            }

            public bool PageIsOn(ScreenType _type)
            {
                if (!PageExist(_type))
                {
                    LogWarning("You Are Trying To Detect IF A Page Is On [" + _type + "], But It Has Not Been Registered");
                    return false;
                }
                return GetPage(_type).isOn;
            }

            public void Log(string msg)
            {
                if (!debug)
                    return;

                Debug.Log("[Page Controller]: " + msg); ;
            }

            public void LogWarning(string msg)
            {
                if (!debug)
                    return;

                Debug.LogWarning("[Page Controller]: " + msg); ;
            }

            #endregion

            #region Private Functions  

            private IEnumerator WaitForPageExit(UIScreen on, UIScreen off)
            {
                while (off.targetState != UIScreen.FLAG_NONE)
                {
                    yield return null;
                }
                TurnPageOn(on.type);
            }

            private void RegisterAllPages()
            {
                foreach (UIScreen page in pages)
                {
                    if (PageExist(page.type))
                    {
                        LogWarning("Page [" + page.type + "] Already Registered : " + page.gameObject.name);
                        return;
                    }

                    m_Pages.Add(page.type, page);
                    Log("Registered New Page [" + page.type + "]");
                }
            }

            private UIScreen GetPage(ScreenType type)
            {
                if (!PageExist(type))
                {
                    LogWarning("The Page [" + type + "] You Are Trying To Access Has Not Been Registered");
                    return null;
                }
                return (UIScreen)m_Pages[type];
            }

            private bool PageExist(ScreenType type)
            {
                return m_Pages.ContainsKey(type);
            }

            #endregion
        }

        public enum ScreenType
        {
            None,
            Loading,
            Menu,
            Setting
        }
    }
}