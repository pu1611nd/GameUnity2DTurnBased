using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestPhotonConnect : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Đang kết nối với Photon...");
        PhotonNetwork.ConnectUsingSettings(); // Kết nối với server Photon dựa trên PhotonServerSettings
    }

    // Khi kết nối tới Master Server thành công
    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Kết nối thành công với Photon Master Server!");
        PhotonNetwork.JoinLobby(); // Optional: join lobby để xem danh sách phòng
    }

    // Khi join lobby thành công
    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Đã vào Lobby thành công! Có thể xem danh sách phòng.");
    }

    // Khi kết nối thất bại
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("❌ Kết nối thất bại: " + cause);
    }
}
