using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeList
{
    public static readonly ItemInfos[] infosFromId = {
        new ItemInfos(0, "air", false, false, 0, 0, 0, 0, 0, 0),
        new ItemInfos(1, "dirt", true, true, 1, 1, 1, 1, 1, 1),
        new ItemInfos(2, "grass", true, true, 7, 1, 2, 2, 2, 2),
        new ItemInfos(3, "stone", true, true, 0, 0, 0, 0, 0, 0),
        new ItemInfos(4, "glass", false, true, 3, 3, 3, 3, 3, 3),
        new ItemInfos(5, "wood", true, true, 6, 6, 5, 5, 5, 5),
        new ItemInfos(6, "leaves", false, true, 16, 16, 16, 16, 16, 16),
    };
}

enum Items
{
    air,
    dirt,
    grass,
    stone,
    glass,
    wood,
    leaves
}