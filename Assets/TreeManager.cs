using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreeManager : MonoBehaviour
{
    private class Node
    {
        public int value;
        public Node left, right, parent;
    }

    [Header("Visuals")]
    public GameObject[] cubeVisuals;      // pool of 7 card objects
    public LineRenderer[] connectors;     // pool of 6 connector lines
    public StatusMessageDisplay statusMessageDisplay;

    [Header("Layout")]
    public Vector3 basePosition = new Vector3(0f, 0.3f, 0f);
    public float xSpacing = 0.45f;   // horizontal distance between adjacent in-order positions
    public float levelHeight = 0.6f; // vertical distance per depth level

    [Header("Cursor")]
    public Color normalColor = Color.white;
    public Color cursorColor = Color.yellow;
    public GameObject contextMenu;
    private Color[] originalColors;

    public bool isActive = false;
    public int maxNodes = 10;

    private Node root;
    private Node cursorNode;

    // built fresh each UpdateVisuals() call
    private List<Node> displayOrder = new List<Node>();
    private Dictionary<Node, int> nodeToCardIndex = new Dictionary<Node, int>();
    private Dictionary<Node, Vector3> nodePosition = new Dictionary<Node, Vector3>();
    private int inOrderCounter;

    private void Awake()
    {
        originalColors = new Color[cubeVisuals.Length];
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            var renderer = cubeVisuals[i]?.GetComponentInChildren<Renderer>();
            if (renderer != null) originalColors[i] = renderer.material.color;
        }
    }

    private void Start()
    {
        ActivateMode();
    }

    public void ActivateMode()
    {
        isActive = true;
        ClearTree();
    }

    public void DeactivateMode()
    {
        isActive = false;
        if (contextMenu != null) contextMenu.SetActive(false);
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            var renderer = cubeVisuals[i]?.GetComponentInChildren<Renderer>();
            if (renderer != null) renderer.material.color = originalColors[i];
        }
    }

    public void ClearTree()
    {
        root = null;
        cursorNode = null;
        UpdateVisuals();
    }

    // ---------- Build from list (BST insert) ----------

    public void BuildFromList(List<int> values)
    {
        root = null;
        cursorNode = null;

        int count = 0;
        foreach (int value in values)
        {
            if (count >= maxNodes)
            {
                statusMessageDisplay?.ShowMessage("Overflow! Only first " + maxNodes + " values used.");
                break;
            }
            InsertBST(value);
            count++;
        }

        cursorNode = root;
        UpdateVisuals();
    }

    private void InsertBST(int value)
    {
        if (root == null)
        {
            root = new Node { value = value };
            return;
        }

        Node current = root;
        while (true)
        {
            if (value < current.value)
            {
                if (current.left == null)
                {
                    current.left = new Node { value = value, parent = current };
                    return;
                }
                current = current.left;
            }
            else
            {
                if (current.right == null)
                {
                    current.right = new Node { value = value, parent = current };
                    return;
                }
                current = current.right;
            }
        }
    }

    // ---------- Traversal ----------

    public void GoToLeftChild()
    {
        if (cursorNode?.left != null) cursorNode = cursorNode.left;
        UpdateVisuals();
    }

    public void GoToRightChild()
    {
        if (cursorNode?.right != null) cursorNode = cursorNode.right;
        UpdateVisuals();
    }

    public void GoToParent()
    {
        if (cursorNode?.parent != null) cursorNode = cursorNode.parent;
        UpdateVisuals();
    }

    // ---------- Delete (leaf-only, same rule as before) ----------

    public void DeleteAtCursor()
    {
        if (cursorNode == null) { statusMessageDisplay?.ShowMessage("Nothing selected."); return; }

        if (cursorNode.left != null || cursorNode.right != null)
        {
            statusMessageDisplay?.ShowMessage("Delete children first (leaf nodes only).");
            return;
        }

        Node parent = cursorNode.parent;
        if (parent == null)
        {
            root = null; // deleting root with no children
        }
        else
        {
            if (parent.left == cursorNode) parent.left = null;
            else if (parent.right == cursorNode) parent.right = null;
        }

        cursorNode = parent; // move cursor up
        UpdateVisuals();
    }

    // ---------- Layout + visuals ----------

    public void UpdateVisuals()
    {
        if (!isActive) return;

        displayOrder.Clear();
        nodeToCardIndex.Clear();
        nodePosition.Clear();
        inOrderCounter = 0;

        if (root != null)
    {
        AssignPositions(root, 0);

        // re-center so root always sits at x = 0 relative to basePosition
        if (nodePosition.ContainsKey(root))
        {
            float rootX = nodePosition[root].x - basePosition.x;
            var keys = new List<Node>(nodePosition.Keys);
            foreach (var key in keys)
            {
                Vector3 p = nodePosition[key];
                p.x -= rootX;
                nodePosition[key] = p;
            }
        }
    }
        // assign cards to nodes, in the order we collected them
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            if (cubeVisuals[i] == null) continue;

            if (i < displayOrder.Count)
            {
                Node node = displayOrder[i];
                nodeToCardIndex[node] = i;

                cubeVisuals[i].SetActive(true);
                cubeVisuals[i].transform.localPosition = nodePosition[node];

                var label = cubeVisuals[i].GetComponentInChildren<TextMeshPro>(true);
                if (label != null)
                {
                    string text = node.value.ToString();
                    if (node == root) text += "\n(ROOT)";
                    label.text = text;
                }

               
            }
            else
            {
                cubeVisuals[i].SetActive(false);
            }
        }

        UpdateConnectors();
        PositionContextMenuAtCursor();
    }

    // in-order traversal: assigns x via visit order, y via depth
