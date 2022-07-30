using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.Audio;
using Utilities.Data;
using static GridHandler;

public class GameplayPopupsManager : MonoBehaviour
{

    #region Public Attributes

    public GameObject BG;

    [Header("PopUps")]
    public GameObject puaseScreen;
    public GameObject settingsScreen;
    public GameObject selectCatagoryScreen;

    [Header("Puase PopUp Elements")]
    public ZoomInOutPopUp pausePopUpAnim;
    public Button resumeBtn;
    public Button settingsBtn;
    public Button homeBtn;

    [Header("Settings PopUp Elements")]
    public ZoomInOutPopUp settingsPopUpAnim;
    public Button moreBtn;
    public Button cancelBtn;
    public Button sfxOnBtn;
    public Button sfxOffBtn;
    public Button musicOnBtn;
    public Button musicOffBtn;

    [Header("Catagory PopUp Elements")]
    public ZoomInOutPopUp catagoryPopUpAnim;
    public RectTransform catagoryListParent;
    public GameObject catagoryUiPrefab;

    [Header("Level Win Screen")]
    public ZoomInOutPopUp levelWinPopUpAnim;
    public TMP_Text totalScoresText;
    public TMP_Text totalQuizesText;
    public TMP_Text totalAnsweredText;
    public Button continueToNextLevelButton;

    [Header("Level Failed Screen")]
    public ZoomInOutPopUp levelFailedAnim;
    public Button levelFaileContinueButton;


    #endregion

    #region Private Attributes

    private GameData gameData;
    private GameplayUIManager gameplayUiManager;
    private List<CatagoryItem> catagoryList;

    #endregion

    #region Initialization Methods

    public void Init(GameData _gameData, GameplayUIManager _uiManager)
    {
        gameData = _gameData;
        gameplayUiManager = _uiManager;

        InitilizePopUpsButtons();
        InitializaVolumeButtons();
        InitializeCatagoryPopUp();

        ChangeSFXVolume(_gameData.sfxOn);
        ChangeMusicVolume(_gameData.musicOn);
    }

    private void InitilizePopUpsButtons()
    {
        homeBtn.onClick.AddListener(MoveToHomeScreen);
        resumeBtn.onClick.AddListener(delegate { PuasePopup(false); });
        settingsBtn.onClick.AddListener(delegate { SettingsPopUp(true); });
        cancelBtn.onClick.AddListener(delegate { SettingsPopUp(false); });
        //selectCatBtn.onClick.AddListener(delegate { CatagorySelectionPopUp(true); });
    }

    private void InitializaVolumeButtons()
    {
        sfxOnBtn.onClick.AddListener(delegate { ChangeSFXVolume(true); });
        sfxOffBtn.onClick.AddListener(delegate { ChangeSFXVolume(false); });
        musicOnBtn.onClick.AddListener(delegate { ChangeMusicVolume(true); });
        musicOffBtn.onClick.AddListener(delegate { ChangeMusicVolume(false); });
    }
    
    private void InitializeCatagoryPopUp()
    {
        catagoryList = new List<CatagoryItem>();

        for (int i = 0; i < gameData.gameLevels.Count; i++)
        {
            if (gameData.gameLevels[i].levelCatagory != Catagory.NONE)
            {
                catagoryList.Add(new CatagoryItem(gameData.gameLevels[i].levelQuizWords.Count, Instantiate(catagoryUiPrefab,
                catagoryListParent).GetComponent<RectTransform>(), gameData.gameLevels[i].levelCatagory, CatagorySelected));
            }
        }
    }

    #endregion

    #region Gameplay Popups Events

    public void CatagorySelectionPopUp(bool state)
    {
        BG.SetActive(state);
        catagoryPopUpAnim.Animate(state);
        catagoryListParent.anchoredPosition = Vector2.zero;
    }

    public void PuasePopup(bool state)
    {
        BG.SetActive(state);
        pausePopUpAnim.Animate(state);
        AudioController.Instance.PlayAudio(AudioName.UI_SFX);
    }

