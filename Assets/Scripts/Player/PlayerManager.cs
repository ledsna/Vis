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
    [HideInInspector] public PlayerSoundFXManager playerSoundFXManager;

    [Header("Flags")] 
    public bool applyRootMotion = false;
    public bool isGrounded = true;
    public bool isJumping = false;
    public bool isLanding = false;
    public bool doNotRevive = false;
    
    [Header("Respawn")]
    [SerializeField] Vector3 respawnPosition;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        locomotionManager = GetComponent<PlayerLocomotionManager>();
        animatorManager = GetComponent<PlayerAnimatorManager>();
        playerSoundFXManager = GetComponent<PlayerSoundFXManager>();
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
        if (!isGrounded)
        {
            transform.SetParent(null);
        }

        RespawnIfNeeded();
    }
    
    public void SaveGameDataToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        currentCharacterData.xPosition = respawnPosition.x;
        currentCharacterData.yPosition = respawnPosition.y;
        currentCharacterData.zPosition = respawnPosition.z;
    }
    
    public void LoadGameDataFromCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        Debug.Log(currentCharacterData.xPosition);
        Debug.Log(currentCharacterData.yPosition);
        Debug.Log(currentCharacterData.zPosition);
        transform.position = new Vector3(
            currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        CameraManager.instance.transform.position = transform.position;
        locomotionManager.ResetYVelocity();
    }
    
    private void RespawnIfNeeded()
    {
        if (doNotRevive || transform.position.y >= -5)
        {
            return;
        }
        
        playerSoundFXManager.PlayDeathSoundFX();
        transform.position = respawnPosition;
        locomotionManager.ResetYVelocity();
    }
    
    private void SetRespawnPoint(Vector3 respawnPoint)
    {
        respawnPosition = respawnPoint;
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Respawn"))
        {
            RespawnPointManager respawnPoint = hit.gameObject.GetComponent<RespawnPointManager>();
            SetRespawnPoint(respawnPoint.GetRespawnPosition());
        }
    }
}
