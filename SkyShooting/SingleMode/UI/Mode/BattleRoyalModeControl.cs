using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyalModeControl : MonoBehaviour
{
    [Header("Photon Survival Mode")]
    [SerializeField]
    private List<GameObject> planeList;

    [SerializeField]
    private List<GameObject> bullletList;


    [SerializeField]
    private float shootTime;


    [SerializeField]
    private float actionTime;
    [SerializeField]
    private float moveTime;

    [SerializeField]
    private int bulletIndex;

    [SerializeField]
    private int plneIndex;

    private void OnEnable()
    {
        plneIndex = 0;
        bulletIndex = 0;
        shootTime = 0f;
    }
    void Update()
    {
        BulletMovement();
        PlaneMovement();
    }

    private void BulletMovement()
    {
        shootTime += Time.deltaTime;

        if (shootTime >= actionTime)
        {
            shootTime = 0f;
            bullletList[bulletIndex].SetActive(true);
            bulletIndex++;
        }

        if (bulletIndex >= bullletList.Count)
        {
            bulletIndex = 0;
        }
    }

    private void PlaneMovement()
    {
        moveTime += Time.deltaTime;
        if (moveTime >= actionTime)
        {
            moveTime = 0f;
            planeList[plneIndex].SetActive(true);
            plneIndex++;
        }

        if (plneIndex >= planeList.Count)
        {
            plneIndex = 0;
        }
    }

    private void OnDisable()
    {
        for(int i=0; i<bullletList.Count; i++)
        {
            bullletList[i].SetActive(false);
        }

        for (int i = 0; i < planeList.Count; i++)
        {
            planeList[i].SetActive(false);
        }
    }
}
