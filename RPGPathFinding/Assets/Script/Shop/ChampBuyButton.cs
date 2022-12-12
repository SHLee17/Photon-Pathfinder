using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampBuyButton : MonoBehaviour
{
    public PlayerManager pm;
    public Text txtName;
    public Image imgCamp;
    public Champion champ;


    public void ClickBuyButton()
    {
        if (champ != null && pm.photonView.IsMine)
        {
            if (pm.BuySuccessChamp(champ))
            {
                ResetChampBuyButton();
            }
        }
    }

    public void ResetChampBuyButton(Champion champion = null)
    {
        if (champion)
        {
            champ = champion;
            txtName.text = champ.strName;
        }
        else
        {
            champ = null;
            txtName.text = "";
        }
    }

    
}
