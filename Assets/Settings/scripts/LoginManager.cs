<<<<<<< Updated upstream
using UnityEngine;
=======
﻿using UnityEngine;
using UnityEngine.UI;
>>>>>>> Stashed changes
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
<<<<<<< Updated upstream
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
=======
    public InputField emailInput;
    public InputField passwordInput;
    public Text messageText;

    private string loginUrl = "https://localhost:7000/api/UsuarioApi/login"; // Reemplazá con tu URL real

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
        LoginRequest loginData = new LoginRequest
        {
            gmail = email,
            contraseña = password
        };

        string jsonData = JsonUtility.ToJson(loginData);
>>>>>>> Stashed changes

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

<<<<<<< Updated upstream
        if (request.result == UnityWebRequest.Result.Success)
        {
            Usuario usuario = JsonUtility.FromJson<Usuario>(request.downloadHandler.text);
            GameManager.Instance.SetUsuario(usuario);
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.LogError("Login fallido: " + request.error);
=======
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

            // Acá podrías guardar los datos y cargar la próxima escena
            messageText.text = "¡Bienvenido, " + usuario.nombreUsuario + "!";
            // SceneManager.LoadScene("MainScene");
        }
        else
        {
            messageText.text = "Error: " + request.responseCode;
>>>>>>> Stashed changes
        }
    }
}

<<<<<<< Updated upstream
    [System.Serializable]
    public class LoginRequest
    {
        public string email;
        public string contrasena;
    }
}
=======
[System.Serializable]
public class LoginRequest
{
    public string gmail;
    public string contraseña;
}
>>>>>>> Stashed changes
