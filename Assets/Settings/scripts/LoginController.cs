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
        string contraseña = contrasenaInput.text;

        if (string.IsNullOrEmpty(gmail) || string.IsNullOrEmpty(contraseña))
        {
            mensajeError.text = "Por favor completá ambos campos.";
            return;
        }

        mensajeError.text = "Iniciando sesión...";
        loginManager.IniciarSesion(gmail, contraseña);
    }
}