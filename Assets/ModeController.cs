using UnityEngine;
using TMPro;

public enum DataMode { Stack, Queue, LinkedList }

public class ModeController : MonoBehaviour
{
    public StackManager stackManager;
    public QueueManager queueManager;
    public LinkedListManager linkedListManager;

    public GameObject[] cubes; // shared, 5 cubes
    public Vector3[] stackPositions;      // localPosition
    public Vector3[] queuePositions;      // localPosition
    public Vector3[] linkedListPositions; // localPosition — new

    public TextMeshProUGUI pushPopLabel1;
    public TextMeshProUGUI pushPopLabel2;
    public TextMeshProUGUI boardInfoLabel;

    private DataMode currentMode = DataMode.Stack;

    public void ToggleMode()
    {
        // cycle Stack -> Queue -> LinkedList -> Stack
        currentMode = (DataMode)(((int)currentMode + 1) % 3);
        ApplyMode();
    }
private void ApplyMode()
{
    stackManager.ResetStack();
    queueManager.ResetQueue();

    // deactivate whichever isn't current, activate the one that is
    if (currentMode != DataMode.LinkedList)
        linkedListManager.DeactivateMode();

    switch (currentMode)
    {
        case DataMode.Stack:
            RepositionCubes(stackPositions);
            pushPopLabel1.text = "PUSH";
            pushPopLabel2.text = "POP";
            boardInfoLabel.text = "Stack (LIFO)\nPush adds to top, Pop removes from top.";
            break;

        case DataMode.Queue:
            RepositionCubes(queuePositions);
            pushPopLabel1.text = "ENQUEUE";
            pushPopLabel2.text = "DEQUEUE";
            boardInfoLabel.text = "Queue (FIFO)\nEnqueue adds to back, Dequeue removes from front.";
            break;

        case DataMode.LinkedList:
            pushPopLabel1.text = "→";
            pushPopLabel2.text = "←";
            boardInfoLabel.text = "Linked List\nTraverse with arrows, use menu to insert/delete.";
            linkedListManager.ActivateMode(); // <-- this now handles cube repositioning itself
            break;
    }
}
    private void RepositionCubes(Vector3[] positions)
    {
        for (int i = 0; i < cubes.Length && i < positions.Length; i++)
        {
            cubes[i].transform.localPosition = positions[i];
        }
    }

    public void OnActionButton1() // reuse as -> (Move Right) in Linked List mode
{
    switch (currentMode)
    {
        case DataMode.Stack: stackManager.Push(GetNextValue()); break;
        case DataMode.Queue: queueManager.Enqueue(); break;
        case DataMode.LinkedList: linkedListManager.MoveRight(); break;
    }
}

public void OnActionButton2() // reuse as <- (Move Left) in Linked List mode
{
    switch (currentMode)
    {
        case DataMode.Stack: stackManager.Pop(); break;
        case DataMode.Queue: queueManager.Dequeue(); break;
        case DataMode.LinkedList: linkedListManager.MoveLeft(); break;
    }
}

    private int GetNextValue()
    {
        // however you're currently generating values for Push/Enqueue
        return Random.Range(1, 100);
    }

}