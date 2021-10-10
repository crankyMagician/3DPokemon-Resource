using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour, ISelectableItem
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text countText;

    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public TMP_Text NameText => nameText;
    public TMP_Text CountText => countText;

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.Name;
        countText.text = $"X {itemSlot.Count}";
    }

    public void OnSelectionChanged(bool selected)
    {
       nameText.color = selected ? GlobalSettings.i.HighlightedColor : Color.black;
    }
}
