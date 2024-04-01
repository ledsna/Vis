using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    PlayerManager player;

    protected virtual void Awake() {
        player = GetComponent<PlayerManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue) 
    {
        player.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        player.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool applyRootMotion = true)
    {
        player.applyRootMotion = applyRootMotion;
        player.animator.CrossFade(targetAnimation, 0.2f);
    }
    
    private void OnAnimatorMove()
    {
        if (player.applyRootMotion) {
            Vector3 velocity = player.animator.deltaPosition;
            player.characterController.Move(velocity);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }
}