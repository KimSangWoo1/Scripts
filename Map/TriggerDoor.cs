using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDoor : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _vector2Int;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.Instance.Notify(EventType.MoveRoom, new EventData.MoveRoomData(_vector2Int));
    }
}
