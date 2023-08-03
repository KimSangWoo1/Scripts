using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleFriend : WhaleBase
{
    private Vector3 startPosition;

    private float helpTime;
    private float lifeTime;
    private void Awake()
    {
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        helpTime = 0f;
    }
    void Start()
    {
        moveSpeed = 12f;
        lifeTime = 5f;
    }

    void Update()
    {
        helpTime += Time.deltaTime;
        if (helpTime < lifeTime)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
        }
        else
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        transform.position = startPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<ObstacleAttacked>().Attacked(2);
        }
    }


}
