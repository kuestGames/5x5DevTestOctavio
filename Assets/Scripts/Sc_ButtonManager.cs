using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevTest.Gameplay
{
    public class Sc_ButtonManager : MonoBehaviour
    {
        [SerializeField] GameObject CreditsScreen;
        [SerializeField] GameObject StartScreen;
        public void StartPressed(int l)
        {
            Sc_GameplayManager.Instance.StartPressed(l);
        }

        public void CreditsPressed(bool activate)
        {
            CreditsScreen.SetActive(activate);
            StartScreen.SetActive(!activate);
        }

        public void RetryPressed()
        {
            Sc_GameplayManager.Instance.Retry();
        }

        public void HomePressed()
        {
            Sc_GameplayManager.Instance.GoToHome();
        }

        public void PausePressed()
        {

        }
    }
}