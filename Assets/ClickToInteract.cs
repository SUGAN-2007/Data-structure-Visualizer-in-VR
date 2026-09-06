using UnityEngine;

public class ClickToInteract : MonoBehaviour
{
    public StackManager stackManager;
    public bool isPushButton;

    private int nextValue = 6; // starts after your initial 1-5 pushes

    void OnMouseDown()
    {
        if (isPushButton)
        {
            stackManager.Push(nextValue);
            nextValue++;
        }
        else
        {
            stackManager.Pop();
        }
    }
}