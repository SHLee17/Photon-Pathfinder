using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameStages
{
    MaintenanceTime,
    Matching,
    ReadyStage,
    StartStage,
    Event,
    EndStage,
    
}

public class GameManager : MonoBehaviourPun
{
    public struct GamePhase
    {
        public int index;
        public int timer;
        public GamePhase(int index, int timer)
        {
            this.index = index;
            this.timer = timer;
        }
    }

    public static GameManager Incetance;

    int timer;
    Dictionary<GameStages, GamePhase> gamePhaseDict;


    [Header("UI Object")]
    [SerializeField]
    UnityEngine.UI.Text txtEventTimer;
    [SerializeField]
    UnityEngine.UI.Text txtTimer;
    [SerializeField]
    UnityEngine.UI.Text txtPhaseStatus;
    [SerializeField]
    UnityEngine.UI.Dropdown dropdownPhase;

    [Header("ETC")]
    public GameStages eGamePhase;
    public OnlinePlayer onlinePlayer;
    public List<PlayerManager> playerList;
    //public List<PlayerManager> playerList;

    private void Awake()
    {
        if (!Incetance)
            Incetance = this;

        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        playerList = new List<PlayerManager>();
        gamePhaseDict = new Dictionary<GameStages, GamePhase>();

        gameObject.name = "Game Manger";
        txtTimer.text = "00:00";

        if (!PhotonNetwork.IsMasterClient)
            dropdownPhase.gameObject.SetActive(false);

        dropdownPhase.options.Clear();

        int j = 0;
        List<string> gamePhaseList = new List<string>();

        foreach (GameStages item in System.Enum.GetValues(typeof(GameStages)))
        {
            gamePhaseList.Add(item.ToString());
            gamePhaseDict.Add(item, new GamePhase(j, 0));
            j++;
        }

        dropdownPhase.AddOptions(gamePhaseList);
        dropdownPhase.onValueChanged.AddListener(delegate { DrodownItemSelected(); });

        eGamePhase = GameStages.MaintenanceTime;

        StartCoroutine(CoroutineUpdate());

    }
    private void Update()
    {
        switch (eGamePhase)
        {
            case GameStages.MaintenanceTime:
                break;
            case GameStages.Matching:
                photonView.RPC("Matching", RpcTarget.AllBuffered, 0, 1);
                StageChange(GameStages.ReadyStage);
                break;
            case GameStages.ReadyStage:
                if (playerList.TrueForAll(p => p.isReady))
                {
                    StageChange(GameStages.StartStage);
                    playerList.ForEach(p => p.GetReady(false));
                }
                break;
            case GameStages.StartStage:
                break;
            case GameStages.EndStage:
                photonView.RPC("EndMatching", RpcTarget.AllBuffered);
                foreach (PlayerManager p in playerList)
                {
                    if (p.isHost)
                    {
                        if (!p.eventCheck[Events.Obstacle])
                            StageChange(GameStages.Event);
                        else
                            StageChange(GameStages.MaintenanceTime);
                    }
                }
                break;

        }

    }
    IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            txtPhaseStatus.text = eGamePhase.ToString();
            yield return null;
        }
    }
    void DrodownItemSelected()
    {
        StageChange(gamePhaseDict.Keys.ElementAt(dropdownPhase.value));
    }
    IEnumerator TimerCoroutain()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            photonView.RPC("TimerRPC", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    void TimerRPC()
    {
        timer++;
        txtTimer.text = System.TimeSpan.FromSeconds(timer).ToString("mm\\:ss");
    }
    [PunRPC]
    void StageChange(int phase)
    {
        eGamePhase = gamePhaseDict.Keys.ElementAt(phase);
        dropdownPhase.value = gamePhaseDict[eGamePhase].index;
    }
    public void StageChange(GameStages eGamePhase)
    {
        photonView.RPC("StageChange", RpcTarget.All, gamePhaseDict[eGamePhase].index);
    }
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            txtTimer.gameObject.SetActive(true);

            GetComponent<Canvas>().worldCamera = Camera.allCameras[1];
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(TimerCoroutain());

            foreach (Photon.Realtime.Player item in PhotonNetwork.PlayerList)
                playerList.Add((item.TagObject as GameObject).GetComponent<PlayerManager>());
        }
    }
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    [PunRPC]
    void Matching(int Hostplayer, int againstPlayer)
    {
        if (PhotonNetwork.PlayerList.Length < 2)
            return;
        if (eGamePhase != GameStages.Matching)
            return;

        playerList[Hostplayer].isHost = true;

        //if (!playerList[Hostplayer].isHost)
        //    return;

        foreach (Node node in playerList[againstPlayer].mapManager.nodeList)
        {
            if (node.ePlayerSide == EPlayerSide.player)
            {
                if (node.champ)
                {
                    int xMaxValeu = playerList[againstPlayer].mapManager.mapSizeH;
                    int index = node.champ.currentNode.x + Mathf.Abs(node.champ.currentNode.y - xMaxValeu) * xMaxValeu;

                    Node temp = playerList[Hostplayer].mapManager.nodeList[index];
                    temp.champ = node.champ;

                    temp.champ.ePlayerSide = EPlayerSide.enemy;

                    temp.champ.transform.SetParent(temp.transform);

                    temp.champ.currentNode = temp;
                    temp.champ.startNode = temp;

                    temp.champ.transform.localPosition = Vector3.zero;

                    playerList[Hostplayer].againstPlayerIndex = againstPlayer;
                    playerList[againstPlayer].againstPlayerIndex = Hostplayer;
                }
            }
        }
    }

    [PunRPC]
    void EndMatching()
    {
        if (eGamePhase == GameStages.EndStage)
        {
            foreach (PlayerManager p in playerList)
            {
                if (p.isHost)
                {
                    foreach (Node node in p.mapManager.nodeList)
                    {
                        if (node.champ)
                        {
                            if (node.champ.ePlayerSide == EPlayerSide.enemy)
                                node.champ.ePlayerSide = EPlayerSide.player;
                            node.champ.parentStorage.transform.SetParent(node.champ.transform);
                            node.champ.transform.localPosition = new Vector3(0, 0.5f, 0);
                            
                        }
                    }
                    p.isHost = false;
                }
            }
        }
    }

    private void OnDestroy()
    {
        Incetance = null;
    }

}
