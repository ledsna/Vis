using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    PlayerControls playerControls;
    [HideInInspector] public PlayerManager player;

    [Header("MOVEMENT INPUT")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;

    [Header("CAMERA MOVEMENT INPUT")]
    public Vector2 cameraMovementInput;

    [FormerlySerializedAs("jumpIsPressed")] [Header("ACTION INPUT")] 
    public bool jumpInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (playerControls is null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.CameraMovement.Movement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
        }

        playerControls.Enable();
    }

    private void Update()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        float moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        HandleJumpInput();
        
        player.animatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            
            // IF WE HAVE MENU OPEN, RETURN WITHOUT DOING ANYTHING
            
            // ATTEMPT TO PERFORM JUMP
            player.locomotionManager.AttemptToPerformJump();
        }
    }
}