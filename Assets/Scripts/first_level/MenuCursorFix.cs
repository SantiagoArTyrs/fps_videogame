using UnityEngine;

public class MenuCursorFix : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // desbloquear
        Cursor.visible = true;                  // mostrar cursor
    }
}
