using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlaneRender : MonoBehaviourPun
{
    [SerializeField]
    private Material[] materials;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(ChangePlaneMat), RpcTarget.All, GameManager.planeNumber);
        }
    }


    [PunRPC]
    private void ChangePlaneMat(int num)
    {
        meshRenderer.material = materials[num];
    }
}
