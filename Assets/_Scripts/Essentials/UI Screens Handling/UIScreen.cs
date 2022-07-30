using System.Collections;
using UnityEngine;
namespace Utilities
{
    namespace UIMenu
    {
        public class UIScreen : MonoBehaviour
        {

            #region Public Attributes

            public ScreenType type;
            public bool animate;

            private bool m_IsOn;
            private ScreenController screenController;
            public static readonly string FLAG_ON = "On";
            public static readonly string FLAG_OFF = "Off";
            public static readonly string FLAG_NONE = "None";

            #endregion

            #region Public Properties

            public string targetState
            {
                get; private set;
            }

            public bool isOn
            {
                get
                {
                    return m_IsOn;
                }
                private set
                {
                    m_IsOn = value;
                }
            }

            #endregion

            #region Public Functions

            public void DisplayScreen(bool _on, ScreenController _screenController)
            {
                if (!screenController) { screenController = _screenController; }

                if (animate)
                {
                    AnimateScreen(_on);
                }
                else
                {
                    if (!_on)
                    {
                        gameObject.SetActive(false);
                        isOn = false;
                    }
                    else
                    {
                        isOn = true;
                    }
                }
            }

            #endregion

            #region Private Functions

            private void AnimateScreen(bool _on)
            {
                targetState = _on ? FLAG_ON : FLAG_OFF;

                // Do Animation Stuff Here

                targetState = FLAG_NONE;

                screenController.Log("Page [" + type + "] Finished Transitioning to " + (_on ? "on" : "off"));

                if (!_on)
                {
                    isOn = false;
                    gameObject.SetActive(false);
                }
                else
                {
                    isOn = true;
                }
            }

            #endregion
        }
    }
}