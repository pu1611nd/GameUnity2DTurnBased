using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject playerButtonPrefab;
    public Transform playerListParent;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);

        foreach (RoomInfo room in roomList)
        {
            GameObject btn = Instantiate(playerButtonPrefab, playerListParent);
            btn.GetComponentInChildren<Text>().text = room.Name + (room.IsOpen ? " (Join)" : " (Watching)");
            btn.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.Name, room.IsOpen));
        }
    }

    void JoinRoom(string roomName, bool isOpen)
    {
        PlayerPrefs.SetInt("isSpectator", isOpen ? 0 : 1); // 0 = chơi, 1 = xem
        PhotonNetwork.JoinRoom(roomName);
    }
}
