using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeList : MonoBehaviour
{
    public Material textureMaterial;
    public readonly ItemInfos[] infosFromId = {
        new ItemInfos(0, "air", false, 0, 0, 0, 0, 0, 0),
        new ItemInfos(1, "dirt", true, 1, 1, 1, 1, 1, 1),
        new ItemInfos(2, "grass", true, 7, 1, 2, 2, 2, 2),
        new ItemInfos(3, "stone", true, 0, 0, 0, 0, 0, 0),
    };
}

enum Items
{
    air,
    dirt,
    grass,
    stone
}