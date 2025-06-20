using UnityEngine;

public class Vaso : MonoBehaviour
{
    public int indice;
    public JuegoDeVasos juego;

    public void Elegir() //prueba
    {
        juego.ElegirVaso(indice);
    }

}