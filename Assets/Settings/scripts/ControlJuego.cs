using UnityEngine;
using System.Collections.Generic;
using Assets.Settings.scripts;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance { get; private set; }

    private string apiUrlBase = "https://localhost:7000/api/PersonajeApi?IdPersonaje=";
    private const string apiUrlPut = "https://localhost:7000/api/PersonajeApi";

    private string urlBaseProgreso = "https://localhost:7000/api/ProgresoApi?id=";

    public int indiceEnemigoActual = 0;
    public Usuario usuario;
    public Personaje personajeJugador;
    public List<Enemigo> listaEnemigos;
    /*
    private List<string> flujoEscenas = new List<string>()
    {
        
        "Combate1",
        "Combate2",
        "Combate3",
        "Combate4",
        "Tienda",
        "EventoDados",
        "Evento1",
        

    };
    */

    private int indiceEscenaActual = 0;
    private bool yaVolvioAlMapa = false;


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

    /*
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
    */

    public Enemigo ObtenerEnemigoActual()
    {
        int indiceEnemigo = VariablesMapa.nivel - 1; // Si tus niveles empiezan en 1
        if (indiceEnemigo >= 0 && indiceEnemigo < listaEnemigos.Count)
            return listaEnemigos[indiceEnemigo];
        else
            return null;
    }

    public void ReiniciarJuego()
    {
        indiceEscenaActual = 0;
        Inicializar();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Titulo");
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("Mapa"); // o escena inicial
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

    public void VolverAlMapa()
    {
        if (yaVolvioAlMapa) return;
        yaVolvioAlMapa = true;

        personajeJugador.monedas += 10;
        GuardarPersonaje(this); // Guarda desde sí mismo (MonoBehaviour)
        VariablesMapa.nivelesCompletados[VariablesMapa.nivel] = true;




        StartCoroutine(ActualizarProgresoYVolver());
    }

    internal void ResetVolverAlMapa()
    {
        yaVolvioAlMapa = false;
    }
    public void VolverDesdeEvento1()
    {
        VariablesMapa.estadoPos2 = 1; // Abre camino de N1 a N2
        VariablesMapa.maxNivel = Mathf.Max(VariablesMapa.maxNivel, 2);
        VariablesMapa.nivelesCompletados[5] = true;
        SceneManager.LoadScene("Mapa");
    }

    // Llama a este método desde el botón "Volver al mapa" en Tienda
    public void VolverDesdeTienda()
    {
        VariablesMapa.estadoPos5 = 1; // Abre camino de N2 a N3
        VariablesMapa.maxNivel = Mathf.Max(VariablesMapa.maxNivel, 3);
        VariablesMapa.nivelesCompletados[6] = true;
        SceneManager.LoadScene("Mapa");
    }

    // Llama a este método desde el botón "Volver al mapa" en Evento2
    public void VolverDesdeEvento2()
    {
        VariablesMapa.estadoPos7 = 1; // Abre camino de N3 a Jefe
        VariablesMapa.maxNivel = Mathf.Max(VariablesMapa.maxNivel, 4);
        VariablesMapa.nivelesCompletados[7] = true;
        SceneManager.LoadScene("Mapa");
    }

    IEnumerator ActualizarProgresoYVolver()
    {
        int idPersonaje = personajeJugador.idPersonaje;
        string urlGet = urlBaseProgreso + idPersonaje;

        // Paso 1: GET progreso actual
        UnityWebRequest getRequest = UnityWebRequest.Get(urlGet);
        getRequest.SetRequestHeader("Content-Type", "application/json");

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener el progreso actual: " + getRequest.error);
            yield break;
        }

        // Deserializar progreso
        Progreso progreso = JsonUtility.FromJson<Progreso>(getRequest.downloadHandler.text);
        int nivelActual = progreso.idNivel;

        Debug.Log("Nivel actual del progreso: " + nivelActual);

        // Paso 2: Verificar si ya es el último nivel
        if (nivelActual >= 4)
        {
            Debug.Log("Ya se alcanzó el nivel máximo. No se actualiza progreso.");
            SceneManager.LoadScene("Mapa");
            yield break;
        }

        int nuevoNivel = nivelActual + 1;

        // Paso 3: PUT con el nuevo nivel
        string urlPut = $"{urlBaseProgreso}{idPersonaje}&nivel={nuevoNivel}";
        UnityWebRequest putRequest = UnityWebRequest.Put(urlPut, new byte[0]);
        putRequest.SetRequestHeader("Content-Type", "application/json");

        yield return putRequest.SendWebRequest();

        if (putRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al actualizar el progreso: " + putRequest.error);
            yield break;
        }

        Debug.Log($"Progreso actualizado correctamente a nivel {nuevoNivel}.");

        // Paso 4: cambiar de escena
        SceneManager.LoadScene("Mapa");
    }
}


[Serializable]
public class Progreso
{
    public int idProgreso;
    public int idNivel;
    public int idPersonaje;
    public string fechaCreacion;
}