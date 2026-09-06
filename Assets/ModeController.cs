using UnityEngine;
using TMPro;

public class ModeController : MonoBehaviour
{
    public StackManager stackManager;
    public QueueManager queueManager;
    public GameObject[] cubes;
    public Vector3[] stackPositions;
    public Vector3[] queuePositions;
    public TextMeshProUGUI pushPopLabel1;
    public TextMeshProUGUI pushPopLabel2;

    private bool isStackMode = true;

    public void ToggleMode()
    {
        isStackMode = !isStackMode;

        stackManager.ResetStack();
        queueManager.ResetQueue();

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].transform.localPosition = isStackMode ? stackPositions[i] : queuePositions[i];
            cubes[i].SetActive(false);
        }

        pushPopLabel1.text = isStackMode ? "PUSH" : "ENQUEUE";
        pushPopLabel2.text = isStackMode ? "POP" : "DEQUEUE";
    }

    public void OnActionButton1()
    {
        if (isStackMode) stackManager.Push(GetNextStackValue());
        else queueManager.Enqueue();
    }

    public void OnActionButton2()
    {
        if (isStackMode) stackManager.Pop();
        else queueManager.Dequeue();
    }

    private int counter = 6;
    private int GetNextStackValue() => counter++;
}   