﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum GameState { FreeRoam, Battle, Dialog, Menu, Cutscene, Paused }

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

    [SerializeField]
    GameState state;

    GameState stateBeforePause;

    //addded

    MenuController menuController;

    [SerializeField]
    TrainerController trainerController;

    [SerializeField] PartyScreen partyScreen;

    public StateMachine<GameController> StateMachine { get; private set; }

    //for Scene Stuff 
    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public static GameController Instance { get; private set; }


    private void Awake()
    {
        menuController = GetComponent<MenuController>();

        MonsterDB.Init();

        MoveDB.Init();

        ConditionsDB.Init();
    }

    private void Start()
    {
        partyScreen.Init();

         //StateMachine = new StateMachine<GameController>(this);
         // StateMachine.ChangeState(GameFreeRoamState.Instance);

        //DialogManager.Instance.OnShowDialog += () => StateMachine.PushState(GameDialogueState.Instance);

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


        /*
         * partyScreen.Init();

            StateMachine = new StateMachine<GameController>(this);
            StateMachine.ChangeState(GameFreeRoamState.Instance);

            DialogManager.Instance.OnShowDialog += () => StateMachine.PushState(GameDialogueState.Instance);
         * 
         * 
         * 
         */

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


        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;
    }


    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePause;
        }
    }

    #region ToBeAdded

    /*
     * 
     * 
    private void Update()
    {
        StateMachine.HandleUpdate();

        // Just for testing
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartBattle();
        }
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Log(StateMachine.CurrentState.ToString());
    }

    public TrainerController Trainer { get; set; }
    public void StartBattle(TrainerController trainer = null)
    {
        Trainer = trainer;
        StateMachine.ChangeState(GameBattleState.Instance);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        StateMachine.ChangeState(GameBusyState.Instance);
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

     */
    #endregion
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
        if (trainerController != null && won == true)
        {
           // trainerController.BattleLost();
            trainerController = null;
        }
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


            //p to pull up the menu
            if (Input.GetKeyDown(KeyCode.P))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }


        //for testing saving and loading 
        /*
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavingSystem.i.Save(saveSlot);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SavingSystem.i.Load(saveSlot);
        }
        */
    }



    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }


    //pause menu
    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            // Pokemon
        }
        else if (selectedItem == 1)
        {
            // Bag
        }
        else if (selectedItem == 2)
        {
            // Save
            SavingSystem.i.Save("saveSlot1");
        }
        else if (selectedItem == 3)
        {
            // Load
            SavingSystem.i.Load("saveSlot1");
        }

        state = GameState.FreeRoam;
    }
}
