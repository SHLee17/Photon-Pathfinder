using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private void Awake() => Screen.SetResolution(960, 540, false);

    private void Start()
    {
        //Connect();
    }

    private void Update() => Debug.Log(PhotonNetwork.NetworkClientState.ToString());

    public void Connect() => PhotonNetwork.ConnectUsingSettings();



}
