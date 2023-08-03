using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePerformance : MonoBehaviour
{
    public Character character;
    private Animator animator;
    [SerializeField]
    public Face face;
    [SerializeField]
    private SkinnedMeshRenderer dragonFace;

    private void Start()
    {
        animator = GetComponent<Animator>();   
    }

    public void init()
    {
        ChageFace(character.normalEmotion);
    }

    public void ChageFace(Emotion emotion)
    {
        dragonFace.material = face.GetFace(emotion);
    }

    public void Action(Act act)
    {
        animator.SetBool("Die", false);
        animator.SetBool("Rest", false);
        animator.SetBool("Talk", false);

        switch (act)
        {
            case Act.IDLE:
                animator.SetInteger("animation", 0);
                break;
            case Act.TALK:
                animator.SetBool("Talk", true);
                break;
            case Act.ROAR:
                animator.SetTrigger("Roar");
                break;
            case Act.JUMP:
                animator.SetTrigger("Jump");
                break;
            case Act.YES:
                animator.SetTrigger("Yes");
                break;
            case Act.REST:
                animator.SetBool("Rest", true);
                break;
            case Act.SHY:
                animator.SetTrigger("Shy");
                break;
            case Act.DIE:
                animator.SetBool("Die", true);
                break;
        }
    }
}
