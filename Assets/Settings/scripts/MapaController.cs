using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapaController : MonoBehaviour
{
    [SerializeField] float velocidad = 5;
    [SerializeField] Transform[] pos;
    //[SerializeField] SpriteRenderer[] posIconos;
    [SerializeField] Transform[] caminos;
    [SerializeField] Transform player;
    //[SerializeField] AudioSource sonidoCamino;

    int playerPos = 0;
    int maxNivel = 7;
    bool puedoMover = false;

    void Awake()
    {
        if (!VariablesMapa.juegoIniciado)
        {
            VariablesMapa.nivel = 0;
            VariablesMapa.maxNivel = 1;
            VariablesMapa.juegoIniciado = true;
            VariablesMapa.estadoPos1 = 2;
            VariablesMapa.estadoPos2 = 2;
            VariablesMapa.estadoPos3 = 2;
            VariablesMapa.estadoPos4 = 2; // Tienda
            VariablesMapa.estadoPos5 = 2; // Nivel3
            VariablesMapa.estadoPos6 = 2; // Evento2
            VariablesMapa.estadoPos7 = 2; // Jefe

        }

        playerPos = VariablesMapa.nivel;
        maxNivel = VariablesMapa.maxNivel;

        puedoMover = false;
        player.position = pos[playerPos].position;

        if (VariablesMapa.estadoPos1 == 1)
        {
            caminos[0].localScale = new Vector3(1, 1, 1);
            //posIconos[0].enabled = true;
        }
        else if (VariablesMapa.estadoPos1 == 2)
        {
            StartCoroutine("AbreCamino", 0);
        }

        AbrirCaminosPorPosicion(playerPos);
        puedoMover = true;

    }

    IEnumerator AbreCamino(int numCamino)
    {
        caminos[numCamino].localScale = new Vector3(1, 0.5f, 1); // Escala final instantánea
        puedoMover = true;
        if (numCamino == 0) VariablesMapa.estadoPos1 = 1;
        if (numCamino == 1)
        {
            VariablesMapa.estadoPos2 = 1;
            maxNivel = Mathf.Max(maxNivel, 2); // Permite avanzar a N2
        }
        if (numCamino == 2) VariablesMapa.estadoPos3 = 1;
        if (numCamino == 3) VariablesMapa.estadoPos4 = 1;
        if (numCamino == 4)
        {
            VariablesMapa.estadoPos5 = 1;
            maxNivel = Mathf.Max(maxNivel, 3); // Permite avanzar a N3
        }
        if (numCamino == 5) VariablesMapa.estadoPos6 = 1;
        if (numCamino == 6)
        {
            VariablesMapa.estadoPos7 = 1;
            maxNivel = Mathf.Max(maxNivel, 4); // Permite avanzar al jefe
        }
        yield break;
    }


    int GetVerticalDestination(int current, int direction)
    {
        // direction: -1 = abajo, 1 = arriba
        if (current == 1 && direction == -1) return 5; // N1 -> Evento1
        if (current == 5 && direction == 1) return 1;  // Evento1 -> N1
        if (current == 2 && direction == -1) return 6; // N2 -> Tienda
        if (current == 6 && direction == 1) return 2;  // Tienda -> N2
        if (current == 3 && direction == -1) return 7; // N3 -> Evento2
        if (current == 7 && direction == 1) return 3;  // Evento2 -> N3
        return -1;
    }
    //a
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (puedoMover)
        {
            int destinoVertical = GetVerticalDestination(playerPos, (int)v);
            if (destinoVertical != -1 && /* aquí tu condición de camino abierto */ v != 0)
            {
                puedoMover = false;
                StartCoroutine("MoverPlayerVertical", destinoVertical);
            }
        }

        if ((h == 1 && playerPos < maxNivel && puedoMover) || (h == -1 && playerPos > 0 && puedoMover))
        {
            puedoMover = false;
            StartCoroutine("MoverPlayer", (int)h);
        }


        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (playerPos == 1)
            {
                Debug.Log("Intentando entrar a Combate1. Completado: " + VariablesMapa.nivelesCompletados[1]);

                if (!VariablesMapa.nivelesCompletados[1])
                {
                    VariablesMapa.nivel = playerPos;
                    CambiarEscena("Pelea2");
                }
                else
                {
                    Debug.Log("¡Este nivel ya fue completado!");
                }
            }
            else if (playerPos == 2)
            {
                Debug.Log("Intentando entrar a Combate2. playerPos: " + playerPos + " nivel completado: " + VariablesMapa.nivelesCompletados[2]);
                if (!VariablesMapa.nivelesCompletados[2])
                {
                    VariablesMapa.nivel = playerPos;
                    CambiarEscena("Combate5");
                }
                else
                {
                    Debug.Log("¡Este nivel ya fue completado!");
                }
            }
            else if (playerPos == 3)
            {
                Debug.Log("Intentando entrar a Combate2. playerPos: " + playerPos + " nivel completado: " + VariablesMapa.nivelesCompletados[2]);
                if (!VariablesMapa.nivelesCompletados[3])
                {
                    VariablesMapa.nivel = playerPos;
                    CambiarEscena("Combate3");
                }
                else
                {
                    Debug.Log("¡Este nivel ya fue completado!");
                }
            }

            else if (playerPos == 4)
            {
                Debug.Log("Intentando entrar a Combate2. playerPos: " + playerPos + " nivel completado: " + VariablesMapa.nivelesCompletados[2]);
                if (!VariablesMapa.nivelesCompletados[3])
                {
                    VariablesMapa.nivel = playerPos;
                    CambiarEscena("Combate1");
                }
                else
                {
                    Debug.Log("¡Este nivel ya fue completado!");
                }
            }
            else if (playerPos == 5) // EventoDados
            {
                CambiarEscena("EventoDados");
            }
            else if (playerPos == 7) // Evento1
            {
                CambiarEscena("Evento1");
            }
            else if (playerPos == 6)
            {
                CambiarEscena("Tienda");
            }
        }

    }

    void AbrirCaminosPorPosicion(int posActual)
    {
        switch (posActual)
        {
            case 0: // Salida
                if (VariablesMapa.estadoPos1 == 2)
                    StartCoroutine("AbreCamino", 0); // Camino_SAaN1
                break;
            case 1: // Nivel1
                if (VariablesMapa.estadoPos2 == 2)
                    StartCoroutine("AbreCamino", 1); // Camino_N1aN2
                if (VariablesMapa.estadoPos3 == 2)
                    StartCoroutine("AbreCamino", 2); // Camino_N1aE1
                break;
            case 2: // Nivel2
                if (VariablesMapa.estadoPos4 == 2)
                    StartCoroutine("AbreCamino", 3); // Camino_N2aTienda
                if (VariablesMapa.estadoPos5 == 2)
                    StartCoroutine("AbreCamino", 4); // Camino_N2aN3
                break;
            case 3: // Nivel3
                if (VariablesMapa.estadoPos6 == 2)
                    StartCoroutine("AbreCamino", 5); // Camino_N3aE2
                if (VariablesMapa.estadoPos7 == 2)
                    StartCoroutine("AbreCamino", 6); // Camino_N3aJefe
                break;
        }
    }

    IEnumerator MoverPlayer(int mov)
    {
        int destino = playerPos + mov;
        if (destino < 0 || destino >= pos.Length)
        {
            puedoMover = true;
            yield break;
        }

        Vector3 distancia = Vector3.zero;
        do
        {
            player.transform.Translate(0.05f * mov, 0, 0);
            distancia = pos[destino].position - player.position;
            yield return new WaitForSeconds(0.01F);
        } while (distancia.sqrMagnitude > 0.001F);
        player.position = pos[destino].position;
        playerPos = destino; // Actualiza aquí
        yield return new WaitForSeconds(0.15F);
        puedoMover = true;
    }


    IEnumerator MoverPlayerVertical(int destino)
    {
        Vector3 distancia = Vector3.zero;
        do
        {
            player.position = Vector3.MoveTowards(player.position, pos[destino].position, 0.05f);
            distancia = pos[destino].position - player.position;
            yield return new WaitForSeconds(0.01F);
        } while (distancia.sqrMagnitude > 0.001F);
        player.position = pos[destino].position;
        playerPos = destino;
        yield return new WaitForSeconds(0.15F);
        puedoMover = true;
    }

    private void OnApplicationQuit()
    {
        VariablesMapa.juegoIniciado = false;
    }

    public void CambiarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }

   
}
