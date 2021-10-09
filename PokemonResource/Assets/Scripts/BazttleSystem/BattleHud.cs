using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleHud : MonoBehaviour
{





    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] HPBar hpBar;


    [SerializeField] GameObject expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Monster _monster;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Monster pokemon)
    {
        _monster = pokemon;

        nameText.text = pokemon.Base.Name;
       // levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);

        //added
        SetLevel();
        SetExp();


        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor },
        };

        SetStatusText();
        _monster.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_monster.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _monster.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_monster.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_monster.HpChanged)
        {
             yield return hpBar.SetHPSmooth((float)_monster.HP / _monster.MaxHp);
            _monster.HpChanged = false;
        }
    }



    public void SetLevel()
    {
        levelText.text = "Level " + _monster.Level;
    }
    public void SetExp()
    {
        if(expBar == null)
        {
            return;
        }

        float normalizedExp = GetNormalizedExp();

        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);

    }

    public IEnumerator SetExpSmooth()
    {
        if(expBar == null)
        {
            yield break;
        }


        float normalizedExp = GetNormalizedExp();

        yield return expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    float GetNormalizedExp()
    {
        int currentLevelExp = _monster.Base.GetExpForLevel(_monster.Level);

        int nextLevelExp = _monster.Base.GetExpForLevel(_monster.Level + 1);

        float normalizedExp = (float)(_monster.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }
}
