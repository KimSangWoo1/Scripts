using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionPresenter : UIBase
{
    OptionView _optionView;

    #region Mono
    private void Awake()
    {
        _optionView = GetComponent<OptionView>();
    }

    void OnEnable()
    {
        // Resolution 
        _optionView.OnResolutionChanged += HandleResolutionChange;
        _optionView.OnWindow += HandleWindow;

        //Audio
        _optionView.OnBgmVolumeChanged += HandleBgmVolumeChanged;
        _optionView.OnSfxVolumeChanged += HandleSfxVolumeChanged;
        _optionView.OnVoiceVolumeChanged += HandleVoiceVolumeChanged;
        _optionView.OnBgmMute += HandleBgmMute;
        _optionView.OnSfxMute += HandleSfxMute;
        _optionView.OnVoiceMute += HandleVoiceMute;

        // Localization
        _optionView.OnLocalizationChanged += HandleLocalizationChanged;

        // Story Mode
        _optionView.OnStoryModeChanged += HandleStoryModeChanged;

        // Menu Btns
        _optionView.OnRestart += HandleRestart;
        _optionView.OnLobby += HandleLobby;
        _optionView.OnQuit += HandleQuit;

        _optionView.OnClose += HandleClose;
        _optionView.RegisterEvent();
        RegisterInput();
    }

    private void OnDisable()
    {
        _optionView.OnResolutionChanged -= HandleResolutionChange;
        _optionView.OnWindow -= HandleWindow;

        _optionView.OnBgmVolumeChanged -= HandleBgmVolumeChanged;
        _optionView.OnSfxVolumeChanged -= HandleSfxVolumeChanged;
        _optionView.OnVoiceVolumeChanged -= HandleVoiceVolumeChanged;
        _optionView.OnBgmMute -= HandleBgmMute;
        _optionView.OnSfxMute -= HandleSfxMute;
        _optionView.OnVoiceMute -= HandleVoiceMute;

        _optionView.OnLocalizationChanged -= HandleLocalizationChanged;

        _optionView.OnRestart -= HandleRestart;
        _optionView.OnLobby -= HandleLobby;
        _optionView.OnQuit -= HandleQuit;

        _optionView.OnClose -= HandleClose;
        _optionView.UnregisterEvent();
        UnRegisterInput();
    }
    #endregion

    #region UIBase Override Method
    public override void Initialize()
    {
        base.Initialize();
        _optionView.Initialize();
    }

    public override void Open()
    {
        base.Open();
        GameManager.Instance.TimeScaleChange(0f);
        bool isLobby = SceneController.IsActiveScene(eSceneType.MainMenuScene);
        _optionView.Msg?.Invoke(new OptionView.OptionMsg(eUIEventType.Open, isLobby));
    }

    public override void Close()
    {
        base.Close();
        GameManager.Instance?.TimeScaleChange(1f);
        _optionView.Msg?.Invoke(new OptionView.OptionMsg(eUIEventType.Close));
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void RegisterInput()
    {
        base.RegisterInput();
        EventManager.Instance?.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), HandleClose));
    }

    public override void UnRegisterInput()
    {
        base.UnRegisterInput();
        EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), HandleClose));
    }
    #endregion

    #region Event Handler
    #region Resolution
    private void HandleResolutionChange(int value)
    {
        OptionManager.Instance.SetResolution(value);
    }

    private void HandleWindow(bool isActive)
    {
        OptionManager.Instance.SetWidow(isActive);
    }
    #endregion

    #region Audio
    private void HandleBgmVolumeChanged(float value)
    {
        OptionManager.Instance.SetBgmVolume(value);
    }

    private void HandleSfxVolumeChanged(float value)
    {
        OptionManager.Instance.SetSfxVolume(value);
    }

    private void HandleVoiceVolumeChanged(float value)
    {
        OptionManager.Instance.SetVoiceVolume(value);
    }

    private void HandleBgmMute(bool isActive)
    {
        OptionManager.Instance.SetBgmMute(isActive);
    }

    private void HandleSfxMute(bool isActive)
    {
        OptionManager.Instance.SetSfxMute(isActive);
    }

    private void HandleVoiceMute(bool isActive)
    {
        OptionManager.Instance.SetVoiceMute(isActive);
    }
    #endregion

    #region Localization
    private void HandleLocalizationChanged(int value)
    {
        OptionManager.Instance.SetLocal(value);
    }
    #endregion

    #region StoryMode
    private void HandleStoryModeChanged(int value)
    {
        OptionManager.Instance.SetPlayingStory(value);
    }
    #endregion

    #region Menu
    private void HandleQuit()
    {
        Close();
        GameManager.Instance.UpdateGameState(eGameState.GameEnd);
    }

    private void HandleLobby()
    {
        Close();
        GameManager.Instance.ChangeScene(eSceneType.MainMenuScene, LoadSceneMode.Additive, true).Forget();
    }

    private void HandleRestart()
    {
        Close();
        GameManager.Instance.ChangeScene(eSceneType.Chapter1, LoadSceneMode.Additive, true).Forget();
    }
    #endregion

    private void HandleClose(InputAction.CallbackContext callbackContext)
    {
        _optionView.Msg?.Invoke(new OptionView.OptionMsg(eUIEventType.Close));
        Close();
    }
    #endregion
}
