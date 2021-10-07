using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [Space(2)]
    [Header("NPC Traits")]
    [Tooltip("The dialogue the npc has")]
    [SerializeField]
    Dialog dialog;
    [Tooltip("The transforms the npc moves to ")]
    [SerializeField] 
    List<Transform> movementPattern;

    [Tooltip("The time between moving to the transforms ")]
    [SerializeField] 
    float timeBetweenPattern;

    [Space(2)]
    [Header("Debug")]
    [SerializeField]
    private Vector3 cubeRadius;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
                idleTimer = 0f;
                state = NPCState.Idle;
            }));
        }
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern].position);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform item in movementPattern)
        {
            Gizmos.DrawWireCube(transform.position, cubeRadius);
        }
       
    }
    */
}

public enum NPCState { Idle, Walking, Dialog }
