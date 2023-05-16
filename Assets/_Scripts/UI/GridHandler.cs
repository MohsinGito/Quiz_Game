using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.Audio;

public class GridHandler : MonoBehaviour
{

    #region Public Attributes

    public Vector2 gridSlotSize;
    public Sprite gridSelectedSprite;
    public Sprite gridUnselectedSprite;
    public GameObject gridElementPrefab;
    public List<GridRow> gridRows;
    public ZoomInOutPopUp gridAnim;
    public delegate void ButtonEvent(int index, Level _level);

    #endregion

    #region Private Attributes

    private string userSelectedWord;
    private List<string> alphabets;
    private List<GridSlot> gridElements;
    private List<GridSlot> selectedSlots;
    private Queue<QuizWord> currentCatagoryQuizWords;

    private GameplayUIManager uiManager;
    private GameplayPopupsManager popupsManager;
    private QuizWord currentQuizWord;
    private ButtonEvent OnClickEvent;
    private int totalQuizes;

    public string CurrentQuizWord
    {
        get { return currentQuizWord.word; }
    }

    #endregion

    #region Public Methods

    public void Init(List<QuizWord> _quizWordsQueue, GameplayUIManager _uiManager, GameplayPopupsManager _popupsManager)
    {
        uiManager = _uiManager;
        popupsManager = _popupsManager;
        currentCatagoryQuizWords = new Queue<QuizWord>(Generics<QuizWord>.Randomize(_quizWordsQueue));

        alphabets = new List<string>();
        gridElements = new List<GridSlot>();
        selectedSlots = new List<GridSlot>();
        totalQuizes = currentCatagoryQuizWords.Count;
        OnClickEvent = GridElementSelected;

        InitilizeGrid();
        SetUpNextWord();
    }

    public void SetUpNextWord()
    {
        if (currentCatagoryQuizWords.Count == 0)
        {
            popupsManager.LevelCompleted(totalQuizes);
            return;
        }

        SetUpGridAccordingToNextWord();

        uiManager.PopulateDisplayCharacterSlots(currentQuizWord.word.Length, currentQuizWord.hint);
        DOVirtual.DelayedCall(0.35f, delegate { gridAnim.Animate(true); });
    }

    public void DeselectLastSlot()
    {
        selectedSlots[selectedSlots.Count - 1].SlotDeselected();
        selectedSlots.RemoveAt(selectedSlots.Count - 1);
        userSelectedWord = userSelectedWord.Remove(userSelectedWord.Length - 1);
    }

    public void RestoreLastQuizWord()
    {
        List<QuizWord> tempList = new List<QuizWord>();
        List<QuizWord> queueList = currentCatagoryQuizWords.ToList();

        tempList.Add(currentQuizWord);
        foreach (QuizWord quizWord in queueList)
            tempList.Add(quizWord);

        currentCatagoryQuizWords = new Queue<QuizWord>(tempList);
    }

    public void SetControlls(bool state)
    {
        foreach (GridSlot gridElement in gridElements)
        {
            gridElement.onClickButton.interactable = state;
        }
    }

    #endregion

    #region Private Methods

    private void InitilizeGrid()
    {
        int slotIndex = 0;
        List<Zoom> zoomElements = new List<Zoom>();

        foreach (GridRow row in gridRows)
        {
            for (int i = 0; i < row.totalSots; i++, slotIndex++)
            {
                GameObject newGridRect = Instantiate(gridElementPrefab, row.rowParent);
                gridElements.Add(new GridSlot(slotIndex, newGridRect.GetComponent<RectTransform>(),
                    gridSlotSize, gridSelectedSprite, gridUnselectedSprite, OnClickEvent));

                zoomElements.Add(new Zoom(0.35f, 0.05f, newGridRect.GetComponent<RectTransform>()));
            }
        }
        gridAnim.ZoomComponent = new ZoomComponent(false, zoomElements);

        for (int i = 65; i <= 90; i++)
            alphabets.Add(((char)i).ToString());
    }

    private void GridElementSelected(int _elementIndex, Level _level)
    {
        if (gridElements[_elementIndex].IsSelected)
            return;

        if (uiManager.HaveEmptySlot())
        {
            selectedSlots.Add(gridElements[_elementIndex]);
            gridElements[_elementIndex].SlotSelected();
            userSelectedWord += gridElements[_elementIndex].frontText.text;
            uiManager.CharacterSelected(gridElements[_elementIndex].frontText.text);
            AudioController.Instance.PlayAudio(AudioName.SELECT);
        }

        if (!uiManager.HaveEmptySlot())
            EvaluateAnswer();
    }

    private void EvaluateAnswer()
    {
        SetControlls(false);
        if (!userSelectedWord.ToUpper().Equals(currentQuizWord.word.ToUpper()))
        {
            RestoreLastQuizWord();
            uiManager.DisplayResult(false);
        }
        else
        {
            uiManager.DisplayResult(true);
        }
    }

    private void SetUpGridAccordingToNextWord()
    {
        userSelectedWord = string.Empty;
        currentQuizWord = currentCatagoryQuizWords.Dequeue();

        string tempWord = currentQuizWord.word + currentQuizWord.word;
        List<char> tempWordCharacter = tempWord.ToArray().ToList();
        List<string> charactersForGrid = new List<string>();

        for (int i = 0; i < gridElements.Count; i++)
        {
            if (i < tempWordCharacter.Count)
                charactersForGrid.Add(tempWordCharacter[i] + "");
            else
                charactersForGrid.Add(alphabets[Random.Range(0, alphabets.Count)]);
        }

        charactersForGrid = Generics<string>.Randomize(charactersForGrid);
        for (int i = 0; i < gridElements.Count; i++)
        {
            gridElements[i].SlotDeselected();
            gridElements[i].parent.localScale = Vector3.zero;
            gridElements[i].frontText.text = charactersForGrid[i].ToUpper();
        }
    }

    #endregion

}

[System.Serializable]
public struct GridRow
{
    public int totalSots;
    public RectTransform rowParent;
}