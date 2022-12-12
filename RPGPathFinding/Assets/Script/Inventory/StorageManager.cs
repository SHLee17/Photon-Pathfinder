using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public enum EPlayerSide
{
    player,
    enemy,
}
public class StorageManager : MonoBehaviourPun
{
    public Champion champ;
    public Obstacle obstacle;
    public EPlayerSide ePlayerSide;
    public int index;
    [SerializeField]
    StorageManager targetStorage;

    public virtual void AddChamp(in Champion _champ)
    {
        if (!champ && photonView.IsMine)
        {
            PhotonNetwork.Instantiate(_champ.name,
                       Vector3.zero,
                       Quaternion.identity);

            photonView.RPC("SetParent", RpcTarget.AllBuffered);
        }
    }
    protected void AddObstacle()
    {
        if (!obstacle && photonView.IsMine)
        {
            PhotonNetwork.Instantiate("Obstacle", Vector3.zero, Quaternion.identity);

            photonView.RPC("SetParent", RpcTarget.AllBuffered);
        }
    }

   [PunRPC]
    protected void SetParent()
    {
        GameObject[] champions = GameObject.FindGameObjectsWithTag("Champion");

        foreach (GameObject item in champions)
        {

            if (item.TryGetComponent(out Champion temp))
            {
                if (temp.photonView.Owner == photonView.Owner)
                {
                    champ = temp;

                    champ.transform.SetParent(transform);
                    champ.ePlayerSide = ePlayerSide;
                    champ.parentStorage = this;
                    champ.LastPos();

                    champ.tag = "Untagged";

                }
            }
            else if(item.TryGetComponent(out Obstacle temp2))
            {
                if (temp2.photonView.Owner == photonView.Owner)
                {
                    obstacle = temp2;

                    obstacle.transform.SetParent(transform);

                    obstacle.transform.localPosition = new Vector3(0, 10, 0);

                    obstacle.tag = "Untagged";

                    //obstacle.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SwapChamp(StorageManager targetStorage)
    { 
        //int playerIndex = Array.IndexOf(PhotonNetwork.PlayerList, player);
        photonView.RPC("SwapChampRPC", RpcTarget.AllBuffered,targetStorage.index);
    }

    [PunRPC]
    public void SwapChampRPC(int nodeIndex)
    {
        targetStorage =
            (photonView.Owner.TagObject as GameObject).
            GetComponent<PlayerManager>().mapManager.champStorageList[nodeIndex];

        if (targetStorage == null || 
            (targetStorage.ePlayerSide != ePlayerSide ))
        {

            if (champ)
                champ.transform.position = champ.parentStorage.transform.position;

            return;
        }
        if(GameManager.Incetance.eGamePhase != GameStages.MaintenanceTime)
        {
            if(targetStorage is Node)
            {
                if (champ)
                    champ.transform.position = champ.parentStorage.transform.position;

                return;
            }
        }

        if (targetStorage is Node)
        {
            if (champ)
            {
                Node tempNode = (Node)targetStorage;
                champ.currentNode = tempNode;
                champ.startNode = tempNode;
                champ.x = tempNode.x;
                champ.y = tempNode.y;
            }
        }

        if (champ)
        {
            champ.transform.SetParent(targetStorage.transform);
            champ.parentStorage = targetStorage;
            Champion temp;

            temp = targetStorage.champ;
            targetStorage.champ = champ;
            champ = temp;
        }

        //targetStorge.champ.ePlayerSide = targetStorge.ePlayerSide;
        if (targetStorage.champ)
            targetStorage.champ.LastPos();
        if(champ)
            champ.LastPos();
    }

 


}
