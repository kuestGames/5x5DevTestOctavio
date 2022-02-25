using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevTest.Gameplay
{
    public class Sc_Bullet : MonoBehaviour
    {
        public bool shotByPlayer = true; //if false, the shot was shot by enemy
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Finish")
            {
                this.gameObject.SetActive(false);
                return;
            }
            if (other.tag == "Player")
            {
                if (!shotByPlayer)
                {
                    Sc_GameplayManager.Instance.PlayerGotHit(transform.position);
                    gameObject.SetActive(false);
                }
            }

            if (other.tag == "Enemy")
            {
                if (shotByPlayer)
                {
                    Sc_GameplayManager.Instance.EnemyGotHit(other.gameObject);
                    gameObject.SetActive(false);
                }
            }

            if (other.tag == "Barrier")
            {

            }

        }
    }
}
