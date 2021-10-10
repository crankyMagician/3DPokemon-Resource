using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PartyMemberUI : MonoBehaviour, ISelectableItem
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] HPBar hpBar;

  //  [SerializeField] Color highlightedColor;

    Monster _monster;




    public void Init(Monster monster)
    {
        _monster = monster;
        UpdateData();

        _monster.OnHPChanged += UpdateData;
    }


    void UpdateData()
    {
        nameText.text = _monster.Base.Name;
        levelText.text = "Lvl " + _monster.Level;
        hpBar.SetHP((float)_monster.HP / _monster.MaxHp);
    }

    public void OnSelectionChanged(bool selected)
    {
        if (selected)
            nameText.color = Color.yellow;
        else
            nameText.color = Color.black;
    }

    public void SetData(Monster monster)
    {
        _monster = monster;

        nameText.text = monster.Base.Name;
        levelText.text = "Lvl " + monster.Level;
        hpBar.SetHP((float)monster.HP / monster.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }
}
