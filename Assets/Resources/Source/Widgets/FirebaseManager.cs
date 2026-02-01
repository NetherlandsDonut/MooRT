using Firebase;
using Firebase.Auth;
using Firebase.Database;

using System.Threading.Tasks;

using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static bool failedToAuth = false;

    public static FirebaseManager Instance;
    public static DatabaseReference dbRef;

    public FirebaseAuth auth;
    public FirebaseUser user;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeFirebase();
    }

    public async void InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase Auth initialized");
            if (Root.CDesktop != null && Root.CDesktop.title == "RetryingAuth")
            {
                Root.CloseDesktop("RetryingAuth");
                Root.SpawnDesktopBlueprint("SuccessfulAuth");
            }
        }
        else
        {
            failedToAuth = true;
            Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            if (Root.CDesktop != null && Root.CDesktop.title == "RetryingAuth")
            {
                Root.CloseDesktop("RetryingAuth");
                Root.SpawnDesktopBlueprint("FailedToAuth");
            }
        }
    }

    public async void Login(string email, string password)
    {
        PlayerPrefs.SetString("SavedEmail", email);
        PlayerPrefs.SetString("SavedPassword", password);
        try
        {
            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            await loginTask;

            user = loginTask.Result.User;

            Debug.Log($"Logged in as: {user.Email} {user.UserId}");

            if (Root.CDesktop.title == "LoggingIn")
            {
                var recordExists = await RecordExistsAsync();
                Root.CloseDesktop("LoggingIn");
                if (recordExists)
                {
                    if (await DownloadUserData())
                        Root.SpawnDesktopBlueprint("MusicReleases");
                    else
                    {
                        if (Root.CDesktop.title == "LoggingIn")
                        {
                            Root.CloseDesktop("LoggingIn");
                            Root.SpawnDesktopBlueprint("FailedToLogin");
                        }
                    }
                }
                else Root.SpawnDesktopBlueprint("AccountInitialisation");
            }
        }
        catch (System.Exception e)
        {
            FirebaseException firebaseEx = e as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            Debug.LogError($"Login failed: {errorCode}");

            if (Root.CDesktop.title == "LoggingIn")
            {
                Root.CloseDesktop("LoggingIn");
                Root.SpawnDesktopBlueprint("FailedToLogin");
            }
        }
    }

    public static async Task<bool> DownloadUserData()
    {
        try
        {
            if (await Serialization.DownloadAccountData())
                return true;
            else
                return false;
        }
        catch (System.Exception e)
        {
            Debug.Log("Firebase read error: " + e);
            return false;
        }
    }

    public static async Task<bool> RecordExistsAsync()
    {
        try
        {
            DataSnapshot snapshot = await dbRef.Child("users").Child(FirebaseManager.Instance.user.UserId).GetValueAsync();
            return snapshot.Exists;
        }
        catch (System.Exception e)
        {
            Debug.Log("Firebase read error: " + e);
            return false;
        }
    }
}