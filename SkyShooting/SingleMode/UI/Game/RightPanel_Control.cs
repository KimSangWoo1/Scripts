using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RightPanel_Control : MonoBehaviour, IPointerDownHandler, IPointerUpHandler ,IDragHandler
{
    public Image fireButton;

    [HideInInspector]
    public bool fire; //발사
    [HideInInspector]
    public bool buster; //가속 

    private Vector3 origin_FirePosition; //초기 발사버튼 위치
    
    private RectTransform panel;
    private RectTransform canvas;
    
    private float panel_HalfWidth;
    
    void Start()
    {
        //Right Panel 크기를 Canvas 크기에 따라 변경한다.
        canvas = this.transform.parent.gameObject.GetComponent<RectTransform>();        
        panel =  GetComponent<RectTransform>();
        //Canvas 넓이 반 설정
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.rect.width / 2);
        //Canvas높이와 동일하게 설정
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.rect.height);
        //Panel 넓이 반
        panel_HalfWidth = panel.sizeDelta.x /2;
        //총 버튼 초기 위치
        origin_FirePosition = fireButton.rectTransform.position;

    }

    public void OnPointerDown(PointerEventData eventData)
    { 
        //부스터 버튼 or Other UI를 걸러내자
        GameObject temp = eventData.pointerCurrentRaycast.gameObject;
        //부스터 버튼일경우
        if (temp.name =="BusterButton" )
        {
            buster = true;
            eventData.selectedObject = null;
        }
        // Other UI 일경우: 총 버튼 위치 이동
        else
        {
            //Right 판넬 넘을경우
            if (-panel_HalfWidth > fireButton.rectTransform.anchoredPosition.x)
            {
                fire = false;
                fireButton.rectTransform.position = origin_FirePosition;
            }
            //Right 판넬안에 정상 터치 할 경우
            else
            {
                fire = true;        
                eventData.selectedObject = fireButton.gameObject;
                fireButton.rectTransform.position = eventData.position;
            }
        } 
    }

    //총 버튼 위치 드래그 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.selectedObject != null)
        {
            //Right 판넬 넘을경우
            if(-panel_HalfWidth >= fireButton.rectTransform.anchoredPosition.x)
            {
                eventData.selectedObject = null;
                OnPointerUp(eventData);
            }
            //드래그 위치에 발사 버튼을 위치시킨다.
            else
            {              
                fireButton.rectTransform.position = eventData.position;
            }         
        }
    }

    //총 버튼 위치 초기화
    public void OnPointerUp(PointerEventData eventData)
    {
        fire = false;
        buster = false;
        fireButton.rectTransform.position = origin_FirePosition;
    }

}
