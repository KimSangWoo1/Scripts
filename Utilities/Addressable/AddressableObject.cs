using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableObject : MonoBehaviour
{
    public AsyncOperationHandle<GameObject> Handle { get; set; }

    private void OnDestroy()
    {
        if (Handle.IsValid())
        {
            Addressables.ReleaseInstance(Handle);
        }
    }
}
