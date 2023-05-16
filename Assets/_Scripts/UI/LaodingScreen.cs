using TMPro;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using Utilities.Audio;
using Utilities.Data;

public class LaodingScreen : Singleton<LaodingScreen>
{

    #region Public Attributes

    public float laodingTime;
    public GameObject loadingPanel;
    public Image loadingBarFill;
    public TMP_Text loadingPercentage;
    public Button playButton;

    #endregion

    #region Main Methods

    public void Init(UnityAction _actionOnClick, GameData _gameData)
    {
        LoadNewScene();
        //_gameData.loadingScreenPopedUp = true;

        playButton.onClick.AddListener(
            _actionOnClick +
            (() =>
            {
                loadingPanel.SetActive(false);
                AudioController.Instance.PlayAudio(AudioName.UI_SFX);
            })
        );

        
    }

    private void LoadNewScene()
    {
        StartCoroutine(DisplayLoadingUI());

        IEnumerator DisplayLoadingUI()
        {
            loadingPanel.SetActive(true);
            playButton.gameObject.SetActive(false);
            loadingBarFill.transform.parent.gameObject.SetActive(true);

            float loadProgress = 0;

            while(loadProgress < laodingTime)
            {
                loadProgress += Time.deltaTime;
                loadingBarFill.fillAmount = (loadProgress / laodingTime);
                loadingPercentage.text = (int)((loadProgress / laodingTime) * 100f) + "%";
                yield return null;
            }

            loadingBarFill.transform.parent.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }
    }

    #endregion

}
