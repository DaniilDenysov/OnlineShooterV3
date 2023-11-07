using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Create new weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private string weaponName = "Generic Weapon",weaponAnimation="Shoot";
    [SerializeField] private int weaponID = 0;
    [Range(0, 1000000)]
    [SerializeField] private int magCapacity = 20;
    [SerializeField] private AnimationCurve damageByDistance;
    [Range(0f, 1000)]
    [SerializeField] private float damage = 10f, reloadTime = 0.1f, cameraShakeForce = 1f, bulletForce=5f,bulletSpeed=5f,fireRate = 5f;
    [SerializeField] private weaponTypes weaponType;
    [SerializeField] private GameObject muzzleFlash;
   // [SerializeField] private Mesh weaponModel,magModel;
    public GameObject MuzzleFlash
    {
        get { return muzzleFlash; }
    }
    public string WeaponName
    {
        get { return weaponName; }
    }
    public string WeaponAnimation
    {
        get { return weaponAnimation; }
    }
    public AnimationCurve DamageByDistance
    {
        get { return damageByDistance; }
    }
    public int WeaponID
    {
        get { return weaponID; }
    }
    public float BulletForce
    {
        get { return bulletForce; }
    }
    public float BulletSpeed
    {
        get { return bulletSpeed; }
    }
    public float FireRate
    {
        get { return fireRate; }
    }

    public int MagCapacity
    {
        get { return magCapacity; }
    }

    public float Damage
    {
        get { return damage; }
    }

    public float ReloadTime
    {
        get { return reloadTime; }
    }

    public float CameraShakeForce
    {
        get { return cameraShakeForce; }
    }

    public weaponTypes WeaponType
    {
        get { return weaponType; }
    }
    
    public enum weaponTypes
    {
        Single,
        Auto,
        Burst,
        Melee
    }

}
