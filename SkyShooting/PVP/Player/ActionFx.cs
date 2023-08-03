using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionFx : MonoBehaviourPun
{
    [SerializeField]
    private FX_SoundControl[] hits;
    [SerializeField]
    private ParticleSystem[] smokes;
    [SerializeField]
    private FX_SoundControl[] deads;

    int myNumber;

    public void SetMyNumber(int number)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_SetMyNumber), RpcTarget.All, number);
        }
    }

    public void Damage()
    {
        hits[myNumber].gameObject.SetActive(true);
        smokes[myNumber].gameObject.SetActive(true);

        if (!hits[myNumber].IsPlaying())
        {
            hits[myNumber].Play();
        }
        if (!smokes[myNumber].isPlaying)
        {
            smokes[myNumber].Play();
        }

        StartCoroutine(nameof(PlayFx));
    }

    public void Dead()
    {
        deads[myNumber].transform.parent = null;
        deads[myNumber].gameObject.SetActive(true);
    }

    IEnumerator PlayFx()
    {
        yield return new WaitUntil( ()=> !smokes[myNumber].isPlaying);

        hits[myNumber].gameObject.SetActive(false);
        smokes[myNumber].gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_SetMyNumber(int number)
    {
        myNumber = number;
    }
}

