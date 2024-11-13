using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class mouseDown : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    // Inspector���� ������ �� �ִ� UnityEvent
    public UnityEvent onMouseDown;
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;
    public UnityEvent onMouseUp;

    // PointerDown �̺�Ʈ�� �߻����� �� ȣ��Ǵ� �޼���
    public void OnPointerDown(PointerEventData eventData)
    {
        // UnityEvent ȣ��
        onMouseDown.Invoke();
    }

    // PointerEnter �̺�Ʈ�� �߻����� �� ȣ��Ǵ� �޼���
    public void OnPointerEnter(PointerEventData eventData)
    {
        // UnityEvent ȣ��
        onMouseEnter.Invoke();
    }

    // PointerExit �̺�Ʈ�� �߻����� �� ȣ��Ǵ� �޼���
    public void OnPointerExit(PointerEventData eventData)
    {
        // UnityEvent ȣ��
        onMouseExit.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // UnityEvent ȣ��
        onMouseUp.Invoke();
    }
}
