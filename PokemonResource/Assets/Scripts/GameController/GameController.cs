using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }

public class GameController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;
    [SerializeField] 
    BattleSystem battleSystem;
    [SerializeField]
    Camera worldCamera;


    [Header("Saving Values")]
    [SerializeField]
    string saveSlot = "saveSlot1";

    GameState state;

    GameState stateBeforePause;

    //addded
    [SerializeField]
    TrainerController trainerController;

    //for Scene Stuff 
    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public static GameController Instance { get; private set; }


    private void Awake()
    {
        MonsterDB.Init();

        MoveDB.Init();

        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterTrainersView += (Collider trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
               // StartCoroutine(trainer.TriggerTrainerBattle(playerController));
                trainerController = trainer;
                StartTrainerBattle();
                //trainer.OnEncountered += StartTrainerBattle;
            }
        };

       // trainerController.OnEncountered += StartTrainerBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }
    void StartTrainerBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        //
        var enemyParty = trainerController;//.GetComponent<MonsterParty>();

        battleSystem.StartTrainerBattle(playerParty, enemyParty);
    }
    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Monster(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy);
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            //wut wut wut 
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }


        //for testing saving and loading 

        if (Input.GetKeyDown(KeyCode.S))
        {
            SavingSystem.i.Save(saveSlot);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SavingSystem.i.Load(saveSlot);
        }
    }



    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }
}
