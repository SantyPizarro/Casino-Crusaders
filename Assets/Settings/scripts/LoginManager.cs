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
    public GameObject panelError;

    private string loginUrl = "https://localhost:7000/api/UsuarioApi/login";

    void Start()
    {
        panelError.SetActive(false); // Asegura que arranque oculto
    }

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            panelError.SetActive(true);
            MostrarError("Por favor, complete todos los campos.");
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
            MostrarError("No se pudo conectar al servidor. Verifique su conexión a Internet o intente más tarde.");
        }
        else if (request.responseCode == 401)
        {
            MostrarError("Credenciales inválidas. Por favor, intente nuevamente.");
        }
        else if (request.responseCode == 200)
        {
            string response = request.downloadHandler.text;
            Usuario usuario = JsonUtility.FromJson<Usuario>(response);

            ControlJuego.Instance.SetUsuario(usuario);

            SceneManager.LoadScene("Titulo");
        }
        else
        {
            MostrarError("Ocurrió un error inesperado. Por favor, intente nuevamente más tarde.");
        }
    }

    void MostrarError(string mensaje)
    {
        messageText.text = mensaje;
        panelError.SetActive(true);
    }
}

[System.Serializable]
public class LoginRequestDto
{
    public string gmail;
    public string contraseña;
}