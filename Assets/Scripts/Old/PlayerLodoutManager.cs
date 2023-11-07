using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLodoutManager : NetworkBehaviour
{
    [SerializeField] private List<LodoutItem> lodoutSlots = new List<LodoutItem>();
    [SerializeField] private GunManager gunManager;
    [SerializeField] private PlayerShootController playerShootController;
    [SyncVar(hook = nameof(OnWeaponChanged))]
    private LodoutItem slotBuffer;

    public void Start ()
    {
        slotBuffer = lodoutSlots[0];
    }

    [System.Serializable]
    public struct LodoutItem
    {
        [SerializeField] private Weapon weaponData;
        public Weapon GetWeaponData
        {
            get { return weaponData; }
        }
        [SyncVar]
        [SerializeField] private int mag,bullets;
        public int GetMag
        {
            get { return mag; }
            set {  mag = value; }
        }
        public int GetBullets
        {
            get { return bullets; }
            set { bullets = value; }
        }
        [SerializeField] private KeyCode key;
        public KeyCode GetKeyCode
        {
            get { return key; }
        }
    }
    [Command]
    public void SetMag (int bulletCount)
    {
        slotBuffer.GetMag = bulletCount;
    }

    public int GetMag() => slotBuffer.GetMag;

    [ClientCallback]
    public void Update()
    {
        if (!isOwned) return;
        if (playerShootController.IsReloading()) return;
        if (!Input.anyKey) return;
        foreach (LodoutItem item in lodoutSlots)
        {
            if (item.GetKeyCode == slotBuffer.GetKeyCode) continue;
            if (IsPressed(item.GetKeyCode))
            {
                slotBuffer = item;
                return;
            }
        }
    }

    public void OnWeaponChanged (LodoutItem oldWeapon, LodoutItem newWeapon)
    {
        gunManager.ChangeWeapon(newWeapon.GetWeaponData.WeaponID);
        Debug.Log("Wpn changed");
    }

    public bool IsPressed(KeyCode key) => Input.GetKey(key);

    public Weapon GetCurrentWeaponData () => slotBuffer.GetWeaponData;
}
