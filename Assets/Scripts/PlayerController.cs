using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private CapsuleCollider capsule;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody _rb;
    private Animator _animator;
    private bool _isGrounded;
    [SerializeField] float jumpVelocity = 15f;
    [SerializeField] float gravityUp = -20f;
    [SerializeField] float gravityDown = -40f;
    [SerializeField] float maxFallSpeed = -80f;

    private float _verticalVelocity;
    
    [SerializeField] float acceleration = 30f;
    [SerializeField] float deceleration = 20f;

    private float _horizontalSpeed;
    public LayerMask groundLayer;
    
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsJumpDown = Animator.StringToHash("IsJumpDown");
    private static readonly int Speed = Animator.StringToHash("Speed");

    void Start() {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }
    
    void Update() {
        Grounded();
        
        _verticalVelocity = _rb.velocity.y;

        float moveInput = Input.GetAxisRaw("Horizontal");
        float speed = Mathf.Abs(moveInput);

        // Animation
        _animator.SetFloat(Speed, speed);
        _animator.SetBool(IsGrounded, _isGrounded);

        // Direction + acceleration
        if (moveInput > 0) {
            _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, moveInput * moveSpeed, acceleration * Time.deltaTime);
            transform.localScale = new Vector3(-1, 1, 1); // Facing left
        } else if (moveInput < 0) {
            _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, moveInput * moveSpeed, acceleration * Time.deltaTime);
            transform.localScale = new Vector3(1, 1, 1); // Facing right
        } else {
            _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, 0, deceleration * Time.deltaTime);
        }

        bool jumpPressed = Input.GetButtonDown("Jump");
        bool jumpHeld = Input.GetButton("Jump");

        if (_isGrounded) {
            _animator.SetBool(IsJumping, !_isGrounded);
            _animator.SetBool(IsJumpDown, !_isGrounded);
            if (jumpPressed) {
                _verticalVelocity = jumpVelocity;
                // Temporarily disable gravity logic to let jump happen
            }
        } else {
            if (_rb.velocity.y > 0f) {
                _animator.SetBool(IsJumping, true);
                if (!jumpHeld) {
                    // Snappy jump cutoff
                    _verticalVelocity *= 0.5f * Time.deltaTime; // or set to a fixed low value
                } else {
                    _verticalVelocity += gravityUp * Time.deltaTime;
                }
            } else if (_rb.velocity.y < 0f) {
                _animator.SetBool(IsJumpDown, true);
                _verticalVelocity += gravityDown * Time.deltaTime;
            } else {
                _verticalVelocity += gravityDown * Time.deltaTime;
            }
        }

        // Apply velocity once
        _rb.velocity = new Vector3(_horizontalSpeed, _verticalVelocity, 0f);

        // Debug.Log("Horizontal Velocity: " + _rb.velocity.x);
    }
        
    void Grounded() {
            
        Vector3 rayOrigin = transform.position + Vector3.up * capsule.center.y;
        float capsuleBottom = rayOrigin.y - (capsule.height / 2f - 0.1f);
        
        // Final ray origin placed at the capsule bottom center
        Vector3 origin = new Vector3(transform.position.x, capsuleBottom, transform.position.z);
        
        _isGrounded = Physics.Raycast(origin, Vector3.down, 0.2f, groundLayer);
        
        // Optional: Draw ray in Scene view
        Debug.DrawRay(origin, Vector3.down * 0.2f, _isGrounded ? Color.green : Color.red);
    }
}