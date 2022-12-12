using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePlayerButton : MonoBehaviour
{
    public Text txtName;
    public Vector3 playerPos;

    private void Start()
    {
        Button temp;

        temp = gameObject.GetComponent<Button>();

        temp.onClick.AddListener(()=> {OnClickPlayerButton(); });
    }

    public void OnClickPlayerButton()
    {
        Camera.main.transform.position = new Vector3(playerPos.x + 40, 40, playerPos.z + 10);
    }
}
