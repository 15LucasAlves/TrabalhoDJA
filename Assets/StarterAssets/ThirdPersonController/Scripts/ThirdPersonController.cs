using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {


        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Header("Crouch Settings")]

        [Tooltip("Crouch speed of the character in m/s")]
        public float CrouchSpeed = 1.5f;


        [Header("Dive Roll Settings")]
        public float DiveSpeed = 5f;
        public float DiveDuration = 0f;
        public float RollForce = 8f;
        public float RollDuration = 1.05f;

        [Header("Player Stamina Settings")]
        private PlayerStamina _playerStamina;


        [Header("Lock-On System")] // o lock baseia-se na distacia entre o playe ro enemy pode ser alterado para direção do jogador maybe?
        public string targetTag = "Enemy";
        public float lockOnRange = 15f;
        private Transform currentLockOnTarget;
        private bool isLockedOn = false;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        //for the attack system
        public bool IsRolling => _isRolling;

        // player crouch
        private int _animIDCrouch;
        private bool _isCrouching = false;

        //player dive roll
        private int _animIDRoll;
        private bool _isRolling = false;
        private float _rollTimer = 0f;


        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        public bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _playerStamina = GetComponent<PlayerStamina>();

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

        }

        private void Update()
        {

            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();



            if (_input.lock_on_input)
            {
                ToggleLockOn();
                _input.lock_on_input = false; // reset the input flag
            }
        }

        private void LateUpdate()
        {
            if (isLockedOn && currentLockOnTarget != null)
            {
                // Smoothly transition to the locked-on camera position
                Vector3 targetDirection = currentLockOnTarget.position - CinemachineCameraTarget.transform.position;
                float targetYaw = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

                // Fix the camera pitch angle to keep it looking up during lock-on (adjust as needed)
                float targetPitch = Mathf.Asin(targetDirection.y / targetDirection.magnitude) * Mathf.Rad2Deg;

                // Set a minimum pitch to prevent the camera from dipping too low (e.g., 10 degrees)
                targetPitch = Mathf.Clamp(targetPitch, 10f, TopClamp);

                // Smooth transition of yaw and pitch
                _cinemachineTargetYaw = Mathf.LerpAngle(_cinemachineTargetYaw, targetYaw, Time.deltaTime * 5f); // Interpolation factor
                _cinemachineTargetPitch = Mathf.LerpAngle(_cinemachineTargetPitch, targetPitch, Time.deltaTime * 5f); // Interpolation factor

                CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
            }
            else
            {
                // Regular camera rotation
                CameraRotation();
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // Clamp camera angles to prevent going beyond the limits
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDCrouch = Animator.StringToHash("Crouch");
            _animIDRoll = Animator.StringToHash("Roll");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }



        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed and crouch is pressed
            bool canSprint = _input.sprint && _playerStamina.currentStamina > 0f;
            float targetSpeed = _input.crouch ? CrouchSpeed : (canSprint ? SprintSpeed : MoveSpeed);

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (isLockedOn && currentLockOnTarget != null)
            {
                // The player should always face the enemy
                Vector3 directionToTarget = currentLockOnTarget.position - transform.position;
                directionToTarget.y = 0; // Ensure we only rotate on the Y-axis (horizontal plane)

                // Smooth rotation towards the enemy
                _targetRotation = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                // Calculate movement direction relative to camera but maintain facing the enemy
                if (_input.move != Vector2.zero)
                {
                    // Get camera forward and right vectors
                    Vector3 cameraForward = _mainCamera.transform.forward;
                    Vector3 cameraRight = _mainCamera.transform.right;
                    cameraForward.y = 0;
                    cameraRight.y = 0;
                    cameraForward.Normalize();
                    cameraRight.Normalize();

                    // Calculate movement direction based on camera orientation
                    Vector3 desiredMoveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
                    _targetRotation = Mathf.Atan2(desiredMoveDirection.x, desiredMoveDirection.z) * Mathf.Rad2Deg;
                }
            }
            else if (_input.move != Vector2.zero)
            {
                // Regular rotation when not locked on
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


            if (_input.sprint && _input.move != Vector2.zero && Grounded && !_isCrouching)
            {
                if (_playerStamina.currentStamina > 0f)
                {
                    _playerStamina.RunStaminaSpending();
                }
            }
            else
            {
                // Regenerate stamina when not sprinting
                _playerStamina.RegenerateStamina();
            }

            //handle rooling 
            if (_input.roll && Grounded && !_isRolling && _playerStamina.currentStamina >= 1.25f)
            {
                // Consume stamina
                _playerStamina.ConsumeStamina(1.25f);

                // Start the roll
                _isRolling = true;
                _rollTimer = RollDuration;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDRoll, true);
                }
            }

            // Handle rolling state
            if (_isRolling)
            {
                _rollTimer -= Time.deltaTime;  // Decrease the roll timer

                if (_rollTimer <= 0f)
                {
                    _isRolling = false;  // End the rolling state when timer reaches 0
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDRoll, false);  // Reset the animation state
                    }

                    Vector3 rollDirection = transform.forward;
                    _controller.Move(rollDirection * RollForce * Time.deltaTime);
                }
                return;
            }

            //handle crouch
            if (_input.crouch && Grounded)
            {
                _isCrouching = true;
            }
            else
            {
                _isCrouching = false;
            }

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                _animator.SetBool(_animIDCrouch, _isCrouching);
            }
        }


        // for camera view enemy lock

        private void ToggleLockOn()
        {
            if (isLockedOn)
            {
                // Unlock
                isLockedOn = false;
                currentLockOnTarget = null;
            }
            else
            {
                // Lock onto the nearest valid target
                Transform nearestTarget = FindNearestTargetWithTag(targetTag);
                if (nearestTarget != null)
                {
                    isLockedOn = true;
                    currentLockOnTarget = nearestTarget;
                }
            }
        }

        private Transform FindNearestTargetWithTag(string tag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            Transform nearest = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < shortestDistance && distance <= lockOnRange)
                {
                    shortestDistance = distance;
                    nearest = target.transform;
                }
            }

            return nearest;
        }






        private void JumpAndGravity()
        {
            if (Grounded && !_isCrouching && !_isRolling)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color Green = new Color(r: 0.0f, g: 1.0f, b: 0.0f, a: 0.35f);
            Color Red = new Color(r: 1.0f, g: 0.0f, b: 0.0f, a: 0.35f);

            if (Grounded) Gizmos.color = Green;
            else Gizmos.color = Red;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
