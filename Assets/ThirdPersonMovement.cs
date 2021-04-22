using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float sandSpeed;
    public float iceSpeed;
    public float concreteSpeed;

    float currentGravity;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float gravityMultiplier = 1f;
    public int jumps = 2;
    bool canJump;

    int remainingJumps;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    RaycastHit groundHit;

    public LayerMask wallMask;
    public float wallDistance;
    public float wallRunGravity;
    public float wallJumpForce;
    bool leftWall;
    bool rightwall;
    bool isWallRunning;
    public float minimumHeight;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;
    bool isGrounded;

    private void Start()
    {
        remainingJumps = jumps;
        currentGravity = gravity;
        concreteSpeed = speed;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        checkTerrain();

        if (isGrounded)
        {
            canJump = true;
            remainingJumps = jumps;
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        checkWall();

        if (canWallRun())
        {
            if (leftWall)
            {
                StartWallRun();
            }
            else if (rightwall)
            {
                StartWallRun();
            }
            else
            {
                stopWallRun();
            }
        }
        else
        {
            stopWallRun();
        }

        if(Input.GetButtonDown("Jump") && remainingJumps > 0 && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * currentGravity * gravityMultiplier);
            remainingJumps--;
        }

        if(remainingJumps <= 0) { canJump = false; }

        velocity.y += 2 * currentGravity * gravityMultiplier * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void StartWallRun()
    {
        speed = 10f;
        canJump = false;
        isWallRunning = true;
        currentGravity = 0;

        controller.Move(Vector3.down * wallRunGravity * gravityMultiplier);

        if (Input.GetButtonDown("Jump"))
        {
            if (leftWall)
            {
                Vector3 wallRunJumpDir = transform.up + leftWallHit.normal;
                controller.SimpleMove(Vector3.zero);
                controller.Move(new Vector3(controller.velocity.x, 0, controller.velocity.z));
                controller.Move(wallRunJumpDir * wallJumpForce);
            }
            if (rightwall)
            {
                Vector3 wallRunJumpDir = transform.up + rightWallHit.normal;
                controller.SimpleMove(Vector3.zero);
                controller.Move(new Vector3(controller.velocity.x, 0, controller.velocity.z));
                controller.Move(wallRunJumpDir * wallJumpForce);
            }
        }
    }

    void checkWall()
    {
        leftWall = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallDistance, wallMask);
        rightwall = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallDistance, wallMask);
    }

    void stopWallRun()
    {
        speed = 6;
        currentGravity = gravity;
        isWallRunning = false;
    }

    bool canWallRun()
    {
        if((leftWall && !rightwall) || (!leftWall && rightwall))
        {
            return true;
        }
        return false;
    }

    void checkTerrain()
    {
        if(Physics.Raycast(groundCheck.position, -transform.up, out groundHit, groundDistance ,groundMask))
        {
            switch (groundHit.transform.tag)
            {
                case ("Sand"):
                    speed = sandSpeed;
                    break;
                case ("Ice"):
                    speed = iceSpeed;
                    break;
                default:
                    speed = concreteSpeed;
                    break;
            }
        }
    }
}
