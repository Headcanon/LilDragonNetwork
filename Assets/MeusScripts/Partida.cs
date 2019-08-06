using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Partida : MonoBehaviour
{
    [SerializeField] Text textoTela;
    [SerializeField] GameObject placar;

    float currentTime = 0;
    [SerializeField] float startTime = 10;

    PlayerControl[] players;
    PlayerControl max;

    private void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        if (currentTime >= 0)
        {
            currentTime -= Time.deltaTime;
            textoTela.text = "Tempo: " + currentTime.ToString("0");
        }
        else
        {
            textoTela.text = "Tempo esgotado!";

            

            //players = FindObjectsOfType<PlayerControl>();
            //if(players == null) { Debug.Log("Deu merda"); }
            //max = players[0];
            //for (int i = 1; i > players.Length; i++)
            //{
            //    if(players[i].killcount > max.killcount) { max = players[i]; }
            //    if (i > players.Length) { return; }
            //}

            //textoTela.text = max.nome; 

            placar.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
