using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIBase : MonoBehaviour, IWindow
{
    [SerializeField]
    protected eUIType _uiType;

    public eUIType GetUIType() => _uiType;

    #region UI - Essential Method
    public virtual void Initialize() { }

    public virtual void Open() { }

    public virtual void Close() 
    {
        UIManager.Instance?.Close<UIBase>(_uiType);
    }

    public virtual void Show()
    {
        RegisterInput();
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        UnRegisterInput();
        this.gameObject.SetActive(false);
    }

    public virtual void Back() { }
    #endregion

    #region UI - Utils
    public virtual void Destroy() 
    {
        Destroy(this.gameObject);
    }

    public virtual void SetParent(Transform parent)
    {
        this.transform.parent = parent;
    }
    #endregion

    #region Event Register
    public virtual void RegisterInput()
    {

    }

    public virtual void UnRegisterInput()
    {

    }
    #endregion

    #region Handler
    protected virtual void HandleClose(InputAction.CallbackContext callbackContext)
    {

    }
    #endregion
}
