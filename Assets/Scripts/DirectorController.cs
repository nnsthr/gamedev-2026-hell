using UnityEngine;
using TMPro;
using System;

public class DirectorController : MonoBehaviour
{
    GameObject timerscoreText;
    GameObject lifeText;
    GameObject player;
    float time = 180f;
    int timermin=0;
    float timersec=0;
    int score=0;
    string scoredigit="00000000";
    int remainHP = 3;
    int remainLife = 3;
    void Start()
    {
        timerscoreText = GameObject.Find("TimerScore");
        lifeText = GameObject.Find("Life");
        player = GameObject.Find("player");
    }

    void Update()
    {
        time -=Time.deltaTime;
        timermin = (int)Math.Floor(time/60);
        timersec = time-timermin*60f;
        remainHP = player.GetComponent<PlayerController>().CheckremainHP();
        score = player.GetComponent<PlayerController>().CheckplScore();

        //表示する
        scoredigit=scoredigit.Remove(7-(score==0?1:(int)Math.Log10(score)))+score.ToString();
        timerscoreText.GetComponent<TextMeshProUGUI>().text = "Time "+ (timermin<=9?"0":"") + timermin.ToString() + "." + (timersec<10?"0":"") + timersec.ToString("F2") + "\nScore " + scoredigit;
        lifeText.GetComponent<TextMeshProUGUI>().text = "HP : " + remainHP.ToString() + "\nLife x " + remainLife.ToString();
        scoredigit="00000000";
    }
}
