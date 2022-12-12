using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public PlayerManager pm;
    public List<ChampBuyButton> buyButtonList;
    public List<Champion> championList;


    private void Start()
    {
        ResetShop();
        if (!pm.photonView.IsMine)
            gameObject.SetActive(false);
        
    }
    public void ResetShop()
    {
        foreach (ChampBuyButton item in buyButtonList)
        {
            int Rand = UnityEngine.Random.Range(0, championList.Count - 1);
            item.ResetChampBuyButton(championList[Rand]);
            item.pm = pm;
        }
    }
    public void ResetButtonClick()
    {
        if (pm.BuySomthing(2))
            ResetShop();

    }
    public Champion ChampSelect(ChampTypes eType)
    {
        foreach (Champion item in championList)
        {
            if (item.eType == eType)
                return item;
        }
        return null;
    }
}
