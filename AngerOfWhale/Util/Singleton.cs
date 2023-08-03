using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                //인스턴스 존재 여부 확인
                _Instance = FindObjectOfType(typeof(T)) as T;
                if (_Instance == null)
                {
                    //인스턴스 생성
                    _Instance = new GameObject(typeof(T).ToString(), typeof(T)).AddComponent<T>();
                    DontDestroyOnLoad(_Instance);
                }
            }
            return _Instance;
        }
    }
}
