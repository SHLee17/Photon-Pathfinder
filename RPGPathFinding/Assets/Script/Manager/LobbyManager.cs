using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";
    int playerNumber { get { return PhotonNetwork.LocalPlayer.ActorNumber - 1; } }
    public GameObject shopManager;

    [Header("Texts")]
    public Text txtConnectionInfo;
    public Text txtUserName;

    [Header("Buttons")]
    public Button playButton;
    public Button playID;

    Stack<int> testStack;

    void Start()
    {
        testStack = new Stack<int>();

        testStack.Push(1);
        testStack.Push(2);
        testStack.Push(3);
        testStack.Push(4);
        testStack.Push(5);
        testStack.Push(6);

        Debug.Log(testStack.Last());
        //foreach (var item in testStack.ToList())
        //{
        //    Debug.Log(item);
        //}

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 5;
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.interactable = false;
        txtConnectionInfo.text = "Offline : 접속중...";


    }

    public override void OnConnectedToMaster()
    {
        playButton.interactable = true;
        txtConnectionInfo.text = "Online : 접속완료";
        PhotonNetwork.NickName = AuthManager.user.DisplayName;
        txtUserName.text = "환영합니다 " + $"<b><size=30>{PhotonNetwork.NickName}</size></b>" + " 님";



        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, null);

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        playButton.interactable = false;
        txtConnectionInfo.text = $"Offline : {cause} - 재접속중...";

        PhotonNetwork.ConnectUsingSettings();
    }
    public void Play()
    {
        PhotonNetwork.LoadLevel("Main Scene");
    }
    public override void OnJoinedLobby()
    {

    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        txtConnectionInfo.text = newPlayer + "님이 입장 하셨습니다.";
        Debug.Log(newPlayer);

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinedRoom" + PhotonNetwork.CurrentRoom);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("Game Manager", Vector3.zero, Quaternion.identity);

        GameObject tempGMobj;
        tempGMobj = PhotonNetwork.Instantiate("Player Manager", Vector3.zero, Quaternion.identity);
        //tempGM.PlayerNumberSet();

    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();


    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        //Debug.Log("Destroy");
    }
}
