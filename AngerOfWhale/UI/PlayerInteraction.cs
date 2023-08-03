using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // Variable
    private Ray ray;
    private Vector3 directPos;

    [SerializeField]
    private bool attack;
    [SerializeField]
    private bool dive;

    // Property 
    [HideInInspector]
    public Vector3 DirectPos { get { return directPos; } }
    public bool Attack { get {return attack;} set { attack = value; } }
    public bool Dive { get { return dive; } set { dive = value; } }

    public void OnPointerDown(PointerEventData eventData)
    {
        directPos = GetDirectPos(eventData.position);
        if (dive)
        {
            attack = true;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!dive && !attack)
            directPos = GetDirectPos(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        directPos = Vector3.zero;
        if(dive)
        {
            attack = true;
            dive = false;
        }
        else
        {
            attack = false;
            dive = true;
        }
    }
    private Vector3 GetDirectPos(Vector2 eventDataPos)
    {
        ray = Camera.main.ScreenPointToRay(eventDataPos);
        directPos = ray.GetPoint(Camera.main.transform.position.y);
        return directPos;
    }
}
