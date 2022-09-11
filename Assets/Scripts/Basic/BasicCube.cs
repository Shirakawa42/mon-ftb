using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicCube
{
    public static readonly Vector3[] cubeVertices = new Vector3[8] {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f)
    };

    public static readonly int[,] cubeIndices = new int[6, 4] {
        {0, 3, 1, 2}, // back face
        {5, 6, 4, 7}, // front face
        {3, 7, 2, 6}, // top face
        {1, 5, 0, 4}, // bottom face
        {4, 7, 0, 3}, // left face
        {1, 2, 5, 6}  // right face
    };

    public static readonly Vector2[] cubeUvs = new Vector2[4] {
        new Vector2(0f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 0f),
        new Vector2(1f, 1f),
    };

    public static readonly Vector3[] faceChecks = new Vector3[6] {
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f)
    };
}
