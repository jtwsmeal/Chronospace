﻿using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rigidBody;
    private float jumpForce = 5f;
    private float horizontalMovementSpeed;
    private float originalForwardMovementSpeed;
    private float forwardMovementSpeed;
    private bool grounded = true;
    private float dashCounter = 0f;

    // Dash length = 2 seconds.
    private float dashLength = 2f;
    public SoundManager soundManager;
    private float mouseSensitivity = 3f;
    public Transform cameraTransform;
    private Vector2 cameraRotation;
    private float maxYAngle = 90f;
    public Animator pendulumAnimator1;
    public Animator pendulumAnimator2;
    public Animator pendulumAnimator3;
    public Animator pendulumAnimator4;
    public Animator droneAnimator1;
    public Animator droneAnimator2;
    public Animator droneAnimator3;
    public Animator droneAnimator4;

    void Start()
    {
        /* Don't show user's cursor in the game, and lock the cursor to avoid going out of the game window.
         * Note that the Escape key can be used to show the cursor again (for example to stop running the game).
         */
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Camera starts out facing forward.
        cameraRotation = new Vector2(0, 0);

        rigidBody = GetComponent<Rigidbody>();

        originalForwardMovementSpeed = 500f;
        forwardMovementSpeed = originalForwardMovementSpeed;
        horizontalMovementSpeed = 500f;
    }

    private void TimeWarp()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            soundManager.PlayTimeWarpSound();
            GameManager.SetTimeWarp();
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            soundManager.PlayDashSound();
            dashCounter = dashLength;
        }
        if (dashCounter > 0f)
        {
            dashCounter -= Time.fixedUnscaledDeltaTime;
            forwardMovementSpeed *= 3;
        }
        else
        {
            forwardMovementSpeed = originalForwardMovementSpeed;
        }
        dashCounter = Mathf.Clamp(dashCounter, 0f, dashLength);
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
         * If the player has entered a collision with a platform, they are grounded.
         */
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = true;
        }
        else if (collision.gameObject.tag.Equals("Pendulum"))
        {
            /*PendulumControl pendulumControl2 = (PendulumControl)collision.gameObject.transform.parent.GetComponent(typeof(PendulumControl));
            Debug.Log(pendulumControl2.getPlayerForceVector());
            rigidBody.AddForce(-500 * pendulumControl2.getPlayerForceVector());*/

            //foreach (PendulumControl pendulumControl in FindObjectsOfType<PendulumControl>())
            //{
            //    if (GameObject.ReferenceEquals(pendulumControl.gameObject, collision.gameObject))
            //    {
            //        Debug.Log(pendulumControl.getPlayerForceVector());
            //        rigidBody.AddForce(pendulumControl.getPlayerForceVector());
            //    }
            //}

            //Debug.Log(((PendulumControl)collision.gameObject.transform.parent.GetComponent(typeof(PendulumControl))) == null);
            //Debug.Log(collision.impulse);
            //if (collision.impulse.x < 0)
            //{
            //    this.rigidBody.AddForce(new Vector3(-500, 0, 0));
            //}
            //else
            //{
            //    this.rigidBody.AddForce(new Vector3(500, 0, 0));
            //}
        }
        //foreach (GameObject pendulum in GameObject.FindGameObjectsWithTag("Pendulum"))
        //{
        //    if (GameObject.ReferenceEquals(pendulum, collision.gameObject))
        //    {
        //        //Debug.Log(pendulum.GetComponent<Rigidbody>().angularVelocity);
        //        Debug.Log(((PendulumControl)pendulum.transform.parent.GetComponent(typeof(PendulumControl))) == null);
        //        Vector3 velocity = ((PendulumControl)pendulum.transform.parent.GetComponent(typeof(PendulumControl))).getPlayerForceVector();
        //        Debug.Log(velocity);
        //    }
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        // If the player has exited a collision with a platform, they are no longer grounded.
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = false;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && this.grounded)
        {
            soundManager.PlayJumpSound();
            this.rigidBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    private void AdjustCamera()
    {
        // Rotate the camera based on mouse movement.
        cameraRotation.x = Mathf.Repeat(cameraRotation.x + Input.GetAxis("Mouse X") * mouseSensitivity, 360);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - Input.GetAxis("Mouse Y") * mouseSensitivity, -maxYAngle, maxYAngle);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // Rotate the player about the Y axis based on the camera's rotation.
        this.transform.eulerAngles = new Vector3(0, cameraRotation.x, 0);
    }

    private void Move()
    {
        float facingAngle = transform.eulerAngles.y * Mathf.PI / 180f;

        float forwardSpeed = (Input.GetAxis("Vertical") * Mathf.Cos(facingAngle) - Input.GetAxis("Horizontal") * Mathf.Sin(facingAngle)) * forwardMovementSpeed * Time.fixedUnscaledDeltaTime;
        float horizontalSpeed = (Input.GetAxis("Vertical") * Mathf.Sin(facingAngle) + Input.GetAxis("Horizontal") * Mathf.Cos(facingAngle)) * horizontalMovementSpeed * Time.fixedUnscaledDeltaTime;
        rigidBody.velocity = new Vector3(horizontalSpeed,
                                         rigidBody.velocity.y,
                                         forwardSpeed);
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        TimeWarp();
        Dash();
        AdjustCamera();
    }
}
