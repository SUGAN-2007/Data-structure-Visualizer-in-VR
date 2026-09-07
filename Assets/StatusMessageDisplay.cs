using UnityEngine;
using TMPro;
using System.Collections;

public class StatusMessageDisplay : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public float displayDuration = 1.5f;

    private Coroutine hideRoutine;

    public void ShowMessage(string message)
    {
        statusText.text = message;

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        statusText.text = "";
    }
}