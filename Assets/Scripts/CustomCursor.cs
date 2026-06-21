using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
