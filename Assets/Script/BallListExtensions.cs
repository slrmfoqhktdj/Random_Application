using UnityEngine;
using UnityEngine.UIElements;
using System;

using System.Collections;
using System.Collections.Generic;

public static class BallListExtensions
{
    /*public static void AddLongClickCallback(this VisualElement element, Action callback, float duration = 1.0f)
    {
        bool isPressed = false;
        Vector2 startPosition = Vector2.zero;

        element.RegisterCallback<PointerDownEvent>(evt =>
        {
            isPressed = true;
            startPosition = evt.localPosition;

            // ExecuteLater 사용 (반환값 무시)
            element.schedule.Execute(() =>
            {
                if (isPressed)
                {
                    callback?.Invoke();
                    isPressed = false;
                }
            }).ExecuteLater((long)(duration * 1000));
        });

        element.RegisterCallback<PointerUpEvent>(evt =>
        {
            isPressed = false;
        });

        element.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (Vector2.Distance(evt.localPosition, startPosition) > 10f)
            {
                isPressed = false;
            }
        });

        element.RegisterCallback<PointerLeaveEvent>(evt =>
        {
            isPressed = false;
        });
    }*/

    private static IVisualElementScheduledItem currentScheduledItem = null;
    private static VisualElement currentPressedElement = null;



    // 각 VisualElement에 등록된 콜백들을 추적하는 Dictionary
    private static Dictionary<VisualElement, CallbackData> registeredCallbacks = new Dictionary<VisualElement, CallbackData>();


    // 콜백 데이터를 저장하는 클래스
    private class CallbackData
    {
        public EventCallback<PointerDownEvent> onPointerDown;
        public EventCallback<PointerUpEvent> onPointerUp;
        public EventCallback<PointerMoveEvent> onPointerMove;
        public EventCallback<PointerLeaveEvent> onPointerLeave;
    }

    public static void AddLongClickCallback(this VisualElement element, Action callback, float duration = 1.0f)
    {
        bool isPressed = false;
        Vector2 startPosition = Vector2.zero;
        IVisualElementScheduledItem scheduledItem = null;

        EventCallback<PointerDownEvent> onPointerDown = evt =>
        {
            // 이전에 예약된 모든 롱클릭 작업 취소
            CancelCurrentLongClick();

            isPressed = true;
            startPosition = evt.localPosition;
            currentPressedElement = element;

            // 새로운 롱클릭 작업 예약
            scheduledItem = element.schedule.Execute(() =>
            {
                if (isPressed && currentPressedElement == element)
                {
                    callback?.Invoke();
                    ResetLongClickState();
                }
            }).StartingIn((long)(duration * 1000));

            currentScheduledItem = scheduledItem;
        };

        EventCallback<PointerUpEvent> onPointerUp = evt =>
        {
            if (isPressed)
            {
                CancelCurrentLongClick();
                ResetLongClickState();
            }
        };

        EventCallback<PointerMoveEvent> onPointerMove = evt =>
        {
            if (isPressed && Vector2.Distance(evt.localPosition, startPosition) > 10f)
            {
                CancelCurrentLongClick();
                ResetLongClickState();
            }
        };

        EventCallback<PointerLeaveEvent> onPointerLeave = evt =>
        {
            if (isPressed)
            {
                CancelCurrentLongClick();
                ResetLongClickState();
            }
        };

         // 콜백 등록
        element.RegisterCallback(onPointerDown);
        element.RegisterCallback(onPointerUp);
        element.RegisterCallback(onPointerMove);
        element.RegisterCallback(onPointerLeave);

        // Dictionary에 저장
        registeredCallbacks[element] = new CallbackData
        {
            onPointerDown = onPointerDown,
            onPointerUp = onPointerUp,
            onPointerMove = onPointerMove,
            onPointerLeave = onPointerLeave
        };

    }

     public static void RemoveLongClickCallback(this VisualElement element)
    {
        if (element == null)
            return;


        // Dictionary에서 콜백 데이터 찾기
        if (registeredCallbacks.TryGetValue(element, out CallbackData callbackData))
        {
            // 모든 콜백 해제
            if (callbackData.onPointerDown != null)
                element.UnregisterCallback(callbackData.onPointerDown);
            
            if (callbackData.onPointerUp != null)
                element.UnregisterCallback(callbackData.onPointerUp);
            
            if (callbackData.onPointerMove != null)
                element.UnregisterCallback(callbackData.onPointerMove);
            
            if (callbackData.onPointerLeave != null)
                element.UnregisterCallback(callbackData.onPointerLeave);

            // Dictionary에서 제거
            registeredCallbacks.Remove(element);
        }

        // 현재 활성화된 롱클릭이 이 요소에 대한 것이면 취소
        if (currentPressedElement == element)
        {
            CancelCurrentLongClick();
            ResetLongClickState();
        }
    }

    private static void CancelCurrentLongClick()
    {
        if (currentScheduledItem != null)
        {
            currentScheduledItem.Pause();
            currentScheduledItem = null;
        }
    }

    private static void ResetLongClickState()
    {
        currentPressedElement = null;
    }
}
