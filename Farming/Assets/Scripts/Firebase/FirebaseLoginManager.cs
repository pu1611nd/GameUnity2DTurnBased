using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("Register")]
    public InputField ipRegisEmail;
    public InputField ipRegisPass;
    public Button buttonRegis;

    [Header("Login")]
    public InputField ipLoginEmail;
    public InputField ipLoginPass;
    public Button loginButton;

    [Header("Switch form")]
    public Button buttonMoveSignIn;
    public Button buttonMoveRegister;

    public GameObject LoginForm;
    public GameObject RegisterForm;

    [Header("Popup UI")]
    public GameObject popupPanel; // panel chứa thông báo
    public Text popupText;
    public float popupDuration = 2f;

    private FirebaseAuth auth;
    private FirebaseDataBaseManager dataBaseManager;


    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dataBaseManager = GetComponent<FirebaseDataBaseManager>();
        buttonRegis.onClick.AddListener(RegisterFirebase);
        loginButton.onClick.AddListener(SignInFirebase);
        buttonMoveSignIn.onClick.AddListener(() => ShowForm("Login"));
        buttonMoveRegister.onClick.AddListener(() => ShowForm("Register"));
    }

    // -------------------------------
    // 🔹 Đăng ký tài khoản mới
    // -------------------------------
    private void RegisterFirebase()
    {
        string email = ipRegisEmail.text.Trim();
        string pass = ipRegisPass.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            ShowPopup("⚠️ Email hoặc mật khẩu trống!");
            return;
        }

        buttonRegis.interactable = false;

        auth.CreateUserWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            buttonRegis.interactable = true;

            if (task.IsCanceled || task.IsFaulted)
            {
                HandleFirebaseRegisterError(task.Exception);
                return;
            }

            FirebaseUser firebaseUser = task.Result.User;
            Debug.Log("✅ Tạo tài khoản thành công: " + firebaseUser.Email);

            // Tạo user mặc định
            // Khi tạo người chơi mới:
            Farm UserFarm = new Farm();
            UserFarm.InitDefaultPlots(6);
            UserFarm.InitDefaultBarns();
            User newUser = new User(firebaseUser.UserId, "New Farmer", 100, 0, UserFarm, new UserItem(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            dataBaseManager.WriteDatabase("Users/" + firebaseUser.UserId, newUser);

            UserManager.Instance.userInGame = newUser;
            // Clear input
            ipRegisEmail.text = "";
            ipRegisPass.text = "";

            // Chuyển scene
            SceneManager.LoadScene("Loading");
        });
    }

    private void HandleFirebaseRegisterError(System.AggregateException exception)
    {
        if (exception != null)
        {
            foreach (var inner in exception.Flatten().InnerExceptions)
            {
                if (inner is FirebaseException firebaseEx)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    switch (errorCode)
                    {
                        case AuthError.EmailAlreadyInUse:
                            ShowPopup("⚠️ Email đã tồn tại!");
                            return;
                        case AuthError.InvalidEmail:
                            ShowPopup("⚠️ Email không hợp lệ!");
                            return;
                        case AuthError.WeakPassword:
                            ShowPopup("⚠️ Mật khẩu quá yếu!");
                            return;
                        default:
                            ShowPopup("❌ Đăng ký thất bại!");
                            return;
                    }
                }
            }
        }
        ShowPopup("❌ Đăng ký thất bại!");
    }

    // -------------------------------
    // 🔹 Đăng nhập tài khoản
    // -------------------------------
    private void SignInFirebase()
    {
        string email = ipLoginEmail.text.Trim();
        string pass = ipLoginPass.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            ShowPopup("⚠️ Email hoặc mật khẩu trống!");
            return;
        }

        loginButton.interactable = false;

        auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            loginButton.interactable = true;

            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser firebaseUser = task.Result.User;
                LoadDataManager.firebaseUser = firebaseUser;


                Debug.Log("✅ Đăng nhập thành công: " + firebaseUser.Email);
                // Clear input
                ipLoginEmail.text = "";
                ipLoginPass.text = "";

                SceneManager.LoadScene("Loading");
            }
            else
            {
                HandleFirebaseLoginError(task.Exception);
            }
        });
    }

    private void HandleFirebaseLoginError(System.AggregateException exception)
    {
        if (exception != null)
        {
            foreach (var inner in exception.Flatten().InnerExceptions)
            {
                if (inner is FirebaseException firebaseEx)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    switch (errorCode)
                    {
                        case AuthError.InvalidEmail:
                            ShowPopup("⚠️ Email không hợp lệ!");
                            return;
                        case AuthError.WrongPassword:
                            ShowPopup("⚠️ Sai mật khẩu!");
                            return;
                        case AuthError.UserNotFound:
                            ShowPopup("⚠️ Tài khoản không tồn tại!");
                            return;
                        default:
                            ShowPopup("❌ Đăng nhập thất bại!");
                            return;
                    }
                }
            }
        }
        ShowPopup("❌ Đăng nhập thất bại!");
    }

    // -------------------------------
    // 🔹 Chuyển Form
    // -------------------------------
    private void ShowForm(string form)
    {
        LoginForm.SetActive(form == "Login");
        RegisterForm.SetActive(form == "Register");
    }

    // -------------------------------
    // 🔹 Hiện popup UI
    // -------------------------------
    private void ShowPopup(string message)
    {
        if (popupPanel != null && popupText != null)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
            CancelInvoke(nameof(HidePopup));
            Invoke(nameof(HidePopup), popupDuration);
        }

        Debug.Log($"📢 {message}");
    }

    private void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

}
