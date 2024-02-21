using System.Collections;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Component.Animating;
using FishNet.Object.Synchronizing;
using FishNet.Object.Prediction;
using FishNet.Transporting;

public class PlayerController : NetworkBehaviour
{
    [SyncVar(OnChange = nameof(OnHealthChange))] public float health;   // Percentage of damage received since the last respawn
    [SyncVar(OnChange = nameof(OnPointsChange))] public int points;     // Amount of times the player threw the opponent out the stage (or the opponent fell)

    float timeSinceLastAttack;  // Used to check if the player can attack
    bool jump;                  // Used to trigger the jump
    bool hitstun;               // Hitstun is a state where the player got hit and can't input any movement, this is used to restrict movement when needed
    bool ledgeGrab;             // The ledge grab is very primitive, could be expanded, for now this checks if the player is hugging the ledge
    int playerSlot;             // Used to know if this player is player 1 or player 2
    [SerializeField] PlayerStats stats; // Holds all the variables used to balance the character
    [SerializeField] Rigidbody rb;
    public PlayerController target;     // A reference to the other player
    [SerializeField] NetworkAnimator anim;
    public SkinController skinController;
    [SerializeField] HitboxController hitboxController;
    [SerializeField] CapsuleCollider col;
    [SerializeField] LayerMask groundLayer;

    // Events to update the UI
    void OnHealthChange(float prev, float next, bool asServer)
    {
        if (asServer)
            return;
        EventManager.Instance.OnHealthChange.Invoke(next, playerSlot);
    }

    void OnPointsChange(int prev, int next, bool asServer)
    {
        if (asServer)
            return;
        EventManager.Instance.OnPointsChange.Invoke(next, playerSlot);
    }

    // Client side prediction set up
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        base.TimeManager.OnTick += TimeManager_OnTick;
        base.TimeManager.OnPostTick += TimeManager_OnPostTick;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        base.TimeManager.OnTick -= TimeManager_OnTick;
        base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
    }

    // This was taken from the official fish-net documentation
    MoveData BuildMoveData()
    {
        if (!base.IsOwner)
            return default;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MoveData md = new MoveData(jump, hitstun, horizontal, vertical);
        jump = false;
        return md;
    }

    // The Move function was adapted from the documentation to suit the project's needs
    [ReplicateV2]
    void Move(MoveData md, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
    {
        CheckGroundCollision();
        if (md.hitstun)
            return;
        if (md.horizontal != 0 && !ledgeGrab)
            HorizontalMovement(md.horizontal, stats.acceleration, stats.maxSpeed);
        if (md.vertical < 0)
        {
            rb.AddForce(Physics.gravity * 2);
            col.center = Vector3.forward;
        }
        if (md.jump)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * stats.jumpSpeed, ForceMode.Impulse);
            stats.jumpCount--;
        }

    }

    void TimeManager_OnTick()
    {
        Move(BuildMoveData());
    }

    void TimeManager_OnPostTick()
    {
        if (IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, rb.velocity, rb.angularVelocity);
            Reconciliation(rd);
        }
    }

    [ReconcileV2]
    void Reconciliation(ReconcileData rd, Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;
        if (rb.isKinematic)
            return;
        rb.velocity = rd.Velocity;
        rb.angularVelocity = rd.AngularVelocity;
    }

    // Used to not run any of this on remote players
    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     if (!base.IsOwner)
    //     {
    //         this.enabled = false;
    //     }
    // }

    void Update()
    {
        if (base.IsOwner)
        {
            FaceTarget();
            Jump();
            Attack();
        }

    }

    // This was also taken from the documentation, adapted the jump count to allow double jump
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && stats.jumpCount > 0)
            jump = true;
    }

    // Calculates and applies the movement
    void HorizontalMovement(float input, float acceleration, float maxSpeed)
    {
        Vector3 force = Vector3.right * input * acceleration;
        rb.AddForce(force);
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = new Vector3(input * maxSpeed, rb.velocity.y, 0f);
    }

    // Uses a short raycast from the base of the character to know if is grounded
    // Given the fact that the game does not use the Z axis to move, I use it to drop and jump to the platforms without any inconvenience
    // If the player is in the air, the collider moves back (respective from the camera, forward globally), when grounded is at the center
    void CheckGroundCollision()
    {
        Vector3 origin = transform.position + Vector3.down * transform.lossyScale.y * 0.9f;
        if (Physics.Raycast(origin, Vector3.down, .15f, groundLayer))
        {
            stats.jumpCount = 2;
            col.center = Vector3.zero;
        }
        else
        {
            stats.jumpCount = stats.jumpCount == 2 ? 1 : stats.jumpCount;
            if (!Physics.Raycast(origin, Vector3.down, .15f, groundLayer))
                col.center = Vector3.forward;
        }
    }

    // Automatically rotates the player to face the target at all times
    void FaceTarget()
    {
        if (target == null)
            return;
        Vector3 dir = (target.transform.position - transform.position).normalized;
        if (dir.x > 0)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (dir.x < 0)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    // Used to set up the UI's values when connection is established
    public void InitializeUI(int slot)
    {
        playerSlot = slot;
        EventManager.Instance.OnHealthChange.Invoke(health, playerSlot);
        EventManager.Instance.OnPointsChange.Invoke(points, playerSlot);
    }

    // The server calls this function to calculate the knockback and hitstun of the player that got hit
    [ObserversRpc]
    public void RpcReceiveHit()
    {
        anim.Play(AnimationNames.TAKEDMG);
        hitstun = true;
        StartCoroutine(HitstunTime());
        
        if (!IsOwner)
            return;

        rb.velocity = Vector3.zero;
        float horizontal = (transform.position - target.transform.position).normalized.x;
        Vector3 force = new Vector3(horizontal, .75f, 0f) * (stats.knockback + health * Constants.KNOCKBACKMODIFIER);
        rb.AddForce(force, ForceMode.Impulse);
    }

    // Calculates the amount of time the player is going to be unable to input movement
    IEnumerator HitstunTime()
    {
        skinController.HitstunIndicator(true);
        float hitstun = health * Constants.HITSTUNMODIFIER;
        yield return new WaitForSeconds(hitstun);
        this.hitstun = false;
        skinController.HitstunIndicator(false);
    }

    // Produces the attack, telling the hitbox to activate and check if triggered
    void Attack()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack < stats.attackDuration + stats.attackCooldown || hitstun)
            return;
        if (Input.GetButtonDown("Fire1"))
        {
            anim.Play(AnimationNames.ATTACK);
            hitboxController.Attack(stats.attackDuration, () => { RpcTriggerHit(target); });
            timeSinceLastAttack = 0;
        }
    }

    // Triggers the attack
    [ServerRpc]
    void RpcTriggerHit(PlayerController toHit)
    {
        toHit.health += Constants.DMGMODIFIER;
        toHit.RpcReceiveHit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner)
            return;
        // Checks if the player is hugging the ledge, allows them to jump higher to recover
        if (other.CompareTag(Tags.LEDGE))
        {
            ledgeGrab = true;
            stats.jumpCount = 1;
            stats.jumpSpeed *= 1.25f;
        }
        // Stops the player when respawning
        if (other.CompareTag(Tags.BLASTZONE))
            rb.velocity = Vector3.zero;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner)
            return;
        // Resets the jump speed previously raised
        if (other.transform.CompareTag(Tags.LEDGE))
        {

            ledgeGrab = false;
            stats.jumpSpeed /= 1.25f;
        }
    }
}
