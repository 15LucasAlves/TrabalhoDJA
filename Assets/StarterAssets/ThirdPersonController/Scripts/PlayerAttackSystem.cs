using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerAttackSystem : MonoBehaviour
    {
        [Header("Attack Settings")]
        public float heavyAttackCooldown = 1.5f;
        public float fastAttackCooldown = 0.5f;

        [Header("Combo Timing")]
        public float comboResetTime = 1.2f; // Time window to chain the next attack
        private float lastAttackTime = 0.5f;

        [Header("Stamina")]
        public float heavyAttackStaminaCost = 6f;
        public float fastAttackStaminaCost = 2f;

        public int maxFastAttacks = 3;

        [Header("References")]
        public Animator animator;
        private float heavyAttackTimer;
        private bool canAttack = true;
        private int fastAttackCount = 0;
        private bool isFastAttacking = false;
        public float fastAttackDelay = 2f; // Delay between fast attacks

        //atencao aqui no start attack
        // StartAttack(false) = fast attacks.
        // StartAttack(true) = heavy attacks.
        bool fastAttack = false; 
        bool heavyAttack = true;

        //damage
        private SwordDamage _swordDamage;

        // Animation
        private int _animIDHeavyAttack;
        private int _animIDFastAttack1;
        private int _animIDFastAttack2;
        private int _animIDFastAttack3;
        private Animator _animator;
        private StarterAssetsInputs _input;
        public bool _hasAnimator;

        private PlayerStamina _playerStamina;

        private void Awake()
        {
            _input = GetComponent<StarterAssetsInputs>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            _playerStamina = GetComponent<PlayerStamina>();
            _swordDamage = GetComponentInChildren<SwordDamage>();
        }

        private void Start()
        {
            heavyAttackTimer = heavyAttackCooldown;
            _hasAnimator = TryGetComponent(out _animator);
            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDHeavyAttack = Animator.StringToHash("HeavyAttack");
            _animIDFastAttack1 = Animator.StringToHash("FastAttack1");
            _animIDFastAttack2 = Animator.StringToHash("FastAttack2");
            _animIDFastAttack3 = Animator.StringToHash("FastAttack3");
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            HandleAttackInput();
            UpdateCooldown();
        }

        private void HandleAttackInput()
        {
            if (_input.FastAttack && canAttack && !isFastAttacking && _playerStamina.CurrentStamina >= fastAttackStaminaCost)
            {
                // If too much time has passed, reset the combo
                if (Time.time - lastAttackTime > comboResetTime)
                {
                    fastAttackCount = 0;
                }

                if (fastAttackCount < maxFastAttacks)
                {
                    PerformFastAttack(fastAttackCount);
                    fastAttackCount++;
                    lastAttackTime = Time.time;
                }

                _input.FastAttack = false;
            }

            // Heavy attack
            if (_input.HeavyAttack && canAttack && _playerStamina.CurrentStamina >= heavyAttackStaminaCost)
            {
                PerformHeavyAttack();
                _input.HeavyAttack = false;
            }
        }


     

        private void PerformFastAttack(int attackIndex)
        {
            if (_hasAnimator)
            {
                switch (attackIndex)
                {
                    case 0: _animator.SetBool(_animIDFastAttack1, true); break;
                    case 1: _animator.SetBool(_animIDFastAttack2, true); break;
                    case 2: _animator.SetBool(_animIDFastAttack3, true); break;
                }
            }

            _playerStamina.ConsumeStamina(fastAttackStaminaCost);

            canAttack = false;
            isFastAttacking = true;

            if (_swordDamage != null)
                _swordDamage.StartAttack(fastAttack);

            StartCoroutine(FastAttackCooldown(attackIndex));
            StartCoroutine(ResetFastAttackDelay());
        }


        private IEnumerator ResetFastAttackDelay()
        {
            yield return new WaitForSeconds(fastAttackDelay);
            isFastAttacking = false;
        }

        private IEnumerator FastAttackCooldown(int attackIndex)
        {
            yield return new WaitForSeconds(fastAttackCooldown);

            if (_swordDamage != null)
                _swordDamage.EndAttack();

            if (_hasAnimator)
            {
                switch (attackIndex)
                {
                    case 0: _animator.SetBool(_animIDFastAttack1, false); break;
                    case 1: _animator.SetBool(_animIDFastAttack2, false); break;
                    case 2: _animator.SetBool(_animIDFastAttack3, false); break;
                }
            }

            canAttack = true;

            // Reset combo if no further input in time
            yield return new WaitForSeconds(comboResetTime);
            if (Time.time - lastAttackTime >= comboResetTime)
            {
                fastAttackCount = 0;
            }
        }



        private void PerformHeavyAttack()
        {
            if (_swordDamage != null)
                _swordDamage.StartAttack(heavyAttack);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHeavyAttack, true);
            }

            _playerStamina.ConsumeStamina(heavyAttackStaminaCost);

            canAttack = false;
            heavyAttackTimer = heavyAttackCooldown;

            StartCoroutine(ResetHeavyAttackBool());
        }

        private IEnumerator ResetHeavyAttackBool()
        {
            yield return new WaitForSeconds(2f);

            if (_swordDamage != null)
                _swordDamage.EndAttack();

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHeavyAttack, false);
            }
        }

        private void UpdateCooldown()
        {
            if (!canAttack)
            {
                heavyAttackTimer -= Time.deltaTime;
                if (heavyAttackTimer <= 0f)
                {
                    canAttack = true;
                }
            }
        }
    }
}
