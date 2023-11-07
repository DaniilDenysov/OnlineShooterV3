using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SynchronizedAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private List<Treshold> _tresholds = new List<Treshold>();
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _velocity;

    [System.Serializable]
    public struct Treshold
    {
        [Range(0, 100)]
        public float min, max;
        public string _triggerName,_crossFadeNmae;
    }


    [ServerCallback]
    void FixedUpdate()
    {
        _velocity = _rigidbody.velocity;
        foreach (Treshold _treshold in _tresholds)
        {
            if (InBetween(_treshold.min,_treshold.max, _velocity.magnitude))
            {
                SetTrigger(_treshold._triggerName);
                PlayCrossfade(_velocity.x, _velocity.z, _treshold._crossFadeNmae);
                return;
            }
        }
    }

    [ServerCallback]
    public bool InBetween(float a, float b, float value)
    {
        return a <= value && b >= value;
    }

    [ClientRpc]
    public void SetTrigger (string _trigger)
    {
        _animator.SetTrigger(_trigger);
    }

    [ClientRpc]
    public void PlayCrossfade(float velocityX, float velocityZ, string name)
    {
        _animator.CrossFade(name, 0.05f);
        _animator.SetFloat("Y", velocityZ, 0.05f, Time.deltaTime);
        _animator.SetFloat("X", velocityX, 0.05f, Time.deltaTime);
    }
}
