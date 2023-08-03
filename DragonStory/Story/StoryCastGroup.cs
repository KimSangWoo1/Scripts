using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCastGroup : MonoBehaviour
{
    public Animator[] actorAnimators;
    public SkinnedMeshRenderer[] actorFaces;
    private int index;

    private NpcControl npcControl;
    //private List<int> npcIndex = new List<int>();
    private void OnEnable()
    {
        npcControl = GameObject.FindObjectOfType<NpcControl>();
        //npcIndex.Clear();

        npcControl.NpcsActiveOff();
        /*
        for (int i=0; i<npcControl.npcs.Length; i++)
        {
            for(int j=0; j < actorAnimators.Length; j++)
            {
                if(actorAnimators[j].gameObject.tag.Equals(npcControl.npcs[i].tag)){
                    npcControl.NpcsActiveOff(i);
                    //npcIndex.Add(i);
                }
            }
        }
        */
    }

    public void ActorReady(Actor actor)
    {
        for(int i=0; i< actorAnimators.Length; i++)
        {
            if (actorAnimators[i].gameObject.tag.Equals(actor.ToString()))
            {
                index = i;
            }
        }
    }

    public void SetEmotion(Material material)
    {
        actorFaces[index].material = material;
    }

    public void ActionReady(Act act)
    {
        actorAnimators[index].SetBool("Die", false);
        actorAnimators[index].SetBool("Rest", false);
        actorAnimators[index].SetBool("Talk", false);
        //actorAnimators[index].ResetTrigger("Roar");
        //actorAnimators[index].ResetTrigger("Jump");
        //actorAnimators[index].ResetTrigger("Yes");
        //actorAnimators[index].ResetTrigger("Shy");

        switch (act)
        {
            case Act.IDLE:
                actorAnimators[index].SetInteger("animation", 0);
                break;
            case Act.TALK:
                actorAnimators[index].SetBool("Talk", true);
                break;
            case Act.ROAR:
                actorAnimators[index].SetTrigger("Roar");
                break;
            case Act.JUMP:
                actorAnimators[index].SetTrigger("Jump");
                break;
            case Act.YES:
                actorAnimators[index].SetTrigger("Yes");
                break;
            case Act.REST:
                actorAnimators[index].SetBool("Rest", true);
                break;
            case Act.SHY:
                actorAnimators[index].SetTrigger("Shy");
                break;
            case Act.DIE:
                actorAnimators[index].SetBool("Die", true);
                break;
        }
    }


    public void StoryEnd()
    {
        for (int i = 0; i < actorAnimators.Length; i++)
        {
            actorAnimators[i].gameObject.SetActive(false);
        }

        npcControl.NpcsActiveOn();

        Destroy(gameObject);
    }
}
