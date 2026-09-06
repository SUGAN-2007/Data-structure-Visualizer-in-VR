using UnityEngine;

public class SwitchModeButton : MonoBehaviour
{
    public ModeController modeController;

    void OnMouseDown()
    {
        modeController.ToggleMode();
    }
}