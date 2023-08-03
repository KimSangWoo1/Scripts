using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_ItemControl : MonoBehaviour
{
    //Manager 싱글톤
    private FX_Manager FXM;
    private DataManager DM;

    //오브젝트 풀링
    [SerializeField]
    private ObjectPooling.FX_State fxState;
    
    //Component
    private ParticleSystem particle;
    private AudioSource audioSource;

    private void Awake()
    {
        //싱글톤
        FXM = FX_Manager.Instance;
        DM = DataManager.Instance;

        particle = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        if (DM.GetSFSXSound())
        {
            audioSource.mute = false;
        }
        else
        {
            audioSource.mute = true;
        }

    }

    void Update()
    {
        //끝났을 경우
        if (particle.isStopped)
        {
            if (fxState == ObjectPooling.FX_State.item)
            {
                FXM.FX_ItemPush(this.gameObject); //Push 및 active 설정
            }
            else if (fxState == ObjectPooling.FX_State.Health)
            {
                FXM.FX_HealthPush(this.gameObject); //Push 및 active 설정
            }
            else if(fxState == ObjectPooling.FX_State.Money)
            {
                FXM.FX_MoneyPush(this.gameObject); //Push 및 active 설정
            }
        }
    }
}
