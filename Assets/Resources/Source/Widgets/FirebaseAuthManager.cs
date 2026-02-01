using Firebase;
using Firebase.Auth;

using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Rendering;

public class FirebaseAuthManager : MonoBehaviour
{
    public static bool failedToAuth = false;

    public static FirebaseAuthManager Instance;

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

    async void Start()
    {
        await InitializeFirebase();
    }

    public async Task InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
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

            Debug.Log($"Logged in as: {user.Email}");

            if (Root.CDesktop.title == "LoggingIn")
            {
                var recordExists = await FirebaseDatabaseManager.RecordExistsAsync();
                Root.CloseDesktop("LoggingIn");
                if (recordExists) Root.SpawnDesktopBlueprint("MusicReleases");
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
}