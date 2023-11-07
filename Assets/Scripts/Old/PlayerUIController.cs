using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerUIController : MonoBehaviour
{
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
      transform.rotation = FindObjectOfType<CinemachineVirtualCamera>().transform.rotation * originalRotation;
    }
}
