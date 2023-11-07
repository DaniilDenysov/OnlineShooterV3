using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlagueDoctorULT : Ability
{
    [SerializeField] private GameObject _skillet;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layer;
    [SerializeField] private float _spawnRange=7f,_spawnCD=1f;
    [SerializeField] private int _tickRate = 1, _unitCount = 5;
    private GameObject _spawnPoint;
    private int _spawnedUnitsCount;

    [Client]
    public override void FixedUpdate()
    {
        if (!isOwned) return;
        if (!Input.GetKeyUp(_abilityKey)) return;
        if (IsOnCD() && IsActivated()) return;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit raycast, _layer)) return;
        if (!(raycast.collider.gameObject.layer == 10 && raycast.collider.gameObject != gameObject)) return;
        _spawnPoint = raycast.collider.gameObject;
        UpdateActivationTime();
        InvokeAbilityCmd();
    }


    [Command]
    public void Spawn ()
    {
        Skillet tmp = Instantiate(_skillet, new Vector3(_spawnPoint.transform.position.x + Random.Range(-_spawnRange, _spawnRange), _spawnPoint.transform.position.y, _spawnPoint.transform.position.z + Random.Range(-_spawnRange, _spawnRange)), Quaternion.identity).GetComponent<Skillet>();
        tmp.SetTarget(_spawnPoint);
        NetworkServer.Spawn(tmp.gameObject);
    }

    public override IEnumerator ActivateContiniousAbility()
    {
        UpdateWaitingTime();
        _spawnedUnitsCount = _unitCount;
        while (Time.time-_activationTime < _abilityTime)
        {

            if (_spawnedUnitsCount > 0 && _spawnPoint!=null)
            {
                Spawn();
                _spawnedUnitsCount--;
            }
            yield return new WaitForSeconds(_spawnCD);
        }
        yield return null;
    }
}
