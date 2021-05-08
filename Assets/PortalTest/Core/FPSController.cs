using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FPSController : PortalTraveller {

    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float crouchSpeed = 1.5f;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;
    public Animator animator;
    public GameObject avatar;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);
    public float rotationSmoothTime = 0.1f;

    CharacterController controller;
    Camera cam;
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    bool jumping;
    float lastGroundedTime;
    bool disabled;
    private bool isCrouching;
    private bool notMine;

    private PhotonView pv;

    void Start ()
    {
        pv = GetComponent<PhotonView>();
        
        cam = Camera.main;
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController> ();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;

        if (pv.IsMine)
        {
            avatar.layer = 6;
        }
    }

    void Update () {
        // if (Input.GetKeyDown (KeyCode.P)) {
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        //     Debug.Break ();
        // }
        // if (Input.GetKeyDown (KeyCode.O))
        // {
        //     FindObjectOfType<PortalManager>().SetUpPortals();
        // }
        
        if (!pv.IsMine)
        {
            if (!notMine)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(GetComponent<CharacterController>());
                notMine = true;
            }
            return;
        }

        if (disabled) {
            return;
        }

        Move();
        Look();
    }

    private void Move()
    {
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

        Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.center = new Vector3(0, -.3f, 0);
            controller.height = 1.3f;
            cam.transform.localPosition = new Vector3(0, 0.22f, 0);
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            controller.center = Vector3.zero;
            controller.height = 2;
            cam.transform.localPosition = new Vector3(0, 0.662f, 0);
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

        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3 (velocity.x, verticalVelocity, velocity.z);
        //Debug.Log(velocity);

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
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
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
    }

}