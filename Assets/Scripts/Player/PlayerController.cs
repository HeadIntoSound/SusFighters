using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Component.Animating;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] Rigidbody rb;
    bool canMove = true;
    [SerializeField] int jumpCount = 2;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask playersLayer;
    public SkinController skinController;
    public Transform target;
    [SerializeField] float rotation = 1;
    Vector3 directionToTarget;
    [SerializeField] float knockback = 2;
    [SerializeField] float range = 1.5f;
    [SerializeField] NetworkAnimator anim;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {

            this.enabled = false;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckGroundCollision();
        CastHit();
    }

    void Movement()
    {
        if (!canMove)
            return;

        FaceTarget();
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal") * rotation, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.W) && jumpCount > 0)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jumpCount--;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void CheckGroundCollision()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer))
        {
            jumpCount = 2;
        }
        else
        {
            jumpCount = jumpCount > 0 ? 1 : 0;
        }
    }

    void FaceTarget()
    {
        if (target == null)
            return;
        directionToTarget = (target.position - transform.position).normalized;
        if (directionToTarget.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            rotation = 1;
        }

        if (directionToTarget.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            rotation = -1;
        }

    }

    [ObserversRpc]
    public void RpcReceiveHit()
    {
        //rb.velocity = Vector3.zero;
        print("got hit");
        rb.AddForce((transform.position - target.position) * 2, ForceMode.Impulse);
    }

    void CastHit()
    {
        if (Input.GetKeyDown(KeyCode.Space) && target != null)
        {
            print("casting");
            anim.Play("Attack");
            RpcCastHitFromServer(transform.position, target.position - transform.position, range, playersLayer.value);
        }
    }

    [ServerRpc]
    void RpcCastHitFromServer(Vector3 from, Vector3 to, float range, int layer)
    {
        print("server processing hit in layer: " + layer.ToString());
        if (Physics.Raycast(from, to, out RaycastHit hit, range, layer))
        {
            if (hit.transform.TryGetComponent(out PlayerController otherPlayer))
            {
                otherPlayer.RpcReceiveHit();
            }
        }
    }
}
