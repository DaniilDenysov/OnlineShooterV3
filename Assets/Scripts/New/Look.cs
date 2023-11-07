using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Look : NetworkBehaviour
{
    [Range(0,1000)]
    [SerializeField] private float _rotationSpeed = 400f;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _validLayers;

    [ClientCallback]
    private void Start()
    {
        if (!isOwned) Destroy(_camera.gameObject);
        else _camera.gameObject.transform.SetParent(null);
    }

    [ClientCallback]
    private void Update()
    {
        if (!isOwned) return;
        if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit _raycast, _validLayers)) return;
        ChangeDir(new Vector3(_raycast.point.x, 0, _raycast.point.z) - new Vector3(0, 0.01f, 0), new Vector3(transform.position.x, 0, transform.position.z));
    }

    [ServerCallback]
    private bool IsValid (Ray ray)
    {
        return Physics.Raycast(ray, out RaycastHit raycast, _validLayers);
    }

    [ServerCallback]
    private RaycastHit GetPoint(Vector3 _screenPosition)
    {
        Physics.Raycast(_camera.ScreenPointToRay(_screenPosition), out RaycastHit _raycast, _validLayers);
        return _raycast;
    }

    [Command]
    private void ChangeDir(Vector3 a, Vector3 b)
    {
        Quaternion tmp = Quaternion.LookRotation(a - b, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tmp, _rotationSpeed * Time.deltaTime);
    }
}
