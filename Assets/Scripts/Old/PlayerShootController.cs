using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;
using System.Threading;

public class PlayerShootController : NetworkBehaviour
{
    [SerializeField] private PlayerLodoutManager lodoutManager;
    [SerializeField] private Transform bulletPoint,shootPoint;
    [SerializeField] private GameObject projectile,bulletHall,sparks;
    [SerializeField] private Animator animator;
    [SerializeField] private CustomNetworkPlayer player;
    public static event Action<string,string,string> OnPlayerKilled;
    public static event Action<string, float> OnDamageDealt;
    private double fireTime = 0f;
    [SyncVar]
    private bool isReloading;
    private GameObject muzzleFlashBuffer;
    public bool IsReloading() => isReloading;
    [SyncVar]
    private float combat = 0f;


    #region Client
    [ClientRpc]
    private void ClientLog(string s)
    {
        Debug.Log(s);
    }

    [ClientCallback]
    void Update()
    { 
        if (!isOwned) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            lodoutManager.SetMag(lodoutManager.GetCurrentWeaponData().MagCapacity);
            return;
        }
        if (lodoutManager.GetCurrentWeaponData().WeaponType != Weapon.weaponTypes.Auto && !Input.GetButtonDown("Fire1")) return;
        if (!Input.GetButton("Fire1")) return;
        if (NetworkTime.time <= fireTime) return;
        if (lodoutManager.GetMag() <= 0) return;
        lodoutManager.SetMag(lodoutManager.GetMag() - 1);
        animator.SetTrigger(lodoutManager.GetCurrentWeaponData().WeaponAnimation);
        fireTime = NetworkTime.time + lodoutManager.GetCurrentWeaponData().FireRate;
        Hit(shootPoint.transform.position, shootPoint.transform.forward,player.GetName());   
        ShakeManager.Shake(lodoutManager.GetCurrentWeaponData().CameraShakeForce);
    }
    #endregion

    #region Server

    [Command]
    private void Reload ()
    {
        isReloading = false;
        lodoutManager.SetMag(lodoutManager.GetCurrentWeaponData().MagCapacity);
    }


    [Command]
    private void Hit (Vector3 origin,Vector3 direction,string p)
    {
        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            OnHit(hit,p);
        }
    }
    
    [Server]
    private bool IsAllowedToShoot ()
    {

        return true;
    }
    [Server]
    private void InstantiateProjectile (Vector3 hit)
    {
        GameObject proj = Instantiate(projectile,bulletPoint.position,Quaternion.identity);
        proj.GetComponent<Projectile>().SetPoint(hit);
        proj.GetComponent<Projectile>().SetSpeed(lodoutManager.GetCurrentWeaponData().BulletSpeed);
        NetworkServer.Spawn(proj, connectionToClient);
    }
    [Server]
    private void InstantiateBulletHall(Vector3 hit)
    {
        GameObject bulletHall = Instantiate(this.bulletHall, hit, Quaternion.identity);
        bulletHall.transform.rotation = Quaternion.LookRotation(transform.forward,Vector3.up);
        NetworkServer.Spawn(bulletHall, connectionToClient);    
    }
    [Server]
    private void InstantiateSparks(Vector3 hit)
    {
        GameObject sparks = Instantiate(this.sparks, hit, Quaternion.LookRotation(-transform.forward, Vector3.up));
        NetworkServer.Spawn(sparks, connectionToClient);
    }
    [Server]
    private void InstantiateMuzzleFlash()
    {
        if (muzzleFlashBuffer != null) return;
        muzzleFlashBuffer = Instantiate(lodoutManager.GetCurrentWeaponData().MuzzleFlash, bulletPoint.position, Quaternion.identity);
        muzzleFlashBuffer.GetComponent<PositionHolder>().SetFixedPosition(bulletPoint);
        NetworkServer.Spawn(muzzleFlashBuffer, connectionToClient);
    }
    [Server]
    public bool IsHitted(RaycastHit hit, int layer) => hit.collider.gameObject.layer == layer;

    [Server]
    private void OnHit(RaycastHit hit,string p)
    {
        Debug.Log($"{p} shooted");
        if (hit.collider.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            float dmg = lodoutManager.GetCurrentWeaponData().Damage; // * lodoutManager.GetCurrentWeaponData().DamageByDistance.Evaluate(Vector3.Distance(transform.position, hit.point));
                                                                     // hit.collider.GetComponent<IDamagable>().OnDamage(dmg);
            if (hit.collider.gameObject.layer == 10) OnDamageDealt?.Invoke(p, lodoutManager.GetCurrentWeaponData().Damage);
            if (damagable.OnDamage(dmg))
            {
                if (hit.collider.gameObject.layer == 10) OnPlayerKilled?.Invoke(p, hit.collider.gameObject.GetComponent<CustomNetworkPlayer>().GetName(), lodoutManager.GetCurrentWeaponData().name);
                Debug.Log($"{p} killed {hit.collider.gameObject.GetComponent<CustomNetworkPlayer>().GetName()} with {lodoutManager.GetCurrentWeaponData().name}");
            }


          //  Debug.Log($"Damage:{dmg}");
        }
        if (IsHitted(hit, 11))
        {
            Vector3 direction = hit.transform.position - transform.position;
            hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * lodoutManager.GetCurrentWeaponData().BulletForce, hit.point, ForceMode.Impulse);
        }
        if (lodoutManager.GetCurrentWeaponData().WeaponType == Weapon.weaponTypes.Melee)
        {
            combat = 3f;
            return;
        }
        InstantiateProjectile(hit.point);
        InstantiateBulletHall(hit.point);
        InstantiateMuzzleFlash();
        InstantiateSparks(hit.point);
    }
    #endregion
}
