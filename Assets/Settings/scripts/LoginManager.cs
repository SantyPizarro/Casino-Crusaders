using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public Text messageText;

    private string loginUrl = "https://localhost:7000/api/UsuarioApi/login"; // Reemplazá con tu URL real

    // private string personajeUrl = "https://localhost:7000/api/PersonajeApi?IdPersonaje=";

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Ingrese todos los campos.";
            return;
        }

        StartCoroutine(LoginCoroutine(email, password));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        LoginRequestDto loginData = new LoginRequestDto
        {
            gmail = email,
            contraseña = password
        };

        string jsonData = JsonUtility.ToJson(loginData);

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            messageText.text = "Error de conexión: " + request.error;
        }
        else if (request.responseCode == 401)
        {
            messageText.text = "Credenciales inválidas.";
        }
        else if (request.responseCode == 200)
        {
            string response = request.downloadHandler.text;
            Usuario usuario = JsonUtility.FromJson<Usuario>(response);

            Debug.Log("Usuario logueado: " + usuario.nombreUsuario + " | idPersonaje: " + usuario.idPersonaje);

            ControlJuego.Instance.SetUsuario(usuario);

            messageText.text = "Login exitoso. Cargando personaje...";

            SceneManager.LoadScene("Titulo");
        }
        else
        {
            messageText.text = "Error: " + request.responseCode;
        }
    }
}

[System.Serializable]
public class LoginRequestDto
{
    public string gmail;
    public string contraseña;
}