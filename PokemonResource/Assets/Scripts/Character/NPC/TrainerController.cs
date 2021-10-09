using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrainerController : MonoBehaviour, ISavable
{


    public NPCState state;

    public event Action OnEncountered;

    [Space(2)]
    [SerializeField]
    public MonsterParty enemyParty;
    /// <summary>
    /// Added properties above line 
    /// </summary>
   
    public string enemyName;

    [Space(2)]
    [SerializeField] 
    Dialog dialog;
    [SerializeField] 
    GameObject exclamation;
    [SerializeField]
    GameObject fov;

    [SerializeField]
    //state 
    bool battleLost = false;
   
    



    
    Character character;
    private void Awake()
    {
        enemyParty = GetComponent<MonsterParty>();
        character = GetComponent<Character>();
    }

    private void Start()
    {
       // SetFovRotation(character.Animator.DefaultDirection);
    }

    public string Name
    {
        get => enemyName;
    }

    public void BattleLost(bool battleFought)
    {
        //change to true if defeated;
        battleLost = battleFought;
        state = NPCState.Interactable;
        if (battleLost)
        {
            TurnOffTrainerBattle();
        }
      //  return battleLost
    }

    private void Update()
    {
        if(state == NPCState.Interactable)
        {
            Debug.Log("Defeated");
            //TurnOffTrainerBattle();
        }
    }

    private void TurnOffTrainerBattle()
    {
       fov.SetActive(false);
        this.gameObject.tag = "Interactable";
        this.gameObject.layer = 10;
    }


    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
        {
            fov.SetActive(false);
        }

      //  state = (enum)state;
    }
    /*
public IEnumerator TriggerTrainerBattle(PlayerController player)
{
   // Show Exclamation
   exclamation.SetActive(true);
   yield return new WaitForSeconds(0.8f);
   exclamation.SetActive(false);

   // Walk towards the player
   var diff = player.transform.position - transform.position;
   var moveVec = diff - diff.normalized;
   moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

   yield return character.Move(moveVec);

   // Show dialog
   StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
   {
       Debug.Log("Starting Trainer Battle");
      // player.public
   }));
 //  TrainerController.OnEncountered



}
*/

    //must be modded for 3D
    /*
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
    */
}
