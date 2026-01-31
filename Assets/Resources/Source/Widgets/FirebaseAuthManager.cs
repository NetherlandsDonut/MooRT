using Firebase;
using Firebase.Auth;

using System.Threading.Tasks;

using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager Instance;

    private FirebaseAuth auth;
    private FirebaseUser user;

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

    async Task InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Firebase Auth initialized");
            Login("netherlands.donut@gmail.com", "Hase³ko2");
        }
        else
        {
            Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
        }
    }

    public async void Login(string email, string password)
    {
        try
        {
            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            await loginTask;

            user = loginTask.Result.User;

            Debug.Log($"Logged in as: {user.Email}");
        }
        catch (System.Exception e)
        {
            FirebaseException firebaseEx = e as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            Debug.LogError($"Login failed: {errorCode}");
        }
    }
}