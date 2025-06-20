using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public string loginUrl = "http://localhost:5000/api/UsuarioApi/login";
    public string siguienteEscena = "Titulo";

    public void IniciarSesion(string email, string contrasena)
    {
        StartCoroutine(LoginCoroutine(email, contrasena));
    }

    IEnumerator LoginCoroutine(string email, string contrasena)
    {
        LoginRequest datos = new LoginRequest { email = email, contrasena = contrasena };
        string json = JsonUtility.ToJson(datos);

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Usuario usuario = JsonUtility.FromJson<Usuario>(request.downloadHandler.text);
            GameManager.Instance.SetUsuario(usuario);
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.LogError("Login fallido: " + request.error);
        }
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string email;
        public string contrasena;
    }
}