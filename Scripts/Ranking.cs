using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ranking : MonoBehaviour {

    private Text Rank;
    private Text Name;
    private Text Score;
    private Text PointCurrentPlayer;
    private string difficulty;
    
	// Use this for initialization
	void Start () {

        Rank = GameObject.Find("Rank").GetComponent<Text>();
        Name = GameObject.Find("Name").GetComponent<Text>();
        Score = GameObject.Find("Score").GetComponent<Text>();
        PointCurrentPlayer = GameObject.Find("PointCurrentPlayer").GetComponent<Text>();
        difficulty = GameObject.Find("Remain_info/Difficulty_value").GetComponent<Text>().text;

        string rank_txt = "Rank\n";
        string name_txt = "Name\n";
        string score_txt = "Score\n";
        string point_txt = "\n";

        string rankFileName = "";

        if (difficulty == "easy")
        {
            rankFileName = "rank_easy.txt";
        }
        else if (difficulty == "normal")
        {
            rankFileName = "rank_normal.txt";
        }
        else if (difficulty == "hard")
        {
            rankFileName = "rank_hard.txt";
        }

        string[] lines = System.IO.File.ReadAllLines(rankFileName);
        foreach (string line in lines)
        {
            Debug.Log(line);
            string[] line_list = line.Split(' ');

            if (line_list[0] == "n")
            {
                point_txt += "\n";
            }
            else if (line_list[0] == "t")
            {
                point_txt += "v\n";
            }
            else
            {
                continue;
            }


            rank_txt += line_list[1] + "\n";
            name_txt += line_list[2] + "\n";
            score_txt += line_list[3] + "\n";
        }

        PointCurrentPlayer.text = point_txt;
        Rank.text = rank_txt;
        Name.text = name_txt;
        Score.text = score_txt;

    }
	
	// Update is called once per frame
	void Update () {

    }
}
