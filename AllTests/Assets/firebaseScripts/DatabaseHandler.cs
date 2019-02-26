using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.UI;
using System.Threading.Tasks;
public class DatabaseHandler : MonoBehaviour {


    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;



    void Start()
    {
        dependencyStatus = FirebaseApp.CheckDependencies();
        if (dependencyStatus != DependencyStatus.Available)
        {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = FirebaseApp.CheckDependencies();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeDatabase();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        else
        {
            // InitializeFirebase();

        }
    }

    public void InitializeDatabase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://vrcycling-6cab8.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
        {

            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
            Debug.Log("Initialized Success");

            //writeNewUser("hhhh", "chanon", "taforyou@hotmail.com");

        }

    }

    public Task writeNewUser(string userId, string name, string email)
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        return reference.Child("user").Child(userId).SetRawJsonValueAsync(json);

        
    }





}


