using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENodeText
{
    number,
    g,
    h,
    f,
    end
}
public enum ETestColor
{
    green,
    red,
    blue
}
public class Node : StorageManager
{
    public bool isBlock;
    public GameObject objNode;

    [HideInInspector]
    public int x, y, g;
    [HideInInspector]
    public float h;
    public float f { get { return g + h; } }
    public Node parentNode;

    public MeshRenderer testMeshRenderer;
    
    public Dictionary<ENodeText, TextMesh> textMeshDict;

    public Vector3 pos;

    Dictionary<ETestColor, Color> testColor;



    // g 이동했던 거리
    // h 목표까지 거리
    // f = g + h  
    // f가 같은 값이면 h 가 작은 것을 택한다


    void Awake()
    {
        textMeshDict = new Dictionary<ENodeText, TextMesh>();

        TextMesh[] temp = transform.GetComponentsInChildren<TextMesh>();

        for (int i = 0; i < (int)ENodeText.end; i++)
        {
            if (temp[i])
                textMeshDict.Add((ENodeText)i, temp[i]);
        }

        testColor = new Dictionary<ETestColor, Color>();

        testColor.Add(ETestColor.green, new Color(0, 255 / 255f, 0, 50 / 255f));
        testColor.Add(ETestColor.red, new Color(255 / 255f, 0, 0, 50 / 255f));
        testColor.Add(ETestColor.blue, new Color(0, 0, 255 / 255f, 50 / 255f));

        isBlock = false;

        TextMeshActive(true);
        TestColorSet(ETestColor.green);

        pos = transform.position;

    }

    private void Start()
    {
        AddObstacle();
    }

    public override void AddChamp(in Champion _champ)
    {
        base.AddChamp(in _champ);

        SwapChamp(this);
    }
    public void TextMeshActive(bool isActive)
    {
        foreach (KeyValuePair<ENodeText, TextMesh> item in textMeshDict)
            item.Value.gameObject.SetActive(isActive);

        testMeshRenderer.gameObject.SetActive(isActive);

        objNode.SetActive(isActive);
    }
    public void TestColorSet(ETestColor color)
    {
        testMeshRenderer.material.color = testColor[color];
    }
    public void CostTextSet()
    { 
        textMeshDict[ENodeText.f].text = f.ToString("N1");
        textMeshDict[ENodeText.g].text = "" + g;
        textMeshDict[ENodeText.h].text = h.ToString("N1");
    }

    public void Obstacle(bool isActive)
    {
        obstacle.animator.SetBool("Active", isActive);
        isBlock = isActive;
    }



}
