using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Audio;
using Utilities.Data;

public class GameplayUIManager : MonoBehaviour
{

    #region Public Attributes

    public GridHandler grid;
    public ZoomInOutPopUp uiAnim;
    public ZoomInOutPopUp displayCharacterAnim;

    [Header("Header Display")]
    public Image timerFill;
    public TMP_Text titleText;
    public TMP_Text hintText;
    public TMP_Text scoresText;
    public TMP_Text coinsText;
    public TMP_Text triesLeftText;
    public TMP_Text buyTimeText;
    public TMP_Text buyHintText;
    public Button buyTimeButton;
    public Button buyHintButton;
    public RectTransform displayCharactersParent;
    public UiHorizontaleShake shakeAnim;

    [Header("Managing Fields")]
    public Vector2 displaySlotSize;
    public Sprite displaySpriteSelected;
    public Sprite displaySpriteDeselected;
    public GameObject displayCharacterElement;

    #endregion

    #region Private Attributes

    private int triesLeft;
    private int totalScores;
    private int currentScores;
    private int currentCharacterIndex;
    private int hintCharacterIndex;
    private GameData gameData;
    private Coroutine timerCoroutine;
    private GameplayPopupsManager popupsManager;
    private List<GridSlot> displayElements;

    #endregion

    #region Public Methods

    public void Init(GameData _gameData, GameplayPopupsManager _popupsManager)
    {
        gameData = _gameData;
        popupsManager = _popupsManager;

        buyTimeButton.onClick.AddListener(BuyTime);
        buyHintButton.onClick.AddListener(BuyAndShowHint);
    }

    public void InitializeLevelBasedOnSelectedCatagory()
    {
        uiAnim.Animate(true);

        triesLeft = gameData.totalTries;
        buyHintText.text = gameData.coinsForHint + "";
        buyTimeText.text = gameData.coinsForTimeIncrement + "";
        triesLeftText.text = "Tries : " + gameData.totalTries;
        titleText.text = gameData.selectedCatagory.ToString().ToUpper();
        totalScores = gameData.GetSelectedCatagoryWords().Count;

        grid.Init(Generics<QuizWord>.Randomize(gameData.GetSelectedCatagoryWords()), this, popupsManager);
        AudioController.Instance.PlayAudio(AudioName.BACKGROUND);
    }

    public void PopulateDisplayCharacterSlots(int characterCount, string hint)
    {
        hintText.text = hint;
        hintCharacterIndex = 0;
        currentCharacterIndex = 0;
        scoresText.text = currentScores + " / " + totalScores;
        displayElements = new List<GridSlot>();
        List<Zoom> zoomElements = new List<Zoom>();


        while (displayCharactersParent.childCount > 0)
        {
            DestroyImmediate(displayCharactersParent.GetChild(0).gameObject);
        }
        for (int i = 0; i < characterCount; i++)
        {
            displayElements.Add(new GridSlot(i, Instantiate(displayCharacterElement,displayCharactersParent).GetComponent
                <RectTransform>(), displaySlotSize, displaySpriteSelected, displaySpriteDeselected));
            displayElements[i].frontText.text = string.Empty;
            displayElements[i].parent.localScale = Vector3.zero;

            zoomElements.Add(new Zoom(0.1f, 0.05f, displayElements[i].parent));
        }

        displayCharacterAnim.ZoomComponent = null; // important step
        displayCharacterAnim.ZoomComponent = new ZoomComponent(true, zoomElements);
        DOVirtual.DelayedCall(0.35f, delegate { displayCharacterAnim.Animate(true); });

        RestartTimer();
        DisplayCoins();
    }
    public void CharacterSelected(string character)
    {
        displayElements[currentCharacterIndex].SlotSelected();
        displayElements[currentCharacterIndex].frontText.text = character;
        displayElements[currentCharacterIndex].frontText.color = Color.white;
        currentCharacterIndex++;
    }

