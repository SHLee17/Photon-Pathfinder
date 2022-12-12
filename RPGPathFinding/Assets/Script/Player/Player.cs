using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public struct ChampInventory
    {
        public ChampInventory(Champion _champ)
        {
            champ = _champ;
            count = new Dictionary<int, int>();
            count.Add(1, 0);
            count.Add(2, 0);
            count.Add(3, 0);
        }

        public Champion champ;
        public Dictionary<int, int> count;
    }

    [SerializeField]
    [Range(0, 20)]
    int speed = 10;
    Dictionary<int, int> expTable;

    public Material material;
    public Vector3 destination;
    public bool isMove;
    public int level;
    public int exp;
    public int gold;


    public List<ChampInventory> champIventory;
    void Awake() 
    {
        gold = 10;
    }

    void Start()
    {
        champIventory = new List<ChampInventory>();
            expTable = new Dictionary<int, int>();
            expTable.Add(1, 0);
            expTable.Add(2, 2);
            expTable.Add(3, 6);
            expTable.Add(4, 10);
            expTable.Add(5, 20);
            expTable.Add(6, 36);
            expTable.Add(7, 56);
            expTable.Add(8, 80);

            gold = 100;
     }
    

    void Update()
    {
        if (photonView.IsMine)
        {
            float playerSpeed = speed * Time.deltaTime;

            if (isMove)
                transform.position = Vector3.MoveTowards(transform.position, destination, playerSpeed);

        }
    }


}
