using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [Range(0.1f, 20.0f)]
    public float heightSacle = 3.0f;
    [Range(0.1f, 40.0f)]
    public float detailScale = 9.0f;
    public bool waves = true;
    public float wavesSpeed = 5.0f;

    private Mesh mesh;
    private Vector3[] vertices;
    MeshCollider meshCollider;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        meshCollider = gameObject.GetComponent<MeshCollider>();
    }

    private void Update()
    {
        GenerateWave();
    }

    void GenerateWave()
    {
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
                CalculationMethod(j + i * 11, i);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }
        
    void CalculationMethod(int i, int j)
    {
        if (waves)
        {
            vertices[i].z = Mathf.PerlinNoise(
                Time.time / wavesSpeed +
                (vertices[i].x + transform.position.x) / detailScale,
                Time.time / wavesSpeed +
                (vertices[i].y + transform.position.y) / detailScale)
                * heightSacle;
            vertices[i].z -= j;
        }
        else
        {
            vertices[i].z = Mathf.PerlinNoise(
                (vertices[i].x + transform.position.x) / detailScale,
                (vertices[i].y + transform.position.y) / detailScale)
                * heightSacle;
            vertices[i].z -= j;
        }
    }
}
