using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Flow : MonoBehaviour
{
    public Transform player;

    [Header("카메라")]
    //카메라 감도
    [SerializeField]
    private float sensitivity;
    //카메라 속도
    [SerializeField]
    private float camSpeed;
    //카메라 높이 
    [SerializeField]
    private float height;

    private AudioListener audioListener;
    
    private void Awake()
    {
        camSpeed = 50f;
        height = 60f;
    }
    private void Start()
    {
        audioListener = GetComponent<AudioListener>();
    }

    private void LateUpdate()
    {

        Vector3 camPosition = new Vector3(player.transform.position.x, height, player.transform.position.z);
        //this.transform.position = Vector3.MoveTowards(this.transform.position, camPosition,Time.deltaTime * camSpeed);
        this.transform.position = Vector3.Slerp(this.transform.position, camPosition, Time.deltaTime * camSpeed);

        if (!player.gameObject.activeSelf)
        {
            audioListener.enabled = true;
        }
    }
}
