using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;



[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SavableEntity))]
public class PlayerController : MonoBehaviour, ISavable
{


    


    [Tooltip("Turn this on to debug in the inspector")]
    public bool debugMode;


    //[SerializeField]
    public GameState state;

    [Tooltip("The player's monster party")]
    public MonsterParty playerParty;


    [Tooltip("The playe's name ")]
    [SerializeField]
    private string playerName;

    //[Tooltip("The event that takes place when the player has an encounter")]
   // public Event encounterEvent;
    public event Action OnEncountered;

    public event Action<Collider> OnEnterTrainersView;

    //testing to start trianaer battle
    public event Action OnEnterTrainerBattle;

    [Space(2)]

    [Header("Values")]

    [Tooltip("The player's movement speed")]

    [SerializeField]

    private float moveSpeed;


    [Tooltip("The transform of the camera")]
    [SerializeField]
    private Transform camTransform;
    //[Tooltip("The input from the player and the keyboard")]

  //  [SerializeField]

   // private Vector2 playerInput;

    [Tooltip("Layermasks")]

    [SerializeField]

    private LayerMask encounterSpace, trainerBattle;

    [Tooltip("The radius of the overlap sphere")]

    [SerializeField]
    private float sphereRadius = 2f;

    [Tooltip("The time value for the camera moving")]
    public float turnSmoothTime = 0.1f;
    [Tooltip("The value to smooth the camera moving")]
    [SerializeField]
    private float turnSmoothVelocity;




    [Space(2)]

    [Header("Bools to control various aspects of the player controller")]

    public bool canMove;



    [Space(2)]

    [Header("Componenets")]


    /*
    [Tooltip("This is the player's physics")]
    [SerializeField]
    private Rigidbody2D playerRigidBody;
    */

    [Tooltip("The built in character controller from Unity")]
    [SerializeField]
    private CharacterController characterController;

    [Tooltip("The anim component for the player")]

    [SerializeField]

    private Animator anim;



    void Start()
    {
        //currentState = PlayerState.walk;
        anim = GetComponent<Animator>();
        // playerRigidBody = GetComponent<Rigidbody2D>();
        //anim.SetFloat("moveX", 0);
        //anim.SetFloat("moveY", 0);

    }
    private void Update()
    {
        if (state != GameState.FreeRoam)
        {
            canMove = false;
           // HandleUpdate();
        } 
        else if (state == GameState.FreeRoam)
        {
            canMove = true;
           // HandleUpdate();
        }
        if (canMove)
        {
            HandleUpdate();
        }
       
        //trying to battle
       // CheckIfInTrainersView();
        // CheckForEncounters();
    }


    public void HandleUpdate()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");

        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //characterController.Move(direction * moveSpeed * Time.deltaTime);
            characterController.Move(moveSpeed * Time.deltaTime * moveDirection.normalized);
        }
    }
    /*
    //is the player colliding with solid objects?
    private bool IsWalkAble(Vector3 targetPos)
    {
        if (Physics.OverlapSphere(targetPos, sphereRadius, solidObjects) != null)
        {
            return false;
        }
        return true;
    }


    //this coroutine processes where the player is moving 
    IEnumerator Move(Vector3 targetPos)
    {
        //the player is moving 
        //isMoving = true;
        //while the targetposition's square maqnitude is greater than mathf.epsilon
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //move towards the transfrom position 
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        //the transform is the same as the target 
        transform.position = targetPos;

        //isMoving = false;

        CheckForEncounters();
    }
    */
    public void CheckForMove(bool ableToMove)
    {
        canMove = ableToMove;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("EncounterSpace"))
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                
                OnEncountered();

                CheckForMove(false);
                if (debugMode)
                {
                    Debug.Log("Encountered! ");
                }

            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Triggered a battle! ");
        if (other.gameObject.CompareTag("EncounterSpace"))
        {
            // OnEncountered();
           // CheckForEncounters();
           
          
           



        }

        if (other.gameObject.CompareTag("Trainer"))
        {
            Debug.Log("Triggered a trainer battle! ");
            CheckForMove(false);
            OnEnterTrainersView?.Invoke(other.gameObject.GetComponent<Collider>());//[hitcol]);
            //var collider = other.gameObject.Ge
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trainer"))
        {
            Debug.Log("Triggered a trainer battle! ");
            OnEnterTrainersView?.Invoke(collision.collider);//[hitcol]);
        }
    }

    //is the player touching grass? Check for battles 
    private void CheckForEncounters()
    {

        if (Physics.OverlapSphere(transform.position, sphereRadius, encounterSpace)  != null)
        {

            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
               
                if (debugMode)
                {
                    Debug.Log("Triggered a wild monster battle! ");
                    //OnEncountered();
                }

                OnEncountered();
            }

        }

    }

   


    private void CheckIfInTrainersView()
    {
      //  var collider = Physics.OverlapSphere(transform.position, sphereRadius, GameLayers.i.FovLayer);
        if (Physics.OverlapSphere(transform.position, sphereRadius, trainerBattle) != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius, trainerBattle);
            
            foreach (Collider hitCol in hitColliders)
            {
                Debug.Log("Triggered a trainer battle! ");
                OnEnterTrainersView?.Invoke(hitCol);//[hitcol]);
            }
            //character.Animator.IsMoving = false;
           // OnEnterTrainersView?.Invoke(collider[]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    #region Saving
    //save
    public object CaptureState()
    {


        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },


            //converts list of monster party data to save data 
            monsters = GetComponent<MonsterParty>().Monsters.Select(p => p.GetSaveData()).ToList()
         };
        //position into float array
       // float[] position = new float[] { transform.position.x, transform.position.y, transform.position.z };

        return saveData;
    }
    //load
    public void RestoreState(object state)
    {
        //state
        var saveData = (PlayerSaveData)state;

        var pos = saveData.position;
        transform.position = new Vector3(pos[0],pos[1], pos[2]);

        //restore party 

        GetComponent<MonsterParty>().Monsters = saveData.monsters.Select(s => new Monster(s)).ToList();
    }
    #endregion
}


[System.Serializable]
public class PlayerSaveData
{


    public float[] position;

    public List<MonsterSaveData> monsters;



}
