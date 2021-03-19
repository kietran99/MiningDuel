using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace MD.UI
{
    public class OffScreenIndicator : NetworkBehaviour
    {
        [SerializeField] GameObject indicator = null;
        private GameObject[] allPlayers = null;
        private List<GameObject> indicators = null;
        private GameObject playerClient = null;
        private int enemyCount = 0;
        private Camera cam =null;


        public override void OnStartClient()
        {
            StartUp();
        }

        void StartUp()
        {
            cam = Camera.main;
            if(allPlayers == null)
            {
                allPlayers = GameObject.FindGameObjectsWithTag("Player");
            }
            enemyCount = allPlayers.Length - 1;
            indicators = new List<GameObject>();
            for(int i = 0; i <enemyCount; i++)
            {
                GameObject indi = Instantiate(indicator,Vector3.zero,Quaternion.identity);
                indicators.Add(indi);
            }
            bool found = false;
            //Get Player codes here
            for(int i = 0; i < allPlayers.Length; i++)
            {
                if(!found)
                {
                    if(allPlayers[i].GetComponent<NetworkIdentity>().hasAuthority)
                    {
                        playerClient = allPlayers[i];
                        found = true;
                    }
                }
                else
                {
                    // for(int j = i - 1; j < allPlayers.Length - 1; j++)
                    // {
                    //     allPlayers[j] = allPlayers[j + 1];
                    // }
                    allPlayers = Array.FindAll(allPlayers,IsNotClientPlayer);
                    break;
                }
            }
        }
        bool IsNotClientPlayer(GameObject other)
        {
            return other != playerClient;
        }
        [ClientCallback]
        void LateUpdate()
        {
            GetIndicator();
        }

        void GetIndicator()
        {
            if(allPlayers == null)
            {
                Debug.Log("No player found, trying to rerun script!");
                StartUp();
                return;
            }
            int j = 0;
            // float[] zRotation = new float[allPlayers.Length];
            Debug.Log("Indicator: Num of Player tag found: "+ allPlayers.Length);
            Debug.Log("Indicator: Num of Indicator created: "+ indicators.Count);
            for(int i = 0; i < allPlayers.Length; i++)
            {
                if(j >= indicators.Count) return;
                float zRotation = indicators[j].transform.rotation.z;
                if(allPlayers[i].GetComponent<NetworkIdentity>().hasAuthority)
                {
                    Vector3 screenPos = cam.WorldToScreenPoint(allPlayers[i].transform.position);
                    if((screenPos.x > 0 && screenPos.x < Screen.width) && (screenPos.y > 0 && screenPos.y < Screen.height))      // ONSCREEN
                    {
                        indicators[j].SetActive(false);
                    }
                    else    // OFFSCREEN
                    {
                        indicators[j].SetActive(true);
                        indicators[j].transform.position = cam.ScreenToWorldPoint(new Vector3(Mathf.Clamp(screenPos.x,0,Screen.width),Mathf.Clamp(screenPos.y,0,Screen.height),screenPos.z));
                        Vector3 direction = indicators[j].transform.position - playerClient.transform.position;
                        float angle = Mathf.Atan2(direction.y,direction.x) * 180f / 3.14f - 90f- zRotation; // Use this if arrow sprite pointing up
                        // float angle = Mathf.Atan2(direction.y,direction.x) * 180f / 3.14f - zRotation // Use this if arrow sprite pointing to the right  
                        // indicators[j].transform.Rotate(0f,0f,angle);
                        indicators[j].transform.rotation = Quaternion.Euler(0f,0f,angle);
                    }   
                    j++;
                }         
            }
        }
    }
}