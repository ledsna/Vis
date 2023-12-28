using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    CharacterController characterController;
    PlayerControls playerControls;

    [Header("MOVEMENT INPUT")]
    [SerializeField] Vector2 movementInput;

    [Header("MOVEMENT SETTINGS")]
    private Vector3 movementDirection;
    [SerializeField] float walkingSpeed = 2;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (playerControls is null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void Update()
    {
        movementDirection = transform.forward * movementInput.y +
            transform.right * movementInput.x;
        movementDirection.Normalize();
        movementDirection.y = 0;

        characterController.Move(movementDirection * walkingSpeed * Time.deltaTime);
    }
}