using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevTest.Utils;
using TMPro;
using UnityEngine.UI;

namespace DevTest.Gameplay
{
    public class Sc_GameplayManager : Sc_Singleton<Sc_GameplayManager>
    {
        [Header("Panels References")]
        [SerializeField] private GameObject FinishScreen;
        [SerializeField] private GameObject StartScreen;
        [SerializeField] private GameObject PauseScreen;
        [SerializeField] private GameObject HUD;

        private int score = 0;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text scoreGainedLabel;
        [SerializeField] private TMP_Text levelLabel;

        private bool started = false;
        private bool finished = false;
        private bool paused = false;

        //Ship Settings
        private int shipLevel = 1;
        private int shipHP = 5;
        [SerializeField] Transform UIHP;

        public Sc_Pool[] bulletPools = new Sc_Pool[4];
        private int[] bulletSpeeds = new int[]{1000,2000,3000,4000};
        private float[] bulletDelays = new float[] { 0.5f, 0.25f, 0.1f, 0.05f };


        private int currentLevel = 0;
        Sc_Level currentLevelData;

        [SerializeField] private Sc_VFXManager vfxManager;
        [SerializeField] private Sc_EnemySystem enemySystem;
        [SerializeField] private Sc_PlayerController playerController;



        void Start()
        {
            //StartGame(1);
            GoToHome();
        }

        public void StartPressed(int l)
        {
            StartScreen.SetActive(false);
            FinishScreen.SetActive(false);
            vfxManager.DisplayMessage("Level" + (l+1));
            scoreLabel.text = "Level " + (l + 1);
            currentLevel = l;
            score = 0;
            shipLevel = 0;
            shipHP = 5;
            SetHPUI();
            scoreLabel.text = "Score";
            enemySystem.LoadGridPositions(); //loads the transform position of each grid cell
            LoadCurrentLevel();  //transaltes the json data into a level object
            enemySystem.InitSystem(currentLevelData);
            
            playerController.fireDelay = bulletDelays[0];
            StartCoroutine(StartDelayed(4));

        }

        public void StartGame()
        {
            started = true;
            finished = false;
            enemySystem.StartSystem();
            HUD.SetActive(true);
            playerController.isPlaying = true;
            Sc_SoundPlayer.sPlayer.Play(1);

        }

        IEnumerator StartDelayed(float delay)
        {
            float timer = 0;

            while (timer< delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            StartGame();
        }



        private void LoadCurrentLevel()
        {
            currentLevelData = Sc_DataReader.Instance.CacheSelectedLevel(currentLevel);

        }

        
        public void FireBullet(Vector3 pos)
        {
            GameObject bullet = bulletPools[shipLevel].GetObj();
            bullet.transform.position = pos;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
            bullet.GetComponent<Rigidbody>().AddForce(Vector3.forward * bulletSpeeds[shipLevel]);
            bullet.GetComponent<Sc_Bullet>().shotByPlayer = true;
            int shotSound = shipLevel+2;
            Mathf.Clamp(shotSound, 2, 5); //ShotSounds
            Sc_SoundPlayer.sPlayer.Play(shotSound);
        }

        public void PlayerGotHit(Vector3 pos)
        {
            vfxManager.PlayerHitVFX(pos);
            if (shipHP<=1)
            {
                HandleFinish(false);
            }
            else
            {
                Sc_SoundPlayer.sPlayer.Play(10); //damageReceived
                shipHP--;

            }
            SetHPUI();
        }

        public void EnemyGotHit(GameObject e)
        {
            int explosionSound = 6; //smallexplosion
            if (shipLevel > 1)
            {
                explosionSound = 7; //bigexplosion
            }
            Sc_SoundPlayer.sPlayer.Play(explosionSound);

            e.GetComponent<Sc_Enemy>().hp--;
            if (e.GetComponent<Sc_Enemy>().hp > 0)
            {
                vfxManager.EnemyHitVFX(e.transform.position, 5);
                return;
            }
            vfxManager.EnemyHitVFX(e.transform.position, 1);
            int newPoints = e.GetComponent<Sc_Enemy>().points;
            scoreGainedLabel.text ="+" +newPoints;
            scoreGainedLabel.GetComponent<Animator>().SetTrigger("Pop");
            score += newPoints;
            scoreLabel.text = "Score["+ score+"]";
            e.SetActive(false);
            enemySystem.enemyObjList.Remove(e);
            if (enemySystem.enemyObjList.Count == 0)
            {
                HandleFinish(true);
            }
        }

        public void EnemyHasShot(Vector3 pos, float force)
        {
            GameObject bullet = bulletPools[0].GetObj();
            bullet.transform.position = pos;
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
            bullet.GetComponent<Rigidbody>().AddForce(Vector3.back * force);
            bullet.GetComponent<Sc_Bullet>().shotByPlayer = false;
            Sc_SoundPlayer.sPlayer.Play(2);
        }

        public void BarrierGotHit(GameObject b)
        {
            vfxManager.EnemyHitVFX(b.transform.position, shipLevel);
            Sc_SoundPlayer.sPlayer.Play(6); //smallexplosion
            //e.SetActive(false);
        }

        public void SpawnPowerUp()
        {
            PowerUp();
        }

        public void PowerUp()
        {
            shipLevel++;
            if (shipLevel>3)
            {
                shipLevel = 3;
            }
            playerController.fireDelay = bulletDelays[shipLevel];
            vfxManager.PowerUpVFX(playerController.transform.position);
        }


        public void HandleFinish(bool won)
        {
            enemySystem.StopSystem();
            finished = true;
            started = false;
            //FinishScreen.SetActive(true);
            playerController.isPlaying = false;
            Sc_SoundPlayer.sPlayer.Stop(1);
            string msg = "";
            if (won)
            {
                FinishScreen.transform.Find("Score").gameObject.SetActive(true);
                FinishScreen.transform.Find("ScoreTitle").gameObject.SetActive(true);
                Sc_SoundPlayer.sPlayer.Play(8);
                msg = "Well Done!";
            }
            else
            {
                FinishScreen.transform.Find("Score").gameObject.SetActive(false);
                FinishScreen.transform.Find("ScoreTitle").gameObject.SetActive(false);
                Sc_SoundPlayer.sPlayer.Play(7); //bigExplosion
                Sc_SoundPlayer.sPlayer.Play(9); //lose
                msg = "You Lose";
            }

            vfxManager.DisplayFinish(won);
            FinishScreen.transform.Find("Score").GetComponent<TMP_Text>().text = score+"";
            FinishScreen.transform.Find("Message").GetComponent<TMP_Text>().text = msg;
            HUD.SetActive(false);


            StartCoroutine(DelayedToggle(4, FinishScreen, true));
        }

        IEnumerator DelayedToggle(float delay, GameObject obj, bool setActive)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(setActive);
        }

        public void Retry()
        {
            StartPressed(currentLevel);
        }

        public void GoToHome()
        {
            FinishScreen.SetActive(false);
            StartScreen.SetActive(true);
            PauseScreen.SetActive(false);
            HUD.SetActive(false);
        }

        private void SetHPUI()
        {
            for (int i = 1; i <= 5; i++)
            {
                if (i<= shipHP)
                {
                    UIHP.Find("" + i).gameObject.SetActive(true);
                }
                else
                {
                    UIHP.Find("" + i).gameObject.SetActive(false);
                }
            }
        }

    }
}
