using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour,IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Transform stick;
    public RectTransform circle;
    [HideInInspector]
    public bool move; //이동

    private Vector3 circleFirstPos; // 원 초기 위치
    private Vector3 stickFirstPos; // 스틱 초기 위치
    private Vector3 direct;  // 이동 방향(회전 방향)
    private Vector2 playerDirect; //플레이어 이동 방향
    private float radius; //반경
    private float canvasWidth; //Canvas 넓이
    void Start()
    {
        //초기 위치 설정
        stickFirstPos = stick.transform.position;
        circleFirstPos = circle.transform.position;
        //반경공식
        radius = circle.sizeDelta.y * 0.5f;
        canvasWidth = transform.parent.GetComponent<RectTransform>().localScale.x;
        radius *= canvasWidth;
    }

    public Vector2 getDirection()
    {
        return playerDirect;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Circle 터치 위치로
        circle.transform.position = eventData.position;
        //스틱 회전 기능 ON
     //   stick.GetComponent<PowerUpRotation>().enabled = true;
        move = true; //이동 ON
    }

    public void OnDrag(PointerEventData eventData)
    {
        //위치
        Vector3 pos = eventData.position;
        playerDirect = pos - stickFirstPos;
        //방향
        direct = (pos - stickFirstPos).normalized;
        //거리
        float distance = Vector3.Distance(pos, stickFirstPos);

        //드래그하는 위치가 반경 밖이냐 안이냐에 따라 스틱 위치 조정
        if (distance < radius)
        {
            stick.position = circle.transform.position + direct * distance;
        }
        else
        {
            stick.position = circle.transform.position + direct * radius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //스틱 회전 기능 Off
        stick.GetComponent<PowerUpRotation>().enabled = false;
        move = false; //이동 OFF
        //초기화
        circle.transform.position = circleFirstPos;
        stick.position = stickFirstPos;
        direct = Vector3.zero;
    }
}
