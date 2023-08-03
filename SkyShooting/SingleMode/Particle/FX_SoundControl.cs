using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_SoundControl : MonoBehaviour
{
    //Manager
    private DataManager DM;
    
    //Component
    private ParticleSystem particle;
    private AudioSource audioSource;

    //Variable
    private float soundTime;
    private bool particlePlay;

    void Awake()
    {
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

    /*
        private void Update()
        {
            //파티클 연출 체크
            if (particle.isPlaying )
            {
                particlePlay = true;
            }
            else
            {
                particlePlay = false;
            }

            //파티클 연출시 오디오 실행
            if (particlePlay)
            {
                soundTime += Time.deltaTime;

                if (particle.main.duration < soundTime)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
            else
            {
                soundTime = 0f;
                audioSource.Stop();
            }
        }
            */

    public void Play()
    {
        particle.Play();
        audioSource.Play();
    }

    public bool IsPlaying()
    {
        return particle.isPlaying;
    }

    private void OnDisable()
    {
        particlePlay = false;
    }
}
