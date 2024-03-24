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
        CameraManager.instance.player = transform;
        WorldSaveGameManager.instance.player = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        animator.SetBool("isGrounded", isGrounded);
    }
    
    public void SaveGameDataToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y + 0.3f;
        currentCharacterData.zPosition = transform.position.z;
        
        // instantly set yVelocity to groundedYVelocity so that we dont float
        locomotionManager.SetYVelocity();
    }

    public void LoadGameDataFromCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        Debug.Log(currentCharacterData.xPosition);
        Debug.Log(currentCharacterData.yPosition);
        Debug.Log(currentCharacterData.zPosition);
        transform.position = new Vector3(
            currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);

        // moving camera to player
        // CameraManager.instance.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
