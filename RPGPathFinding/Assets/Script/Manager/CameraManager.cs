using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public float panSpeed = 40f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    void Update()
    {

            Vector3 pos = transform.position;

        //if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        //    pos.z += panSpeed * Time.deltaTime;
        //if (Input.mousePosition.y <= panBorderThickness)
        //    pos.z -= panSpeed * Time.deltaTime;
        //if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        //    pos.x += panSpeed * Time.deltaTime;
        //if (Input.mousePosition.x <= panBorderThickness)
        //    pos.x -= panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            pos.x += panSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.W))
                pos.x -= panSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                pos.z += panSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                pos.z -= panSpeed * Time.deltaTime;


            transform.position = pos;
        
    }
}
