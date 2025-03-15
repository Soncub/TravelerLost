using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private CharacterController characterController;
    private Vector3 direction;

    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float speed;
    private Camera mainCamera;

    private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float velocity;

    [SerializeField] private float jumpPower;
    public AudioSource playerSounds;
    public Animator animator;
    int isWalkingHash;
    bool isWalking;
    int isJumpingHash;
    bool isJumping;
    int isFallingHash;
    bool isFalling;
    int isLandingHash;
    bool isLanding;
    public ParticleSystem jumpParticles;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GameObject.Find("MC Animations1").GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        isJumpingHash = Animator.StringToHash("IsJumping");
        isFallingHash = Animator.StringToHash("IsFalling");
        isLandingHash = Animator.StringToHash("IsGrounded");
    }

    private void Update()
    {
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();
        isWalking = animator.GetBool(isWalkingHash);
        isJumping = animator.GetBool(isJumpingHash);
        isFalling = animator.GetBool(isFallingHash);
        isLanding = animator.GetBool(isLandingHash);
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && velocity < 0.0f)
        {
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isLandingHash, true);
            velocity = -1.0f;
        }
        else
        {
            animator.SetBool(isFallingHash, true);
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }
        direction.y = velocity;
    }
    private void ApplyRotation()
    {
        if (input.sqrMagnitude == 0) return;
        direction = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(input.x, 0.0f, input.y);
        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    private void ApplyMovement()
    {
        characterController.Move(direction * speed * Time.deltaTime);
    }
    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
        animator.SetBool(isWalkingHash, true);
        if (input.sqrMagnitude == 0)
        {
            animator.SetBool(isWalkingHash, false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (this.enabled == true)
        {
            if (IsGrounded())
            {
                animator.SetBool(isJumpingHash, true);
                animator.SetBool(isLandingHash, false);
                playerSounds.Play();
                jumpParticles.Play();
            }
            if (!context.started) return;
            if (!IsGrounded()) return;
            velocity += jumpPower;
        }
    }

    private bool IsGrounded() => characterController.isGrounded;

    public void EnablePlayerController()
    {
        this.enabled = true;
    }

    public void DisablePlayerController()
    {
        this.enabled = false;
    }
}