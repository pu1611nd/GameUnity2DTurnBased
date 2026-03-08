using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;

/// <summary>
/// Quản lý dữ liệu người chơi hiện tại trong game.
/// - Giữ lại xuyên suốt giữa các scene.
/// - Dễ dàng truy cập qua UserManager.Instance.
/// - Hỗ trợ lưu / tải lại dữ liệu từ Firebase.
/// </summary>
public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }

    [Header("User Data")]
    public User userInGame; // dữ liệu người chơi hiện tại

    private FirebaseAuth auth;
    private FirebaseUser firebaseUser;
    private FirebaseDataBaseManager firebaseData;

    private void Awake()
    {
        // ✅ Đảm bảo singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        auth = FirebaseAuth.DefaultInstance;
        firebaseUser = auth.CurrentUser;

        // Tìm FirebaseDataBaseManager trong scene (nếu có)
        firebaseData = FindObjectOfType<FirebaseDataBaseManager>();
    }

    private void Start()
    {
        if (firebaseUser == null)
        {
            Debug.LogWarning("⚠️ Chưa có người dùng đăng nhập.");
            return;
        }

        Debug.Log($"👤 UserManager khởi tạo cho user: {firebaseUser.UserId}");
    }

    /// <summary>
    /// Lưu dữ liệu người chơi hiện tại lên Firebase.
    /// </summary>
    public void SaveUserData()
    {
        if (firebaseData == null)
        {
            firebaseData = FindObjectOfType<FirebaseDataBaseManager>();
            if (firebaseData == null)
            {
                Debug.LogError("❌ Không tìm thấy FirebaseDataBaseManager — không thể lưu dữ liệu!");
                return;
            }
        }

        if (firebaseUser == null)
        {
            Debug.LogError("❌ Không có người dùng đăng nhập — không thể lưu!");
            return;
        }

        if (userInGame == null)
        {
            Debug.LogError("❌ userInGame null — không có dữ liệu để lưu!");
            return;
        }

        string path = $"Users/{firebaseUser.UserId}";
        firebaseData.WriteDatabase(path, userInGame);

        Debug.Log($"💾 Dữ liệu người chơi [{userInGame.Name}] đã được lưu lên Firebase!");
    }

    /// <summary>
    /// Tải lại dữ liệu từ Firebase (thường dùng khi reload scene).
    /// </summary>
    public void ReloadUserData(Action onComplete = null)
    {
        if (firebaseData == null || firebaseUser == null)
        {
            Debug.LogError("❌ Không thể tải lại — firebaseData hoặc firebaseUser null.");
            return;
        }

        string path = $"Users/{firebaseUser.UserId}";
        firebaseData.ReadObject<User>(path, user =>
        {
            if (user != null)
            {
                userInGame = user;
                Debug.Log($"🔄 Dữ liệu người chơi [{userInGame.Name}] đã được tải lại.");
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy dữ liệu người chơi trên Firebase!");
            }
        });
    }

    /// <summary>
    /// Reset dữ liệu người chơi về mặc định (chỉ dùng khi debug/test).
    /// </summary>
    public void ResetUserData()
    {
        if (firebaseUser == null) return;

        userInGame = new User();
        SaveUserData();
        Debug.Log("🔁 Đã reset dữ liệu người chơi về mặc định.");
    }

    private void OnApplicationQuit()
    {
        SaveUserData();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SaveUserData();
        }
    }
}
