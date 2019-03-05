

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;


public class FireBaseController : MonoBehaviour
{

    
    protected FirebaseAuth auth;
    protected FirebaseAuth otherAuth;
    protected Dictionary<string, FirebaseUser> userByAuth =
    new Dictionary<string, FirebaseUser>();

    protected bool signInAndFetchProfile = false;
    private bool fetchingToken = false;

    public bool usePasswordInput = false;

    private uint phoneAuthTimeoutMs = 60 * 1000;
    private string phoneAuthVerificationId;
    

    public string logText = "";
    protected string email = "";
    protected string password = "";
    protected string displayName = "";
    protected string phoneNumber = "";
    protected string receivedCode = "";
    protected bool detailsSet=false;
    protected bool dataSet = false;
    protected UserDetail userDetail;
    protected List<Sessions> sessions;

    private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
    {
        ApiKey = "",
        AppId = "",
        ProjectId = ""
    };

    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;




    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("FireBaseObject");
        if (objs.Length > 1) { Destroy(this.gameObject); }
        else { DontDestroyOnLoad(this.gameObject); }



        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("dependencies ok");
                InitializeFirebase();
                InitializeDatabase();
               
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

        sessions = new List<Sessions>();
    }



    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.



    void Start()
      {


        userDetail = new UserDetail(0,0,0);
          
      }




    public void InitializeDatabase()
    {
        Debug.Log("Initializing database ");
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://vrcycling-6cab8.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
        {

            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
            Debug.Log("Initialized database Success");

            //writeNewUser("hhhh", "chanon", "taforyou@hotmail.com");
            StartDataChangeListener();
        }

    }


    public Task SetRawJsonValueAsync(string userID,object obj)
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        
        string json = JsonUtility.ToJson(obj);

        return reference.Child("user").Child(userID).SetRawJsonValueAsync(json)
        .ContinueWith((task) => {

            if (LogTaskCompletion(task, "Save Session"))
            {
                //var user = task.Result;
                //DisplayDetailedUserInfo(user, 1);
                // return UpdateUserProfileAsync(newDisplayName: newDisplayName);
            }
            return task;
        }).Unwrap(); 


    }

    public Task SaveSessionAsync(float distanceKM, float timeMins, float calories)
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        string nowTime = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
        

        string json = JsonUtility.ToJson(new Session(distanceKM,timeMins,calories));

        string[] userName = auth.CurrentUser.Email.Split('.');

        return reference.Child(userName[0]).Child("Sessions").Child(nowTime).SetRawJsonValueAsync(json)
        .ContinueWith((task) => {

            LogTaskCompletion(task, "Save Session");
  
            
            return task;
        }).Unwrap();


    }


    public Task SaveUserDetailsAsync(float height, float weight, float age)
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        


        string json = JsonUtility.ToJson(new UserDetail(height, weight, age));

        string[] userName = auth.CurrentUser.Email.Split('.');

        return reference.Child(userName[0]).Child("Details").SetRawJsonValueAsync(json)
        .ContinueWith((task) => {

            LogTaskCompletion(task, "Save Details");

            if(logText.Equals("Save Details Completed"))
            userDetail = new UserDetail(height, weight, age);


            return task;
        }).Unwrap();


    }

    public void StartDataChangeListener()
    {

        string[] userName = auth.CurrentUser.Email.Split('.');
        Debug.Log("Started listner");
        FirebaseDatabase.DefaultInstance
          .GetReference(userName[0]+"/Details")
          .ValueChanged += HandleValueChanged; 
    }

    void HandleValueChanged(object sender2, ValueChangedEventArgs e2)  {
        detailsSet = false;
              if (e2.DatabaseError != null)
              {
                  Debug.LogError(e2.DatabaseError.Message);
                  return;
              }
                      Debug.Log("Received values for Details");
              

              if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
              {

           // Debug.Log("got data");
            //      foreach (var childSnapshot in e2.Snapshot.Children)
            //      {
            //    Debug.Log("In loop");


                var snap = e2.Snapshot;
            //     Debug.Log("cal:" + childSnapshot.Child("Calories").Value.ToString());
            //  Debug.Log("weig:" + childSnapshot.Child("Weight").Value.ToString());
            //  Debug.Log("age:" + childSnapshot.Child("Age").Value.ToString());
            userDetail.Height = float.Parse(snap.Child("Height").Value.ToString());
            userDetail.Weight = float.Parse(snap.Child("Weight").Value.ToString());
            userDetail.Age = float.Parse(snap.Child("Age").Value.ToString());
            detailsSet = true;
            //    }
        }
          }

    public bool detailsIsLoaded() {
        return detailsSet;
    }

    public Task loadUserDetailsAsync() {

     string[] userName = auth.CurrentUser.Email.Split('.');


      return  FirebaseDatabase.DefaultInstance
      .GetReference(userName[0]+"/Details")
      .GetValueAsync().ContinueWith(task => {


          if (task.IsFaulted)
          {
              Debug.Log("user detail load failed");
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snap = task.Result;
              // Do something with snapshot...
              userDetail.Height = float.Parse(snap.Child("Height").Value.ToString());
              userDetail.Weight = float.Parse(snap.Child("Weight").Value.ToString());
              userDetail.Age    = float.Parse(snap.Child("Age").Value.ToString());
              Debug.Log("height:"+userDetail.Height);
              Debug.Log("weight:" + userDetail.Height);
              Debug.Log("age:" + userDetail.Height);
          }
          return task;
      }).Unwrap();

    }






    public UserDetail  getUserDetails( )
    {
        return userDetail;

    }

    public Task loadUserDataAsync()
    {
        dataSet = false;
        sessions.Clear();
        string[] userName = auth.CurrentUser.Email.Split('.');

        Debug.Log("Loading data"); 
        return FirebaseDatabase.DefaultInstance
        .GetReference(userName[0] + "/Sessions")
        .GetValueAsync().ContinueWith(task => {


            if (task.IsFaulted)
            {
                Debug.Log("user detail load failed");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;
                if (snap != null && snap.ChildrenCount > 0)
                {

                    Debug.Log(snap.ChildrenCount);
                    foreach (var ses in snap.Children)
                    {

                        Debug.Log(ses.Key);
                        Sessions sess = new Sessions(
                                             float.Parse(ses.Child("DistanceKM").Value.ToString()),
                                             float.Parse(ses.Child("TimeMins").Value.ToString()),
                                             float.Parse(ses.Child("Calories").Value.ToString()),
                                             ses.Key

                                            );
                        sessions.Add(sess);
                        Debug.Log("Got Data:"+sess.DistanceKM);
                    }
                }
                dataSet = true;
          }
            return task;
        }).Unwrap();

    }






    public List<Sessions> getUserData()
    {


        return sessions;

    }

    public bool dataIsLoaded()
    {
        return dataSet;
    }


    protected void InitializeFirebase()
    {
        DebugLog("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
         auth.StateChanged += AuthStateChanged;
         auth.IdTokenChanged += IdTokenChanged;
        // Specify valid options to construct a secondary authentication object.
        if (otherAuthOptions != null &&
            !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
              String.IsNullOrEmpty(otherAuthOptions.AppId) ||
              String.IsNullOrEmpty(otherAuthOptions.ProjectId)))
        {

            try
            {
                DebugLog("using secondary options");
                otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
                  otherAuthOptions, "Secondary"));
                otherAuth.StateChanged += AuthStateChanged;
                otherAuth.IdTokenChanged += IdTokenChanged;
            }
            catch (Exception)
            {
                DebugLog("ERROR: Failed to initialize secondary authentication object.");
            }
        }
        AuthStateChanged(this, null);


        Debug.Log("Setting up Firebase Auth complete");



    }


    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
               // DebugLog("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                // DebugLog("Signed in " + user.UserId);
                // displayName = user.DisplayName ?? "";
                //DisplayDetailedUserInfo(user, 1);
            }
        }
    }

    // Track ID token changes.
    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
          //  senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
           //   task =>  DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {

        bool complete = false;

        try
        {
            if (task.IsCanceled)
            {
                DebugLog(operation + " canceled.");
            }
            else if (task.IsFaulted)
            {
                //DebugLog(operation + " encounted an error.");
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    string authErrorCode = "";
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        authErrorCode = String.Format("Error: {0}",
                          ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                    }
                    DebugLog(authErrorCode); //+ exception.ToString());
                    Debug.Log(authErrorCode + exception.ToString());
                }
            }
            else if (task.IsCompleted)
            {
                DebugLog(operation + " Completed");
                complete = true;
            }

        }
        catch (System.Exception) { }
        return complete;
    }


    public void DebugLog(string s)
    {
        logText += s ;
        Debug.Log(s);
        // logText += s + "\n";

        //  while (logText.Length > kMaxLogSize)
        //   {
        //      int index = logText.IndexOf("\n");
        //      logText = logText.Substring(index + 1);
        //   }

    }


    protected void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        DisplayUserInfo(user, indentLevel);
        DebugLog(String.Format("{0}Anonymous: {1}", indent, user.IsAnonymous));
        DebugLog(String.Format("{0}Email Verified: {1}", indent, user.IsEmailVerified));
        DebugLog(String.Format("{0}Phone Number: {1}", indent, user.PhoneNumber));
        var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
        var numberOfProviders = providerDataList.Count;
        if (numberOfProviders > 0)
        {
            for (int i = 0; i < numberOfProviders; ++i)
            {
                DebugLog(String.Format("{0}Provider Data: {1}", indent, i));
                DisplayUserInfo(providerDataList[i], indentLevel + 2);
            }
        }
    }


    protected void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
        {"Display Name", userInfo.DisplayName},
        {"Email", userInfo.Email},
        {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
        {"Provider ID", userInfo.ProviderId},
        {"User ID", userInfo.UserId}
      };
         foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }

        
    }


    // Create a user with the email and password.
    public Task CreateUserWithEmailAsync(string emai, string pass)
    {
        email = emai;
        password = pass;

        
        //DebugLog(String.Format("Attempting to create user {0}...", email));
        //DisableUI();

        // This passes the current displayName through to HandleCreateUserAsync
        // so that it can be passed to UpdateUserProfile().  displayName will be
        // reset by AuthStateChanged() when the new user is created and signed in.
        // string newDisplayName = displayName;
        string newDisplayName = displayName;
        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
          .ContinueWith((task) => {

              if (LogTaskCompletion(task, "User Creation"))
              {
                  //var user = task.Result;
                  //DisplayDetailedUserInfo(user, 1);
                 // return UpdateUserProfileAsync(newDisplayName: newDisplayName);
              }
              return task;
          }).Unwrap();
    }

    public Task SigninWithEmailCredentialAsync(string emai,string pass)
    {
        //DebugLog(String.Format("Attempting to sign in as {0}...", email));


        return auth.SignInWithCredentialAsync(
          Firebase.Auth.EmailAuthProvider.GetCredential(emai, pass)).ContinueWith(task =>
            LogTaskCompletion(task, "Sign in")

               

                
                );

  
                
                  
    }
   
    public Task UpdateUserProfileAsync(string newDisplayName = null)
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to update user profile");
            return Task.FromResult(0);
        }
        displayName = newDisplayName ?? displayName;
        DebugLog("Updating user profile");

        return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
        {
            DisplayName = displayName,
            PhotoUrl = auth.CurrentUser.PhotoUrl,
        }).ContinueWith(task => {

            if (LogTaskCompletion(task, "User profile"))
            {
                DisplayDetailedUserInfo(auth.CurrentUser, 1);
            }
        });
    }


    public void SignOut()
    {
        DebugLog("Signing out.");
        auth.SignOut();
        new changeScenes().goToLogin();
    }


    public string getCurrUserEmail() {

        return auth.CurrentUser.Email;
    }

   
}



public class Session
{
    public float DistanceKM;
    public float TimeMins;
    public float Calories;


    public Session(float distanceKM, float timeMins, float calories)
    {
        DistanceKM = distanceKM;
        TimeMins = timeMins;
        Calories = calories;
    }

}

public class Sessions
{
    public float DistanceKM;
    public float TimeMins;
    public float Calories;
    public string key;

    public Sessions(float distanceKM, float timeMins, float calories,string key)
    {
        DistanceKM = distanceKM;
        TimeMins = timeMins;
        Calories = calories;
        this.key = key;
    }

}

public class UserDetail
{
    public float Height;
    public float Weight;
    public float Age;


    public UserDetail(float height, float weight, float age)
    {
        Height = height;
        Weight = weight;
        Age = age;
    }

}
public class User
    {
        public string username;
        public string email;

        public User()
        {
        }

        public User(string username, string email)
        {
            this.username = username;
            this.email = email;
        }

    }

