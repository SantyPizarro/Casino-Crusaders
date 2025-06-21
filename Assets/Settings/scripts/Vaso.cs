using UnityEngine;

public class Vaso : MonoBehaviour
{
    public int indice;
    public JuegoDeVasos juego;

    public void Elegir() //prueba
    {
        Debug.Log("vaso elegido");
        juego.ElegirVaso(indice);
    }

}