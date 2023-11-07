using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Projectile : NetworkBehaviour
{
    [Range(0,100)]
    [SerializeField] private float speed = 5f,lifetime = 1f;
    [SerializeField] private Vector3 hitPoint;
    [SerializeField] private GameObject _sparks, _bulletHole;
    public static event Action<GameObject,GameObject,string> OnHit;
    private Quaternion _bulletHoleDir, _sparksDir;
    private bool _spawned;
    private string _owner;
    private Vector3 _startPos;
    private float _currSpeed, _dis,_fdis;

    [Server]
    private void Start()
    {
        _startPos = transform.position;
        Invoke(nameof(DestroyProjectile),lifetime);
    }

    public void SetPoint (Vector3 hitPoint)
    {
        this.hitPoint = hitPoint;
        _dis = Vector3.Distance(transform.position, hitPoint);
        _fdis = _dis;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void DestroyProjectile ()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    void Update()
    {
        _dis -= speed * Time.deltaTime;
       transform.position = Vector3.Lerp(_startPos,hitPoint,1-(_dis/_fdis));
    }
}
