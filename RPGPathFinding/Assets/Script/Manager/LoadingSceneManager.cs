using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject wave;
    [SerializeField]
    Rigidbody rb;

    static string nextScene;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading Scene");
    }
    void Start()
    {
        //rb.constraints = RigidbodyConstraints.FreezePositionY;
        StartCoroutine(LoadSceneProcess());
    }

    void Update()
    {
        
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = true;

        float timer;
        while (!op.isDone)
        {
            yield return null;

            Vector3 wavePos = wave.transform.position;

            if (op.progress < 0.9f)
            {
                wave.transform.position = new Vector3(0, wave.transform.position.y + op.progress, 0);
            }
            else
            {
                timer = Time.deltaTime * 0.1f;
                wave.transform.position = new Vector3(0, Mathf.Lerp(wave.transform.position.y, 11, timer), 0);
                if (wavePos.y >= 10)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
