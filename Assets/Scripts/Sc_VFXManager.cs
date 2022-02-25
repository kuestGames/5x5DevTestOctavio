using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DevTest.Gameplay
{
    public class Sc_VFXManager : MonoBehaviour
    {

        [SerializeField] private Sc_CameraController cameraController;
        [SerializeField] private ParticleSystem displayStripe;
        [SerializeField] private ParticleSystem displayStripeLose;
        [SerializeField] private ParticleSystem displayStripeWin;
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private TMP_Text displayText2;

        [SerializeField] private GameObject hurtOverlay;
        [SerializeField] private GameObject powerUpOverlay;

        [SerializeField] private Sc_Pool[] explosionPools = new Sc_Pool[3];
        public void PlayerHitVFX(Vector3 pos)
        {

            cameraController.ShakeCamera();
            hurtOverlay.SetActive(false);
            hurtOverlay.SetActive(true);
            GameObject exp = explosionPools[2].GetObj();
            exp.transform.position = pos;
            exp.SetActive(true);

        }

        public void EnemyHitVFX(Vector3 pos, int l)
        {
            GameObject exp;
            if (l>2)
            {
                exp = explosionPools[1].GetObj();
            }
            else
            {
                exp = explosionPools[0].GetObj();
            }
            exp.transform.position = pos;
            exp.SetActive(true);
           
        }

        public void BarrierHitVFX(Vector3 pos)
        {

        }

        public void DisplayMessage(string msg)
        {
            displayText.text = msg;
            displayText.gameObject.SetActive(false);
            displayText.gameObject.SetActive(true);
            displayStripe.gameObject.SetActive(false);
            displayStripe.gameObject.SetActive(true);
        }

        public void DisplayFinish(bool won)
        {
            string msg = "";
            if (won)
            {
                displayStripeWin.gameObject.SetActive(false);
                displayStripeWin.gameObject.SetActive(true);
                msg = "Success";
            }
            else
            {
                displayStripeLose.gameObject.SetActive(false);
                displayStripeLose.gameObject.SetActive(true);
                msg = "Failure";
            }

            displayText2.text = msg;
            displayText2.gameObject.SetActive(false);
            displayText2.gameObject.SetActive(true);

        }

        public void PowerUpVFX(Vector3 pos)
        {
            powerUpOverlay.SetActive(false);
            powerUpOverlay.SetActive(true);
        }


    }
}