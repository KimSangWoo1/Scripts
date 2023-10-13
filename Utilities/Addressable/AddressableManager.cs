using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class AddressableManager : Singleton<AddressableManager>
{
    AsyncOperationHandle<IList<UnityEngine.Object>> loadHandle;

    #region Download
    public void Download(IList<string> keys, Action onEvent = null)
    {
        Debug.Log("Resource Download Start");
        AsyncOperationHandle<long> opSizeHandle = Addressables.GetDownloadSizeAsync(keys);
        opSizeHandle.Completed += (opSize) =>
        {
            if (opSize.Status == AsyncOperationStatus.Succeeded)
            {
                long updateLabelSize = opSize.Result;
                if (updateLabelSize > 0)
                {
                    Debug.Log("[다운로드] Size : " + updateLabelSize);
                    StartCoroutine(coDownload(keys));
                }
                else
                {
                    Debug.Log("[다운로드] 할 항목 없음");
                }
            }
        };
        Addressables.Release(opSizeHandle);
        Debug.Log("Resource Download End");
        onEvent?.Invoke();
    }

    //다운로드
    private IEnumerator coDownload(IList<string> keys)
    {
        var opDownloadHandle = Addressables.DownloadDependenciesAsync(keys, false);
        while (!opDownloadHandle.IsDone || opDownloadHandle.Status == AsyncOperationStatus.None)
        {
            float percent = opDownloadHandle.GetDownloadStatus().Percent;
            Debug.Log($"{percent * 100} / {100} %");
            yield return new WaitForEndOfFrame();
        }
        Debug.Log($"[다운로드]  종료 : {opDownloadHandle.IsDone} , 결과 : {opDownloadHandle.Status}");

        Addressables.Release(opDownloadHandle);
        Debug.Log("[다운로드] Proccess End");
    }

    public void DownloadCheck(string key)
    {
        AsyncOperationHandle<long> opSizeHandle = Addressables.GetDownloadSizeAsync(key);
        opSizeHandle.Completed += (opSize) =>
        {
            if (opSize.Status == AsyncOperationStatus.Succeeded)
            {
                long updateLabelSize = opSize.Result;
                if (updateLabelSize > 0)
                {
                    Debug.Log(key + " -> Download Size : " + updateLabelSize);
                    //StartCoroutine(coDownLoad(key));
                }
                else
                {
                    Debug.Log($"{key} last version");
                }
            }
        };
        Addressables.Release(opSizeHandle);
    }

    #endregion

    #region Load Object
    public void LoadObjects<T>(IList<string> keys, Addressables.MergeMode mergeMode) where T : UnityEngine.Object
    {
        Debug.Log($"[Addressable Load] 시작, 머지모드 : {mergeMode}");

        loadHandle = Addressables.LoadAssetsAsync<UnityEngine.Object>(keys,
                addressables =>
                {
                    if (typeof(GameObject) == addressables.GetType())
                    {

                    }
                }, mergeMode, true);

        Debug.Log("[Addressable Load] End");
    }

    public async UniTask<T> LoadObject<T>(string key) where T : UnityEngine.Object
    {
        T Tobject = null;
        var handle = Addressables.LoadAssetAsync<T>(key);

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                Tobject = op.Result;
                if (Tobject != null)
                {
                    Debug.Log($"[Addressable Asset Load] Asset Load Succeseded  {key}");
                }
            }
            else
            {
                Debug.LogWarning($"[Addressable Asset Load] Asset Load Fail : {key} {op.Status}");
            }
        };
        await handle.Task;
        Addressables.Release(handle);
        return Tobject;
        //return handle.Result;
    }

    #endregion

    #region Load Data
    public async UniTask<List<T>> LoadDataList<T>(IList<string> dataList, Action callback = null) where T: UnityEngine.Object
    {
        List<T> resultList = new List<T>();

        for (int i = 0; i < dataList.Count; i++)
        {
            string dataKey = dataList[i];
            var result = await LoadData<T>(dataKey);
            resultList.Add(result);

        }
        callback?.Invoke();

        return resultList;
    }

    public async UniTask<T> LoadData<T>(string key) where T : UnityEngine.Object
    {
        T data = null;
        bool isValid = await CheckResourcePath(key);
        if (!isValid)
        {
            Debug.LogWarning("[Addressable Load] Key Not Found Addressable Path");
            return data;
        }

        AsyncOperationHandle<T> loadDataHandle = Addressables.LoadAssetAsync<T>(key);
        loadDataHandle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                data = op.Result;
                if (data != null)
                {
                    Debug.Log($"[Addressable Load] Data Load Succeseded  {key}");
                }
            }
            else
            {
                Debug.LogWarning($"[Addressable Load] Data Load Fail : {key} {op.Status}");
            }
        };
        await loadDataHandle.Task;
        Addressables.Release(loadDataHandle);
        return data;
    }

    #endregion

    #region Instace Object
    public async UniTask<T> InstanceObject<T>(string key, Transform parent = null) where T : UnityEngine.Object
    {
        var handle = Addressables.InstantiateAsync(key, parent);
        var gameObject = await handle.Task;

        var addressableObejct = gameObject.GetComponent<AddressableObject>();
        if(addressableObejct == null)
        {
            addressableObejct = gameObject.AddComponent<AddressableObject>();
        }
        addressableObejct.Handle = handle;
        return handle.Result as T;
    }
    #endregion

    #region Patch 
    public void Patch()
    {
        StartCoroutine(coPatch());
    }
    public IEnumerator coPatch()
    {
        Debug.Log("[업데이트] 시작");

        List<string> catalogsToUpdate = new List<string>();
        AsyncOperationHandle<List<string>> checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
        checkForUpdateHandle.Completed += op =>
        {
            catalogsToUpdate.AddRange(op.Result);
        };
        yield return checkForUpdateHandle;

        if (catalogsToUpdate.Count > 0)
        {
            Debug.Log("[업데이트] 항목 있음");

        }
        else
        {
            Debug.Log("[업데이트] 항목 없음");
        }
    }
    #endregion


    public async UniTask<long> CheckSize(IList<string> keys)
    {
        long size = 0L;
        AsyncOperationHandle<long> opSizeHandle = Addressables.GetDownloadSizeAsync(keys);
        opSizeHandle.Completed += (opSize) =>
        {
            if (opSize.Status == AsyncOperationStatus.Succeeded)
            {
                size = opSize.Result;
                if (size <= 0)
                {
                    Debug.Log("[Addressable] 다운로드 할 항목 없음");
                }
            }
        };
        await opSizeHandle.Task;
        return size;
    }

    private async UniTask<bool> CheckResourcePath(string key)
    {
        bool result = false;
        var operation = Addressables.LoadResourceLocationsAsync(key);
        operation.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                if (op.Result.Count > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
        };
        await operation.Task;

        return result;
    }
}
