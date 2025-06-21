using UnityEngine;
using System.Collections.Generic;
using Assets.Settings.scripts;
using System.Collections;
using UnityEngine.Networking;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance { get; private set; }

    private string apiUrlBase = "https://localhost:7000/api/PersonajeApi?idPersonaje=";

    public int indiceEnemigoActual = 0;
    public Usuario usuario;
    public Personaje personajeJugador;
    public List<Enemigo> listaEnemigos;
    private List<string> flujoEscenas = new List<string>()
    {
        "Combate1", //pelea-evento-pelea-tienda-pelea-evento-pelea(jefe) //acá iria el mapa de por medio
        "Evento1",
        "Combate2",

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
            new Enemigo(1, "As 1", 60, 12, 3),
            new Enemigo(2, "As 2", 70, 14, 4),
            new Enemigo(3, "As 3", 80, 16, 5),
            new Enemigo(4, "As 4", 90, 18, 6),
            new Enemigo(5, "As 5", 100, 20, 7)
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
}