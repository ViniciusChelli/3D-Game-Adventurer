using System.Collections.Generic;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    //List

    public GameObject[] inimigos;

    public GameObject especialEnemy;
    public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        //inicio ; condicao ; passo
        //for (int i = 0; i < inimigos.Length; i++)
        //{
        //    inimigos[i].SetActive(false);
        //}

        ////tipo ; colecao (lista/array)
        //foreach (GameObject inimigo in inimigos)
        //{
        //    inimigo.SetActive(false);
        //}


        //for (int i = 0; i < enemies.Count; i++)
        //{
        //    enemies[i].SetActive(false);
        //}

        //foreach (GameObject inimigo in enemies)
        //{
        //    inimigo.SetActive(false);
        //}

        Debug.Log(enemies[2].name);
    }
}
