using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.XR;

public class PlayerLocomotionManager : MonoBehaviour
{
    private PlayerManager player;

    [Header("MOVEMENT SETTINGS")]
    private Vector3 movementDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float airSpeed = 0.5f;
    [SerializeField] float rotationSpeed = 15;

    [Header("Ground check & Jumping")] 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckSphereRadius = 1;
    [SerializeField] private Vector3 yVelocity;
    [SerializeField] private float gravityForce = -5.55f;
    [SerializeField] private float groundedYVeloctiy = -20;
    [SerializeField] private float fallStartVelocity = -5;
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
            if (yVelocity.y < 0)
            {
                inAirTime = 0;
                fallVelocityHasBeenSet = false;
                yVelocity.y = groundedYVeloctiy;
            }
        }
        else
        {
            if (!player.isJumping && !fallVelocityHasBeenSet)
            {
                fallVelocityHasBeenSet = true;
                yVelocity.y = fallStartVelocity;
            }

            inAirTime += Time.deltaTime;

            yVelocity.y += gravityForce * Time.deltaTime;

            player.characterController.Move(yVelocity * Time.deltaTime);
        }
        
        
        
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        movementDirection = CameraManager.instance.transform.forward * PlayerInputManager.instance.verticalInput +
                            CameraManager.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        movementDirection.Normalize();
        movementDirection.y = 0;

        if (player.isInAir)
        {
            player.characterController.Move(movementDirection * airSpeed * Time.deltaTime);
        }
        else
        {
            player.characterController.Move(movementDirection * walkingSpeed * Time.deltaTime);
        }
    }
    
    private void HandleRotation() 
    {
        Vector3 targetRotationDirection = 
            CameraManager.instance.transform.forward * PlayerInputManager.instance.verticalInput + 
            CameraManager.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero) {
            targetRotationDirection = transform.forward;
        }
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void HandleJump()
    {
        player.animatorManager.PlayTargetActionAnimation("jump", true);
    }

    private void HandleGroundCheck()
    {
        // if we collide with ground layer, we are grounded
        player.isGrounded = Physics.CheckSphere(player.transform.position, groundCheckSphereRadius, groundLayer);
    }

    // Draws a sphere that represents our ground check hitbox
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(player.transform.position, groundCheckSphereRadius);
    }
}
