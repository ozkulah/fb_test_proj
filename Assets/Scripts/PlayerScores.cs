using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour
{
    public Text questionContent;
    public Text ans1Text;
    public Text ans2Text;
    public Text ans3Text;
    public Text ans4Text;
    public GameObject enterPanel;
    public GameObject questionPanel;
    public GameObject waitingPanel;
    public GameObject endGamePanel;

    private DatabaseReference mDatabaseRef;
    public GameRoom gameroom;
    internal string gameroomId;
    internal int currentQuestionId = -1;
    internal int maxQuestionIndex = 3;
    internal bool success = false;

    // Start is called before the first frame update
    void Start()
    {
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fb-testproject-90c4b.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        
    }

    public void fillQuestion(int questionId)
    {
        if (maxQuestionIndex >= questionId)
        {
            try
            {
                currentQuestionId = questionId;
                questionContent.text = gameroom.questions[questionId].que;
                Debug.Log("Question : " + gameroom.questions[questionId].que);
                ans1Text.text = gameroom.questions[questionId].qAns1;
                ans2Text.text = gameroom.questions[questionId].qAns2;
                ans3Text.text = gameroom.questions[questionId].qAns3;
                ans4Text.text = gameroom.questions[questionId].qAns4;
                enterPanel.SetActive(false);
                waitingPanel.SetActive(false);
                questionPanel.SetActive(true);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
        else
        {
            endGamePanel.SetActive(true);
            questionPanel.SetActive(false);
        }
    }

    public void submitAnswer(int key)
    {
        string playerId = gameroom.players[0].pId;
        string questionId = gameroom.questions[0].queId;
        string answer = "";
        switch (key)
        {
            case 1:
                answer = ans1Text.text;
                break;
            case 2:
                answer = ans2Text.text;
                break;
            case 3:
                answer = ans3Text.text;
                break;
            case 4:
                answer = ans4Text.text;
                break;
            default:
                break;
        }
        bool isTrue = false;
        if(gameroom.questions[currentQuestionId].qTrueAns == answer)
        {
            isTrue = true;
        }
        Answer ans = new Answer(playerId, questionId, answer, isTrue);
        string json = "";
        try
        {
            json = JsonConvert.SerializeObject(ans);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        try
        {
            DatabaseReference
                //Push creates a new unique key automatically for our level
                reference = mDatabaseRef.Child("gamerooms/" + gameroomId + "/answers/"+ currentQuestionId + "/" +Configuration.CurrentUser.userId);//Child("gamerooms").Child(gameroomId).Child("answers").Push();
            _ = UploadLevelAsync(reference, json, OnUploadLevelSuccess, OnUploadLevelFailed);
            //RestClient.Post("https://fb-testproject-90c4b.firebaseio.com/" + "gamerooms.json", ga);
            Debug.Log("Done");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }

        FirebaseDatabase.DefaultInstance.GetReference("gamerooms/" + gameroomId + "/answers/" + currentQuestionId).ChildAdded += OpponentPlayerAnswerWait;
        
    }

    private void OpponentPlayerAnswerWait(object sender, ChildChangedEventArgs e)
    {
        _ = mDatabaseRef.Child("gamerooms/" + gameroomId + "/answers/" + currentQuestionId)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  // Handle the error...
                  Debug.Log("Error reaching vals");
              }
              else if (task.IsCompleted)
              {
                  if (task.Result.ChildrenCount >= 2)
                  {
                      FirebaseDatabase.DefaultInstance.GetReference("gamerooms/" + gameroomId + "/answers/" + currentQuestionId).ChildAdded -= OpponentPlayerAnswerWait;
                      success = true;
                  }
              }       
          });
    }

    private void Update()
    {
        if (success)
        {
            fillQuestion(currentQuestionId + 1);
            success = false;
        }
    }

    public bool opponentCheck()
    {

        _ = mDatabaseRef.Child("gamerooms")
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  // Handle the error...
                  Debug.Log("Error reaching vals");
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot gamerooms = task.Result;
                  foreach (var gr in gamerooms.Children)
                  {
                      Debug.Log(gr.Key);
                      Debug.Log(gr.Child("players").ChildrenCount);
                      if (gr.Child("players").ChildrenCount < 2
                                && gr.Child("players/0/pId").Value.ToString() != Configuration.CurrentUser.userId)
                      {
                          gameroomId = gr.Key;
                          try
                          {
                              gameroom = JsonConvert.DeserializeObject<GameRoom>(gr.GetRawJsonValue());
                              Debug.Log(gameroom.questions[0].que);
                          }
                          catch (Exception ex)
                          {
                              Debug.Log(ex);
                          }
                          success = true;
                          Debug.Log("Find new opponent... : " + gameroomId);

                          startGame();
                          break;
                      }
                  }
              }
          });
        return success;
    }

    public void startGame()
    {
        Debug.Log("Start Game...");
        string json = "";
        try
        {
            json = JsonConvert.SerializeObject(new Player(Configuration.CurrentUser.userId, Configuration.CurrentUser.userName, ""));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        try
        {
            DatabaseReference
                //Push creates a new unique key automatically for our level
                reference = mDatabaseRef.Child("gamerooms/" + gameroomId + "/players/1");//Child("gamerooms").Child(gameroomId).Child("answers").Push();
            _ = UploadLevelAsync(reference, json, OnUploadLevelSuccess, OnUploadLevelFailed);
            Debug.Log("Done");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
        Debug.Log("End Game...");
    }

    public void enterGameRoom()
    {
        if (opponentCheck())
        {
            Debug.Log("Asyncron not coming to here...");
        }
        else
        {
            Debug.Log("Create room...");
            createGameRoom();
        }

    }

    public void createGameRoom()
    {
        string json;
        try
        {
            gameroom = MockGameRoom.createMockGameRoom();
            json = JsonConvert.SerializeObject(gameroom);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            json = "{'aa':4, 'bb':25}";
        }
        Debug.Log("Create new room");
        try
        {
            DatabaseReference
                //Push creates a new unique key automatically for our level
                reference = mDatabaseRef.Child("gamerooms").Push();
            Debug.Log(reference.Key);
            gameroomId = reference.Key;
            _ = UploadLevelAsync(reference, json, OnUploadLevelSuccess, OnUploadLevelFailed);
            //RestClient.Post("https://fb-testproject-90c4b.firebaseio.com/" + "gamerooms.json", ga);
            Debug.Log("Done");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
        enterPanel.SetActive(false);
        waitingPanel.SetActive(true);
        FirebaseDatabase.DefaultInstance.GetReference("gamerooms/"+gameroomId+"/players").ChildAdded += OpponentPlayerCome;
        Debug.Log("Child added");

    }

    

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
    }

    void OpponentPlayerCome(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("Opponent waiting...");
        //TODO first fill gameroom object for second opponent
        if (opponentCheck())
        {
            Debug.Log("Opponent come...");
            FirebaseDatabase.DefaultInstance.GetReference("gamerooms/" + gameroomId + "/players").ChildAdded -= OpponentPlayerCome;
        }
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        Debug.Log(args.Snapshot.Key);
        Debug.Log("Child changed");
    }

    public async Task UploadLevelAsync(DatabaseReference reference, string json, Action OnSuccess, Action<AggregateException> OnError)
    {
        Debug.Log("task UploadLevelAsync...");
        AggregateException exception = null;
        
        await reference.SetRawJsonValueAsync(json).ContinueWith(
            task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    exception = task.Exception;
                    Debug.Log("exce..");
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


    

}
