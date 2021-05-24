using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FPSController : PortalTraveller, IDamagable {

    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float crouchSpeed = 1.5f;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;
    public Animator animator;
    public float maxHealth;
    public float currenthealth;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);
    public float rotationSmoothTime = 0.1f;
    public bool isSeeker;
    public bool isGrounded;
    public bool onLadder;

    CharacterController controller;
    public Transform camTransform;
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    public float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    bool jumping;
    float lastGroundedTime;
    bool disabled;
    private bool isCrouching;
    float fireRate = 0.9f;
    float nextFire;
    private bool hasSetCrouch;
    
    private PlayerManager playerManager;
    private GameManager gameManager;
    private PhotonView pv;
    private CrouchCheck crouchCheck;
    private AudioSource audio;
    private PlayerSounds playerSounds;
    private CapsuleCollider cc;
    private HealthBar healthBar;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        pv = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        
        if(!pv.IsMine)
        {
            Destroy(GetComponent<CharacterController>());
            cc = gameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
            cc.height = 1.85f;
            cc.center = new Vector3(0, -0.1f, 0);
            return;
        }
        
        crouchCheck = transform.GetChild(2).GetComponent<CrouchCheck>();
        audio = GetComponent<AudioSource>();
        playerSounds = GetComponent<PlayerSounds>();
        healthBar = FindObjectOfType<HealthBar>();
        
        Camera.main.transform.SetParent(camTransform);
        Camera.main.transform.localPosition = Vector3.zero;
        camTransform.localPosition = new Vector3(0, 0.662f, 0);
        Cursor.lockState = CursorLockMode.Locked;
        
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController> ();

        yaw = transform.eulerAngles.y;
        pitch = camTransform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;

        if(isSeeker)
        {
            maxHealth = 99999;
            currenthealth = maxHealth;
            runSpeed = 6.06f;
            healthBar.SetMaxHealth((int)currenthealth);
            return;
        }

        currenthealth = maxHealth;
        healthBar.SetMaxHealth((int)currenthealth);
    }

    void Update () 
    {
        if (Input.GetKeyDown (KeyCode.P)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break ();
        }

        if (!pv.IsMine)
        {
            return;
        }

        if (disabled) {
            return;
        }
        
        Move();
        Look();
        Interact();
        if(isSeeker)
        {
            Shoot();
        }
    }

    private IEnumerator Punch()
    {
        animator.SetBool("isPunching", true);
        yield return new WaitForSeconds(0.85f);
        animator.SetBool("isPunching", false);
    }
    
    

    private void Move()
    {
        Vector2 input = Vector2.zero;
        if (!onLadder)
        {
            input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        }

        Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);

        if (Input.GetKey(KeyCode.LeftControl) && !hasSetCrouch)
        {
            isCrouching = true;
            controller.center = new Vector3(0, -.35f, 0);
            controller.height = 1.3f;
            camTransform.localPosition = new Vector3(0, 0.22f, 0);
            pv.RPC("RPC_Crouching", RpcTarget.Others);
            hasSetCrouch = true;
        }
        
        if (!Input.GetKey(KeyCode.LeftControl) && !crouchCheck.shouldCrouch && hasSetCrouch)
        {
            isCrouching = false;
            controller.center = Vector3.zero;
            controller.height = 2;
            camTransform.localPosition = new Vector3(0, 0.662f, 0);
            pv.RPC("RPC_DoneCrouching", RpcTarget.Others);
            hasSetCrouch = false;
        }

        float currentSpeed = walkSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if(!isCrouching && Input.GetKey (KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
            
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);
        
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isSprinting", Input.GetKey(KeyCode.LeftShift));
        if (input.x != 0 || input.y != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (!onLadder)
            verticalVelocity -= gravity * Time.deltaTime;

        velocity = new Vector3 (velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move (velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below) {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f)) {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }
    }

    [PunRPC]
    private void RPC_Crouching()
    {
        cc.center = new Vector3(0, -.35f, 0);
        cc.height = 1.3f;
        camTransform.localPosition = new Vector3(0, 0.22f, 0);
    }
    
    [PunRPC]
    private void RPC_DoneCrouching()
    {
        cc.center = Vector3.zero;
        cc.height = 2;
        camTransform.localPosition = new Vector3(0, 0.662f, 0);
    }

    private void Look()
    {
        float mX = Input.GetAxisRaw ("Mouse X");
        float mY = Input.GetAxisRaw ("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt (mX * mX + mY * mY);
        if (mMag > 5) {
            mX = 0;
            mY = 0;
        }

        yaw += mX * mouseSensitivity;
        pitch -= mY * mouseSensitivity;
        pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        camTransform.localEulerAngles = Vector3.right * smoothPitch;
    }

    private void Shoot()
    {
        if(Input.GetMouseButton(0) && Time.time > nextFire)
        {        
            nextFire = Time.time + fireRate;
            StartCoroutine("Punch");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 2f)) 
            {
                if (hit.transform.gameObject.GetComponent<IDamagable>() != null)
                {
                    hit.transform.gameObject.GetComponent<IDamagable>().TakeDamage(30);
                    playerSounds.SoundPunch(true);
                    return;
                }
                playerSounds.SoundPunch(true);
                return;
            }
            playerSounds.SoundPunch(false);
        }
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
            int layerMask = 1 << 7;
            if (Physics.Raycast(ray, out hit, 5f, layerMask)) {
                if (hit.transform.gameObject.GetComponent<IInteractable>() != null)
                {
                    hit.transform.gameObject.GetComponent<IInteractable>().Interact();
                }
            }
        }
    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms ();

        TeleportSyncer();
    }

    public void TeleportSyncer()
    {
        StartCoroutine("TeleportSync");
    }

    private IEnumerator TeleportSync()
    {
        pv.RPC("RPC_Teleport", RpcTarget.All);
        yield return new WaitForSeconds(0.4f);
        pv.RPC("RPC_TeleportDone", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Teleport()
    {
        if(pv.IsMine)
            return;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    [PunRPC]
    void RPC_TeleportDone()
    {
        if(pv.IsMine)
            return;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void TakeDamage(float damage)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!pv.IsMine)
            return;

        currenthealth -= damage;
        healthBar.SetHealth((int)currenthealth);
        
        if(currenthealth <= 0)
        {
            if (!gameManager.nextSeekerFound)
            {
                gameManager.nextSeeker = true;
            }
            pv.RPC("RPC_NextSeekerFound", RpcTarget.All);
            pv.RPC("RPC_NextSpectator", RpcTarget.Others);
            //Camera.main.gameObject.AddComponent<Observer>();
            Die();
        }
    }

    [PunRPC]
    private void RPC_NextSeekerFound()
    {
        gameManager.nextSeekerFound = true;
        gameManager.UpdatePlayerCount(true);
    }

    [PunRPC]
    private void RPC_NextSpectator()
    {
        if (transform.GetChild(0).transform.childCount > 0)
        {
            transform.GetChild(0).transform.GetChild(0).GetComponent<Observer>().GoNextPlayer();
        }
        if (FindObjectOfType<Observer>() != null)
        {
            FindObjectOfType<Observer>().alivePlayers.Remove(gameObject);
        }
    }

    private void Die()
    {
        playerManager.Die(false);
    }

}