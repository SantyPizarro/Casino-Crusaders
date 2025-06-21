using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JuegoDeVasos : MonoBehaviour
{
    public GameObject[] vasos;
    public Transform[] posiciones;
    public GameObject recompensa;
    public Text mensaje;
    public Button botonComenzar;
    public Sprite spriteVida;
    public Sprite spriteAtaque;
    public Sprite spriteDefensa;
    public Image imagenRecompensa;

    private int indiceCorrecto;
    private bool puedeElegir = false;

    private int statElegida = -1;

    public Vector3 posicionCentro;
    public float duracionMovimiento = 1f;

    void Start()
    {
        botonComenzar.onClick.AddListener(ComenzarJuego);
        mensaje.text = "Pulsa comenzar para mezclar los vasos.";

        recompensa.SetActive(true);
    }

    void ComenzarJuego()
    {
        botonComenzar.interactable = false;
        mensaje.text = "Observa bien...";

        recompensa.SetActive(false);

        indiceCorrecto = Random.Range(0, vasos.Length);

        statElegida = Random.Range(0, 3);
        switch (statElegida)
        {
            case 0: imagenRecompensa.sprite = spriteVida; break;
            case 1: imagenRecompensa.sprite = spriteAtaque; break;
            case 2: imagenRecompensa.sprite = spriteDefensa; break;
        }

        StartCoroutine(DestacarVasoCorrecto());
        StartCoroutine(EjecutarMezclaConDelay());
    }

    IEnumerator MezclarVasos()
    {
        int mezclas = 10;
        float duracionMovimiento = 0.3f;

        for (int i = 0; i < mezclas; i++)
        {
            int a = Random.Range(0, vasos.Length);
            int b;
            do
            {
                b = Random.Range(0, vasos.Length);
            } while (a == b);

            yield return StartCoroutine(IntercambiarPosiciones(vasos[a], vasos[b], duracionMovimiento));
        }

        mensaje.text = "¡Elige un vaso!";
        puedeElegir = true;
    }

    public void ElegirVaso(int indiceElegido)
    {
        if (!puedeElegir) return;

        puedeElegir = false;
        recompensa.SetActive(true);
        botonComenzar.gameObject.SetActive(false);

        var personaje = ControlJuego.Instance.personajeJugador;

        if (indiceElegido == indiceCorrecto)
        {
            string mensajeRecompensa = statElegida == 0 ? "vida" : statElegida == 1 ? "ataque" : "defensa";
            mensaje.text = "¡Correcto! tu recompensa es..." + mensajeRecompensa;

            switch (statElegida)
            {
                case 0:
                    personaje.vidaMaxima += 5;
                    personaje.vidaActual += 5;
                    Debug.Log(personaje.vidaActual);
                    break;
                case 1:
                    personaje.danoAtaque += 5;
                    Debug.Log(personaje.danoAtaque);
                    break;
                case 2:
                    personaje.defensa += 5;
                    Debug.Log(personaje.defensa);
                    break;
            }

            imagenRecompensa.enabled = true;
            StartCoroutine(terminarJuego(true));
        }
        else
        {
            mensaje.text = "¡Fallaste!";
            imagenRecompensa.enabled = false;
            StartCoroutine(terminarJuego(true));
        }

        botonComenzar.interactable = true;
    }


    IEnumerator MoverRecompensaAlVaso(Vector3 destino)
    {
        imagenRecompensa.rectTransform.position = destino;
        imagenRecompensa.enabled = true;

        Vector3 posOriginal = destino;
        Vector3 posLevantada = posOriginal + new Vector3(0, 30, 0);

        float duracionLevantada = 0.3f;
        float tiempo = 0;

        while (tiempo < duracionLevantada)
        {
            vasos[indiceCorrecto].transform.position = Vector3.Lerp(posOriginal, posLevantada, tiempo / duracionLevantada);
            tiempo += Time.deltaTime;
            yield return null;
        }

        vasos[indiceCorrecto].transform.position = posLevantada;

        yield return new WaitForSeconds(1.2f);
    }


    IEnumerator terminarJuego(bool acerto)
    {
        if (acerto)
        {
            yield return StartCoroutine(MoverRecompensaAlVaso(vasos[indiceCorrecto].transform.position));
        }

        yield return new WaitForSeconds(1.5f);

        ControlJuego.Instance.AvanzarASiguienteEscena();
    }

    IEnumerator IntercambiarPosiciones(GameObject vasoA, GameObject vasoB, float duracion)
    {
        Vector3 inicioA = vasoA.transform.position;
        Vector3 inicioB = vasoB.transform.position;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            vasoA.transform.position = Vector3.Lerp(inicioA, inicioB, t);
            vasoB.transform.position = Vector3.Lerp(inicioB, inicioA, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        vasoA.transform.position = inicioB;
        vasoB.transform.position = inicioA;
    }

    IEnumerator EjecutarMezclaConDelay(float delay = 1.2f)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(MezclarVasos());
    }


    //esta para image
    IEnumerator DestacarVasoCorrecto()
    {
        Image imagenVaso = vasos[indiceCorrecto].GetComponent<Image>();
        if (imagenVaso == null) yield break;

        int parpadeos = 4;
        float intervalo = 0.2f;

        Color colorOriginal = imagenVaso.color;
        Color colorDestacado = Color.yellow;

        for (int i = 0; i < parpadeos; i++)
        {
            imagenVaso.color = colorDestacado;
            yield return new WaitForSeconds(intervalo);
            imagenVaso.color = colorOriginal;
            yield return new WaitForSeconds(intervalo);
        }
    }

}
