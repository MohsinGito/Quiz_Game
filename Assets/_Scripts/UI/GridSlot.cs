using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GridHandler;

[System.Serializable]
public class GridSlot
{
    public TMP_Text frontText;
    public Image backgroundIcon;
    public Button onClickButton;
    public RectTransform parent;

    private Sprite selectedSp;
    private Sprite unSelectedSp;
    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
    }

    public GridSlot(int _index, RectTransform _parent, Vector2 _size, Sprite _s1, Sprite _s2, ButtonEvent onClickEvenet = null)
    {
        parent = _parent;
        parent.sizeDelta = _size;
        selectedSp = _s1;
        unSelectedSp = _s2;

        backgroundIcon = _parent.GetComponent<Image>();
        frontText = _parent.GetChild(0).GetComponent<TMP_Text>();

        if (onClickEvenet != null)
        {
            onClickButton = _parent.gameObject.AddComponent<Button>();
            onClickButton.onClick.AddListener(delegate { onClickEvenet(_index); });
            onClickButton.colors = GetDefaultColorBlock();
        }
        else
        {
            frontText.fontSize = 75f;
        }

        SlotDeselected();
    }

    private ColorBlock GetDefaultColorBlock()
    {
        ColorBlock colorblock = new ColorBlock();
        colorblock.colorMultiplier = 1;
        colorblock.normalColor = Color.white;
        colorblock.pressedColor = Color.white;
        colorblock.selectedColor = Color.white;
        colorblock.disabledColor = Color.white;
        colorblock.highlightedColor = Color.white;
        return colorblock;
    }

    public void SlotSelected()
    {
        isSelected = true;
        backgroundIcon.sprite = selectedSp;
    }

    public void SlotDeselected()
    {
        isSelected = false;
        backgroundIcon.sprite = unSelectedSp;
    }

}