using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireBaseLogin : MonoBehaviour
{
    
    public static Firebase.Auth.FirebaseAuth auth;
    public static Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();
    
    private bool fetchingToken = false;
    private string logText = "";
    protected string email = "";
    protected string password = "";
    protected string displayName = "";
    protected bool signInAndFetchProfile = false;
    private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
    {
        ApiKey = "",
        AppId = "",
        ProjectId = ""
    };

    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public InputField Email, Password;

    public virtual void Start()
    {
     
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
     
    }

    protected void DisplayProfile<T>(IDictionary<T, object> profile, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        foreach (var kv in profile)
        {
            var valueDictionary = kv.Value as IDictionary<object, object>;
            if (valueDictionary != null)
            {
                DebugLog(String.Format("{0}{1}:", indent, kv.Key));
                DisplayProfile<object>(valueDictionary, indentLevel + 1);
            }
            else
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, kv.Key, kv.Value));
            }
        }
    }

    // Display user information reported
    protected void DisplaySignInResult(Firebase.Auth.SignInResult result, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        DisplayDetailedUserInfo(result.User, indentLevel);
        var metadata = result.Meta;
        if (metadata != null)
        {
            DebugLog(String.Format("{0}Created: {1}", indent, metadata.CreationTimestamp));
            DebugLog(String.Format("{0}Last Sign-in: {1}", indent, metadata.LastSignInTimestamp));
        }
        var info = result.Info;
        if (info != null)
        {
            DebugLog(String.Format("{0}Additional User Info:", indent));
            DebugLog(String.Format("{0}  User Name: {1}", indent, info.UserName));
            DebugLog(String.Format("{0}  Provider ID: {1}", indent, info.ProviderId));
            DisplayProfile<string>(info.Profile, indentLevel + 1);
        }
    }

    // Display user information.
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

    // Display a more detailed view of a FirebaseUser.
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


    // Track state changes of the auth object.
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
                DebugLog("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                DebugLog("AuthStateChanged Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                DisplayDetailedUserInfo(user, 1);
            }
        }
    }

    void OnDestroy()
    {
        Debug.Log("Destry...");
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth.IdTokenChanged -= IdTokenChanged;
            auth = null;
        }
        if (otherAuth != null)
        {
            otherAuth.StateChanged -= AuthStateChanged;
            otherAuth.IdTokenChanged -= IdTokenChanged;
            otherAuth = null;
        }
        //Firebase.FirebaseApp.DefaultInstance.Dispose();
    }

    public void DebugLog(string s)
    {
        Debug.Log(s);
        
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }

        //scrollViewVector.y = int.MaxValue;
        
    }
    // Track ID token changes.
    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
              task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }

    }

    protected void InitializeFirebase()
    {
        DebugLog("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
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
    }

    protected bool LogTaskCompletion(System.Threading.Tasks.Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugLog(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugLog(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                DebugLog(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugLog(operation + " completed");
            complete = true;
        }
        return complete;
    }

    // Create a user with the email and password.
    public System.Threading.Tasks.Task CreateUserWithEmailAsync()
    {
        DebugLog(String.Format("Attempting to create user {0}...", email));
        //DisableUI();

        // This passes the current displayName through to HandleCreateUserAsync
        // so that it can be passed to UpdateUserProfile().  displayName will be
        // reset by AuthStateChanged() when the new user is created and signed in.
        string newDisplayName = displayName;
        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
          .ContinueWithOnMainThread((task) => {
              //EnableUI();
              if (LogTaskCompletion(task, "User Creation"))
              {
                  var user = task.Result;
                  DisplayDetailedUserInfo(user, 1);
                  return UpdateUserProfileAsync(newDisplayName: newDisplayName);
              }
              return task;
          }).Unwrap();
    }

    public System.Threading.Tasks.Task UpdateUserProfileAsync(string newDisplayName = null)
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to update user profile");
            return System.Threading.Tasks.Task.FromResult(0);
        }
        displayName = newDisplayName ?? displayName;
        DebugLog("Updating user profile");
        //DisableUI();
        return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
        {
            DisplayName = displayName,
            PhotoUrl = auth.CurrentUser.PhotoUrl,
        }).ContinueWithOnMainThread(task => {
            //EnableUI();
            if (LogTaskCompletion(task, "User profile"))
            {
                DisplayDetailedUserInfo(auth.CurrentUser, 1);
            }
        });
    }

    // Sign-in with an email and password.
    public System.Threading.Tasks.Task SigninWithEmailAsync()
    {
        DebugLog(String.Format("Attempting to sign in as {0}...", email));
        //DisableUI();
        if (signInAndFetchProfile)
        {
            return auth.SignInAndRetrieveDataWithCredentialAsync(
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWithOnMainThread(
                HandleSignInWithSignInResult);
        }
        else
        {
            return auth.SignInWithEmailAndPasswordAsync(email, password)
              .ContinueWithOnMainThread(HandleSignInWithUser);
        }
    }

    public System.Threading.Tasks.Task SigninWithEmailCredentialAsync()
    {
        DebugLog(String.Format("Attempting to sign in as {0}...", email));
        //DisableUI();
        if (signInAndFetchProfile)
        {
            return auth.SignInAndRetrieveDataWithCredentialAsync(
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWithOnMainThread(
                HandleSignInWithSignInResult);
        }
        else
        {
            return auth.SignInWithCredentialAsync(
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWithOnMainThread(
                HandleSignInWithUser);
        }
    }

    // Sign out the current user.
    protected void SignOut()
    {
        DebugLog("Signing out.");
        auth.SignOut();
    }

    // Attempt to sign in anonymously.
    public System.Threading.Tasks.Task SigninAnonymouslyAsync()
    {
        DebugLog("Attempting to sign anonymously...");
        //DisableUI();
        return auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(HandleSignInWithUser);
    }

    public void AuthenticateToGameCenter()
    {
#if UNITY_IOS
        Social.localUser.Authenticate(success => {
            Debug.Log("Game Center Initialization Complete - Result: " + success);
        });
#else
        Debug.Log("Game Center is not supported on this platform.");
#endif
    }

    public System.Threading.Tasks.Task SignInWithGameCenterAsync()
    {
        var credentialTask = Firebase.Auth.GameCenterAuthProvider.GetCredentialAsync();
        var continueTask = credentialTask.ContinueWithOnMainThread(task => {
            if (!task.IsCompleted)
                return null;

            if (task.Exception != null)
                Debug.Log("GC Credential Task - Exception: " + task.Exception.Message);

            var credential = task.Result;

            var loginTask = auth.SignInWithCredentialAsync(credential);
            return loginTask.ContinueWithOnMainThread(HandleSignInWithUser);
        });

        return continueTask;
    }

    // Called when a sign-in without fetching profile data completes.
    void HandleSignInWithUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> task)
    {
        //EnableUI();
        if (LogTaskCompletion(task, "Sign-in"))
        {
            DebugLog(String.Format("{0} signed in", task.Result.DisplayName));
        }
    }

    // Called when a sign-in with profile data completes.
    void HandleSignInWithSignInResult(System.Threading.Tasks.Task<Firebase.Auth.SignInResult> task)
    {
        //EnableUI();
        if (LogTaskCompletion(task, "Sign-in"))
        {
            DisplaySignInResult(task.Result, 1);
        }
    }


    public void LoginButtonPressed()
    {
        email = Email.text;
        password = Password.text;
        SigninWithEmailAsync();
        Configuration.CurrentUser.userName = auth.CurrentUser.DisplayName;
        Configuration.CurrentUser.userEmail = auth.CurrentUser.Email;
        Configuration.CurrentUser.userId = auth.CurrentUser.UserId;
        SceneManager.LoadSceneAsync(1);
    }

    public void LoginGoogleButtonPressed()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential = null; //Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

    }

    public void LoginAnonymysButtonPressed()
    {
        SigninAnonymouslyAsync();
        Debug.Log("Anonym");
        SceneManager.LoadSceneAsync(1);
    }
    public void CreateNewUserButtonPressed()
    {
        email = Email.text;
        password = Password.text;
        displayName = "Hakkı";
        CreateUserWithEmailAsync();
        Debug.Log("create new user...");
        SceneManager.LoadSceneAsync(1);
    }

}
