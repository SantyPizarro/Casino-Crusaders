using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ControlTitulo : MonoBehaviour {

    public void Jugar()
    {
        ControlJuego.Instance.CargarPersonajeYEmpezarJuego(this);
    }
    public void Salir()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }



}
