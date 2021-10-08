using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleUnit : MonoBehaviour
{


    [SerializeField]
    bool isPlayerUnit;

    /*
   [SerializeField]
   string trainerName;

   public string TrainerName
   {
       get { return trainerName;  }
   }


   [SerializeField] 
   bool isEnemyUnit;


     public bool IsEnemyUnit
   {
       get { return isEnemyUnit; }
   }

   */
    [SerializeField] 
    BattleHud hud;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

  
    public BattleHud Hud
    {
        get { return hud; }
    }

    public Monster Monster { get; set; }

    Image image;
   // Vector3 orginalPos;
    Color originalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
       // orginalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Monster monster)
    {
        Monster = monster;
        if (isPlayerUnit)
            image.sprite = Monster.Base.BackSprite;
        else
            image.sprite = Monster.Base.FrontSprite;

        hud.SetData(Monster);

        image.color = originalColor;
       // PlayEnterAnimation();
    }
    /*
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, orginalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, orginalPos.y);

        image.transform.DOLocalMoveX(orginalPos.x, 1f);
    }
    */

    /*
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }
    */
    /*
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }
    */
    /*
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
    */
}
