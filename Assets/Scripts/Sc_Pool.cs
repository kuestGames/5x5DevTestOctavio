using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevTest.Gameplay
{
    public class Sc_Pool : MonoBehaviour
    {
        public GameObject ob;
        public int amount = 10;
        public bool willGrow = false;
        private List<GameObject> obs;



        // Use this for initialization
        void Start()
        {
            obs = new List<GameObject> { };
            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(ob);
                obj.SetActive(false);
                obs.Add(obj);
            }
        }

        public GameObject GetObj()
        {
            for (int i = 0; i < obs.Count; i++)
            {
                if (!obs[i].activeInHierarchy)
                {
                    return obs[i];
                }
            }

            if (willGrow)
            {
                GameObject obj = Instantiate(ob);
                obs.Add(obj);
                return obj;
            }
            else
            {
                int tempCount = obs.Count - 1;
                GameObject tempOBJ = obs[tempCount];
                obs.RemoveAt(tempCount);
                obs.Insert(0, tempOBJ);
                tempOBJ.SetActive(false);
                return tempOBJ;
            }
        }

        public void RePool()
        {

        }

        public void DeactivateAll()
        {
            if (obs != null)
            {
                for (int k = 0; k < obs.Count; k++)
                {
                    obs[k].SetActive(false);
                }
            }
        }
    }
}
