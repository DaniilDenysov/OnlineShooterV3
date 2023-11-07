using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Skillet : NetworkBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private NavMeshAgent _agent;
    [Range(0,100)]
    [SerializeField] private float _atackRate = 1f,_damage = 10f,_distance=1f;
    [Range(0, 1000)]
    [SerializeField] private float _lifetime=5f;

    [SerializeField] private LayerMask _layer;
    [SerializeField] private Transform _atackPoint;
    [SerializeField] private Animator _animator;
    [SyncVar]
    private float _lastAtack;

    private Vector3 _prevPos;

    [Server]
    void Start()
    {
        _lastAtack = Time.time;
        Invoke(nameof(DestroyUnit),_lifetime);
        _prevPos = transform.position;
    }

    [Server]
    void Update()
    {
        Vector3 _dir = (transform.position - _prevPos);
        _animator.SetFloat("X",Mathf.Clamp(_dir.x,-1f,1f));
        _animator.SetFloat("Y",Mathf.Clamp(_dir.z,-1f,1f));
        if (_target == null) return;
        _agent.SetDestination(_target.transform.position);
        if (!(Time.time - _lastAtack >= _atackRate)) return;
        Atack();
        _lastAtack = Time.time;
    }



    [Server]
    public void DestroyUnit ()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void Atack()
    {
        Ray ray = new Ray(_atackPoint.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit,_distance,_layer))
        {
            if (hit.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                damagable.OnDamage(_damage);
            }
        }
    }
    [Server]
    public void SetTarget (GameObject _target)
    {
        this._target = _target;
    }
}
