using UnityEngine;

public class ClickToInteract : MonoBehaviour
{
    public ModeController modeController;
    public bool isFirstButton; // true = push/enqueue button, false = pop/dequeue button

    void OnMouseDown()
    {
    if (modeController != null)
            modeController.OnActionButton1();
        else
            modeController.OnActionButton2();
    }
}