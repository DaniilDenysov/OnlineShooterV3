using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SlidingDoor : NetworkBehaviour
{
    [SerializeField] private KeyCode openKey = KeyCode.E;
    private Animator animator;
    [SyncVar(hook = nameof(OnStateChanged))]
    private bool isOpen;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<CustomNetworkPlayer>(out CustomNetworkPlayer customNetworkPlayer))
        {
            Debug.Log(customNetworkPlayer.isLocalPlayer);
            if (!customNetworkPlayer.isLocalPlayer) return;
            Debug.Log("Key" + Input.GetKey(openKey));
            if (!Input.GetKeyUp(openKey)) return;
            isOpen = !isOpen;
        }
    }

    private void OnStateChanged (bool oldValue, bool newValue)
    {
        if (!newValue) animator.SetTrigger("Open");
        else animator.SetTrigger("Close");
    }
}
