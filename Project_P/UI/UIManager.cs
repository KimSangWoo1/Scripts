using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private Stack<IWindow> _uiStack = new Stack<IWindow>();
    public int StackCount => _uiStack.Count;

    private Transform _uiRepository;
    public Transform UIReposotory { get => _uiRepository ?? FindObjectOfType<UIRepository>().transform; set => _uiRepository = value; }

    private const string FloatDamage = "FloatingDamage";
    #region Open Close
    public async void Open<T>(eUIType uiType) where T : IWindow
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsSceneStart == true);
        GameObject uiObject = await AddressableManager.Instance.InstanceObject<GameObject>(uiType.ToString(), UIReposotory);
        Debug.Log($"[UI] {uiType} Open");

        T ui = uiObject.GetComponent<T>();
        ui.Initialize();
        Push(ui);
    }

    public async UniTask AsyncOpen<T>(eUIType uiType) where T : IWindow
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsSceneStart == true);
        GameObject uiObject = await AddressableManager.Instance.InstanceObject<GameObject>(uiType.ToString(), UIReposotory);
        Debug.Log($"[UI] {uiType} Open");

        T ui = uiObject.GetComponent<T>();
        ui.Initialize();
        Push(ui);
    }

    public void SafeOpen<T>(eUIType uiType) where T : IWindow
    {
        if (_uiStack.Count > 0)
        {
            var peekUi = _uiStack.Peek();
            if (uiType != peekUi.GetUIType())
            {
                Open<T>(uiType);
            }
            else
            {
                peekUi.Close();
                Open<T>(uiType);
            }
        }
        else
        {
            Open<T>(uiType);
        }
    }

    public async void NoticeOpen(eUIType uiType, eNoticeType noticeType) 
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsSceneStart == true);
        GameObject uiObject = await AddressableManager.Instance.InstanceObject<GameObject>(uiType.ToString(), UIReposotory);
        Debug.Log($"[Notice] {uiType} Open");

        NoticeMessagePresenter noticeMessage = uiObject.GetComponent<NoticeMessagePresenter>();
        noticeMessage.Initialize();
        noticeMessage.Open();
        noticeMessage.SetMessage(noticeType);
    }

    public void Close<T>(eUIType uiType, bool isInputChange = true) where T : IWindow
    {
        while (_uiStack.Count>0)
        {
            var window = Pop(isInputChange) as UIBase;
            var windowType = window.GetUIType();
            window?.Destroy();

            Debug.Log($"[UI] {windowType} Close");
            if (windowType == uiType)
            {
                break;
            }
        }
    }

    public void Show<T>(eUIType uiType) where T : IWindow
    {
        for (int i = 0; i <_uiStack.Count; i++)
        {
            var ui = _uiStack.Peek();
            if(uiType == ui.GetUIType())
            {
                ui.Show();
                break;
            }
            else
            {
                _uiStack.Pop().Destroy();
            }
        }
    }

    public void ShowDamage(Vector3 position, int damage)
    {
        PoolParams damageParams;
        damageParams.Tag = FloatDamage;
        damageParams.Owner = gameObject;
        damageParams.Position = position;
        damageParams.Rotation = Quaternion.identity;
        damageParams.Parent = null;

        var floatingDamage = ObjectPoolManager.Instance.GetFromPool(damageParams) as FloatingDamage;
        floatingDamage.SetDamage(damage);
    }

    public void Hide<T>(eUIType uiType) where T : IWindow
    {
        for (int i = 0; i < _uiStack.Count; i++)
        {
            var ui = _uiStack.Peek();
            if (uiType == ui.GetUIType())
            {
                ui.Hide();
                break;
            }
        }
    }

    #endregion

    #region Push & Pop
    private void Push<T>(T window) where T : IWindow
    {
        if (_uiStack.Count == 0)
        {
            EventManager.Instance.Notify(EventType.InputChanged, new EventData.InputChageData(eActionMapType.UI));
        }
        else
        {
            _uiStack.Peek()?.Hide();
        }

        if (!_uiStack.Contains(window))
        {
            window.Open();
            _uiStack.Push(window);
        }
        else
        {
            window.Destroy();
        }
    }

    private IWindow Pop(bool isInputChange)
    {
        IWindow window = null;
        if (_uiStack.Count > 0)
        {
            window = _uiStack.Pop();
        }

        if (_uiStack.Count == 0)
        {
            if (isInputChange)
            {
                EventManager.Instance.Notify(EventType.InputChanged, new EventData.InputChageData(eActionMapType.Player));
                InteractionCheck();
            }
        }
        else
        {
            _uiStack.Peek().Show();
        }
        return window;
    }
    #endregion

    #region input - UI Open
    public void OpenOption(InputAction.CallbackContext context)
    {
        AudioManager.Instance.PausePlaySFX();
        SafeOpen<OptionPresenter>(eUIType.Option);
    }

    public void OpenBook(InputAction.CallbackContext context)
    {
        Open<BookPresenter>(eUIType.Book);
    }
    #endregion

    public void Clear()
    {
        for(int i=0; i<_uiStack.Count; i++)
        {
            var window = _uiStack.Pop();
            if(window != null)
            {
                window.Destroy();
            }
        }
        _uiStack.Clear();
    }

    // Player가 Interaction안에 들어와있는지 체크 (현재 한 곳에서 Input 기능 건드는게 아니라서 임시로 여기서 더블 체크)
    private void InteractionCheck()
    {
        if (GameManager.Instance.IsInPlayerInteraction)
        {
            EventManager.Instance.Notify(EventType.InputEnable, new EventData.InputEnableData(eActionMapType.Player, KeyType.Attack.ToString(), false));
        }
    }
}

public interface IWindow
{
    void Initialize();
    void Open();
    void Close();
    void Show();
    void Hide();
    void Destroy();
    void SetParent(Transform parent);
    eUIType GetUIType();
}

public interface IEventRegister
{
    void RegisterEvent();
    void UnregisterEvent();
}

public interface IMessageReceiver
{
    void ReceiveMsg(IMessage msg);
}

public interface IMessage
{

}

public struct UIMsg : IMessage
{
    public eUIEventType uiEventType;

    public UIMsg(eUIEventType uiEventType) : this()
    {
        this.uiEventType = uiEventType;
    }
}
