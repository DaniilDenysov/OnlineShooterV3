using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunManager : NetworkBehaviour
{
    [SerializeField] private List<Gun> weapons = new List<Gun>();
    [SyncVar(hook = nameof(OnWeaponChanged))]
    [SerializeField] private int currentWeaponBuffer;
    [SerializeField] private Transform RH_point, LH_point,weaponHolder;
    private bool isAiming;

    private float diff = 0.1f;

    [Server]
    public void ChangeWeapon (int id)
    {
        if (id <= weapons.Capacity-1)
        {
            currentWeaponBuffer = id;
        }
    }

   /* [ClientCallback]
    private void FixedUpdate()
    {
          if (Input.GetKey(KeyCode.C))
          {
              weaponHolder.position = new Vector3(weaponHolder.position.x, 1, weaponHolder.position.z);
              return;
          }
        if (!isOwned) return;
        if (Input.GetButton("Fire2"))
        {
            //isAiming = !isAiming;
            OnAim(new Vector3(weapons[currentWeaponBuffer].GetGun().transform.position.x, 1.3695f, weapons[currentWeaponBuffer].GetGun().transform.position.z));
            return;
        }
         else if (Input.GetButtonUp("Fire2") && isAiming)
         {
             isAiming = !isAiming;
             Vector3 downPosition = weapons[currentWeaponBuffer].GetGun().transform.position;
             OnAim(new Vector3(downPosition.x, 1.1f, downPosition.z), 1);
             return;
         }
        OnAim(new Vector3(weapons[currentWeaponBuffer].GetGun().transform.position.x, 1.1f, weapons[currentWeaponBuffer].GetGun().transform.position.z));
    } */

   /* public void OnAim (Vector3 newPos)
    {
        weapons[currentWeaponBuffer].GetGun().transform.position = newPos;
        RH_point.position = new Vector3(RH_point.position.x, newPos.y,RH_point.position.z);
        LH_point.position = new Vector3(LH_point.position.x, newPos.y, LH_point.position.z);
    }*/

    [System.Serializable]
    public struct Gun
    {
        [SerializeField] private GameObject gun;
        public GameObject GetGun() => gun;
        [SerializeField] private Transform RH_point, LH_point;
        public Transform GetRHPoint() => RH_point;
        public Transform GetLHPoint() => LH_point;
    }

    public void OnWeaponChanged(int oldWeapon,int newWeapon)
    {
        RH_point.transform.position = weapons[newWeapon].GetRHPoint().position;
        LH_point.transform.position = weapons[newWeapon].GetLHPoint().position;
        RH_point.transform.rotation = weapons[newWeapon].GetRHPoint().rotation;
        LH_point.transform.rotation = weapons[newWeapon].GetLHPoint().rotation;
        weapons[oldWeapon].GetGun().SetActive(false);
        weapons[newWeapon].GetGun().SetActive(true);
    }
}
