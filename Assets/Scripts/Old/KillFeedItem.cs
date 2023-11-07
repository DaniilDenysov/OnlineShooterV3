using UnityEngine;
using TMPro;

public class KillFeedItem : MonoBehaviour
{
    [SerializeField] private TMP_Text display;

    public void InitializeData (string arg1,string arg2,string arg3)
    {
        display.text = $"{arg1} > {arg3} > {arg2}";
    }

    public void DestroyItem ()
    {
        Destroy(gameObject);
    }
}
