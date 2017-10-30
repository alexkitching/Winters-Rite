//////////////////////////////////////////////////////////////////////////
// File: <PlayerAttack.cs>
// Author: <Alex Kitching (Edited), Unity(Source)>
// Date Created: <8/03/17>
// Brief: <Script handling Players Controls.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerAudioController))]
    [RequireComponent(typeof(AudioSource))]

    public class PlayerController : NetworkBehaviour
    {
        #region Variables
        [SerializeField]
        private Camera o_Camera;
        [SerializeField]
        private bool bIsWalking;
        [SerializeField]
        private float fWalkSpeed;
        [SerializeField]
        private float fRunSpeed;
        [SerializeField]
        [Range(0f, 1f)]
        private float fRunstepLenghten;
        [SerializeField]
        private float fJumpSpeed;
        [SerializeField]
        private float fStickToGroundForce;
        [SerializeField]
        private float fGravityMultiplier;
        [SerializeField]
        private MouseLook s_MouseLook;
        [SerializeField]
        private bool bUseFovKick;
        [SerializeField]
        private FOVKick s_FovKick = new FOVKick();
        [SerializeField]
        private bool bUseHeadBob;
        [SerializeField]
        private CurveControlledBob s_HeadBob = new CurveControlledBob();
        [SerializeField]
        private LerpControlledBob s_JumpBob = new LerpControlledBob();
        [SerializeField]
        private float fStepInterval;

        private bool bJump;
        private float fYRotation;
        private Vector2 vInput;
        private Vector3 vMoveDir = Vector3.zero;
        private CharacterController s_CharacterController;
        private PlayerAudioController s_AudioController;
        private Player s_Player;
        private CollisionFlags m_CollisionFlags;
        private bool bPreviouslyGrounded;
        private Vector3 vOriginalCameraPosition;
        private float fStepCycle;
        private float fNextStep;
        private bool bJumping;
        private bool bThirstIncreased;
        
        #endregion

        // Start Function used for Initialisation
        private void Start()
        {
            s_CharacterController = GetComponent<CharacterController>();
            s_AudioController = GetComponent<PlayerAudioController>();
            s_Player = GetComponent<Player>();
            vOriginalCameraPosition = o_Camera.transform.localPosition;
            s_FovKick.Setup(o_Camera);
            s_HeadBob.Setup(o_Camera, fStepInterval);
            fStepCycle = 0f;
            fNextStep = fStepCycle / 2f;
            bJumping = false;
            s_MouseLook.Init(transform, o_Camera.transform);
            Cursor.lockState = CursorLockMode.Locked; // Cursor Locked to Center of Screen
            Cursor.visible = false; // Cursor not visible
        }


        // Update is called once per frame
        private void Update()
        {
            if (PauseMenu.bIsOn) // Pause menu is on, exit
                return;
            
            RotateView();

            // The jump state needs to read here to make sure it is not missed
            if (!bJump)
            {
                bJump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            // When Player is sprinting, increase thirst rate
            if (!bIsWalking && !bThirstIncreased)
            {
                s_Player.MultiplyThirstRate();
                bThirstIncreased = true;
            }
            else if (bIsWalking && bThirstIncreased) // When Player is not sprinting reset thirst rate if previously increased
            {
                s_Player.ResetThirstRate();
                bThirstIncreased = false;
            }

            if (!bPreviouslyGrounded && s_CharacterController.isGrounded)
            {
                StartCoroutine(s_JumpBob.DoBobCycle());
                s_AudioController.PlayLandingSound(fNextStep, fStepCycle);
                vMoveDir.y = 0f;
                bJumping = false;
            }
            if (!s_CharacterController.isGrounded && !bJumping && bPreviouslyGrounded)
            {
                vMoveDir.y = 0f;
            }

            bPreviouslyGrounded = s_CharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // Always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * vInput.y + transform.right * vInput.x;

            // Get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, s_CharacterController.radius, Vector3.down, out hitInfo,
                               s_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            vMoveDir.x = desiredMove.x * speed;
            vMoveDir.z = desiredMove.z * speed;

            // Check Character is on Ground
            if (s_CharacterController.isGrounded)
            {
                vMoveDir.y = -fStickToGroundForce;

                if (bJump)
                {
                    vMoveDir.y = fJumpSpeed;
                    s_AudioController.PlayJumpSound();
                    bJump = false;
                    bJumping = true;
                }
            }
            else
            {
                vMoveDir += Physics.gravity * fGravityMultiplier * Time.fixedDeltaTime;
            }
            m_CollisionFlags = s_CharacterController.Move(vMoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            s_MouseLook.UpdateCursorLock();
        }

        private void ProgressStepCycle(float speed)
        {
            if (s_CharacterController.velocity.sqrMagnitude > 0 && (vInput.x != 0 || vInput.y != 0))
            {
                fStepCycle += (s_CharacterController.velocity.magnitude + (speed * (bIsWalking ? 1f : fRunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(fStepCycle > fNextStep))
            {
                return;
            }

            fNextStep = fStepCycle + fStepInterval;

            s_AudioController.PlayFootStepAudio(s_CharacterController);
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!bUseHeadBob)
            {
                return;
            }
            if (s_CharacterController.velocity.magnitude > 0 && s_CharacterController.isGrounded)
            {
                o_Camera.transform.localPosition =
                    s_HeadBob.DoHeadBob(s_CharacterController.velocity.magnitude +
                                      (speed * (bIsWalking ? 1f : fRunstepLenghten)));
                newCameraPosition = o_Camera.transform.localPosition;
                newCameraPosition.y = o_Camera.transform.localPosition.y - s_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = o_Camera.transform.localPosition;
                newCameraPosition.y = vOriginalCameraPosition.y - s_JumpBob.Offset();
            }
            o_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            if (PauseMenu.bIsOn) //Pause Input
            {
                speed = 0; 
                return;
            }
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = bIsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            bIsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // Set the desired speed to be walking or running
            speed = bIsWalking ? fWalkSpeed : fRunSpeed;
            vInput = new Vector2(horizontal, vertical);

            // Normalize input if it exceeds 1 in combined length:
            if (vInput.sqrMagnitude > 1)
            {
                vInput.Normalize();
            }

            // Handle speed change to give an FOV kick
            // Only if the player is going to a run, is running and the FOVkick is to be used
            if (bIsWalking != waswalking && bUseFovKick && s_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!bIsWalking ? s_FovKick.FOVKickUp() : s_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            s_MouseLook.LookRotation(transform, o_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            // Don't move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(s_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
