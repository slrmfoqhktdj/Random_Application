using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace Popup
{
    [UxmlElement]
    public partial class AlertDialog : VisualElement
    {
        //[UnityEngine.Scripting.Preserve]
        //public new class UxmlFactory : UxmlFactory<Dialog> { }

        private const string styleResource = "DialogStyleSheet";
        private const string ussPopup = "dialog_window";
        private const string ussPopupContainer = "dialog_container";
        private const string ussMsgContainer = "msg_container";
        private const string ussHorContainer = "horizontal_container";
        private const string ussPopupMsg = "popup_msg";
        private const string ussPopupButton = "popup_button";
        private const string ussConfirm = "button_confirm";

        private Label MsgLabel;

        public AlertDialog()
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

            MsgLabel = new Label();
            MsgLabel.AddToClassList(ussPopupMsg);
            horizontalContainerText.Add(MsgLabel);

            

            //button section
            VisualElement horizontalContainerButton = new VisualElement();
            horizontalContainerButton.AddToClassList(ussHorContainer);
            window.Add(horizontalContainerButton);

            Button confirmButton = new Button() { text = "확인" };
            confirmButton.AddToClassList(ussPopupButton);
            confirmButton.AddToClassList(ussConfirm);
            horizontalContainerButton.Add(confirmButton);

            confirmButton.clicked += OnConfirm;
        
        }

        public void setMsg(string msg)
        {
            MsgLabel.text = msg;
        }

        public event Action confirmed;
        

        private void OnConfirm()
        {
            SoundManager.Instance.ButtonClickSoundPlay();

            confirmed?.Invoke();
        }

        
    }
}

