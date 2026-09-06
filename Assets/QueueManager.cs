using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Settings")]
    [SerializeField] private int maxSize = 5;
    [SerializeField] private GameObject[] cubeVisuals;

    private Queue<int> queue = new Queue<int>();
    private int nextValue = 1;

    public void ResetQueue()
    {
        queue.Clear();
        nextValue = 1;
        foreach (var cube in cubeVisuals)
            cube.SetActive(false);
    }

    public void Enqueue()
    {
        if (queue.Count >= maxSize) { Debug.Log("Queue Overflow!"); return; }
        queue.Enqueue(nextValue);
        nextValue++;
        UpdateVisuals();
        Debug.Log("Enqueued: " + (nextValue - 1));
    }

    public void Dequeue()
    {
        if (queue.Count == 0) { Debug.Log("Queue Underflow!"); return; }
        int value = queue.Dequeue();
        UpdateVisuals();
        Debug.Log("Dequeued: " + value);
    }

    void UpdateVisuals()
    {
        int[] items = queue.ToArray();
        for (int i = 0; i < cubeVisuals.Length; i++)
            cubeVisuals[i].SetActive(i < items.Length);
    }
}