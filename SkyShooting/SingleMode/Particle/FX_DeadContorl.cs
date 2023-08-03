using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_DeadContorl : MonoBehaviour
{
    private FX_Manager FXM;
    private DataManager DM;

    [SerializeField]
    private ObjectPooling.DeadState deadState;

    private ParticleSystem particleSystem;
    private AudioSource audioSource;

    private void Awake()
    {
        //싱글톤 생성
        FXM = FX_Manager.Instance;
        DM = DataManager.Instance;

        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        particleSystem.Play();
        audioSource.Play();
    }
    void Start()
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
        if (particleSystem.isStopped)
        {
            FXM.FX_Push(this.gameObject, deadState); //Push 및 active 설정
        }
    }
}
