using UnityEngine;
using UnityEngine.UI;

public class WhenMenuLoaded : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(()=> LevelLoader.instance.LoadScene("Level_1"));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
