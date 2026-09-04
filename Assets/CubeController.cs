using UnityEngine;

public class CubeController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Cube Started!");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * 2f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * 2f * Time.deltaTime);
        }
    }
}