using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Quản lý đọc/ghi dữ liệu từ Firebase Realtime Database.
/// Hỗ trợ: WriteDatabase, ReadDatabase, ReadObject, ReadList, DeleteData.
/// </summary>
public class FirebaseDataBaseManager : MonoBehaviour
{
    public static FirebaseDataBaseManager Instance { get; private set; }

    private DatabaseReference reference;

    private void Awake()
    {
        // ✅ Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ✅ Khởi tạo Firebase
        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log("✅ FirebaseDataBaseManager đã khởi tạo.");
    }

    // ======================================================
    // 🔹 Ghi dữ liệu object vào Firebase (object -> JSON)
    // ======================================================
    public void WriteDatabase(string path, object data, Action<bool> onComplete = null)
    {
        if (!CheckReference(onComplete)) return;

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        reference.Child(path).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"✅ Đã ghi dữ liệu vào: {path}");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"❌ Lỗi ghi dữ liệu ({path}): {task.Exception}");
                onComplete?.Invoke(false);
            }
        });
    }

    // ======================================================
    // 🔹 Đọc dữ liệu snapshot từ Firebase
    // ======================================================
    public void ReadDatabase(string path, Action<DataSnapshot> onSuccess, Action onNotFound = null)
    {
        if (!CheckReference(null)) return;

        reference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists) onSuccess?.Invoke(snapshot);
                else
                {
                    Debug.LogWarning($"⚠️ Không tìm thấy dữ liệu tại {path}");
                    onNotFound?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"❌ Lỗi đọc dữ liệu ({path}): {task.Exception}");
            }
        });
    }

    // ======================================================
    // 🔹 Đọc dữ liệu & parse thẳng về object T
    // ======================================================
    public void ReadObject<T>(
        string path,
        Action<T> onSuccess,
        Action onNotFound = null,
        Action<Exception> onError = null)
    {
        if (!CheckReference(null)) { onError?.Invoke(new NullReferenceException("Firebase reference null")); return; }

        reference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                HandleError(path, task.Exception, onError);
                return;
            }

            if (!task.IsCompletedSuccessfully)
            {
                HandleError(path, task.Exception, onError, "Nhiệm vụ đọc Firebase không hoàn tất");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                Debug.LogWarning($"⚠️ Không có dữ liệu tại {path}");
                onNotFound?.Invoke();
                return;
            }

            string json = snapshot.GetRawJsonValue();
            if (string.IsNullOrEmpty(json))
            {
                HandleError(path, new Exception("JSON rỗng"), onError);
                return;
            }

            try
            {
                T obj = JsonConvert.DeserializeObject<T>(json);
                onSuccess?.Invoke(obj);
            }
            catch (Exception ex)
            {
                HandleError(path, ex, onError, $"Lỗi parse JSON: {json}");
            }
        });
    }

    // ======================================================
    // 🔹 Xóa dữ liệu Firebase
    // ======================================================
    public void DeleteData(string path, Action<bool> onComplete = null)
    {
        if (!CheckReference(onComplete)) return;

        reference.Child(path).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"🗑️ Đã xóa dữ liệu tại: {path}");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"❌ Lỗi xóa dữ liệu ({path}): {task.Exception}");
                onComplete?.Invoke(false);
            }
        });
    }

    // ======================================================
    // 🔹 Đọc danh sách object List<T> từ Firebase
    // ======================================================
    public void ReadList<T>(
        string path,
        Action<List<T>> onSuccess,
        Action onNotFound = null,
        Action<Exception> onError = null)
    {
        if (!CheckReference(null)) { onError?.Invoke(new NullReferenceException("Firebase reference null")); return; }

        reference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                HandleError(path, task.Exception, onError);
                return;
            }

            if (!task.IsCompletedSuccessfully)
            {
                HandleError(path, task.Exception, onError, "Nhiệm vụ đọc Firebase không hoàn tất");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                Debug.LogWarning($"⚠️ Không có dữ liệu tại {path}");
                onNotFound?.Invoke();
                return;
            }

            try
            {
                List<T> result = new List<T>();
                foreach (var child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    if (string.IsNullOrEmpty(json)) continue;
                    T obj = JsonConvert.DeserializeObject<T>(json);
                    result.Add(obj);
                }
                Debug.Log($"✅ Đã tải {result.Count} đối tượng từ {path}");
                onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                HandleError(path, ex, onError, "Lỗi parse danh sách JSON");
            }
        });
    }

    // ======================================================
    // 🔹 Helper kiểm tra reference Firebase
    // ======================================================
    private bool CheckReference(Action<bool> onComplete)
    {
        if (reference == null)
        {
            Debug.LogError("❌ Firebase reference chưa khởi tạo!");
            onComplete?.Invoke(false);
            return false;
        }
        return true;
    }

    // ======================================================
    // 🔹 Helper xử lý lỗi
    // ======================================================
    private void HandleError(string path, Exception ex, Action<Exception> callback, string extraMessage = "")
    {
        string message = string.IsNullOrEmpty(extraMessage) ? ex.Message : $"{extraMessage}: {ex.Message}";
        Debug.LogError($"❌ [Firebase] Lỗi tại {path}: {message}");
        callback?.Invoke(ex);
    }
}
