using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Mirror;

[RequireComponent(typeof(Lodout))]
public class Shoot : NetworkBehaviour
{

    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Lodout _lodout;
    [SerializeField] private LayerMask layerMask;
    private GameObject _muzzleFlashBuffer;
    [SerializeField] private GameObject _projectile, _bulletHole, _sparks;
    public static event Action<string,float> OnPlayerShoot;
  //  public static event Action<string,string,string> OnPlayerKill; //killer wpn victim
   // public static event Action<string, float> OnDamageDealt;

    private float _lastTimeShooted;

    [Server]
    private void Start()
    {
        _lastTimeShooted -= _lodout.GetCurrentWeapon().weaponData.FireRate;
        Projectile.OnHit += Projectile_OnHit;    
    }

    private void Projectile_OnHit(GameObject arg1, GameObject arg2, string arg3)
    {
        OnHit(arg1,arg2,arg3);
    }

    public enum ClickState
    {
        None,
        Up,
        Down,
        Free
    }

    [ClientCallback]
    void Update()
    {
        if (!isOwned) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdReload();
            return;
        }
        if (!Input.GetButton("Fire1")) return;
        ClickState _click = ClickState.None;
        if (Input.GetButton("Fire1")) _click = ClickState.Free;
        if (Input.GetButtonUp("Fire1")) _click = ClickState.Up;
        if (Input.GetButtonDown("Fire1")) _click = ClickState.Down;
        Debug.Log($"Is correct click");
        Debug.Log("Shoot!");
        CmdShoot(_click);
    }

    [ServerCallback]
    public bool IsCorrectClick (ClickState clickState)
    {
        if (_lodout.GetCurrentWeapon().weaponData.WeaponType == Weapon.weaponTypes.Auto && clickState==ClickState.Free) return true;
        if (_lodout.GetCurrentWeapon().weaponData.WeaponType != Weapon.weaponTypes.Auto && (clickState == ClickState.Down || clickState == ClickState.Up)) return true;
        return false;
    }

    [Command]
    public void CmdReload ()
    {
       // _lodout.SetMag(lodoutManager.GetCurrentWeaponData().MagCapacity);
    }

    [TargetRpc]
    public void Shooted ()
    {
      //  OnPlayerShoot?.Invoke("", _lodout.GetCurrentWeapon().weaponData.CameraShakeForce);
    }

    [Server]
    private void Hit(Vector3 origin, Vector3 direction, string p)
    {
        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
           
            StartCoroutine(ChainedBullet(hit.point,hit.distance,origin,direction));
        }
    }

    [ServerCallback]
    public bool IsHitted(RaycastHit hit, int layer) => hit.collider.gameObject.layer == layer;

    [ServerCallback]
    public bool IsHitted(GameObject hit, int layer) => hit.layer == layer;

    [Server]
    private void OnHit(GameObject proj, GameObject hit, string p)
    {
        Debug.Log($"{p} shooted");
        if (hit.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            float dmg = _lodout.GetCurrentWeapon().weaponData.Damage; // * lodoutManager.GetCurrentWeaponData().DamageByDistance.Evaluate(Vector3.Distance(transform.position, hit.point));
                                                                      // hit.collider.GetComponent<IDamagable>().OnDamage(dmg);
           // if (hit.gameObject.layer == 10) OnDamageDealt?.Invoke(p, _lodout.GetCurrentWeapon().weaponData.Damage);
            if (damagable.OnDamage(dmg))
            {
                if (hit.gameObject.layer == 10) PlayerAction.OnPlayerKill(p, hit.GetComponent<CustomNetworkPlayer>().GetName(), _lodout.GetCurrentWeapon().weaponData.name);
                //    Debug.Log($"{p} killed {hit.collider.gameObject.GetComponent<CustomNetworkPlayer>().GetName()} with {_lodout.GetCurrentWeapon().weaponData.name}");
            }


            //  Debug.Log($"Damage:{dmg}");
        }
        if (IsHitted(hit, 11))
        {
            Vector3 direction = hit.transform.position - transform.position;
            hit.GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * _lodout.GetCurrentWeapon().weaponData.BulletForce, proj.transform.position, ForceMode.Impulse);
        }
        if (_lodout.GetCurrentWeapon().weaponData.WeaponType == Weapon.weaponTypes.Melee)
        {
            //combat = 3f;
            return;
        }
      //  InstantiateProjectile(hit.transform.position);
        //   InstantiateBulletHall(hit.point);
        //  InstantiateSparks(hit.point);
    }


    [Server]
    IEnumerator ChainedBullet (Vector3 _hitPoint,float distance,Vector3 origin, Vector3 direction)
    {
        Projectile _proj = InstantiateProjectile(_hitPoint, "").GetComponent<Projectile>();
        float _rayLen = 2;
        float _rayNums = distance / _rayLen;
        float _rayTime = distance / _lodout.GetCurrentWeapon().weaponData.BulletSpeed;
        float _rayW8Time = _rayTime / _rayNums;
        float _dis = distance;
        float _fdis = _dis;


        Vector3 _newDest = origin + (direction * _rayLen);

        for (int i = 0;i<_rayNums;i++)
        {

            _newDest += (direction * _rayLen);
            _proj.SetPoint(_newDest);
            Ray ray = new Ray(_newDest, direction);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                OnHit(hit, GetComponent<CustomNetworkPlayer>().GetName());
                _proj.SetPoint(hit.point);
                break;
            }
            yield return new WaitForSeconds(_rayW8Time);
        }
    }


    [Server]
    private void OnHit(RaycastHit hit, string p)
    {
        Debug.Log($"{p} shooted");
        if (hit.collider.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            float dmg = _lodout.GetCurrentWeapon().weaponData.Damage; // * lodoutManager.GetCurrentWeaponData().DamageByDistance.Evaluate(Vector3.Distance(transform.position, hit.point));
                                                                     // hit.collider.GetComponent<IDamagable>().OnDamage(dmg);
           // if (hit.collider.gameObject.layer == 10) OnDamageDealt?.Invoke(p, _lodout.GetCurrentWeapon().weaponData.Damage);
            if (damagable.OnDamage(dmg))
            {
                if (hit.collider.gameObject.layer == 10) PlayerAction.OnPlayerKill(p, hit.collider.gameObject.GetComponent<CustomNetworkPlayer>().GetName(), _lodout.GetCurrentWeapon().weaponData.name);
            //    Debug.Log($"{p} killed {hit.collider.gameObject.GetComponent<CustomNetworkPlayer>().GetName()} with {_lodout.GetCurrentWeapon().weaponData.name}");
            }


            //  Debug.Log($"Damage:{dmg}");
        }
        if (IsHitted(hit, 11))
        {
            Vector3 direction = hit.transform.position - transform.position;
            hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * _lodout.GetCurrentWeapon().weaponData.BulletForce, hit.point, ForceMode.Impulse);
        }
        if (_lodout.GetCurrentWeapon().weaponData.WeaponType == Weapon.weaponTypes.Melee)
        {
            //combat = 3f;
            return;
        }
        //InstantiateProjectile(hit.point);
        InstantiateBulletHall(hit.point);
        InstantiateMuzzleFlash(_shootPoint.position);
        InstantiateSparks(hit.point);
    }

    [ClientRpc]
    public void PlaySleeve ()
    {
        _lodout.GetCurrentWeapon().PlaySleeves();
    }

    [Command]
    public void CmdShoot (ClickState _click,NetworkConnectionToClient conn=null)
    {
        if (!IsCorrectClick(_click)) return;
        if (!IsAllowedToShoot()) return;
        _lastTimeShooted = Time.time;
        //PlaySleeve();
        Hit(_shootPoint.transform.position, _shootPoint.transform.forward,conn.identity.gameObject.name);
        Shooted();
        InstantiateMuzzleFlash(_shootPoint.position);
    }

    [ServerCallback]
    private GameObject InstantiateProjectile(Vector3 _hit,string _owner)
    {
        GameObject proj = Instantiate(_projectile, _shootPoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetPoint(_hit);
       // proj.GetComponent<Projectile>().SetDirections(Quaternion.LookRotation(transform.forward, Vector3.up), Quaternion.LookRotation(-transform.forward, Vector3.up));
        proj.GetComponent<Projectile>().SetSpeed(_lodout.GetCurrentWeapon().weaponData.BulletSpeed);
       
        NetworkServer.Spawn(proj, connectionToClient);
        return proj;
    }
    [Server]
    private void InstantiateBulletHall(Vector3 _hit)
    {
        GameObject bulletHall = Instantiate(this._bulletHole, _hit, Quaternion.identity);
        bulletHall.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        NetworkServer.Spawn(bulletHall, connectionToClient);
    }
    [Server]
    private void InstantiateSparks(Vector3 _hit)
    {
        GameObject sparks = Instantiate(this._sparks, _hit, Quaternion.LookRotation(-transform.forward, Vector3.up));
        NetworkServer.Spawn(sparks, connectionToClient);
    }
    [Server]
    private void InstantiateMuzzleFlash(Vector3 _hit)
    {
        if (_muzzleFlashBuffer != null) return;
        _muzzleFlashBuffer = Instantiate(_lodout.GetCurrentWeapon().weaponData.MuzzleFlash, _hit, Quaternion.identity);
        _muzzleFlashBuffer.GetComponent<PositionHolder>().SetFixedPosition(_shootPoint);
        NetworkServer.Spawn(_muzzleFlashBuffer, connectionToClient);
    }

    [ServerCallback]
    public bool IsAllowedToShoot ()
    {
        Debug.Log("Smth");
        return _lastTimeShooted + _lodout.GetCurrentWeapon().weaponData.FireRate <= Time.time;
    }

}
