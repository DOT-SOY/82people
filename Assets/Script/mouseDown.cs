using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class mouseDown : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    // Inspector에서 설정할 수 있는 UnityEvent
    public UnityEvent onMouseDown;
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;
    public UnityEvent onMouseUp;

    // PointerDown 이벤트가 발생했을 때 호출되는 메서드
    public void OnPointerDown(PointerEventData eventData)
    {
        // UnityEvent 호출
        onMouseDown.Invoke();
    }

    // PointerEnter 이벤트가 발생했을 때 호출되는 메서드
    public void OnPointerEnter(PointerEventData eventData)
    {
        // UnityEvent 호출
        onMouseEnter.Invoke();
    }

    // PointerExit 이벤트가 발생했을 때 호출되는 메서드
    public void OnPointerExit(PointerEventData eventData)
    {
        // UnityEvent 호출
        onMouseExit.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // UnityEvent 호출
        onMouseUp.Invoke();
    }
}
