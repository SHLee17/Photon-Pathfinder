using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    public bool isFireBaseReady { get; private set; }
    public bool isSignInOnProgress { get; private set; }

    [Header("Input Fields")]
    public InputField emailField;
    public InputField passwordField;
    public InputField userNameField;

    [Header("Texts")]
    public Text txtWarning;
    public Text txtEmail;

    [Header("Interaction Button")]
    public Text txtInteraction;
    public Button interactionButton;

    [Header("Cancel Button")]
    public Button cancelButton;

    [Header("Lobby")]
    public GameObject lobby;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;
    public static FirebaseUser user;

    [Header("etc")]
    public string tempUserName;
    public GameObject idScrollView;
    public List<Button> idButtonList;


    List<string> idList;

    //bool isCheckName { get { return Regex.IsMatch(userNameField.text, "^[a-zA-Z가-힣]+[0-9]{2,8}$"); } }

    private void Awake()
    {
        Screen.SetResolution(1280, 960, false);
    }


    private void Start()
    {
        idList = new List<string>();
        emailField.Select();
        interactionButton.interactable = false;
        lobby.SetActive(false);
        txtEmail.gameObject.SetActive(false);
        userNameField.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        idScrollView.gameObject.SetActive(false);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus result = task.Result;

            if (result != DependencyStatus.Available)
            {
                Debug.LogError(message: result);
                isFireBaseReady = false;
            }
            else
            {
                isFireBaseReady = true;
                firebaseApp = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            interactionButton.interactable = isFireBaseReady;
        });

        idList.Add("azurite17@gmail.com");
        idList.Add("op_en2@naver.com");

        for (int i = 0; i < idList.Count; i++)
            idButtonList[i].GetComponentInChildren<Text>().text = idList[i];

        idButtonList[0].onClick.AddListener(() => { IDSelect(0); });
        idButtonList[1].onClick.AddListener(() => { IDSelect(1); });

        emailField.text = idList[0];
        passwordField.text = "123123";
    }

    void IDSelect(int i) 
    {
        emailField.text = idButtonList[i].GetComponentInChildren<Text>().text;

        idScrollView.gameObject.SetActive(false);
    }



    private void Update()
    {
        if (emailField.isFocused)
            idScrollView.gameObject.SetActive(true);
        //else
        //    idScrollView.gameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (emailField.isFocused)
                passwordField.Select();
            else if (passwordField.isFocused)
                interactionButton.Select();
            else if (interactionButton.enabled)
                emailField.Select();
            else
                emailField.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (interactionButton.interactable)
                LogIn();
        }

        if (user != null)
        {

            txtEmail.gameObject.SetActive(true);
            txtEmail.text = user.Email;
            emailField.gameObject.SetActive(false);
            passwordField.gameObject.SetActive(false);

            if (string.IsNullOrEmpty(tempUserName))
            {
                lobby.SetActive(false);
                userNameField.gameObject.SetActive(true);
                userNameField.Select();
                txtInteraction.text = "Enter";
            }
            else
            {
                lobby.SetActive(true);
                //Debug.Log(user.DisplayName);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                string name = userNameField.text;

                if (Regex.IsMatch(name, "^[A-Za-z가-힣]+[0-9]{2,8}$"))
                {
                    UserProfile profile = new UserProfile
                    {
                        DisplayName = name,
                    };

                    Debug.Log("체크");
                    lobby.SetActive(true);

                    user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                            return;
                        if (task.IsFaulted)
                            return;
                        Debug.Log(userNameField.text + "성공");
                        tempUserName = user.DisplayName;
                        if (cancelButton.gameObject.activeSelf)
                            cancelButton.gameObject.SetActive(false);
                    });
                }
                else
                {
                    Debug.Log("2글자 이상 8글자 이하만 허용됩니다.");
                    txtWarning.gameObject.SetActive(true);
                    txtWarning.text = "2글자 이상 8글자 이하만 허용됩니다.";
                }
            }
        }
    }
    public void ChangeName()
    {
        tempUserName = "";
        cancelButton.gameObject.SetActive(true);
    }
    public void CancelChangeName() => tempUserName = user.DisplayName;

  
    public void LogIn()
    {
        if (!isFireBaseReady || isSignInOnProgress)
            return;

        if (user == null)
        {
            isSignInOnProgress = true;
            interactionButton.interactable = false;

            //firebaseAuth.CreateUserWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWithOnMainThread()

            firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWithOnMainThread(
                continuation: task =>
                 {
                     Debug.Log(message: $"Sign in Status : {task.Status}");

                     isSignInOnProgress = false;
                     interactionButton.interactable = true;

                     if (task.IsFaulted)
                         Debug.LogError(task.Exception);
                     else if (task.IsCanceled)
                         Debug.LogError(message: "Sign-in canceled");
                     else
                     {
                         user = task.Result;
                         tempUserName = user.DisplayName;
                         Debug.Log(user.Email);
                         Debug.Log("성공");
                     }
                 });
        }
        //else
        //    NameCreate();
    }


}
