using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;


public class TiendaController : MonoBehaviour
{
    public int monedas;
    public int vidaActual;
    public int vidaMaxima;
    public int armadura;
    public int dano;
    public Button botonVolverMapa;

    public Text TextoMonedas;
    public Text TextoVida;
    public Text TextoVidaMax;
    public Text TextoArmadura;
    public Text TextoDano;

    private const int COSTO = 5;

    // URL de la API (puedes cambiar el ID si lo necesitas)
    public string apiUrlPut = "https://localhost:7000/api/PersonajeApi";


    private Dictionary<string, Objeto> objetosDisponibles = new Dictionary<string, Objeto>();
    public string apiUrlObjetos = "https://localhost:7000/api/ObjetoApi";

    void Start()
    {
        StartCoroutine(CargarObjetosDesdeAPI());
        ControlJuego.Instance.ResetVolverAlMapa();
        botonVolverMapa.gameObject.SetActive(true);

        // Agregar listener manualmente para evitar conflictos desde el Inspector
        botonVolverMapa.onClick.RemoveAllListeners();
        botonVolverMapa.onClick.AddListener(() =>
        {
            Debug.Log("Botón Volver al Mapa presionado");
            ControlJuego.Instance.VolverDesdeTienda();
        });

        vidaActual = ControlJuego.Instance.personajeJugador.vidaActual;
        vidaMaxima = ControlJuego.Instance.personajeJugador.vidaMaxima;
        dano = ControlJuego.Instance.personajeJugador.danoAtaque;
        armadura = ControlJuego.Instance.personajeJugador.defensa;
        monedas = ControlJuego.Instance.personajeJugador.monedas;

        ActualizarUI();
    }


    public void ComprarVidaMax()
    {
        Comprar("VidaMax", () => vidaMaxima += objetosDisponibles["VidaMax"].estadistica);
    }

    public void ComprarArmadura()
    {
        Comprar("Armadura", () => armadura += objetosDisponibles["Armadura"].estadistica);
    }

    public void ComprarVida()
    {
        Comprar("Pocion", () =>
        {
            vidaActual += objetosDisponibles["Pocion"].estadistica;
            if (vidaActual > vidaMaxima)
                vidaActual = vidaMaxima;
        });
    }

    public void ComprarDano()
    {
        Comprar("Espada", () => dano += objetosDisponibles["Espada"].estadistica);
    }

    void ActualizarUI()
    {
        TextoMonedas.text = "Monedas: " + monedas;
        TextoVida.text = "Vida: " + vidaActual;
        TextoVidaMax.text = "Vida Máx: " + vidaMaxima;
        TextoArmadura.text = "Armadura: " + armadura;
        TextoDano.text = "Daño: " + dano;
    }

    private void Comprar(string nombreObjeto, System.Action aplicarEfecto)
    {
        if (!objetosDisponibles.ContainsKey(nombreObjeto))
        {
            Debug.LogError($"Objeto '{nombreObjeto}' no encontrado.");
            return;
        }

        Objeto obj = objetosDisponibles[nombreObjeto];

        if (monedas >= obj.precio)
        {
            aplicarEfecto.Invoke();
            monedas -= obj.precio;
            ActualizarUI();
        }
        else
        {
            Debug.Log("No tienes suficientes monedas.");
        }
    }

    public void GuardarPersonaje()
    {
        ControlJuego.Instance.personajeJugador.vidaActual = vidaActual;
       ControlJuego.Instance.personajeJugador.vidaMaxima = vidaMaxima  ;
       ControlJuego.Instance.personajeJugador.danoAtaque = dano ;
        ControlJuego.Instance.personajeJugador.defensa = armadura;
         ControlJuego.Instance.personajeJugador.monedas = monedas;
        ControlJuego.Instance.GuardarPersonaje(this);
    }

    IEnumerator CargarObjetosDesdeAPI()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrlObjetos);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"objetos\":" + request.downloadHandler.text + "}";
            ObjetoWrapper wrapper = JsonUtility.FromJson<ObjetoWrapper>(json);

            objetosDisponibles = wrapper.objetos.ToDictionary(o => o.nombre, o => o);
            Debug.Log("Objetos cargados correctamente.");
        }
        else
        {
            Debug.LogError("Error al cargar objetos: " + request.error);
        }
    }

    //public IEnumerator PutPersonaje()
    //{
    //    // Armamos el objeto personaje con los valores actuales de la tienda
    //    Personaje personaje = new Personaje()
    //    {
    //        idPersonaje = 1, 
    //        vidaActual = vidaActual,
    //        vidaMaxima = vidaMaxima,
    //        defensa = armadura,
    //        danoAtaque = dano,
    //        monedas = monedas
    //    };

    //    string json = JsonUtility.ToJson(personaje);

    //    UnityWebRequest request = new UnityWebRequest(apiUrlPut, "PUT");
    //    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
    //    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log("Personaje actualizado correctamente");
    //        SceneManager.LoadScene("Titulo"); 
    //    }
    //    else
    //    {
    //        Debug.LogError("Error al actualizar personaje: " + request.error);
    //        SceneManager.LoadScene("Titulo");
    //    }
    //}
}

[System.Serializable]
public class Objeto
{
    public int idObjeto;
    public string nombre;
    public int estadistica;
    public int precio;
}

[System.Serializable]
public class ObjetoWrapper
{
    public List<Objeto> objetos;
}

