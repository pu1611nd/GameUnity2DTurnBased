using System.Collections;
using System.Collections.Generic;
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

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        buttonRegis.onClick.AddListener(RegisterFirebase);
        loginButton.onClick.AddListener(SignInFirebase);
        buttonMoveSignIn.onClick.AddListener(SwitchForm);
        buttonMoveRegister.onClick.AddListener(SwitchForm);

    }

    private void RegisterFirebase()
    {
        string email = ipRegisEmail.text;
        string pass = ipRegisPass.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("tao thanh cong");
                return;
            }
            if (task.IsCanceled)
            {
                Debug.Log("dang ky bi huy" + task.Exception);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("dang ky that bai" + task.Exception);
                return;

            }
        });

    }

    private void SignInFirebase()
    {
        string email = ipLoginEmail.text;
        string pass = ipLoginPass.text;
        auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Đăng nhập thành công");
                FirebaseUser user = task.Result.User;

                // Kiểm tra xem scene có tên đúng không
                if (Application.CanStreamedLevelBeLoaded("PlayScene"))
                {
                    SceneManager.LoadScene("PlayScene");
                }
                else
                {
                    Debug.LogError("Tên scene không chính xác hoặc scene chưa được thêm vào Build Settings.");
                }
            }
            if (task.IsCanceled)
            {
                Debug.Log("dang nhap bi huy" + task.Exception);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("dang nhap that bai" + task.Exception);
                return;

            }
        });

    }

    public void SwitchForm()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);
    }

}
