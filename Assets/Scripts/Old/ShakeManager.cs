using Cinemachine;
using UnityEngine;

public class ShakeManager : MonoBehaviour
{
    private static CinemachineImpulseSource cinemachineImpulseSource;

    private void Start()
    {
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        Shoot.OnPlayerShoot += Shoot_OnPlayerShoot;
    }

    private void Shoot_OnPlayerShoot(string arg1, float arg2)
    {
        Shake(arg2);
    }

    public static void Shake (float intensity)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
