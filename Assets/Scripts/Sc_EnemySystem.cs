using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevTest.Utils;

namespace DevTest.Gameplay
{
    public class Sc_EnemySystem : MonoBehaviour
    {

        [SerializeField] private GameObject[] enemyPrefabs = new GameObject[4];
        [SerializeField] private Sc_Pool[] enemyPools = new Sc_Pool[4];
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform gridPositionsHolder;


        public List<List<Vector3>> gridPositions = new List<List<Vector3>>();

        [HideInInspector] public List<GameObject> enemyObjList = new List<GameObject>();

        [SerializeField] private List<Transform> systemRows = new List<Transform>();


        //Flags and settings
        private bool isPlayig = false;
        private bool isPaused = false;
        private int startingStep = 0;
        private int currentStep = 0;
        private float stepSpeedAdd = 10f; //how much duration will decrease each step from max duration
        private Vector2 stepMinMaxSpeed = new Vector2(0.25f, 2f);
        private Vector2 minMaxPitch = new Vector2(1.5f, 3);
        private float stepPitchAdd = 0.5f; //how much pitch will be added each step from min pitch
        private float failurePoint = -12f;

        private float currentSpeed = 10f;
        private float currentPitch = 0.75f;


        //Enemy Attack Variables
        private Vector2 minMaxBulletDelay = new Vector2(0.5f, 1.5f);
        private float bulletDelaySubtract = 0.5f;
        private float bulletTimer = 0;

        private Vector2 minMaxBulletForce = new Vector2(500f, 4000f);
        private float bulletForceAdd = 0.5f;

        private float currentBulletDelay = 1f;

        private bool moveToLeft = false;
        private int movementType = 0; //0=uniform grid, 1=bricked alternate grid

        private float xMoveMax = 16f;
        private float zStepOffset = 2f;
        private float jumpDuration = 0.3f;
        [SerializeField] private AnimationCurve jumpCurve;


        public void LoadGridPositions()
        {
            int rowPointer = 0;
            gridPositions = new List<List<Vector3>>();
            foreach (Transform row in gridPositionsHolder)
            {
                //Transform rowTransform
                gridPositions.Add(new List<Vector3>());
                foreach (Transform slot in row)
                {
                    gridPositions[rowPointer].Add(slot.position);
                }
                rowPointer++;
            }

            stepSpeedAdd = (stepMinMaxSpeed.y - stepMinMaxSpeed.x) / rowPointer;
            stepPitchAdd = (minMaxPitch.y - minMaxPitch.x) / rowPointer;
            bulletDelaySubtract = (minMaxBulletDelay.y - minMaxBulletDelay.x) / rowPointer;
            bulletForceAdd = (minMaxBulletForce.y - minMaxBulletForce.x) / rowPointer;
        }

        public void SpawnGrid(Sc_Level currentLevelData)
        {
            enemyObjList = new List<GameObject>();
            movementType = currentLevelData.layout;
            for (int i = 0; i < gridPositions.Count; i++)
            {
                int orientation = 1;
                if (i % 2 == 0)
                {
                    orientation = -1;
                }

                Vector3 rowPos = gridPositions[i][0];
                rowPos.x = 0;
                systemRows[i].position = rowPos;
                for (int j = 0; j < gridPositions[i].Count; j++)
                {
                    int slotValue = currentLevelData.gridContent[i][j];
                    if (slotValue > 0)
                    {
                        //GameObject spawnedObj = Instantiate(enemyPrefabs[slotValue], gridPositions[i][j], Quaternion.identity, systemRows[i]);
                        GameObject spawnedObj = enemyPools[slotValue].GetObj();
                        spawnedObj.transform.position = gridPositions[i][j];
                        spawnedObj.transform.rotation = Quaternion.identity;
                        spawnedObj.transform.parent = systemRows[i];
                        spawnedObj.SetActive(true);

                        enemyObjList.Add(spawnedObj);
                        int pointsToGive = 10;
                        int basehp = 1;
                        switch (slotValue)
                        {
                            case 1:
                                pointsToGive = 30;
                                break;
                            case 2:
                                pointsToGive = 20;
                                basehp = 2;
                                break;
                            case 3:
                                pointsToGive = 10;
                                basehp = 3;
                                break;
                            case 4:
                                pointsToGive = 100;
                                basehp = 3;
                                break;
                            default:
                                break;
                        }
                        spawnedObj.GetComponent<Sc_Enemy>().points = pointsToGive;
                        spawnedObj.GetComponent<Sc_Enemy>().orientation = orientation;
                        spawnedObj.GetComponent<Sc_Enemy>().hp = basehp;

                    }
                }
            }
            startingStep = currentLevelData.startingStep-2;
            //startingStep = 2;
            currentStep = startingStep;

            currentSpeed = stepMinMaxSpeed.x + (stepSpeedAdd * (currentStep));
            currentPitch = minMaxPitch.x + (stepPitchAdd * (currentStep));
            currentBulletDelay = minMaxBulletDelay.y - (bulletDelaySubtract * (currentStep));
        }

        public void InitSystem(Sc_Level currentLevelData)
        {
            ResetSystem();
            SpawnGrid(currentLevelData);
        }

        public void StartSystem()
        {
            isPlayig = true;
        }

