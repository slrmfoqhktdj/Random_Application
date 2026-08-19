using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace Popup
{
    [UxmlElement]
    public partial class Dialog : VisualElement
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
        private const string ussCancel = "button_cancel";
        private const string ussConfirm = "button_confirm";

        private Label numLabel;
        private Label msgLabel;

        public Dialog()
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

            numLabel = new Label();
            numLabel.AddToClassList(ussPopupMsg);
            horizontalContainerText.Add(numLabel);

            msgLabel = new Label();
            msgLabel.AddToClassList(ussPopupMsg);
            horizontalContainerText.Add(msgLabel);

            //button section
            VisualElement horizontalContainerButton = new VisualElement();
            horizontalContainerButton.AddToClassList(ussHorContainer);
            window.Add(horizontalContainerButton);

            Button confirmButton = new Button() { text = "확인" };
            confirmButton.AddToClassList(ussPopupButton);
            confirmButton.AddToClassList(ussConfirm);
            horizontalContainerButton.Add(confirmButton);

            Button cancelButton = new Button() { text = "취소" };
            cancelButton.AddToClassList(ussPopupButton);
            cancelButton.AddToClassList(ussCancel);
            horizontalContainerButton.Add(cancelButton);

            confirmButton.clicked += OnConfirm;
            cancelButton.clicked += OnCancel;
        }

        public void setNumMsg(string msg)
        {
            numLabel.text = msg;
        }

        public void setMsg(string msg)
        {
            msgLabel.text = msg;
        }

        public event Action confirmed;
        public event Action canceled;

        private void OnConfirm()
        {

            SoundManager.Instance.ButtonClickSoundPlay();
            confirmed?.Invoke();
        }

        private void OnCancel()
        {
            SoundManager.Instance.ButtonClickSoundPlay();
            canceled?.Invoke();
        }
    }
}

