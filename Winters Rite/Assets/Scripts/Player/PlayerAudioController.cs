//////////////////////////////////////////////////////////////////////////
// File: <PlayerAttack.cs>
// Author: <Alex Kitching (Edited), Unity(Source)>
// Date Created: <8/03/17>
// Brief: <Script handling Players Audio.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{

    #region Variables
    [SerializeField]
    private AudioClip[] ac_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField]
    private AudioClip ac_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField]
    private AudioClip ac_LandSound;           // the sound played when character touches back on ground.

    private AudioSource m_AudioSource;
    #endregion

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayFootStepAudio(CharacterController a_CharacterController)
    {
        if (!a_CharacterController.isGrounded) // Character is floating 
        {
            return;
        }

        // Pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, ac_FootstepSounds.Length);
        m_AudioSource.clip = ac_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        ac_FootstepSounds[n] = ac_FootstepSounds[0];
        ac_FootstepSounds[0] = m_AudioSource.clip;
    }

    public void PlayJumpSound()
    {
        m_AudioSource.clip = ac_JumpSound;
        m_AudioSource.Play();
    }

    public void PlayLandingSound(float a_fNextStep, float a_fStepCycle)
    {
        m_AudioSource.clip = ac_LandSound;
        m_AudioSource.Play();
        a_fNextStep = a_fStepCycle + .5f;
    }
}
