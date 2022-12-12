using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UICamera : MonoBehaviour
{
    public Camera thisCamera;



    private void Awake() =>
        DontDestroyOnLoad(this);

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
        LayerMasyOn(false);
    }

    public void LayerMasyOn(bool on)
    {
        if (on)
        {
            thisCamera.cullingMask = 1 << LayerMask.NameToLayer("ShopManager");
            //thisCamera.cullingMask |= 1 << LayerMask.NameToLayer("UI");
        }
        else
            thisCamera.cullingMask = 0;
    }
}
