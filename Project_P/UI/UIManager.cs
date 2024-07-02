using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private Stack<IWindow> _uiStack = new Stack<IWindow>();

    private Transform _uiRepository;
    public Transform UIReposotory { get => _uiRepository ?? FindObjectOfType<UIRepository>().transform; set => _uiRepository = value; }

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

    public void Close<T>(eUIType uiType) where T : IWindow
    {
        while (_uiStack.Count>0)
        {
            var window = Pop() as UIBase;
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

    private IWindow Pop()
    {
        IWindow window = null;
        if (_uiStack.Count > 0)
        {
            window = _uiStack.Pop();
        }

        if (_uiStack.Count == 0)
        {
            EventManager.Instance.Notify(EventType.InputChanged, new EventData.InputChageData(eActionMapType.Player));
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
        Open<OptionPresenter>(eUIType.Option);
    }

    public void OpenBook(InputAction.CallbackContext context)
    {
        Open<BookView>(eUIType.Book);
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

    #region 임시 
    public GameObject GetPeek()
    {
        if (_uiStack.Count > 0)
        {
            var ui = (UIBase)_uiStack.Peek();
            return ui.gameObject;
        }
        else
        {
            return null;
        }
    }
    #endregion
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

public interface IEventView
{
    void RegisterEvent();
    void UnregisterEvent();

    void ReceiveMsg(IMessage msg);
}

public interface IMessage
{

}
