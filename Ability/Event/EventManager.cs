using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �̹� ���Ǵ� �κе��� �ֱ� ������ ��ü������ �����ϱ�� �����
///  private readonly Dictionary<EventType, Action<EventType, Component, object[]>> _eventMap = new(); ����
///  private readonly Dictionary<EventType, Action<EventType, IEventData>> _eventMap = new(); �� �ٲ���ϴ�.
///  ������ ���� IEventData �κи� �����Ͽ� �ڵ� Ž��, ������ �м�, ����� ���� �� �� �ֵ��� �����߽��ϴ�.  
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