    public bool HaveEmptySlot()
    {
        return !(currentCharacterIndex == displayElements.Count);
    }

    public void DisplayResult(bool pass)
    {
        StopTimer();
        if (pass)
        {
            currentScores += 1;
            gameData.GiveReward();
            DOVirtual.DelayedCall(0.25f, delegate
            {
                grid.SetControlls(true);
                grid.SetUpNextWord();
            });
            AudioController.Instance.PlayAudio(AudioName.WIN_SFX);
        }
        else
        {
            shakeAnim.Shake();
            AudioController.Instance.PlayAudio(AudioName.LOOSE_SFX);

            if (IsAllTriesCompleted())
            {
                DOVirtual.DelayedCall(1f, popupsManager.LevelFailed);
                return;
            }
            DOVirtual.DelayedCall(1.25f, delegate
            {
                RestartTimer();
                RemoveAllSlots();
                grid.SetControlls(true);
            });
        }

        DisplayCoins();
    }

    public void RemoveLastSlot()
    {
        if (currentCharacterIndex == 0) { return; }

        displayElements[currentCharacterIndex - 1].SlotDeselected();
        displayElements[currentCharacterIndex - 1].frontText.text = string.Empty;

        currentCharacterIndex -= 1;
        grid.DeselectLastSlot();

        AudioController.Instance.PlayAudio(AudioName.UI_SFX);
    }

    public void RemoveAllSlots()
    {
        for(int i = 0; i < displayElements.Count; i++)
        {
            RemoveLastSlot();
        }
    }

    public void DisplayCoins()
    {
        coinsText.text = gameData.coinsEarned.ToString();
        DataController.Instance.Coins = gameData.coinsEarned;
    }

    #endregion

    #region Private Methods

    private void BuyAndShowHint()
    {
        if (hintCharacterIndex == displayElements.Count)
            return;

        if (gameData.coinsEarned < gameData.coinsForHint)
            return;

        hintCharacterIndex += 1;
        gameData.TakeCoinsForHint();

        DisplayCoins();
        RemoveAllSlots();
        DisplayHints();

        AudioController.Instance.PlayAudio(AudioName.UI_SFX);
    }

    private void DisplayHints()
    {
        for (int i = 0; i < hintCharacterIndex; i++)
        {
            displayElements[i].frontText.color = Color.white;
            displayElements[i].frontText.text = (grid.CurrentQuizWord.
                ToCharArray()[i] + "").ToUpper();
        }
    }

    private void StartTimer()
    {
        timerCoroutine = StartCoroutine(Timer());
        IEnumerator Timer()
        {
            float duration = gameData.GetLevelTimeDuration();
            float time = gameData.GetLevelTimeDuration();

            timerFill.fillAmount = 1f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                timerFill.fillAmount = time / duration;
                yield return null;
            }

            TimeUp();
        }
    }

    private void StopTimer()
    {
        if (timerCoroutine != null) { StopCoroutine(timerCoroutine); }
    }

    public void RestartTimer()
    {
        StopTimer();
        StartTimer();
    }

    private void BuyTime()
    {
        if (gameData.coinsEarned >= gameData.coinsForTimeUp)
        {
            RestartTimer();
            gameData.TakeCoinsForTimeIncrement();
            DisplayCoins();
            AudioController.Instance.PlayAudio(AudioName.UI_SFX);
        }
    }

    private void TimeUp()
    {
        if(IsAllTriesCompleted())
        {
            popupsManager.LevelFailed();
        }
        else
        {
            gameData.TakeCoinsForTimeUp();
            grid.RestoreLastQuizWord();
            grid.SetUpNextWord();
        }

        AudioController.Instance.PlayAudio(AudioName.LOOSE_SFX);
    }

    private bool IsAllTriesCompleted()
    {
        if (triesLeft > 0)
        {
            triesLeft -= 1;
            triesLeftText.text = "Tries : " + triesLeft;
            return false;
        }
        return true;
    }

        #endregion

    }