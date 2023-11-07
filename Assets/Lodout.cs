using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lodout : NetworkBehaviour
{
    [SerializeField] private List<LodoutWeapon> _lodoutWeapons = new List<LodoutWeapon>();
    [SyncVar(hook = nameof(OnWeaponChanged))]
    [SerializeField] private int _currentWeaponIndex;
    [SerializeField] private Transform _RH, _LH;

    [System.Serializable]
    public struct LodoutWeapon
    {
        public KeyCode key;
        public Weapon weaponData;
        public GameObject weaponObject;
        public Transform LH,RH;
        public void ChangeState() => weaponObject.SetActive(!weaponObject.active);
        public ParticleSystem _sleeves;
        public void PlaySleeves ()
        {
            _sleeves.Play();
        }
    }

    [ServerCallback]
    public LodoutWeapon GetCurrentWeapon() => _lodoutWeapons[_currentWeaponIndex];

    [ClientCallback]
    void Update()
    {
        if (!isOwned) return;
        if (!Input.anyKey) return;
        for (int i = 0;i<_lodoutWeapons.Count;i++)
        {
            if (Input.GetKey(_lodoutWeapons[i].key))
            {
                ChangeWeapon(i);
                return;
            }                                                                                                                                       
        }
    }

    [Command]
    private void ChangeWeapon (int _index)
    {
        _currentWeaponIndex = _index;
    }

    private void OnWeaponChanged (int _oldLodoutWeaponIndex, int _newLodoutWeaponIndex)
    {
        _lodoutWeapons[_newLodoutWeaponIndex].ChangeState();
        _lodoutWeapons[_oldLodoutWeaponIndex].ChangeState();
        //LH
        _RH.transform.position = _lodoutWeapons[_newLodoutWeaponIndex].RH.position;
        _RH.transform.rotation = _lodoutWeapons[_newLodoutWeaponIndex].RH.rotation;
        //RH
        _LH.transform.position = _lodoutWeapons[_newLodoutWeaponIndex].LH.position;
        _LH.transform.rotation = _lodoutWeapons[_newLodoutWeaponIndex].LH.rotation;
    }
}
