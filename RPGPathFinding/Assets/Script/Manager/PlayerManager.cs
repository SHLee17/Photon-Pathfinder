using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public enum Events
{
    Obstacle
}
public class PlayerManager : MonoBehaviourPun, IPunInstantiateMagicCallback
{

    int actorNum { get { return photonView.Owner.ActorNumber - 1; } }
    bool isMine { get { return photonView.IsMine; } }


    StorageManager currentSelectStorage;
    List<Node> nodeChamp;
    Vector3 pos;
    [SerializeField]
    GameManager GM;

    [Header("Unity Object")]
    public MapManager mapManager;
    [SerializeField]
    ShopManager shopManager;
    [SerializeField]
    Player player;

    [Header("ETC")]
    public bool isDebug;
    public bool isDrag;
    public bool isHost;
    public int againstPlayerIndex;
    public bool isReady;
    public Vector3 cameraPos;
    public Dictionary<Events, bool> eventCheck;
    [Range(0, 1)]
    public float dealay;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            if (isMine)
            {
                Vector3 pos = Camera.main.transform.position;
                Camera.main.transform.position = new Vector3(pos.x, pos.y, mapManager.transform.position.z);
                shopManager.gameObject.SetActive(true);
                shopManager.GetComponent<Canvas>().worldCamera = Camera.allCameras[1];
            }

        }
    }


    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        pos = new Vector3(0, 0, actorNum * 60);
    }

    private void Start()
    {
        nodeChamp = new List<Node>();
        eventCheck = new Dictionary<Events, bool>();
        eventCheck.Add(Events.Obstacle, false);
        isDebug = false;
        isReady = false;
        isHost = false;
        isDrag = false;
        shopManager.gameObject.SetActive(false);
        gameObject.name = photonView.Owner.NickName + " PM";
        cameraPos = Camera
            .main
            .transform
            .position;
        transform.position = pos;
        player.transform.position = 
            new Vector3(
             mapManager.startPos.position.x,
             0.5f,
             mapManager.startPos.position.z);
        photonView.RPC("PlayerInfoSetRPC", RpcTarget.AllBuffered);
        mapManager.nodeList[0].AddChamp(shopManager.ChampSelect(ChampTypes.Knight));
        //mapManager.nodeList[Index(mapManager.mapSizeV - 1, 0)].AddChamp(shopManager.championList[0]);

    }
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1
            && isMine)
        {
            int testY = mapManager.mapSizeV - 1;
            int testX = 0;
            Node testNode = mapManager.nodeList[Index(testY, testX)];

            if (Input.GetKeyDown(KeyCode.F1))
                isDebug = !isDebug;

            if (isDebug)
            {
                testNode.AddChamp(shopManager.ChampSelect(ChampTypes.Dummy));

                StartCoroutine(ObstacleActive(true, 4, 5));

                StartCoroutine(ObstacleActive(true, 6, 1));

            }
            else
            {
                if (testNode.champ)
                {
                    PhotonNetwork.Destroy(testNode.champ.gameObject);

                    StartCoroutine(ObstacleActive(false, 4, 5));

                    StartCoroutine(ObstacleActive(false, 6, 1));
                }
            }


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            #region Mouse Left Click Interaction
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ChampStorage")))
                {
                    StorageManager tempStorage = hit.collider.GetComponent<StorageManager>();

                    if (GM.eGamePhase == GameStages.MaintenanceTime)
                    {
                        if (tempStorage.champ)
                        {
                            if (tempStorage.champ.photonView.IsMine)
                            {
                                isDrag = true;
                                currentSelectStorage = tempStorage;
                            }
                        }
                        #region Debug
                        else
                        {
                            if (tempStorage)
                            {
                                if (tempStorage is Node)
                                {
                                    Node tempNode = tempStorage as Node;

                                    if (tempNode.isBlock)
                                    {
                                        tempNode.TestColorSet(ETestColor.green);
                                        tempNode.isBlock = false;
                                    }
                                    else
                                    {
                                        tempNode.TestColorSet(ETestColor.red);
                                        tempNode.isBlock = true;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else if(GM.eGamePhase == GameStages.Event)
                    {
                        //if (isHost)
                        //{
                        //    tempStorage.obstacle.animator.SetTrigger("");
                        //}
                    }
                    else
                    {
                        if (tempStorage is ChampInventorySlot)
                        {
                            if (tempStorage.champ)
                            {
                                if (tempStorage.champ.photonView.IsMine)
                                {
                                    isDrag = true;
                                    currentSelectStorage = tempStorage;
                                }
                            }
                        }
                    }

                }
                if (!currentSelectStorage)
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
                        {

                            MapManager map = hit.collider.GetComponentInParent<MapManager>();
                            if (map && map.photonView.IsMine)
                            {

                                player.isMove = true;
                                player.destination = new Vector3(hit.point.x, 0.5f, hit.point.z);

                            }

                        }
                        else
                            player.isMove = false;
                    }
                }

            }
            else if (isDrag)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
                {
                    if (currentSelectStorage)
                        currentSelectStorage.champ.transform.position = new Vector3(hit.point.x, 0.5f, hit.point.z);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (currentSelectStorage)
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ChampStorage")))
                        {
                            StorageManager temp = hit.collider.GetComponent<StorageManager>();

                            currentSelectStorage.SwapChamp(temp);

                        }
                        else
                            currentSelectStorage.champ.transform.position = currentSelectStorage.champ.parentStorage.transform.position;

                        currentSelectStorage = null;

                        isDrag = false;
                    }
                }
            }

            
            #endregion
            if(GM.eGamePhase == GameStages.ReadyStage)
            {

                if (PhotonNetwork.PlayerList.Length < 2)
                {
                    if (!nodeChamp.Any())
                        nodeChamp = mapManager.nodeList.FindAll(x => x.champ != null);

                    isHost = true;
                }
                else
                {
                    if (isHost)
                    {
                        if (!nodeChamp.Any())
                            nodeChamp = mapManager.nodeList.FindAll(x => x.champ != null);
                    }
                    else
                    {
                        if (!isReady)
                        {
                            Vector3 pos = new Vector3(GM.playerList[againstPlayerIndex].transform.position.x + 40,
                                40,
                                GM.playerList[againstPlayerIndex].transform.position.z + 10);

                            if (Vector3.Distance(Camera.main.transform.position, pos) > 0.1f)
                            {
                                Camera.main.transform.position =
                                    Vector3.MoveTowards(
                                    Camera.main.transform.position,
                                    pos,
                                    15.0f * Time.deltaTime);
                            }
                            else
                                GetReady(true);
                        }

                    }
                }
                if (!nodeChamp.TrueForAll(node => node.champ.isMoving))
                    GetReady(true);
            }
            #region StartStage
            else if (GM.eGamePhase == GameStages.StartStage)
            {
                if (PhotonNetwork.PlayerList.Length > 1)
                {
                    if (!isHost)
                        return;
                }

                foreach (Node item in nodeChamp)
                {
                    Champion champ = item.champ;

                    if (!champ.isFindingSuccess && 
                        !champ.isMoving && 
                        champ.eType != ChampTypes.Dummy)
                    {
                        if (!champ.targetChamp)
                            champ.targetChamp = FindTarget(champ);

                        else
                            Pathfinding(champ);

                        if (champ.finalStack.Count > 0)
                        {
                            Node temp = champ.finalStack.Pop();
                            if (temp != champ.targetChamp.currentNode)
                            {
                                champ.currentNode = temp;
                                champ.startNode = temp;
                                champ.MoveChamp(temp.transform.position);
                            }
                        }
                    }
                }

                if (nodeChamp.TrueForAll(node => node.champ.isFindingSuccess))
                {
                    if (!nodeChamp.TrueForAll(node => node.champ.isMoving))
                    {
                            GM.StageChange(GameStages.EndStage);
                    }
                }
            }
            //else if(GM.eGamePhase == GameStages.Event)
            //{

            //    if (isHost)
            //    {
            //        StartCoroutine(ObstacleActive(true, 4, 5));

            //        StartCoroutine(ObstacleActive(true, 6, 1));


            //        foreach (Node item in nodeChamp)
            //        {
            //            if (item.champ)
            //            {
            //                item.champ.isFindingSuccess = false;

            //                if (item.champ.ePlayerSide == EPlayerSide.enemy)
            //                {

            //                    //item.SwapChamp(mapManager.nodeList[Index(mapManager.mapSizeV - 1, mapManager.mapSizeH - 1)]);
            //                    //item.champ
            //                }
            //            }
            //        }


            //        GM.StageChange(GameStages.StartStage);
            //        eventCheck[Events.Obstacle] = true;


            //    }
            //}
            #endregion
        }
    }

    public void GetReady(bool isReady) =>
        photonView.RPC("GetReadyRPC", RpcTarget.AllBuffered, isReady);
    [PunRPC]
    public void GetReadyRPC(bool isReady) =>
        this.isReady = isReady;

    [PunRPC]
    public void PlayerInfoSetRPC()
    {
        while (!GM)
            GM = GameManager.Incetance;

        GM.onlinePlayer.buttonList[actorNum].gameObject.SetActive(true);
        GM.onlinePlayer.buttonList[actorNum].txtName.text = photonView.Owner.ToString();
        GM.onlinePlayer.buttonList[actorNum].playerPos = pos;
    }

    public bool BuySomthing(int cost)
    {
        if (isMine)
        {
            if (player.gold >= cost)
                return true;
        }
        return false;
    }
    public bool BuySuccessChamp(Champion champ, bool isEmptyInven = true)
    {
        if (BuySomthing(champ.cost))
        {
            foreach (StorageManager storage in mapManager.champStorageList)
            {
                if (!storage.champ)
                {
                    if (storage.ePlayerSide == EPlayerSide.player)
                    {
                        if (isEmptyInven)
                        {
                            if (storage is ChampInventorySlot)
                            {
                                ChampInventorySlot temp = (ChampInventorySlot)storage;
                                temp.AddChamp(champ);
                                return true;
                            }
                        }
                        else
                        {
                            if (storage is Node)
                            {
                                Node tempNode = (Node)storage;
                                tempNode.AddChamp(champ);
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
  

    void  Pathfinding(Champion champ)
    {
        if (champ.finalStack.Any())
        {
            if (champ.finalStack.ElementAt(champ.Range) == champ.targetChamp.currentNode)
            {
                champ.isFindingSuccess = true;
                return;
            }

            if (champ.finalStack.Last() == champ.targetChamp.currentNode)
            {
                if (champ.finalStack.Last().champ)
                    return;
            }

            foreach (Node item in mapManager.nodeList)
            {
                item.g = 0;
                item.h = 0;
            }

            champ.tempStackList.Clear();
            champ.finalStack.Clear();
            champ.openList.Clear();
            champ.closeList.Clear();
        }

        if (champ.openList.Count > mapManager.nodeList.Count || champ.isFindingSuccess)
            return;
        champ.openList.Add(champ.currentNode);

        while (champ.openList.Count > 0)
        {
            champ.currentNode = champ.openList[0];

            List<Node> tempF = champ.openList.OrderBy(x => x.f).ToList();

            if (tempF.Count > 1)
            {
                if (tempF[0].f == tempF[1].f)
                    champ.currentNode = champ.openList.OrderBy(x => x.h).ToList().FirstOrDefault();
                else
                    champ.currentNode = tempF[0];
            }
            else
                champ.currentNode = tempF[0];

            champ.openList.Remove(champ.currentNode);
            champ.closeList.Add(champ.currentNode);

            if (champ.currentNode == champ.targetChamp.currentNode)
            {
                Node targetCurNode = champ.targetChamp.currentNode;
                while (targetCurNode != champ.startNode)
                {
                    champ.tempStackList.Add(targetCurNode);
                    champ.finalStack.Push(targetCurNode);
                    targetCurNode = targetCurNode.parentNode;
                }

                champ.currentNode = champ.startNode;

 
                return;
            }

            Node curNode = champ.currentNode;


            if (curNode.y % 2 == 0)
            {
                Parallel.Invoke(
                    () => { OpenListAdd(champ, curNode.y - 1, curNode.x + 1); },
                    () => { OpenListAdd(champ, curNode.y - 1, curNode.x); },
                    () => { OpenListAdd(champ, curNode.y, curNode.x + 1); },
                    () => { OpenListAdd(champ, curNode.y, curNode.x - 1); },
                    () => { OpenListAdd(champ, curNode.y + 1, curNode.x); },
                    () => { OpenListAdd(champ, curNode.y + 1, curNode.x + 1); }
                    );

                //OpenListAdd(champ, curNode.y - 1, curNode.x + 1);
                //OpenListAdd(champ, curNode.y - 1, curNode.x);
                //OpenListAdd(champ, curNode.y, curNode.x + 1);
                //OpenListAdd(champ, curNode.y, curNode.x - 1);
                //OpenListAdd(champ, curNode.y + 1, curNode.x);
                //OpenListAdd(champ, curNode.y + 1, curNode.x + 1);

            }
            else
            {

                Parallel.Invoke(
                   () => { OpenListAdd(champ, curNode.y - 1, curNode.x - 1); },
                   () => { OpenListAdd(champ, curNode.y - 1, curNode.x); },
                   () => { OpenListAdd(champ, curNode.y, curNode.x + 1); },
                   () => { OpenListAdd(champ, curNode.y, curNode.x - 1); },
                   () => { OpenListAdd(champ, curNode.y + 1, curNode.x - 1); },
                   () => { OpenListAdd(champ, curNode.y + 1, curNode.x); }
                   );

                //OpenListAdd(champ, curNode.y - 1, curNode.x - 1);
                //OpenListAdd(champ, curNode.y - 1, curNode.x);
                //OpenListAdd(champ, curNode.y, curNode.x + 1);
                //OpenListAdd(champ, curNode.y, curNode.x - 1);
                //OpenListAdd(champ, curNode.y + 1, curNode.x - 1);
                //OpenListAdd(champ, curNode.y + 1, curNode.x);
            }
        }
    }

    void OpenListAdd(Champion champ, int checkY, int checkX)
    {
        int nodeIndex = Index(checkY, checkX);
        //try
        //{
            if (nodeIndex < mapManager.nodeList.Count && nodeIndex >= 0)
            {
                if (!mapManager.nodeList[nodeIndex].isBlock &&
                    !champ.closeList.Contains(mapManager.nodeList[nodeIndex]))
                {
                    if (mapManager.nodeList[nodeIndex].champ)
                    {
                        if (mapManager.nodeList[nodeIndex].champ != champ.targetChamp)
                            return;
                    }

                    Node neighborNode = mapManager.nodeList[nodeIndex];

                    int moveCost = champ.currentNode.g + 10;

                    if (moveCost < neighborNode.g || !champ.openList.Contains(neighborNode))
                    {
                        neighborNode.g = moveCost;

                    Vector3 tempVec = neighborNode.pos - champ.targetChamp.currentNode.pos;

                    neighborNode.h = tempVec.sqrMagnitude;

                    //neighborNode.CostTextSet();

                    neighborNode.parentNode = champ.currentNode;

                        champ.openList.Add(neighborNode);
                    }

                }
            }
        //}
        //catch
        //{

        //    Debug.Log("NodeIndex Out Range " + nodeIndex);
        //    Debug.Log(mapManager.nodeList[nodeIndex]);

        //}
    }

    [SerializeField]
    List<Champion> oppositeChamp = new List<Champion>();
    Champion FindTarget(Champion _champ)
    {
        oppositeChamp.Clear();

        List<Champion> targetCheck()
        {
            foreach (Node node in nodeChamp)
            {
                Champion champ = node.champ;
                if (_champ.ePlayerSide != champ.ePlayerSide)
                    oppositeChamp.Add(champ);
            }
            return oppositeChamp;
        }

        Champion neareastChamp = targetCheck().OrderBy(obj =>
        {
            return Vector3.Distance(_champ.transform.position, obj.transform.position);
        }).FirstOrDefault();


        return neareastChamp;
    }
    System.Collections.IEnumerator ObstacleActive(bool isActive, int line, int exceptionNode)
    {
        for (int x = 0; x < mapManager.mapSizeH; x++)
        {
            if (x == exceptionNode)
                continue;

            yield return new WaitForSeconds(dealay);

            mapManager.nodeList[Index(line, x)].Obstacle(isActive);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    int Index(int y, int x)
    {
        return y * mapManager.mapSizeH + x;
    }
}
