using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public string apiUrlGet = "https://localhost:5001/api/PersonajeApi?idPersonaje=";
    public string loginUrl = "https://localhost:5001/api/UsuarioApi/login";
    public string siguienteEscena = "Titulo";

    void Awake()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
    }

    public void IniciarSesion(string gmail, string contraseña) { 
    
        StartCoroutine(LoginCoroutine(gmail, contraseña));
    }



    IEnumerator LoginCoroutine(string gmail, string contraseña)
    {
        LoginRequest datos = new LoginRequest { Gmail = gmail, Contrasena = contraseña };
        string json = JsonUtility.ToJson(datos);

        Debug.Log("json: " + json);
        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("web: " + request.result);
        if (request.result == UnityWebRequest.Result.Success)
        {
            Usuario usuario = JsonUtility.FromJson<Usuario>(request.downloadHandler.text);
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
                apiUrlGet = apiUrlGet + usuario.idPersonaje;
                StartCoroutine(ObtenerPersonaje());
            }
            
            SceneManager.LoadScene("Titulo");
        }
        else
        {
            Debug.LogError("Login fallido: " + request.error);
            Debug.Log("Código HTTP: " + request.responseCode);
            Debug.Log("Respuesta del servidor: " + request.downloadHandler.text);
        }
    }

        IEnumerator ObtenerPersonaje()
        {
            UnityWebRequest request = UnityWebRequest.Get(apiUrlGet);
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

    [System.Serializable]
    public class LoginRequest
    {
        public string Gmail;       // Mayúscula
        public string Contrasena;  // Sin tilde
    }
}