using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [Tooltip("Turn this on to debug in the inspector")]
    public bool debugMode;



    [Space(2)]

    [Header("Values")]

    [Tooltip("The player's movement Speed")]

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

    private LayerMask solidObjects, encounterSpace;

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

        float horizontal = Input.GetAxisRaw("Horizontal");

        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg * camTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveSpeed * Time.deltaTime * moveDirection.normalized);
        }

    }
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

    //is the player touching grass? Check for battles 
    private void CheckForEncounters()
    {

        if (Physics.OverlapSphere(transform.position, sphereRadius, encounterSpace) != null)
        {

            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                if (debugMode)
                {
                    Debug.Log("Triggered a battle! ");
                }
            }

        }

    }
}
