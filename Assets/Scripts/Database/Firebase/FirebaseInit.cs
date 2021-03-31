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

            // StartReadAllUsersName();
        }


        IEnumerator WaitForFirebaseAppInitThenExec(Action action)
        {
            while (!firebaseAppInitialized)
            {
                yield return null;
            }

            action();
        }

        void StartReadAllUsersName()
        {
            StartCoroutine(WaitForFirebaseAppInitThenExec(ReadAllUsersName));
        }

        void ReadAllUsersName()
        {
            db = FirebaseFirestore.DefaultInstance;
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
                            // text.text += String.Format("User: {0}\n", document.Id);
                        }
                    });
        }
    }
}
