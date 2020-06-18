using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour
{
    public Text loginInfo;
    public Text scoreText;
    public InputField nameText;
    private System.Random random = new System.Random();

    public static int playerScore;
    public static string playerName;
    private DatabaseReference mDatabaseRef;

    // Start is called before the first frame update
    void Start()
    {
        /*
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fb-testproject-90c4b.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        playerScore = random.Next(0, 101);
        scoreText.text = "Score: " + playerScore;
        */

        loginInfo.text = Configuration.CurrentUser.userEmail + "\n" + Configuration.CurrentUser.userId
            + "\n" + Configuration.CurrentUser.userName;
    }

    public void onSubmit()
    {
        playerName = nameText.text;
        postToDatabase();
    }

    public async Task UploadLevelAsync(User user, Action OnSuccess, Action<AggregateException> OnError)
    {
        Debug.Log("task op");
        string json = JsonConvert.SerializeObject(user);
        AggregateException exception = null;
        //Push creates a new unique key automatically for our level
        await mDatabaseRef.Child("users").Push().SetRawJsonValueAsync(json).ContinueWith(
            task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    exception = task.Exception;
                }
            }
            );
        if (exception != null)
        {
            OnError(exception);
        }
        else
        {
            OnSuccess();
        }
    }
    void OnUploadLevelSuccess()
    {
        Debug.Log("Upload success");
    }

    void OnUploadLevelFailed(AggregateException exception)
    {
        Debug.LogError(exception.ToString());
    }
    public void postToDatabase()
    {
        User user = new User();
        Debug.Log(user.userName + ": " + user.userScore);
        try
        {
            //_ = UploadLevelAsync(user, OnUploadLevelSuccess, OnUploadLevelFailed);
            RestClient.Post("https://fb-testproject-90c4b.firebaseio.com/" + playerName + ".json", user);
            Debug.Log("Done");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
        
        
    }

}
