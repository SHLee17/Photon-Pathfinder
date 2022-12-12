using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviourPun
{
    [SerializeField]
    PlayerManager pm;

    public List<Node> nodeList;

    public Node node;
    public ChampInventorySlot champInvenTile;
    public List<StorageManager> champStorageList;
    public int mapSizeV, mapSizeH;
    public Transform startPos;

    [SerializeField]
    Transform playerInven, enemyInven, playerNode, enemyNode;

    [PunRPC]
    private void Init()
    {

        champStorageList = new List<StorageManager>();
        nodeList = new List<Node>();

        mapSizeH = 7;
        mapSizeV = 4;
        int index = 0;

        foreach (Transform item in playerInven)
        {
            ChampInventorySlot temp = item.GetComponent<ChampInventorySlot>();
            temp.ePlayerSide = EPlayerSide.player;
            champStorageList.Add(temp);
            temp.index = index;
            index++;
        }

        int x = 0;
        int y = 0;

        foreach (Transform item in playerNode)
        {
            Node temp = item.GetComponent<Node>();
            temp.name = y + " , " + x;
            temp.index = index;
            temp.ePlayerSide = EPlayerSide.player;
            champStorageList.Add(temp);
            nodeList.Add(temp);

            index++;
            x++;

            if (x == mapSizeH)
            {
                y++;
                x = 0;
            }
        }

        foreach (Transform item in enemyNode)
        {
            Node temp = item.GetComponent<Node>();
            temp.name = y + " , " + x;
            temp.index = index;
            temp.ePlayerSide = EPlayerSide.enemy;
            champStorageList.Add(temp);
            nodeList.Add(temp);

            index++;
            x++;

            if (x == mapSizeH)
            {
                y++;
                x = 0;
            }

        }

        foreach (Transform item in enemyInven)
        {
            ChampInventorySlot temp = item.GetComponent<ChampInventorySlot>();
            temp.ePlayerSide = EPlayerSide.enemy;
            champStorageList.Add(temp);
            temp.index = index;
            index++;
        }

        foreach (StorageManager item in champStorageList)
        {
            if (item is Node)
            {
                Node temp = item as Node;

                temp.TextMeshActive(false);
            }
        }
    }
    private void Awake()
    {
        //photonView.RPC("Init", RpcTarget.AllBuffered);

        mapSizeH = 7;
        mapSizeV = 8;

        champStorageList = new List<StorageManager>();
        nodeList = new List<Node>();

        int index = 0;

        foreach (Transform item in playerInven)
        {
            ChampInventorySlot temp = item.GetComponent<ChampInventorySlot>();
            temp.ePlayerSide = EPlayerSide.player;
            champStorageList.Add(temp);
            temp.index = index;
            index++;
        }

        int x = 0;
        int y = 0;

        foreach (Transform item in playerNode)
        {
            Node temp = item.GetComponent<Node>();
            temp.name = y + " , " + x;
            temp.x = x;
            temp.y = y;
            temp.index = index;
            temp.ePlayerSide = EPlayerSide.player;
            champStorageList.Add(temp);
            nodeList.Add(temp);

            index++;
            x++;

            if (x == mapSizeH)
            {
                y++;
                x = 0;
            }
        }

        foreach (Transform item in enemyNode)
        {
            Node temp = item.GetComponent<Node>();
            temp.name = y + " , " + x;
            temp.x = x;
            temp.y = y;
            temp.index = index;
            temp.ePlayerSide = EPlayerSide.enemy;
            champStorageList.Add(temp);
            nodeList.Add(temp);

            index++;
            x++;

            if (x == mapSizeH)
            {
                y++;
                x = 0;
            }

        }

        foreach (Transform item in enemyInven)
        {
            ChampInventorySlot temp = item.GetComponent<ChampInventorySlot>();
            temp.ePlayerSide = EPlayerSide.enemy;
            champStorageList.Add(temp);
            temp.index = index;
            index++;
        }


        foreach (StorageManager item in champStorageList)
        {
            if (item is Node)
            {
                Node temp = item as Node;

                temp.TextMeshActive(false);
            }
        }
    }



    void Update()
    {
            if (photonView.IsMine && SceneManager.GetActiveScene().buildIndex == 1)
            {
                //if (Input.GetKeyDown(KeyCode.F1))
                //    isTest = !isTest;

                ActiveField(pm.isDrag && (GameManager.Incetance.eGamePhase == GameStages.MaintenanceTime),
                    EPlayerSide.player);
            }
    }

    void ActiveField(bool isActive, EPlayerSide side /*= EPlayerSide.None*/)
    {
        foreach (StorageManager item in champStorageList)
        {
            if (item is Node)
            {
                Node temp = item as Node;
                if (item.ePlayerSide == side)
                    temp.objNode.SetActive(isActive);
                //else if (item.ePlayerSide == EPlayerSide.None)
                //{
                //    temp.objNode.SetActive(isActive);
                //}
            }
        }
    }

    public Champion BuyChampion(ref int money, Champion champ)
    {
        Champion tempChamp;

        if(money >= champ.cost)
        {
            money -= champ.cost;
            GameObject temp;

            temp = PhotonNetwork.Instantiate(champ.name, Vector3.zero, Quaternion.identity);

            tempChamp = temp.GetComponent<Champion>();

            return tempChamp;
        }

        return null;
    }
}
