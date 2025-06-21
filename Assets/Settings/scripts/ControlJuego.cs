using UnityEngine;
using System.Collections.Generic;
using Assets.Settings.scripts;
using System.Collections;
using UnityEngine.Networking;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance { get; private set; }

    private string apiUrlBase = "https://localhost:7000/api/PersonajeApi?IdPersonaje=";
    private const string apiUrlPut = "https://localhost:7000/api/PersonajeApi";

    public int indiceEnemigoActual = 0;
    public Usuario usuario;
    public Personaje personajeJugador;
    public List<Enemigo> listaEnemigos;
    private List<string> flujoEscenas = new List<string>()
    {
        
        "Combate1",
        "Combate3",
        "Combate2",
        "Combate4",
        "Tienda",
        "EventoDados",
        "Evento1",
        

    };

    private int indiceEscenaActual = 0;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPersonaje(Personaje personaje)
    {
        personajeJugador = personaje;
    }

    public void SetUsuario(Usuario usuario)
    {
        this.usuario = usuario;
    }

    public void Inicializar()
    {
        listaEnemigos = new List<Enemigo>()
        {
            new Enemigo(1, "Carta", 60, 12, 3),
            new Enemigo(2, "Ficha", 70, 14, 4),
            new Enemigo(3, "Slot", 80, 16, 5),
            new Enemigo(4, "Dragon", 90, 18, 6),

        };
    }

    public void AvanzarASiguienteEscena()
    {
        indiceEscenaActual++;
        if (indiceEscenaActual >= flujoEscenas.Count)
        {
            Debug.Log("no hay más escenas");
            return;
        }

        string siguienteEscena = flujoEscenas[indiceEscenaActual];
        Debug.Log("Cargando escena: " + siguienteEscena);
        UnityEngine.SceneManagement.SceneManager.LoadScene(siguienteEscena);
    }

    public Enemigo ObtenerEnemigoActual()
    {
        int indiceEnemigo = indiceEscenaActual / 2;
        if (indiceEnemigo < listaEnemigos.Count)
            return listaEnemigos[indiceEnemigo];
        else
            return null;
    }

    public void ReiniciarJuego()
    {
        indiceEscenaActual = 0;
        Inicializar();
        UnityEngine.SceneManagement.SceneManager.LoadScene(flujoEscenas[0]);
    }

    public void CargarPersonajeYEmpezarJuego(MonoBehaviour caller)
    {

        Debug.Log("id personaje" + usuario.idPersonaje);

        if (usuario != null)
        {
            caller.StartCoroutine(DescargarPersonajeDesdeApi(usuario.idPersonaje));
        }
        else
        {
            Debug.LogError("Usuario no tiene personaje asignado");
        }
    }

    private IEnumerator DescargarPersonajeDesdeApi(int idPersonaje)
    {
        string url = apiUrlBase + idPersonaje;
        UnityWebRequest request = UnityWebRequest.Get(url);

        Debug.Log("Consultando API de personaje: " + url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Personaje personaje = JsonUtility.FromJson<Personaje>(json);
            Debug.Log("JSON: " + json);
            Debug.Log("ID: " + personaje.idPersonaje);
            Debug.Log("vida: " + personaje.vidaActual);
            Debug.Log("ataque: " + personaje.danoAtaque);
            SetPersonaje(personaje);

            Debug.Log("Personaje recibido: " + personaje.idPersonaje);

            Inicializar(); // inicializa enemigos u otros datos del juego
            UnityEngine.SceneManagement.SceneManager.LoadScene("Combate1"); // o escena inicial
        }
        else
        {
            Debug.LogError("Error al obtener personaje: " + request.error);
        }
    }

    public void GuardarPersonaje(MonoBehaviour caller)
    {
        if (personajeJugador == null)
        {
            Debug.LogError("No hay personaje cargado para guardar.");
            return;
        }

        caller.StartCoroutine(GuardarPersonajeCoroutine());
    }


    private IEnumerator GuardarPersonajeCoroutine()
    {
        string json = JsonUtility.ToJson(personajeJugador);
        UnityWebRequest request = new UnityWebRequest(apiUrlPut, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Enviando PUT con personaje: " + json);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Personaje actualizado correctamente.");
        }
        else
        {
            Debug.LogError("Error al actualizar personaje: " + request.error);
        }
    }
}