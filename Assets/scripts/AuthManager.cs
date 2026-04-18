using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using System.Collections;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    [Header("Login")]
    public TMP_InputField loginEmail, loginPassword;
    public TMP_Text loginError;

    [Header("Register")]
    public TMP_InputField regEmail, regPassword, regUsername;
    public TMP_Text regError;

    FirebaseAuth auth;
    bool firebaseReady = false;

    void Awake()
    {
        if (instance == null) 
        { 
            instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        StartCoroutine(InitFirebase());
    }

    IEnumerator InitFirebase()
    {
        Debug.Log("Initializing Firebase...");
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        float timeout = 10f;
        while (!dependencyTask.IsCompleted && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (!dependencyTask.IsCompleted)
        {
            Debug.LogWarning("Firebase timed out.");
            yield break;
        }

        if (dependencyTask.Result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            firebaseReady = true;
            Debug.Log("Firebase ready!");
        }
        else
        {
            Debug.LogError("Firebase failed: " + dependencyTask.Result);
        }
    }

    public void LoginButton() => StartCoroutine(Login());
    public void RegisterButton() => StartCoroutine(Register());

    IEnumerator Login()
    {
        Debug.Log("Login clicked. Firebase ready: " + firebaseReady);
        if (!firebaseReady) { loginError.text = "Not ready."; yield break; }
        loginError.text = "Logging in...";
        var task = auth.SignInWithEmailAndPasswordAsync(loginEmail.text, loginPassword.text);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogError("Login error: " + task.Exception.Message);
            loginError.text = "Login failed.";
        }
        else
        {
            Debug.Log("Login success!");
            SceneManager.LoadScene("SampleScene");
        }
    }

    IEnumerator Register()
    {
        Debug.Log("Register clicked. Firebase ready: " + firebaseReady);
        if (!firebaseReady) { regError.text = "Not ready."; yield break; }
        if (regUsername.text.Length < 2) { regError.text = "Username too short."; yield break; }
        regError.text = "Registering...";
        var task = auth.CreateUserWithEmailAndPasswordAsync(regEmail.text, regPassword.text);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogError("Register error: " + task.Exception.Message);
            regError.text = "Registration failed.";
        }
        else
        {
            Debug.Log("Register success! UID: " + auth.CurrentUser.UserId);
            LeaderboardManager.instance.InitUser(auth.CurrentUser.UserId, regUsername.text);
            SceneManager.LoadScene("SampleScene");
        }
    }

    public string GetUserId() => auth?.CurrentUser?.UserId;
}