using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShildControl : MonoBehaviour
{
    private Renderer render;

    public float shildBrokenSpeed; //보호막 부서짐 속도
    private float shildBrokenValue; //보호막 보호막 값

    private float disolveMin;
    private float disolveMax;

    void Awake()
    {
        render = GetComponent<Renderer>();

        disolveMin = -0.3f;
        disolveMax = 0.15f;
    }
    private void OnEnable()
    {
        shildBrokenValue = disolveMax;
    }


    public void OnBroken(bool active)
    {
        StartCoroutine(SheildBroken(active));
    }
    //��ȣ�� ����
    IEnumerator SheildBroken(bool active)
    {
        while (shildBrokenValue >= disolveMin)
        {
            shildBrokenValue = Mathf.Lerp(shildBrokenValue, disolveMin-0.01f, Time.smoothDeltaTime * shildBrokenSpeed);
            render.material.SetFloat("_Displacement", shildBrokenValue); // 0.6f ~ -0.2f DisolveEdgeThickness
            yield return null;
        }
        shildBrokenValue = disolveMax;
        render.material.SetFloat("_Displacement", disolveMax);
        if (!active)
        {
            gameObject.SetActive(false);
        }
    }
}
