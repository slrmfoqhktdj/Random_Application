using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace Popup
{
    [UxmlElement]
    public partial class SettingDialog : VisualElement
    {
        //[UnityEngine.Scripting.Preserve]
        //public new class UxmlFactory : UxmlFactory<Dialog> { }

        private const string styleResource = "SettingDialogStyleSheet";
        private const string ussPopup = "dialog_window";
        private const string ussPopupContainer = "dialog_container";
        private const string ussMsgContainer = "msg_container";
        private const string ussHorContainer = "horizontal_container";
        private const string ussPopupMsg = "popup_msg";
        private const string ussPopupSlider = "popup_slider";
        private const string ussPopupButton = "popup_button";
        private const string ussConfirm = "button_confirm";
        private const string ussCancel = "button_cancel";
        private Button confirmButton;
        private SliderInt btnSoundSlider;
        private SliderInt playSoundSlider;
        private int btnSoundVolume;
        private int playSoundVolume;

        public SettingDialog()
        {
            //uss 파일 지정
            styleSheets.Add(Resources.Load<StyleSheet>(styleResource));

            AddToClassList(ussPopupContainer);

            VisualElement window = new VisualElement();
            window.AddToClassList(ussPopup);
            hierarchy.Add(window);

            //text section
            VisualElement horizontalContainerText = new VisualElement();
            horizontalContainerText.AddToClassList(ussMsgContainer);
            window.Add(horizontalContainerText);

            //btnSoundVolume = PlayerPrefsManager.Instance.GetSDButton();
            btnSoundVolume = PlayerPrefsManager.GetSDButton();
            btnSoundSlider = new SliderInt();
            btnSoundSlider.name = "btnSoundSlider";
            btnSoundSlider.lowValue = 0;
            btnSoundSlider.highValue = 10;
            btnSoundSlider.value = btnSoundVolume;
            btnSoundSlider.AddToClassList(ussPopupSlider);
            btnSoundSlider.label = "버튼음량";
            btnSoundSlider.RegisterValueChangedCallback(OnBtnSoundSliderChange);
            horizontalContainerText.Add(btnSoundSlider);

            //playSoundVolume = PlayerPrefsManager.Instance.GetSDPlay();
        playSoundVolume = PlayerPrefsManager.GetSDPlay();
            playSoundSlider = new SliderInt();
            playSoundSlider.name = "playSoundSlider";
            playSoundSlider.lowValue = 0;
            playSoundSlider.highValue = 10;
            playSoundSlider.value = playSoundVolume;
            playSoundSlider.AddToClassList(ussPopupSlider);
            playSoundSlider.label = "배경소리";
            playSoundSlider.RegisterValueChangedCallback(OnPlaySoundSliderChange);
            horizontalContainerText.Add(playSoundSlider);

            //button section
            VisualElement horizontalContainerButton = new VisualElement();
            horizontalContainerButton.AddToClassList(ussHorContainer);
            window.Add(horizontalContainerButton);

            confirmButton = new Button() { text = "확인" };
            confirmButton.AddToClassList(ussPopupButton);
            confirmButton.AddToClassList(ussConfirm);
            confirmButton.SetEnabled(false);
            horizontalContainerButton.Add(confirmButton);

            Button cancelButton = new Button() { text = "취소" };
            cancelButton.AddToClassList(ussPopupButton);
            cancelButton.AddToClassList(ussCancel);
            horizontalContainerButton.Add(cancelButton);

            confirmButton.clicked += OnConfirm;
            cancelButton.clicked += OnCancel;

        }

        private void OnBtnSoundSliderChange(ChangeEvent<int> evt)
        {
            Debug.Log($"버튼음량 : {evt.newValue}");
            if (btnSoundVolume != evt.newValue)
                confirmButton.SetEnabled(true);
        }

        private void OnPlaySoundSliderChange(ChangeEvent<int> evt)
        {
            Debug.Log($"배경소리 : {evt.newValue}");
            if (playSoundVolume != evt.newValue)
                confirmButton.SetEnabled(true);
        }

        public event Action confirmed;
        public event Action canceled;

        private void OnConfirm()
        {
            SoundManager.Instance.ButtonClickSoundPlay();

            // PlayerPrefsManager.Instance.SetSDButton(btnSoundSlider.value);
            // PlayerPrefsManager.Instance.SetSDPlay(playSoundSlider.value);

            PlayerPrefsManager.SetSDButton(btnSoundSlider.value);
            PlayerPrefsManager.SetSDPlay(playSoundSlider.value);
            
            SoundManager.Instance.SetButtonVolume(btnSoundSlider.value);
            SoundManager.Instance.SetPlayVolume(playSoundSlider.value);
            SoundManager.Instance.SetCompleteVolume(playSoundSlider.value);

            confirmed?.Invoke();
        }
        
         private void OnCancel()
        {
            SoundManager.Instance.ButtonClickSoundPlay();
            canceled?.Invoke();
        }

        
    }
}

