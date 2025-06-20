using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class ControladorLogin : MonoBehaviour
{
    public TMP_InputField gmailInput;
    public TMP_InputField contrasenaInput;
    public Text mensajeError;
    public LoginManager loginManager;

    public void OnClickIniciarSesion()
    {
        string gmail = gmailInput.text;
        string contrase�a = contrasenaInput.text;

        if (string.IsNullOrEmpty(gmail) || string.IsNullOrEmpty(contrase�a))
        {
            mensajeError.text = "Por favor complet� ambos campos.";
            return;
        }

        mensajeError.text = "Iniciando sesi�n...";
        loginManager.IniciarSesion(gmail, contrase�a);
    }
}