using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Mirror;

public class PlayerMovementManager : NetworkBehaviour
{

    [SerializeField] private Camera camera;
    [SerializeField] private GameObject aim;
    [SerializeField] private GameObject vc;
    [SerializeField] private LayerMask layer;
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private Animator animator2;
    [SerializeField] private SynchronizedCustomSlider slider;
    [Range(0, 1000)]
    [SerializeField] private float movementSpeed = 10, rotationSpeed = 100,rotationUnit = 10,movementAcceleration=5,currentSpeed,jumpForce;
    private float angle = 0;
    [SyncVar]
    private float blendWalk = 0f;

    [Header("Dash settings")]
    [SerializeField] private float dashSpeed,dashDuration; 

    [SyncVar]
    private bool isDashing;

    [SyncVar]
    private Vector2 dir;
    private Rigidbody rb;
    [SyncVar]
    private Vector3 prevPosition;

    private float shorizontal, svertical, svelocityZ, svelocityX;
    Vector3 smovement;

    private void Start()
    {
        if (!isOwned) Destroy(camera.gameObject);
        else
        {
            vc.transform.SetParent(null);
        }
        prevPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    [Command]
    private void CmdWalk (Vector3 movement)
    {
        rb.velocity = movement;
    }
    [Command]
    private void CmdIdle()
    {
        rb.velocity = new Vector3();
    }

    [Command]
    private void CmdChangeDir(Vector3 a,Vector3 b)
    {
        Quaternion tmp = Quaternion.LookRotation(a - b, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tmp, rotationSpeed * Time.deltaTime);
    }
    [Command]
    private void CmdJump()
    {
        Vector3 dir = CalculateDir();
        rb.AddForce(new Vector3(dir.x * jumpForce, dir.y * jumpForce, dir.z * jumpForce), ForceMode.Impulse);
    }
    private Vector3 CalculateDir ()
    {
        return (transform.position-prevPosition).normalized;
    }

    [ClientCallback]
    private void Update()
    {

        if (!isOwned) return;
        if (isDashing) return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit raycast, layer)) return;
        CmdChangeDir(new Vector3(raycast.point.x, 0, raycast.point.z) - new Vector3(0, 0.01f, 0), new Vector3(transform.position.x, 0, transform.position.z));
        if (movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= currentSpeed;
            CmdWalk(movement);
        }
        else
        {
            CmdIdle();
        }

        if (Input.GetKeyDown(KeyCode.Space) && slider.GetCurrentValue() >= 50)
        {
            Vector3 dir = transform.forward;
            dir.Normalize();
            shorizontal = horizontal;
            svertical = vertical;
            svelocityX = velocityX;
            svelocityZ = velocityZ;
            smovement = movement;
            isDashing = true;
            GetComponent<MeshTrail>().Activate();
            CmdDash(dir);
            slider.SetCurrentValue(slider.GetCurrentValue() - 50);
            return;
        }
        //if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift) && slider.GetCurrentValue() == 100) { animator.SetTrigger("Jump");  Vector3 dir = CalculateDir(); slider.SetCurrentValue(slider.GetCurrentValue()-100); CmdJump(); PlayCrossfade(velocityX, velocityZ, "JumpMovement"); return; }
        prevPosition = transform.position;
        if (Input.GetKey(KeyCode.C))
        {
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            currentSpeed = movementSpeed + movementAcceleration;
            return; }
        else
        {
            currentSpeed = movementSpeed;
        }

    }
    public void PlayCrossfade (float velocityX,float velocityZ,string name)
    {
        animator2.CrossFade(name, 0.1f);
        animator2.SetFloat("Y", velocityZ, 0.1f, Time.deltaTime);
        animator2.SetFloat("X", velocityX, 0.1f, Time.deltaTime);
    }

    [Command]
    private void CmdDash(Vector3 direction)
    {
        rb.velocity = direction *= dashSpeed;
        Invoke(nameof(StopDash),dashDuration);
    }

    [ClientRpc]
    private void StopDash ()
    {
        isDashing = false;
    }

}
