using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "HitAction", menuName = "Create new hit action")]
public class ShootNode : ScriptableObject
{
    public string action;
    public int actionLayer;
}
