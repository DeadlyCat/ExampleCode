using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class UseEnemyCloch : MonoBehaviour
{
    string Link = "https://animedance.ru/";
    string[] Use;
    [SerializeField] int[] UseCloch = new int[7];
    EnemyCloth enemyCloth;
    void Awake()
    {

        string account = PlayerPrefs.GetString("Account");
        string[] words = account.Split(new char[] { ',' });
        string login = words[0];
        StartCoroutine(CallCloch(login));

    }
    void Start()
    {

        enemyCloth = GetComponent<EnemyCloth>();

        for (int i = 0; i < enemyCloth.Hats.Count; i++)
        {
            enemyCloth.Hats[i].SetActive(false);
        }
        for (int i = 0; i < enemyCloth.Hairs.Count; i++)
        {
            enemyCloth.Hairs[i].SetActive(false);
        }
        for (int i = 0; i < enemyCloth.Top.Count; i++)
        {
            enemyCloth.Top[i].SetActive(false);
        }
        for (int i = 0; i < enemyCloth.Bottom.Count; i++)
        {
            enemyCloth.Bottom[i].SetActive(false);
        }
        for (int i = 0; i < enemyCloth.Shoes.Count; i++)
        {
            enemyCloth.Shoes[i].SetActive(false);
        }

    }
    IEnumerator CallCloch(string login)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("Login", login));
        form.Add(new MultipartFormDataSection("Statys", "ClochEnemy"));
        form.Add(new MultipartFormDataSection("Server", PlayerPrefs.GetString("ServerName")));
        UnityWebRequest www = UnityWebRequest.Post(Link, form);
        yield return www.SendWebRequest();
        string res = DownloadHandlerBuffer.GetContent(www).Trim();
        Use = res.Split(new char[] { ',' });
      
        enemyCloth.Hats[Convert.ToInt32(Use[0])].SetActive(true);
        enemyCloth.Hairs[Convert.ToInt32(Use[1])].SetActive(true);
        enemyCloth.Top[Convert.ToInt32(Use[2])].SetActive(true);
        enemyCloth.Bottom[Convert.ToInt32(Use[3])].SetActive(true);
        enemyCloth.Shoes[Convert.ToInt32(Use[4])].SetActive(true);
        enemyCloth.body.GetComponent<Renderer>().material = enemyCloth.Underwear[Convert.ToInt32(Use[5])];
        enemyCloth.eyes.GetComponent<Renderer>().materials[0].mainTexture = enemyCloth.eyesL[Convert.ToInt32(Use[6])];
        enemyCloth.eyes.GetComponent<Renderer>().materials[1].mainTexture = enemyCloth.eyesR[Convert.ToInt32(Use[6])];
        StopCoroutine(CallCloch(login));
    }
    
    void Update()
    {
        

    }
}
