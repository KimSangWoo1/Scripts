using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Map")]
    public Map map;

    [Header("벽들")]
    public GameObject upWall;
    public GameObject downWall;
    public GameObject leftWall;
    public GameObject rightWall;

    Vector3 euler;
    private float size;
    void Start()
    {
        size = map.scale;
        euler = map.transform.rotation.eulerAngles;

        //크기 설정
        upWall.transform.localScale = new Vector3(size * 10f, 10f, 1f);
        downWall.transform.localScale = new Vector3(size * 10f, 10f, 1f);
        leftWall.transform.localScale = new Vector3(size * 10f, 10f, 1f);
        rightWall.transform.localScale = new Vector3(size * 10f, 10f, 1f);

        //각도 설정
        transform.rotation = Quaternion.Euler(euler);

        //위치 설정
        upWall.transform.position = new Vector3(0f, 5f, map.rend.bounds.extents.z);
        downWall.transform.position = new Vector3(0f, 5f, -map.rend.bounds.extents.z);
        leftWall.transform.position = new Vector3(-map.rend.bounds.extents.x, 5f, 0f);
        rightWall.transform.position = new Vector3(map.rend.bounds.extents.x, 5f, 0f);

        //Active
        upWall.SetActive(true);
        downWall.SetActive(true);
        leftWall.SetActive(true);
        rightWall.SetActive(true);

    } 
}