private void AssignPositions(Node node, int depth)
{
    if (node == null) return;

    AssignPositions(node.left, depth + 1);

    displayOrder.Add(node);
    float x = (inOrderCounter - (CountNodes(root) - 1) / 2f) * xSpacing;
    float y = -depth * levelHeight;
    nodePosition[node] = basePosition + new Vector3(x, y, 0);
    inOrderCounter++;

    AssignPositions(node.right, depth + 1);
}

    private int CountNodes(Node node)
    {
        if (node == null) return 0;
        return 1 + CountNodes(node.left) + CountNodes(node.right);
    }

    private void UpdateConnectors()
    {
        if (connectors == null) return;

        int connectorIndex = 0;
        foreach (var node in displayOrder)
        {
            if (node.parent == null) continue; // root has no incoming connector
            if (connectorIndex >= connectors.Length) break;

            var connector = connectors[connectorIndex];
            if (connector == null) { connectorIndex++; continue; }

            bool parentHasCard = nodeToCardIndex.ContainsKey(node.parent);
            bool selfHasCard = nodeToCardIndex.ContainsKey(node);

            if (parentHasCard && selfHasCard)
            {
                connector.gameObject.SetActive(true);
                connector.SetPosition(0, cubeVisuals[nodeToCardIndex[node.parent]].transform.position);
                connector.SetPosition(1, cubeVisuals[nodeToCardIndex[node]].transform.position);
            }
            else
            {
                connector.gameObject.SetActive(false);
            }
            connectorIndex++;
        }

        // hide any unused connectors
        for (int i = connectorIndex; i < connectors.Length; i++)
        {
            if (connectors[i] != null) connectors[i].gameObject.SetActive(false);
        }
    }

    private void PositionContextMenuAtCursor()
    {
        if (!isActive || contextMenu == null || cursorNode == null || !nodeToCardIndex.ContainsKey(cursorNode)) return;
        contextMenu.SetActive(true);
        int idx = nodeToCardIndex[cursorNode];
        contextMenu.transform.position = cubeVisuals[idx].transform.position + Vector3.up * 0.25f;
    }
    public void DeleteNode(int cardIndex)
{
    if (cardIndex >= displayOrder.Count) return;
    Node target = displayOrder[cardIndex];

    if (target.left != null || target.right != null)
    {
        statusMessageDisplay?.ShowMessage("Delete children first (leaf nodes only).");
        return;
    }

    Node parent = target.parent;
    if (parent == null)
        root = null;
    else if (parent.left == target)
        parent.left = null;
    else if (parent.right == target)
        parent.right = null;

    UpdateVisuals();
}
}