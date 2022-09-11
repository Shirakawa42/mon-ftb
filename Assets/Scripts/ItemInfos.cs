using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfos
{
    public int id;
    public string name;
    public bool opaque = true;
    public int[] faces = new int[6];

    public ItemInfos(int id, string name, bool opaque, int topFace, int bottomFace, int leftFace, int rightFace, int frontFace, int backFace)
    {
        this.id = id;
        this.name = name;
        this.opaque = opaque;
        this.faces[0] = backFace;
        this.faces[1] = frontFace;
        this.faces[2] = topFace;
        this.faces[3] = bottomFace;
        this.faces[4] = leftFace;
        this.faces[5] = rightFace;
    }
}
