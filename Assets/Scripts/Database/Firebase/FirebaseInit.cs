using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Unity;
using Firebase.Firestore;
using System;
using UnityEngine.Events;

namespace MD.Database
{
    public class FirebaseInit : MonoBehaviour
    {
        FirebaseApp app;
        FirebaseFirestore db;
        bool firebaseAppInitialized = false;
        public static FirebaseInit instance = null;
        Dictionary<string, object> playerDoc = null;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

            }
            else
            {
                Destroy(gameObject);
            }
        }
        void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    app = Firebase.FirebaseApp.DefaultInstance;
                    firebaseAppInitialized = true;
                    // app.Options.DatabaseUrl = "";
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
            });
            TryGetData();
            // StartReadAllUsersName();
            TryGetPlayerInfo();
        }


        IEnumerator WaitForFirebaseAppInitThenExec(Action action)
        {
            while (!firebaseAppInitialized)
            {
                yield return null;
            }

            action();
        }

        void TryGetData()
        {
            StartCoroutine(WaitForFirebaseAppInitThenExec(GetData));
        }

        void GetData()
        {
            db = FirebaseFirestore.DefaultInstance;
        }

        

        void TryGetPlayerInfo()
        {
            StartCoroutine(WaitForFirebaseAppInitThenExec(GetPlayerInfo));
        }

        void GetPlayerInfo()
        {
            bool check;
            string id = SavePlayerData.GetPlayerID(out check);
            if(!check)
            {
                Debug.Log("Firebase: Create new Document!");
                string name = SavePlayerData.LoadPlayerData().Name;
                Dictionary<string, object> user =new Dictionary<string, object>
                {
                    {"name", name},
                    {"totalWin", 0},
                };
                playerDoc = user;
                db.Collection("players").AddAsync(user).ContinueWithOnMainThread(task =>
                {
                    DocumentReference addedDocRef = task.Result;
                    SavePlayerData.SaveId(new PlayerID(addedDocRef.Id));
                });
                Debug.Log(String.Format("Firebase: Player Info \n Name : {0} \n TotalWin :{1} \n",playerDoc["name"],playerDoc["totalWin"]));
                return;
            }
            CollectionReference userRef = db.Collection("players");
            userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot doc in snapshots.Documents)
                {
                    if(doc.Id == id)
                    {
                        playerDoc = doc.ToDictionary();
                    }
                }
                Debug.Log(String.Format("Firebase: Player Info \n Name : {0} \n TotalWin :{1} \n",playerDoc["name"],playerDoc["totalWin"]));
            });
        }

        void TryReadAllUsersName()
        {
            StartCoroutine(WaitForFirebaseAppInitThenExec(ReadAllUsersName));
        }

        void ReadAllUsersName()
        {
            CollectionReference usersRef = db.Collection("players");
            usersRef
                .GetSnapshotAsync()
                .ContinueWithOnMainThread(
                    task =>
                    {
                        QuerySnapshot snapshot = task.Result;
                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            Dictionary<string, object> doc = document.ToDictionary();
                            Debug.Log(String.Format("Database => User name: {0}", doc["name"]));
                        }
                    });
        }
    }
}
