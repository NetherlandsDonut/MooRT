using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
    public static DatabaseReference dbRef;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized");
            }
            else
            {
                Debug.LogError("Firebase dependencies not resolved: " + task.Result);
            }
        });
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
            DataSnapshot snapshot = await dbRef.Child("users").Child(FirebaseAuthManager.Instance.user.UserId).GetValueAsync();
            return snapshot.Exists;
        }
        catch (System.Exception e)
        {
            Debug.Log("Firebase read error: " + e);
            return false;
        }
    }
}