using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapaController : MonoBehaviour
{
    [SerializeField] float velocidad = 5;
    [SerializeField] Transform[] pos;
    [SerializeField] SpriteRenderer[] posIconos;
    //[SerializeField] SpriteRenderer[] posIconos;
    [SerializeField] Transform[] caminos;
    [SerializeField] Transform player;
    //[SerializeField] AudioSource sonidoCamino;

    int playerPos = 0;
    int maxNivel = 7;
    bool puedoMover = false;

    void Awake()
    {
        StartCoroutine(ActualizarColoresConDelay());
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
            //caminos[0].localScale = new Vector3(1, 1, 1);
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
       // caminos[numCamino].localScale = new Vector3(1, 1, 1); // Escala final instant�nea
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
    
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (puedoMover)
        {
            int destinoVertical = GetVerticalDestination(playerPos, (int)v);
            if (destinoVertical != -1 && v != 0 && EsCasillaDesbloqueada(destinoVertical))
            {
                puedoMover = false;
                StartCoroutine("MoverPlayerVertical", destinoVertical);
            }
        }

        int destino = playerPos + (int)h;
        if ((h != 0 && puedoMover && destino >= 0 && destino < pos.Length && EsCasillaDesbloqueada(destino)))
        {
            puedoMover = false;
            StartCoroutine("MoverPlayer", (int)h);
        }


        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            switch (playerPos)
            {
                case 1: // Nivel 1
                    Debug.Log("Intentando entrar a Pelea2. Completado: " + VariablesMapa.nivelesCompletados[1]);
                    if (!VariablesMapa.nivelesCompletados[1])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("Combate1");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;

                case 2: // Nivel 2
                    Debug.Log("Intentando entrar a Combate5. Completado: " + VariablesMapa.nivelesCompletados[2]);
                    if (!VariablesMapa.nivelesCompletados[2])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("Combate2");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;

                case 3: // Nivel 3
                    Debug.Log("Intentando entrar a Combate3. Completado: " + VariablesMapa.nivelesCompletados[3]);
                    if (!VariablesMapa.nivelesCompletados[3])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("Combate3");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;

                case 4: // Jefe final 
                    Debug.Log("Intentando entrar a Combate7. Completado: " + VariablesMapa.nivelesCompletados[4]);
                    if (!VariablesMapa.nivelesCompletados[4])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("Combate4");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;

                case 5: // Evento dados
                    if (!VariablesMapa.nivelesCompletados[5])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("EventoDados");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;
                  //  VariablesMapa.nivel = playerPos;
                   // CambiarEscena("EventoDados");
                    //break;

                case 6: // Tienda
                    if (!VariablesMapa.nivelesCompletados[6])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("Tienda");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;
                  //  VariablesMapa.nivel = playerPos;
                   // CambiarEscena("Tienda");
                    //break;

                case 7: // Evento 1
                    if (!VariablesMapa.nivelesCompletados[7])
                    {
                        VariablesMapa.nivel = playerPos;
                        CambiarEscena("Evento1");
                    }
                    else
                    {
                        Debug.Log("�Este nivel ya fue completado!");
                    }
                    break;
                 //   VariablesMapa.nivel = playerPos;
                    //CambiarEscena("Evento1");
                    //break;

                default:
                    Debug.Log("Posici�n inv�lida o no asignada a una escena.");
                    break;
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
        playerPos = destino; // Actualiza aqu�
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

    IEnumerator ActualizarColoresConDelay()
    {
        yield return new WaitForEndOfFrame();
        ActualizarColoresCasillas();
    }

    void ActualizarColoresCasillas()
    {
        for (int i = 0; i < posIconos.Length; i++)
        {
            bool bloqueado = false;

            switch (i)
            {
                case 1: bloqueado = VariablesMapa.estadoPos1 != 1; break;
                case 2: bloqueado = VariablesMapa.estadoPos2 != 1; break;
                case 3: bloqueado = VariablesMapa.estadoPos3 != 1; break;
                case 4: bloqueado = VariablesMapa.estadoPos4 != 1; break;
                case 5: bloqueado = VariablesMapa.estadoPos5 != 1; break;
                case 6: bloqueado = VariablesMapa.estadoPos6 != 1; break;
                case 7: bloqueado = VariablesMapa.estadoPos7 != 1; break;
            }

            if (bloqueado)
            {
                posIconos[i].color = Color.gray; // Bloqueado = gris
            }
            else
            {
                posIconos[i].color = Color.white; // Desbloqueado = blanco
            }
        }
    }
    bool EsCasillaDesbloqueada(int destino)
    {
        switch (playerPos)
        {
            case 0:
                return destino == 1 && VariablesMapa.estadoPos1 == 1;

            case 1:
                if (destino == 2) return VariablesMapa.estadoPos2 == 1; // N1 ? N2
                if (destino == 5) return VariablesMapa.estadoPos3 == 1; // N1 ? Evento1
                break;

            case 2:
                if (destino == 1) return VariablesMapa.estadoPos2 == 1; // N2 ? N1
                if (destino == 3) return VariablesMapa.estadoPos5 == 1; // N2 ? N3
                if (destino == 6) return VariablesMapa.estadoPos4 == 1; // N2 ? Tienda
                break;

            case 3:
                if (destino == 2) return VariablesMapa.estadoPos5 == 1; // N3 ? N2
                if (destino == 4) return VariablesMapa.estadoPos7 == 1; // N3 ? Jefe
                if (destino == 7) return VariablesMapa.estadoPos6 == 1; // N3 ? Evento2
                break;

            case 4:
                return destino == 3 && VariablesMapa.estadoPos7 == 1; // Jefe ? N3

            case 5:
                return destino == 1 && VariablesMapa.estadoPos3 == 1; // Evento1 ?? N1

            case 6:
                return destino == 2 && VariablesMapa.estadoPos4 == 1; // Tienda ?? N2

            case 7:
                return destino == 3 && VariablesMapa.estadoPos6 == 1; // Evento2 ?? N3
        }

        return false;
    }
}
