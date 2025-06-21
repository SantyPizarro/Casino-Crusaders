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

    private string personajeUrl = "https://localhost:7000/api/PersonajeApi?IdPersonaje=";

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
            
            ControlJuego.Instance.SetUsuario(usuario);
         if (usuario.idPersonaje == null)
         {
             Personaje personaje = new Personaje();
             personaje.vidaMaxima = 100;
             personaje.vidaActual = 50;
             personaje.dañoAtaque = 10;
             personaje.defensa = 10;
             personaje.monedas = 100;

             ControlJuego.Instance.SetPersonaje(personaje);
         }
         else {
             personajeUrl = personajeUrl + usuario.idPersonaje;
             StartCoroutine(ObtenerPersonaje());
         }

         SceneManager.LoadScene("Titulo");

            // Acá podrías guardar los datos y cargar la próxima escena
            // SceneManager.LoadScene("MainScene");
        }
        else
        {
            messageText.text = "Error: " + request.responseCode;
        }
    }

    IEnumerator ObtenerPersonaje()
    {
        UnityWebRequest request = UnityWebRequest.Get(personajeUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Personaje personaje = JsonUtility.FromJson<Personaje>(json);
            ControlJuego.Instance.SetPersonaje(personaje);

        }
        else
        {
            Debug.LogError("Error al obtener personaje: " + request.error);
        }
    }
}

[System.Serializable]
public class LoginRequestDto
{
    public string gmail;
    public string contraseña;
}