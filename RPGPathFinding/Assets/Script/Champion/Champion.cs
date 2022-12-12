using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ChampTypes
{
    Knight,
    Dummy
    
}
//public enum EChampSide
//{
//    Moon,
//    Sun,
//    End
//}

public class Champion : MonoBehaviourPun 
{
    float tValue;
    Vector3 startPos;
    Vector3 endPos;
    float height = 3.5f;

    [Range(0,1)]
    public float test;

    public bool isMoving;
    public bool isFindingSuccess;

    public ChampTypes eType;
    //public EChampSide eSide;
    public EPlayerSide ePlayerSide;
    public int star, cost, x, y;
    public string strName;
    public Sprite sprImage;
    //public Vector3 lastPos;
    
    //public ChampManager champManager;

    public int startHP = 100, startMP = 0, HP, MP, Range;
    public float speed;
    public bool isDead;

    public List<Node> openList, closeList;
    public Stack<Node> finalStack;
    public StorageManager parentStorage;
    public Node currentNode, startNode;
    public Champion targetChamp;
    public Animator animator;


    public List<Node> tempStackList = new List<Node>();

    protected virtual void Start()
    {
        openList = new List<Node>();
        closeList = new List<Node>();
        finalStack = new Stack<Node>();
        isDead = false;
        isMoving = false;
        isFindingSuccess = false;
        speed = 1f;
        HP = startHP;
        MP = startMP;
    }


    public void OnDamage(int damage)
    {
        HP -= damage;

        if (HP <= 0 && !isDead)
        {
            Destroy(gameObject);
        }
    }

    public void LastPos() => photonView.RPC("LastPosRPC", RpcTarget.AllBuffered);
    [PunRPC]
    public void LastPosRPC()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    Vector3 Bezier(Vector3 start , Vector3 end, float value)
    {
        Vector3 startH = start + new Vector3(0, height, 0);
        Vector3 endH = end + new Vector3(0, height, 0);

        Vector3 A = Vector3.Lerp(start, startH, value);
        Vector3 B = Vector3.Lerp(startH, endH, value);
        Vector3 C = Vector3.Lerp(endH, end, value);

        Vector3 D = Vector3.Lerp(A, B, value);
        Vector3 E = Vector3.Lerp(B, C, value);

        Vector3 F = Vector3.Lerp(D, E, value);

        return F;
    }


    public IEnumerator Appearance()
    {

        transform.localPosition = new Vector3(0, 10, 0);

        Vector3 endPos = new Vector3(0, 0.5f, 0);
        isMoving = true;

        while (transform.localPosition.y > endPos.y)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 2f);
            yield return null;
        }

        
        isMoving = false;
        Debug.Log(isMoving);
    }


    [PunRPC]
    public void MoveRPC(Vector3 destination) => StartCoroutine(Move(destination));
    public void MoveChamp(Vector3 destination) => photonView.RPC("MoveRPC", RpcTarget.AllBuffered, destination);
    public IEnumerator Move(Vector3 destination)
    {
        isMoving = true;
        startPos = transform.position;
        endPos = destination;

        Vector3 addPos = new Vector3(0, 0.5f, 0);

        startPos += addPos;
        endPos += addPos;

        tValue = 0;

        while (tValue < 1)
        {
            tValue += Time.deltaTime * speed;
            transform.position = Bezier(startPos, endPos, tValue);
            yield return null;
        }

        isMoving = false;
    }


    public void RequestOwnerShip(Photon.Realtime.Player successors)
    {
        if (photonView.gameObject != gameObject)
            return;

        if (photonView.Owner != successors)
            photonView.TransferOwnership(successors);
    }
}

