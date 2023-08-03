using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcControl : MonoBehaviour
{
    public GameObject[] npcs;

    public void NpcsActiveOn()
    {
        for(int i=0; i < npcs.Length; i++)
        {
            if (i == 0)
            {
                npcs[0].SetActive(true);
            }
            else
            {
                npcs[i].transform.parent.gameObject.SetActive(true);
            }
        }
    }

    public void NpcsActiveOff(int index)
    {
        if (index==0)
        {
            npcs[0].SetActive(false);
        }
        else{
            npcs[index].transform.parent.gameObject.SetActive(false);
        }
    }
    public void NpcsActiveOff( )
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            if (i == 0)
            {
                npcs[i].transform.gameObject.SetActive(false);
            }
            else
            {
                npcs[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
