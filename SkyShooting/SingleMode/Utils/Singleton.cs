using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static object _Lock = new object();
    private static T _Instance;
    public static T Instance
    {
        get
        {
            lock (_Lock) //Thread Safe
            {
                if (_Instance == null)
                {
                    //인스턴스 존재 여부 확인
                    _Instance = FindObjectOfType(typeof(T)) as T;
                    if (_Instance == null)
                    {
                        //인스턴스 생성
                        _Instance = new GameObject(typeof(T).ToString(), typeof(T)).AddComponent<T>();

                        /*
                        GameObject singleton = new GameObject();
                        _Instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString() + " (Singleton)";
                        */
                        DontDestroyOnLoad(_Instance);

                    }
                }
            }
            return _Instance;
        }
    }
}