    public void SettingsPopUp(bool state)
    {
        BG.SetActive(state);
        AudioController.Instance.PlayAudio(AudioName.UI_SFX);

        if (state)
        {
            puaseScreen.SetActive(false);
            settingsScreen.SetActive(true);
            settingsPopUpAnim.Animate(true);
        }
        else
        {
            settingsScreen.SetActive(false);
            puaseScreen.SetActive(true);
            PuasePopup(true);
        }
    }

    public void CatagorySelected(int catIndex)
    {
        CatagorySelectionPopUp(false);
        gameData.selectedCatagory = (Catagory)catIndex;
        gameplayUiManager.InitializeLevelBasedOnSelectedCatagory();
        AudioController.Instance.PlayAudio(AudioName.UI_SFX);
        selectCatagoryScreen.SetActive(false);
    }

    public void MoveToHomeScreen()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void LevelCompleted(int _totalQuizes)
    {
        BG.SetActive(true);

        totalQuizesText.text = _totalQuizes + "";
        totalAnsweredText.text = _totalQuizes + "";
        totalScoresText.text = (_totalQuizes * 170) + "";

        levelWinPopUpAnim.Animate(true);
        continueToNextLevelButton.onClick.AddListener(MoveToHomeScreen);
    }

    public void LevelFailed()
    {
        BG.SetActive(true);
        levelFailedAnim.Animate(true);
        levelFaileContinueButton.onClick.AddListener(MoveToHomeScreen);
    }

    #endregion

    #region Volume Settings

    public void ChangeSFXVolume(bool state)
    {
        gameData.sfxOn = state;
        DataController.Instance.Sfx = state ? 1 : 0;
        AudioController.Instance.PlayAudio(AudioName.UI_SFX);

        if (state)
        {
            sfxOnBtn.GetComponent<Image>().enabled = true;
            sfxOffBtn.GetComponent<Image>().enabled = false;
            AudioController.Instance.UnMuteTrack("Gameplay SFX");
            AudioController.Instance.UnMuteTrack("Win/Loos SFX");
        }
        else
        {
            sfxOnBtn.GetComponent<Image>().enabled = false;
            sfxOffBtn.GetComponent<Image>().enabled = true;
            AudioController.Instance.MuteTrack("Gameplay SFX");
            AudioController.Instance.MuteTrack("Win/Loos SFX");
        }
    }

    public void ChangeMusicVolume(bool state)
    {
        gameData.musicOn = state;
        DataController.Instance.Music = state ? 1 : 0;
        AudioController.Instance.PlayAudio(AudioName.UI_SFX);

        if (state)
        {
            musicOnBtn.GetComponent<Image>().enabled = true;
            musicOffBtn.GetComponent<Image>().enabled = false;
            AudioController.Instance.UnMuteTrack("Background Music");
        }
        else
        {
            musicOnBtn.GetComponent<Image>().enabled = false;
            musicOffBtn.GetComponent<Image>().enabled = true;
            AudioController.Instance.MuteTrack("Background Music");
        }
    }

    #endregion

}

public struct CatagoryItem
{
    public Catagory catagory;
    public RectTransform parent;
    public Button onClickBtn;
    public TMP_Text quizesText;
    public TMP_Text displayText;

    public CatagoryItem(int _quizes, RectTransform _parent, Catagory _catagory, ButtonEvent _event)
    {
        parent = _parent;
        catagory = _catagory;

        displayText = parent.GetChild(1).GetComponent<TMP_Text>();
        quizesText = parent.GetChild(2).GetComponent<TMP_Text>();
        onClickBtn = parent.GetChild(3).GetComponent<Button>();

        quizesText.text = _quizes + " / " + _quizes;
        displayText.text = catagory.ToString().ToUpper();
        onClickBtn.onClick.AddListener(delegate { _event((int)_catagory); });
    }

}