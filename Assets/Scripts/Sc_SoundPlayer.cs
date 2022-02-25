//Basic sript to play specific SFX
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevTest.Utils
{
    public class Sc_SoundPlayer : MonoBehaviour
    {
        public static Sc_SoundPlayer sPlayer;

        [SerializeField] AudioSource[] gameSounds = new AudioSource[10];
        void Awake()
        {
            if (sPlayer == null)
            {
                sPlayer = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Play(int s)
        {
            AudioSource audioSource = gameSounds[s];
            if (s>1)
            {
                
                audioSource.PlayOneShot(audioSource.clip);
            }
            else
            {
                audioSource.Play();
            }
        }

        public void Stop(int s)
        {
            if (s > 0)
            {
                AudioSource audioSource = gameSounds[s];
                audioSource.Stop();
            }
        }

        public void ChangePitch(int s, float value)
        {
            gameSounds[s].pitch = value;
        }

    }
}