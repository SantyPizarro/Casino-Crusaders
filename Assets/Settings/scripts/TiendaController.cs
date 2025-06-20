﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class TiendaController : MonoBehaviour
{
    public int monedas = 100;
    public int vidaActual = 50;
    public int vidaMaxima = 100;
    public int armadura = 0;
    public int dano = 0;

    public Text TextoMonedas;
    public Text TextoVida;
    public Text TextoVidaMax;
    public Text TextoArmadura;
    public Text TextoDano;

    private const int COSTO = 5;

    // URL de la API (puedes cambiar el ID si lo necesitas)
    public string apiUrl = "https://localhost:7000/api/PersonajeApi?idPersonaje=1";
    public string apiUrlPut = "https://localhost:7000/api/PersonajeApi";

    void Start()
    {
        Debug.Log("TextoMonedas es null? " + (TextoMonedas == null));
        Debug.Log("TextoVida es null? " + (TextoVida == null));
        StartCoroutine(ObtenerDatosPersonaje());
    }

    IEnumerator ObtenerDatosPersonaje()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Personaje personaje = JsonUtility.FromJson<Personaje>(json);

            vidaActual = personaje.vidaActual;
            vidaMaxima = personaje.vidaMaxima;
            dano = personaje.dañoAtaque;
            armadura = personaje.defensa;

            ActualizarUI();
        }
        else
        {
            Debug.LogError("Error al obtener personaje: " + request.error);
        }
    }

    public void ComprarArmadura()
    {
        if (monedas >= COSTO)
        {
            armadura += 5;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    public void ComprarVida()
    {
        if (monedas >= COSTO)
        {
            vidaActual += 25;
            if (vidaActual > vidaMaxima)
                vidaActual = vidaMaxima;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    public void ComprarVidaMax()
    {
        if (monedas >= COSTO)
        {
            vidaMaxima += 5;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    public void ComprarDano()
    {
        if (monedas >= COSTO)
        {
            dano += 5;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    void ActualizarUI()
    {
        TextoMonedas.text = "Monedas: " + monedas;
        TextoVida.text = "Vida: " + vidaActual;
        TextoVidaMax.text = "Vida Máx: " + vidaMaxima;
        TextoArmadura.text = "Armadura: " + armadura;
        TextoDano.text = "Daño: " + dano;
    }

    public void GuardarPersonaje()
    {
        StartCoroutine(PutPersonaje());
    }

    public IEnumerator PutPersonaje()
    {
        // Armamos el objeto personaje con los valores actuales de la tienda
        Personaje personaje = new Personaje()
        {
            idPersonaje = 1, // O el ID que corresponda (también podrías usar GameManager.Instance.UsuarioActual.idPersonaje)
            vidaActual = vidaActual,
            vidaMaxima = vidaMaxima,
            defensa = armadura,
            dañoAtaque = dano
        };

        string json = JsonUtility.ToJson(personaje);

        UnityWebRequest request = new UnityWebRequest(apiUrlPut, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Personaje actualizado correctamente");
            SceneManager.LoadScene("Titulo"); // Cambiá por el nombre real de tu escena
        }
        else
        {
            Debug.LogError("Error al actualizar personaje: " + request.error);
            SceneManager.LoadScene("Titulo");
        }
    }
}

