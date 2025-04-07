using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool forcedCrouch = false;
        private bool _crouch;
        public bool crouch => forcedCrouch || _crouch;
        public bool roll;
        public bool lock_on_input = false;
        public bool HeavyAttack;
        public bool FastAttack;


        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

    

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnCrouch(InputValue value)
        {
            _crouch = value.isPressed;
        }

        public void OnRoll(InputValue value)
        {
            RollInput(value.isPressed);
        }

        public void OnLockon(InputValue value)
        {
            LockonInput(value.isPressed);
        }


        public void OnHeavyAttack(InputValue value)
        {
            HeavyAttackInput(value.isPressed);
        }

        public void OnFastAttack(InputValue value)
        {
            FastAttackInput(value.isPressed);
        }


#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void CrouchInput(bool newCrouchState)
        {
            _crouch = newCrouchState;
        }

        public void RollInput(bool newRollState)
        {
            roll = newRollState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        public void LockonInput(bool newLockonstate)
        {
            lock_on_input = newLockonstate;
        }


        public void HeavyAttackInput(bool newHeavyAttackstate)
        {
            HeavyAttack = newHeavyAttackstate;
        }

        public void FastAttackInput(bool newFastAttackstate)
        {
            FastAttack = newFastAttackstate;
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}