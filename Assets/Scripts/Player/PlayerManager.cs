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
    
    [FormerlySerializedAs("respawnPoint")]
    [Header("Respawn")]
    [SerializeField] Vector3 respawnPosition;
    
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

        if (transform.position.y <= -5)
        {
            ReviveCharacter();
        }
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

        locomotionManager.ResetYVelocity();
    }

    public void ReviveCharacter()
    {
        transform.position = respawnPosition;
        locomotionManager.ResetYVelocity();
    }

    public void SetRespawnPoint(Vector3 respawnPoint)
    {
        respawnPosition = respawnPoint;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Respawn")) // Assuming your respawn point has the tag "Respawn"
        {
            RespawnPointManager respawnPoint = hit.gameObject.GetComponent<RespawnPointManager>();
            Debug.Log("Character hit the respawn zone.");
            SetRespawnPoint(respawnPoint.GetRespawnPosition());
        }
    }
}
