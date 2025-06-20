using UnityEngine;
using TMPro; // Asegurate de usar TextMeshPro
using UnityEngine.UI;

public class ControladorLogin : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField contrasenaInput;
    public Text mensajeError;
    public LoginManager loginManager;

    public void OnClickIniciarSesion()
    {
        string email = emailInput.text;
        string contrasena = contrasenaInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
        {
            mensajeError.text = "Por favor completá ambos campos.";
            return;
        }

        mensajeError.text = "Iniciando sesión...";
        loginManager.IniciarSesion(email, contrasena);
    }
}