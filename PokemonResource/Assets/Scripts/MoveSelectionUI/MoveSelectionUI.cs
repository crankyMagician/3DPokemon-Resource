using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MoveSelectionUI : MonoBehaviour
{

    [SerializeField]

    List<TMP_Text> moveTexts;


    [SerializeField]
    private Color highlightColor;

    int currentSelection = 0;
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }


    //which move to forget? pass an action back to battle system to change state 
    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentSelection;

        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentSelection;
        }

        currentSelection = Mathf.Clamp(currentSelection, 0, MonsterBase.MaxNumOfMoves);
        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentSelection);
        }

    }



    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < MonsterBase.MaxNumOfMoves+1; i++)
        {
            if(i == selection)
            {
                moveTexts[i].color = highlightColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }
}
