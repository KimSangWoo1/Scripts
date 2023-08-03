using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shuffle
{
    public static List<GameObject> ShufflEnmey<T>(List<GameObject> list, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, list.Count);
            GameObject tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }
        return list;
    }
}
