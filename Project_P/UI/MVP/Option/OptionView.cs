using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class OptionView : MonoBehaviour, IEventView
{
    [Header("Group")]
    [SerializeField] OptionItemGroup _resolutionItemGroup;
    [SerializeField] OptionItemGroup _localizationItemGroup;
    [SerializeField] OptionItemGroup _storyItemGroup;
    [SerializeField] OptionItemGroup _menuItemGroup;

    [Header("Slider")]
    [SerializeField] SliderItem _bgmSlider;
    [SerializeField] SliderItem _sfxSlider;
    [SerializeField] SliderItem _voiceSlider;

    [Header("CheckBox")]
    [SerializeField] CheckBoxItem _windowCheckBox;
    [SerializeField] CheckBoxItem _bgmMuteCheckBox;
    [SerializeField] CheckBoxItem _sfxMuteCheckBox;
    [SerializeField] CheckBoxItem _voiceMuteCheckBox;

    [Header("Selector")]
    [SerializeField] OptionSelector _optionSelector;

    [Header("Audio SO")]
    [SerializeField] UIAudioSO _uiAudioSO;

    //Event
    public Action<IMessage> Msg;
    public Action<InputAction.CallbackContext> OnClose;

    public event UnityAction<bool> OnWindow;
    public event UnityAction<bool> OnBgmMute;
    public event UnityAction<bool> OnSfxMute;
    public event UnityAction<bool> OnVoiceMute;

    public event UnityAction<float> OnBgmVolumeChanged;
    public event UnityAction<float> OnSfxVolumeChanged;
    public event UnityAction<float> OnVoiceVolumeChanged;

    public event UnityAction<int> OnLocalizationChanged;
    public event UnityAction<int> OnStoryModeChanged;
    public event UnityAction<int> OnResolutionChanged;

    public event UnityAction OnRestart;
    public event UnityAction OnLobby;
    public event UnityAction OnQuit;

    public void Initialize()
    {
        //Audio
        _bgmSlider.SetValue(DataManager.Instance.OptionData.BgmVolume);
        _sfxSlider.SetValue(DataManager.Instance.OptionData.SfxVolume);
        _voiceSlider.SetValue(DataManager.Instance.OptionData.VoiceVolume);

        //Local
        int localNumber = (int)DataManager.Instance.OptionData.LocalLanguageType;
        _optionSelector.CurrentSelect = localNumber;
        _localizationItemGroup.OptionItems[localNumber].OnSelected();

        //Story
        int storyNumber = (int)DataManager.Instance.OptionData.StoryPlayType;
        _optionSelector.CurrentSelect = storyNumber;
        _storyItemGroup.OptionItems[storyNumber].OnSelected();

        //Resolution
        int resolutionNumber = (int)DataManager.Instance.OptionData.ResolutionType;
        _optionSelector.CurrentSelect = resolutionNumber;
        _optionSelector.ArrowMove();
        _resolutionItemGroup.OptionItems[(int)DataManager.Instance.OptionData.ResolutionType].OnSelected();

        //CheckBox
        _windowCheckBox.OnSelect(DataManager.Instance.OptionData.IsWindow);
        _bgmMuteCheckBox.OnSelect(DataManager.Instance.OptionData.IsBgmMute);
        _sfxMuteCheckBox.OnSelect(DataManager.Instance.OptionData.IsSfxMute);
        _voiceMuteCheckBox.OnSelect(DataManager.Instance.OptionData.IsVoiceMute);
    }

    private void OpenView(bool isLobby)
    {
        AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Open, 0));
        _menuItemGroup.gameObject.SetActive(!isLobby);
    }
    
    private void CloseView()
    {
        AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Close, 0));
    }

    #region Implement Event
    public void RegisterEvent()
    {
        Msg += ReceiveMsg;

        //Resolution
        _resolutionItemGroup.GroupAddListener(ResolutionUpdate);
        _windowCheckBox.AddListener(SelectWindowMode);

        //Audio
        _bgmSlider.AddListener(BgmVolumeUpate);
        _sfxSlider.AddListener(SfxVolumeUpate);
        _voiceSlider.AddListener(VoiceVolumeUpate);
        _bgmMuteCheckBox.AddListener(SelectBgmMute);
        _sfxMuteCheckBox.AddListener(SelectSfxMute);
        _voiceMuteCheckBox.AddListener(SelectVoiceMute);

        // Localization
        _localizationItemGroup.GroupAddListener(LocalizationUpdate);

        // Story
        _storyItemGroup.GroupAddListener(StoryModeUpdate);

        // Menu Buttons
        _menuItemGroup.IndividualAddListener(OnRestart, OnLobby, OnQuit);
    }

    public void UnregisterEvent()
    {
        Msg -= ReceiveMsg;

        _resolutionItemGroup.GroupAllRemoveListener();
        _windowCheckBox.RemoveAllListeners();

        _bgmSlider.RemoveAllListeners();
        _sfxSlider.RemoveAllListeners();
        _voiceSlider.RemoveAllListeners();
        _bgmMuteCheckBox.RemoveAllListeners();
        _sfxMuteCheckBox.RemoveAllListeners();
        _voiceMuteCheckBox.RemoveAllListeners();

        _localizationItemGroup.GroupAllRemoveListener();

        _storyItemGroup.GroupAllRemoveListener();

        _menuItemGroup.GroupAllRemoveListener();
    }

    public void ReceiveMsg(IMessage msg)
    {
        var newMsg = (OptionMsg)msg;
        switch (newMsg.uiEventType)
        {
            case eUIEventType.Open:
                OpenView(newMsg.isLobby);
                break;
            case eUIEventType.Close:
                CloseView();
                break;
        }
    }
    #endregion

    #region Resolution
    public void ResolutionUpdate()
    {
        for(int i=0; i< _resolutionItemGroup.OptionItems.Count; i++)
        {
            if (_resolutionItemGroup.OptionItems[i].IsSeleted && _optionSelector.CurrentSelect != i)
            {
                _resolutionItemGroup.OptionItems[i].OnSelect(false);
                break;
            }
        }
        OnResolutionChanged.Invoke(_optionSelector.CurrentSelect);
    }

    private void SelectWindowMode()
    {
        OnWindow.Invoke(_windowCheckBox);
    }
    #endregion

    #region Audio

    private void BgmVolumeUpate(float value)
    {
        OnBgmVolumeChanged.Invoke(value);
    }
    private void SfxVolumeUpate(float value)
    {
        OnSfxVolumeChanged.Invoke(value);
    }

    private void VoiceVolumeUpate()
    {
        OnVoiceVolumeChanged.Invoke(_bgmSlider.Value);
    }

    private void SelectBgmMute()
    {
        OnBgmMute.Invoke(_bgmMuteCheckBox.IsSeleted);
    }

    private void SelectSfxMute()
    {
        OnSfxMute.Invoke(_sfxMuteCheckBox.IsSeleted);
    }

    private void SelectVoiceMute()
    {
        OnVoiceMute.Invoke(_voiceMuteCheckBox.IsSeleted);
    }
    #endregion

    #region Localization
    public void LocalizationUpdate()
    {
        for (int i = 0; i < _localizationItemGroup.OptionItems.Count; i++)
        {
            if (_localizationItemGroup.OptionItems[i].IsSeleted && _optionSelector.CurrentSelect != i)
            {
                _localizationItemGroup.OptionItems[i].OnSelect(false);
                break;
            }
        }
        OnLocalizationChanged.Invoke(_optionSelector.CurrentSelect);
    }
    #endregion

    #region Story Mode
    public void StoryModeUpdate()
    {
        for (int i = 0; i < _storyItemGroup.OptionItems.Count; i++)
        {
            if (_storyItemGroup.OptionItems[i].IsSeleted && _optionSelector.CurrentSelect != i)
            {
                _storyItemGroup.OptionItems[i].OnSelect(false);
                break;
            }
        }
        OnStoryModeChanged.Invoke(_optionSelector.CurrentSelect);
    }
    #endregion

    #region Message
    public struct OptionMsg : IMessage
    {
        public eUIEventType uiEventType;
        public bool isLobby;
        public OptionMsg(eUIEventType uiEventType, bool isLobby = false) : this()
        {
            this.uiEventType = uiEventType;
            this.isLobby = isLobby;
        }
    }
    #endregion
}
