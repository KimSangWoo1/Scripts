using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControl : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        particle.Play(true);
    }

    private void Update()
    {
        if (particle.isStopped)
        {
            gameObject.SetActive(false);
        }
    }
}
