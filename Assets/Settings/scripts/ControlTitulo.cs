using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlTitulo : MonoBehaviour {
    public void Jugar()
    {
        SceneManager.LoadScene("Combate");
    }
    public void Salir()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
