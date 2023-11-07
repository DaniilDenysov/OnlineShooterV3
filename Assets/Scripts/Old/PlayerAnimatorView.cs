using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;

public class PlayerAnimatorView : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private TwoBoneIKConstraint RH, LH;
    [SerializeField] private List<AnimationNode> nodes = new List<AnimationNode>();
    [SyncVar]
    private Vector3 velocity;

    [System.Serializable]
    struct AnimationNode
    {
        [SerializeField] private string name,triggerName;
        public string GetName() => name;
        public string GetTriggrtName() => name;
        [Range(0,10)]
        [SerializeField] private float lowerLimit, upperLimit;
        public float GetLowerLimit() => lowerLimit;
        public float GetUpperLimit() => upperLimit;
    }

    void Update()
    {
        if (isServer)
        {
            velocity = rb.velocity;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            RH.weight = 0;
            animator.SetTrigger("Grenade");
            animator.CrossFade("Weapon", 0.1f);
            return;
        }
        else if (Input.GetKeyUp(KeyCode.G))
        {
            RH.weight = 1;
        }
        if (InBetween(0, 0.1f, velocity.magnitude))
        {
            animator.SetTrigger("Idle");
            PlayCrossfade(velocity.x, velocity.z, "WalkMovement");
            return;
        }
        if (InBetween(0.1f,4,velocity.magnitude))
        {
            animator.SetTrigger("Walk");
            PlayCrossfade(velocity.normalized.x, velocity.normalized.z, "WalkMovement");
            return;
        }
        if (InBetween(4, 5, velocity.magnitude))
        {
            animator.SetTrigger("Run");
            PlayCrossfade(velocity.x, velocity.z, "RunMovement");
            return;
        }
        if (velocity.magnitude > 5)
        {
            animator.SetTrigger("Jump");
            PlayCrossfade(velocity.x,velocity.z, "JumpMovement");
            return;
        }
    }

    public bool InBetween (float a, float b,float value)
    {
        return a <= value && b >= value;
    }

    public void PlayCrossfade(float velocityX, float velocityZ, string name)
    {
        animator.CrossFade(name, 0.1f);
        animator.SetFloat("Y", velocityZ, 0.1f, Time.deltaTime);
        animator.SetFloat("X", velocityX, 0.1f, Time.deltaTime);
    }

}
