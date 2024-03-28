using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLocomotionManager : MonoBehaviour
{
    private PlayerManager player;

    [Header("MOVEMENT SETTINGS")]
    private Vector3 movementDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 3;
    [SerializeField] float rotationSpeed = 15;
    
    [Header("JUMP SETTINGS")]
    private Vector3 jumpDirection;
    [SerializeField] private float jumpHeight = 3;
    [SerializeField] private float jumpForwardVelocity = 3;
    [SerializeField] private float freeFallVelocity = 2;

    [Header("Ground check & Jumping")] 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckSphereRadius = 2;
    [SerializeField] Vector3 yVelocity; // FORCE THAT PULLS OUR CHARACTER UP OR DOWN
    [SerializeField] private float gravityForce = -5.55f;
    [SerializeField] private float groundedYVelocity = -20; // force at which character is sticking to the ground
    // while he is grounded
    [SerializeField] private float fallStartVelocity = -5; // force at which character starts to fall when he become 
    // ungrounded 
    private bool fallVelocityHasBeenSet = false;
    protected float inAirTime = 0;
    
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleGroundCheck();
        
        if (player.isGrounded)
        {
            // if we are not attempting to jump
            if (yVelocity.y < 0)
            {
                inAirTime = 0;
                fallVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            // if we are not jumping and fall velocity has not been set yet
            if (!player.isJumping && !fallVelocityHasBeenSet)
            {
                yVelocity.y = fallStartVelocity;
                fallVelocityHasBeenSet = true;
            }

            inAirTime += Time.deltaTime;
            player.animator.SetFloat("inAirTimer", inAirTime);
            
            yVelocity.y += gravityForce * Time.deltaTime;

        }
        
        // we always apply gravity force
        player.characterController.Move(yVelocity * Time.deltaTime);
        
        HandleMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void HandleMovement()
    {
        if (!player.isGrounded || player.isJumping || player.isLanding)
        {
            return;
        }
        
        movementDirection = CameraManager.instance.transform.forward * PlayerInputManager.instance.verticalInput;
        movementDirection += CameraManager.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        movementDirection.y = 0;
        movementDirection.Normalize();

        if (movementDirection != Vector3.zero)
        {
            player.playerSoundFXManager.PlayStepSoundFX();
        }
         
        player.characterController.Move( runningSpeed * Time.deltaTime * movementDirection);
    }
    
    private void HandleRotation() 
    {
        Vector3 targetRotationDirection = 
            CameraManager.instance.transform.forward * PlayerInputManager.instance.verticalInput + 
            CameraManager.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        targetRotationDirection.y = 0;
        targetRotationDirection.Normalize();

        if (targetRotationDirection == Vector3.zero) {
            targetRotationDirection = transform.forward;
        }
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.characterController.Move( jumpForwardVelocity * Time.deltaTime * jumpDirection);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (player.isGrounded)
        {
            return;
        }

        Vector3 freeFallDirection;
        
        freeFallDirection = CameraManager.instance.transform.forward * PlayerInputManager.instance.verticalInput;
        freeFallDirection += CameraManager.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        freeFallDirection.y = 0;
        freeFallDirection.Normalize();

        player.characterController.Move(freeFallVelocity * Time.deltaTime * freeFallDirection);
    }

    public void AttemptToPerformJump()
    {
        // if we are already in a jump, we do not allow jump until our current jump is finished
        if (player.isJumping)
        {
            return;
        }

        // if we are not grounded, we do not allow jump
        if (!player.isGrounded)
        {
            return;
        }
        
        // play jumping animation
        player.animatorManager.PlayTargetActionAnimation("jump_start", false);

        player.isJumping = true;

        jumpDirection = CameraManager.instance.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += CameraManager.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;
        jumpDirection.Normalize();
    }

    public void ApplyJumpingVelocity()
    {
        // APPLY AN UPWARD VELOCITY
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    private void HandleGroundCheck()
    {
        // if we collide with ground layer, we are grounded
        player.isGrounded = Physics.CheckSphere(player.transform.position, groundCheckSphereRadius, groundLayer);
    }

    public void ResetYVelocity()
    {
        yVelocity.y = 0;

        StartCoroutine(SetYVelocity());
    }
    
    private IEnumerator SetYVelocity()
    {
        yield return new WaitForSeconds(0.2f);

        yVelocity.y = groundedYVelocity;
    }

    // Draws a sphere that represents our ground check hitbox
    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawSphere(player.transform.position, groundCheckSphereRadius);
    // }
}
