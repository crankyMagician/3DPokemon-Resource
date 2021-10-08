using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }// TrainerBattle, 
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [Header("Battle Components")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    //ADDED
    //
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private TrainerController trainerController;

    [Space(3)]
    [Header("Catching Monsters")]
    [SerializeField]
    int catchRate = 2555;



    [Space(3)]
    [Header("Items")]

    [SerializeField]
    private GameObject monsterBallObj;



    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;

   
    [Tooltip("The party of the player ")]
    MonsterParty playerParty;
    [Tooltip("A wild monster ")]
    Monster wildMonsters;
    [SerializeField]
    [Tooltip("The party of the enemy ")]
    MonsterParty enemyParty;
    [Tooltip("The bool to determine if this is a trainer battle")]
    public bool isTrainerBattle;



    ////////
    /// <summary>
    /// Trying to trinaer battle 
    /// </summary>
    
    public void StartTrainerBattle(MonsterParty playerParty, TrainerController trainerController)
    {
        this.playerParty = playerParty;

        this.trainerController = trainerController;

        this.enemyParty = trainerController.enemyParty;

        Debug.Log("Began trainter battle");
        //start a battle passing the trainer controller to put the name in the dialogoue box
        StartCoroutine(SetupTrainerBattle(trainerController));
    }


    public IEnumerator SetupTrainerBattle(TrainerController enemyParty)
    {
        //trainerController = enemyParty;

        playerUnit.Setup(playerParty.GetHealthyPokemon());

        enemyUnit.Setup(enemyParty.enemyParty.GetHealthyPokemon());

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Monster.Moves);

        yield return dialogBox.TypeDialog($"{enemyParty.enemyName} wants to battle ");
        isTrainerBattle = true;
        //state =BattleState.TrainerBattle;
        ActionSelection();
    }
    /// <summary>
    /// Above this line ia augmented base code
    /// </summary>

    public void StartBattle(MonsterParty playerParty, Monster wildMonsters)
    {
        this.playerParty = playerParty;

        this.wildMonsters = wildMonsters;

        

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildMonsters);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Monster.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared.");

        ActionSelection();
    }

    //If you run from battle Run Turn as run else the battle is over and the battle was not won 
    void RunningFromBattle()
    {

      // if(state != BattleState.TrainerBattle)
        //{

      
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {

                StartCoroutine(RunTurns(BattleAction.Run));

                //   yield return SwitchPokemon(selectedPokemon);

            }

            else
            {
                state = BattleState.BattleOver;
                OnBattleOver(false);
                // Debug.Log("Ran from battle");
            }
      //  }
        //fix here 
        // StartCoroutine(RunTurns(BattleAction.Run));
        // StartCoroutine(RunTurns(BattleAction.Run));
       // StartCoroutine(FailedToRunFromBattle());

    }
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Monsters.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        Debug.Log("Open party screen");
        state = BattleState.PartyScreen;
       // partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        
       

        
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Monster.CurrentMove = playerUnit.Monster.Moves[currentMove];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            int playerMovePriority = playerUnit.Monster.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Base.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Monster;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Monsters[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }

            else if(playerAction == BattleAction.Run)
            {
                Debug.Log("Failed turn taking place");
                state = BattleState.Busy;
                yield return FailedToRunFromBattle();
              //  FailedRunFromBattle(enemyUnit, playerUnit, enemyMove);
            }

            else if(playerAction == BattleAction.UseItem)
            {
                yield return ThrowPokeball();
            }


            // Enemy Turn
            var enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }
   
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
       
        
        ////////
        /////
        //
        bool canRunMove = sourceUnit.Monster.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Monster);

         
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Monster);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
        {

           // sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            //targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Monster, targetUnit.Monster, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Monster.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Monster, targetUnit.Monster, secondary.Target);
                }
            }

            if (targetUnit.Monster.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.Name} Fainted");
                //targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name}'s attack missed");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Monster source, Monster target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Statuses like burn or psn will hurt the pokemon after the turn
        sourceUnit.Monster.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Monster.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} Fainted");
            //sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    bool CheckIfMoveHits(Move move, Monster source, Monster target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Monster pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                Debug.Log("Get new pokemon ");
                OpenPartyScreen();
            }

            else
            {
                BattleOver(false);
                Debug.Log("Lost");
            }

        }
        //added
        if (!faintedUnit.IsPlayerUnit && isTrainerBattle)
        {
            var nextPokemon = enemyParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                Debug.Log("Putting out new enemy pokemon");
                enemyUnit.Setup(nextPokemon);
            }
            if(nextPokemon == null)
            {
                Debug.Log("Trainter battle over?");
                isTrainerBattle = false;
                BattleOver(true); ;
            }

           // else
                //Debug.Log("Trainter battle over?");
                //isTrainerBattle = false;
             //   BattleOver(false); ;
         // */
        }
        if (!faintedUnit.IsPlayerUnit && !isTrainerBattle)
        {
            Debug.Log("Won the battle");
            BattleOver(true);
        }
        /*
        else
        {
            Debug.Log("Here lies my problem");
            BattleOver(true);
        }
        */
           
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(ThrowPokeball());
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                // Monster
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
                if (!isTrainerBattle)
                {
                    RunningFromBattle();
                }
                else
                {
                    Debug.Log("You cannot run from a trainer battle");
                }
               
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Monster.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Monster.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Monsters.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Monsters[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if (selectedMember == playerUnit.Monster)
            {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }
    IEnumerator FailedToRunFromBattle()
    {
        yield return dialogBox.TypeDialog($"You failed to run from battle!");

        state = BattleState.RunningTurn;
    }
    IEnumerator SwitchPokemon(Monster newPokemon)
    {
        if (playerUnit.Monster.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Monster.Base.Name}");
          //  playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        state = BattleState.RunningTurn;
    }
    /// 
    /// 
    /// /Cleaning up trainer battles 
    /// 

    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        var nextPokemon = enemyParty.GetHealthyPokemon();

        enemyUnit.Setup(nextPokemon);


        yield return dialogBox.TypeDialog($"Trainer changes monsters");

        state = BattleState.RunningTurn;
    }



    /*
     * Adding Items 
     */
    //
    //first throw pokeball
    #region Items
    #region Catching Monsters 
    IEnumerator ThrowPokeball()
    {

        state = BattleState.Busy;


        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You cannot catch another trainer's monster ");

            state = BattleState.RunningTurn;

            yield break;
        }
        
        yield return dialogBox.TypeDialog($"Throws a pokeball ");
        //v

        var pokeballObj = monsterBallObj.GetComponent<Sprite>();
        Instantiate(monsterBallObj, playerUnit.transform.position, Quaternion.identity);


        int shakeCount = TryToCatchPokemon(enemyUnit.Monster);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);

            
        }

        if(shakeCount == 4)
        {
            //the monster has been caught 
            yield return dialogBox.TypeDialog($"{ enemyUnit.Monster.Base.Name} was captured ");

            playerParty.AddMonster(enemyUnit.Monster);
            //destroy the pokemon
            Destroy(pokeballObj);
            //the battle is done
            BattleOver(true);


        }
        else
        {
            //the monster has escaped 
            yield return new WaitForSeconds(1f);

            if(shakeCount < 2)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} broke free");

            }
            else
            {
                yield return dialogBox.TypeDialog($"Almost caught it");
            }

            Destroy(pokeballObj);


            state = BattleState.RunningTurn;
        }

    }



    int TryToCatchPokemon(Monster monster)
    {
        float a = (3* monster.MaxHp - 2 * monster.HP) * monster.Base.CatchRate * ConditionsDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHp);

        if(a >= 255)
        {
            return 4;
        }

        float b =1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while(shakeCount < 4)
        {
            if(UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }

            ++shakeCount;
        }

        return shakeCount;

    }
    #endregion Catching Monsters
    #endregion Items


}
