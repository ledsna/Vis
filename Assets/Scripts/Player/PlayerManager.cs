using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerLocomotionManager locomotionManager;
    [HideInInspector] public PlayerAnimatorManager animatorManager;

    [Header("Flags")] 
    public bool applyRootMotion = false;
    public bool isInAir = false;
    public bool isGrounded = true;
    public bool isJumping = false;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        locomotionManager = GetComponent<PlayerLocomotionManager>();
        animatorManager = GetComponent<PlayerAnimatorManager>();
    }

    private void Start()
    {
        PlayerInputManager.instance.player = this;
    }

    private void Update()
    {
        animator.SetBool("isGrounded", isGrounded);
    }
}
