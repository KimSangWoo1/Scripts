using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 이미 사용되는 부분들이 있기 때문에 전체적으로 수정하기는 어려워
///  private readonly Dictionary<EventType, Action<EventType, Component, object[]>> _eventMap = new(); 에서
///  private readonly Dictionary<EventType, Action<EventType, IEventData>> _eventMap = new(); 로 바꿨습니다.
///  데이터 전달 IEventData 부분만 수정하여 코드 탐색, 데이터 분석, 디버깅 등을 할 수 있도록 변경했습니다.  
/// </summary>
public class EventManager : Singleton<EventManager>
{
    private readonly Dictionary<EventType, Action<EventType, IEventData>> _eventMap = new();

    public void Subscribe(EventType type, Action<EventType, IEventData> action)
    {
        if (!_eventMap.ContainsKey(type))
        {
            _eventMap[type] = action;
            return;
        }
        
        _eventMap[type] += action;
    }
    
    public void Unsubscribe(EventType type, Action<EventType, IEventData> action)
    {
        if (!_eventMap.ContainsKey(type)) return;
        _eventMap[type] -= action;
    }

    public void Notify(EventType type, IEventData data)
    {
        if (_eventMap.TryGetValue(type, out var action)) action.Invoke(type, data);
    }
}