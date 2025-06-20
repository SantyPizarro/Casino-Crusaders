using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Settings.scripts;

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

    void Start()
    {
        botonComenzar.onClick.AddListener(ComenzarJuego);
        mensaje.text = "Pulsa comenzar para mezclar los vasos.";
    }

    void ComenzarJuego()
    {
        botonComenzar.interactable = false;
        mensaje.text = "Observa bien...";
        indiceCorrecto = Random.Range(0, vasos.Length);
        recompensa.transform.position = vasos[indiceCorrecto].transform.position;
        recompensa.SetActive(false);
        StartCoroutine(MezclarVasos());
    }

    IEnumerator MezclarVasos()
    {
        int mezclas = 5;
        for (int i = 0; i < mezclas; i++)
        {
            int a = Random.Range(0, vasos.Length);
            int b;
            do
            {
                b = Random.Range(0, vasos.Length);
            } while (a == b);

            // animacion inicial
            Vector3 posA = vasos[a].transform.position;
            vasos[a].transform.position = vasos[b].transform.position;
            vasos[b].transform.position = posA;

            yield return new WaitForSeconds(0.6f);
        }

        mensaje.text = "¡Elige un vaso!";
        puedeElegir = true;
    }

    public void ElegirVaso(int indiceElegido)
    {
        if (!puedeElegir) return;

        puedeElegir = false;
        recompensa.SetActive(true);

        if (indiceElegido == indiceCorrecto)
        {

            int statElegida = Random.Range(0, 3); // 0 = vida, 1 = ataque, 2 = defensa
            string mensajeRecompensa = statElegida == 0 ? "vida" : statElegida == 1 ? "ataque" : "defensa";
            var personaje = ControlJuego.Instance.personajeJugador;

            mensaje.text = "¡Correcto! tu recompensa es..." + mensajeRecompensa;

            switch (statElegida)
            {
                case 0:
                    personaje.vida_maxima += 5;
                    personaje.vida_actual += 5; // para que lo cure
                    imagenRecompensa.sprite = spriteVida;
                    Debug.Log(personaje.vida_actual);
                    break;
                case 1:
                    personaje.daño_ataque += 5;
                    imagenRecompensa.sprite = spriteAtaque;
                    Debug.Log(personaje.daño_ataque);

                    break;
                case 2:
                    personaje.defensa += 5;
                    imagenRecompensa.sprite = spriteDefensa;
                    Debug.Log(personaje.defensa);
                    break;
            }
        }
        else
        {
            mensaje.text = "¡Fallaste!";
            imagenRecompensa.sprite = null;
        }

        botonComenzar.interactable = true;
    }
}
