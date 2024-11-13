using UnityEngine;

public class ToggleWindow : MonoBehaviour
{
    public Animator animator;
    private bool IsOpen = false;

    public void Toggle()
    {
        IsOpen = !IsOpen;
        animator.SetBool("IsOpen", IsOpen);
    }
}
