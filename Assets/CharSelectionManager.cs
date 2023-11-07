using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharSelectionManager : NetworkBehaviour
{
    [SerializeField] private List<Character> _characters = new List<Character>();
    [SerializeField] private RespawnController _respawnController;
    [SerializeField] private GameObject _selectionScreen;
    private int _selectedChar;
    private GameObject _currentCharacter;

    [System.Serializable]
    public struct Character
    {
        [SerializeField] private GameObject _prefab,_visual;
        public GameObject GetPrefab() => _prefab;
        public GameObject GetVisual() => _visual;
    }

    [ClientRpc]
    public void ActivateSelection()
    {
        _selectionScreen.SetActive(true);
    }


    public void Spawn ()
    {
        CmdSpawn();
    }

    public void CmdSpawn()
    {
        _respawnController.Respawn(_selectedChar);
    }

    public void ChangeState (GameObject _obj)
    {
        _obj.SetActive(_obj.active ? false : true);
    }

    public void SelectCharacter (int ID)
    {
        _characters[_selectedChar].GetVisual().SetActive(false);
        _characters[ID].GetVisual().SetActive(true);
        _selectedChar = ID;
    }
}
