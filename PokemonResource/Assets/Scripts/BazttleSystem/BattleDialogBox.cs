using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleDialogBox : MonoBehaviour
{
    [Tooltip("The amount of letters that appear per second")]
    [SerializeField] 
    int lettersPerSecond = 30;
    [SerializeField] 
    Color highlightedColor;
    
    [Tooltip("The text in the dialogue box ")]
    [SerializeField] 
    TMP_Text dialogText;
    [Tooltip("The action selector")]
    [SerializeField]
    GameObject actionSelector;
    [Tooltip("The move selector")]
    [SerializeField]
    GameObject moveSelector;
    [Tooltip("The move details")]
    [SerializeField]
    GameObject moveDetails;
    

    [Tooltip("A list of texts from the action selector")]
    [SerializeField]
    List<TMP_Text> actionTexts;
    [Tooltip("A list of texts from the move screen")]
    [SerializeField]
    List<TMP_Text> moveTexts;
    [Tooltip("The power left in a move  ")]
    [SerializeField] 
    TMP_Text ppText;
    [Tooltip("The text for the type ")]
    [SerializeField] 
    TMP_Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }


    //setting up the text 
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }
    //enabling the dialog text 
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i=0; i<actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i=0; i<moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i=0; i<moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }
}