        public void ResetSystem()
        {
            currentStep = 0;
            currentSpeed = stepMinMaxSpeed.x;
            currentPitch = minMaxPitch.x;
            currentBulletDelay = minMaxBulletDelay.x;
            Sc_SoundPlayer.sPlayer.ChangePitch(1, currentPitch);
            for (int i = 0; i < enemyPools.Length; i++)
            {
                enemyPools[i].DeactivateAll();
            }
        }

        public void PauseSystem(bool pause)
        {
            isPaused = pause;
            if (isPaused)
            {
                //display Pause Screen
            }
        }

        public void StopSystem()
        {
            isPlayig = false;
        }

        private void FixedUpdate()
        {
            if (!isPlayig || isPaused)
            {
                return;
            }
            bulletTimer += Time.fixedDeltaTime;
            if (bulletTimer >= currentBulletDelay)
            {
                bulletTimer = 0;
                SystemAttack();

            }
            MoveSystem();
        }

        private void SystemAttack()
        {
            int r = Random.Range(0, enemyObjList.Count);
            Sc_GameplayManager.Instance.EnemyHasShot(enemyObjList[r].transform.position, bulletForceAdd + minMaxBulletForce.x);
        }

        private void MoveSystem()
        {
            bool finishedStep = false;
            bool reachedFinal = false;
            //row method
            /*for (int i = 0; i < systemRows.Count; i++)
            {
                int moveOrientation = 1;
                if (movementType == 1)
                {
                    if (i % 2 == 0)
                    {
                        moveOrientation = -1;
                    }
                }
                if (moveToLeft)
                {
                    moveOrientation = moveOrientation * (-1);
                }
                //Vector3 newPosition = systemRows[i].position;
                Vector3 target = systemRows[i].position;
                target.x = target.x + (Time.fixedDeltaTime * moveOrientation);
                if (moveOrientation == 1)
                {
                    if (target.x >= xMoveMax)
                    {
                        finishedStep = true;
                    }
                }
                else
                {
                    if (target.x <= (xMoveMax * (-1)))
                    {
                        finishedStep = true;
                    }
                }
                systemRows[i].position = target;
            }*/

            //cell method
            for (int i = 0; i < enemyObjList.Count; i++)
            {
                int moveOrientation = 1;
                if (movementType == 1)
                {
                    moveOrientation = enemyObjList[i].GetComponent<Sc_Enemy>().orientation;

                }
                if (moveToLeft)
                {
                    moveOrientation = moveOrientation * (-1);
                }
                //Vector3 newPosition = systemRows[i].position;
                Vector3 target = enemyObjList[i].transform.position;
                target.x = target.x + ((Time.fixedDeltaTime * moveOrientation)*currentSpeed);
                if (moveOrientation == 1)
                {
                    if (target.x >= xMoveMax)
                    {
                        finishedStep = true;
                    }
                }
                else
                {
                    if (target.x <= (xMoveMax * (-1)))
                    {
                        finishedStep = true;
                    }
                }
                enemyObjList[i].transform.position = target;
                if (target.z<=failurePoint)
                {
                    reachedFinal = true;
                }
            }
            if (reachedFinal)
            {
                Sc_GameplayManager.Instance.HandleFinish(false);
                return;
            }
            if (finishedStep)
            {
                NextStep();
            }

        }

        private void NextStep()
        {
            
            //if last step then finish
            currentStep++;
            Debug.Log(currentStep + "");
            currentSpeed = stepMinMaxSpeed.x + (stepSpeedAdd * (currentStep));
            currentPitch = minMaxPitch.x + (stepPitchAdd * (currentStep));
            currentBulletDelay = minMaxBulletDelay.y - (bulletDelaySubtract * (currentStep));
            moveToLeft = !moveToLeft;
            Sc_SoundPlayer.sPlayer.ChangePitch(1, currentPitch);
            if (currentStep>2 && currentStep % 2 == 0)
            {
                Sc_GameplayManager.Instance.SpawnPowerUp();
            }


            StartCoroutine(JumpToNextStep());

            /*for (int i = 0; i < systemRows.Count; i++)
            {
                StartCoroutine(JumpToNextStep(i));
            }*/

        }

        IEnumerator JumpToNextStep() {
            isPlayig = false;
            for (int i = 0; i < systemRows.Count; i++)
            {
                StartCoroutine(RowJump(i));
                if(i< (systemRows.Count-1))
                {
                       yield return new WaitForSeconds(0.05f);
                }
                else
                {
                    yield return null;
                }
            }
            isPlayig = true;
            
        } 


        IEnumerator RowJump(int row)
        {
            //isPlayig = false;
            float timer = 0;
            Vector3 originalPos = systemRows[row].position;
            Vector3 targetPos = originalPos;
            while (timer < jumpDuration)
            {
                float normalizedTimer = Mathf.InverseLerp(0, jumpDuration, timer);
                targetPos.y = jumpCurve.Evaluate(normalizedTimer);
                targetPos.z = Mathf.Lerp(originalPos.z, originalPos.z-zStepOffset, normalizedTimer);
                systemRows[row].position = targetPos;


                timer += Time.deltaTime;
                yield return null;
            }
            originalPos.z -= zStepOffset;
            systemRows[row].position = originalPos;
            //isPlayig = true;
        }


    }
}