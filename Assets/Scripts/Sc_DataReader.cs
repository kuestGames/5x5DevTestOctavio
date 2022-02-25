using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace DevTest.Utils
{
    public class Sc_DataReader : Sc_Singleton<Sc_DataReader>
    {
        JSONNode levelsJSON;
        private List<JSONNode> levelsList = new List<JSONNode>();
        // Start is called before the first frame update
        void Awake()
        {
            TextAsset textAsset = (TextAsset)Resources.Load("LevelsData");
            string content = textAsset.ToString();
            levelsJSON = JSONNode.Parse(content);
            LoadLevels();
        }

        private void LoadLevels()
        {
            foreach (var jsonLevel in levelsJSON.Values)
            {
                levelsList.Add(jsonLevel);
            }
        }

        public Sc_Level CacheSelectedLevel(int l)
        {
            Sc_Level lev = new Sc_Level();
            int rowPointer = 0;
            int slotPointer = 0;
            foreach (var row in levelsList[l]["rows"].Values)
            {
                lev.gridContent.Add(new List<int>());
                foreach (var slot in row.Values)
                {
                    int.TryParse(slot.Value, out int sValue);
                    lev.gridContent[rowPointer].Add(sValue);
                    slotPointer++;
                }
                rowPointer++;
            }
            //lev.speed = levelsList[l]["speed"].Value;
            float.TryParse(levelsList[l]["speed"].Value, out lev.speed);
            int.TryParse(levelsList[l]["layout"].Value, out lev.layout);
            int.TryParse(levelsList[l]["startingStep"].Value, out lev.startingStep);
            return lev;

        }

    }
}