using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour {

    public GameObject Character;
    public bool start_timer;
    public bool end_game;
    public bool win;
    public float time;

    private Transform ScoreText;
    private Camera mainCamera;
    private float countDown_time;
    private GameObject Count_down;
    private GameObject Game_over;
    private Color original_color;
    private GameObject CountDownTemplate;
    private GameObject GameOverTemplate;
    private GameObject WinnerTemplate;
    private GameObject RemainInfo;
    private GameObject Winner;
    private float counter;




	// Use this for initialization
	void Start () {

        mainCamera = Character.transform.Find("Pivot/Camera").gameObject.GetComponent<Camera>();
        ScoreText = Character.transform.Find("Canvas/Score");
        RemainInfo = GameObject.Find("Remain_info");

        ScoreText.localPosition = GetScoreTextPosition(); ;
        time = 0;
        countDown_time = 3;
        start_timer = false;
        end_game = false;
        win = false;


        // count down
        CountDownTemplate = GameObject.Find("Template/CountDown_template");
        Count_down = Instantiate(CountDownTemplate, Character.transform.Find("Canvas"));

        Count_down.name = "Count_down";
        Count_down.GetComponent<Text>().enabled = true;
        Count_down.transform.localPosition = Vector3.zero;
        original_color = Count_down.GetComponent<Text>().color;

        // gameover animation
        GameOverTemplate = GameObject.Find("Template/GameOver_template");

        WinnerTemplate = GameObject.Find("Template/Winner_template");
    }
	
	// Update is called once per frame
	void Update () {

        if (!end_game)
        {
            if (start_timer)
            {
                time += Time.deltaTime;
                if (Character.transform.Find("Canvas/Count_down") != null)
                {
                    if (time > 1)
                    {
                        Destroy(Character.transform.Find("Canvas/Count_down").gameObject);
                    }
                    else
                    {
                        ColorFadded();
                    }
                }
            }
            else
            {
                CountDown();
                ColorFadded();
            }

            // Update timmer text:
            ScoreText.GetComponent<Text>().text = string.Format("S c o r e : {0}", time);
            ScoreText.localPosition = GetScoreTextPosition();


            if (GameObject.Find("/" + Character.name + "_lost"))
            {
                end_game = true;

                // visible cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // gameover animation
                Game_over = Instantiate(GameOverTemplate, Character.transform.Find("Canvas"));
                Game_over.transform.Find("GameOver_text").gameObject.GetComponent<Text>().enabled = true;
                Game_over.transform.Find("GameOver_text").gameObject.GetComponent<Text>().text = "Game Over!";
                Game_over.transform.localPosition = GetGameOverPossition();
                Game_over.GetComponent<AudioSource>().enabled = true;

                if(Character.name == "Character") GameObject.Find("BGM").SetActive(false);

                counter = 0;
            }
            else if(GameObject.Find("/" + Character.name + "_finished"))
            {
                end_game = true;
                win = true;

                // visible cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // finish animation
                Game_over = Instantiate(GameOverTemplate, Character.transform.Find("Canvas"));
                Game_over.transform.Find("GameOver_text").gameObject.GetComponent<Text>().enabled = true;
                Game_over.transform.Find("GameOver_text").gameObject.GetComponent<Text>().text = "Finish!";
                Game_over.transform.localPosition = GetGameOverPossition();

                counter = 0;
            }

        }
        else if (counter < 6)
        {
            counter += Time.deltaTime;
        }
        else
        {
            if (Character.name == "Character")
            {
                if (win)
                {
                    WriteScoreToTxt(RemainInfo.transform.Find("Difficulty_value").gameObject.GetComponent<Text>().text);
                    SceneManager.LoadScene("RankBoard");
                }
                else
                {
                    if (Character.transform.Find("Canvas/BackToMenu") == null)
                    {
                        GameObject BackToMenuTemplate = GameObject.Find("Template/BackToMenu_template");
                        GameObject BackToMenu = Instantiate(BackToMenuTemplate, Character.transform.Find("Canvas"));
                        BackToMenu.name = "BackToMenu";
                        BackToMenu.GetComponent<Image>().enabled = true;
                        BackToMenu.GetComponent<Button>().enabled = true;
                        BackToMenu.transform.Find("Text").gameObject.GetComponent<Text>().enabled = true;
                        BackToMenu.transform.localPosition = new Vector3(0f, -mainCamera.pixelHeight / 4f, 0f);
                    }
                }

            }
            else if ((Character.name == "Character1" && GameObject.Find("Character2/Canvas/Score").GetComponent<Score>().end_game) ||
                     (Character.name == "Character2" && GameObject.Find("Character1/Canvas/Score").GetComponent<Score>().end_game))
            {

                if (GameObject.Find("Remain_info/Winner") == null) {
                    Winner = Instantiate(WinnerTemplate, RemainInfo.transform);
                    Winner.GetComponent<Text>().enabled = true;
                    Winner.name = "Winner";
                }


                if (GameObject.Find("Remain_info/Winner") != null)
                {
                    Winner = GameObject.Find("Remain_info/Winner");

                    if (Character.name == "Character1")
                    {
                        GameObject Character2 = GameObject.Find("Character2/Canvas/Score");

                        if (!win && !Character2.GetComponent<Score>().win)
                        {
                            // both lose
                            if (time > Character2.GetComponent<Score>().time)
                            {
                                // P1 win
                                Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                            }
                            else
                            {
                                // P2 win
                                Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                            }
                        }
                        else if (!win && Character2.GetComponent<Score>().win)
                        {
                            // P1 lose P2 win
                            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                        }
                        else if (win && !Character2.GetComponent<Score>().win)
                        {
                            //P1 win P2 lose
                            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                        }

                        if (Character2.GetComponent<Score>().counter > 6)
                        {
                            DontDestroyOnLoad(RemainInfo);
                            SceneManager.LoadScene("TwoPlayerResult");
                        }

                    }
                    else if (Character.name == "Character2")
                    {
                        GameObject Character1 = GameObject.Find("Character1/Canvas/Score");

                        if (!win && !Character1.GetComponent<Score>().win)
                        {
                            // both lose
                            if (time > Character1.GetComponent<Score>().time)
                            {
                                // P2 win
                                Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                            }
                            else
                            {
                                // P1 win
                                Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                            }
                        }
                        else if (!win && Character1.GetComponent<Score>().win)
                        {
                            // P2 lose P1 win
                            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                        }
                        else if (win && !Character1.GetComponent<Score>().win)
                        {
                            //P2 win P1 lose
                            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                        }


                        if (Character1.GetComponent<Score>().counter > 6)
                        {
                            DontDestroyOnLoad(RemainInfo);
                            SceneManager.LoadScene("TwoPlayerResult");
                        }

                    }
                }





                //if (Character.name == "Character1")
                //{
                //    GameObject Character2 = GameObject.Find("Character2/Canvas/Score");

                //    if (!win && !Character2.GetComponent<Score>().win)
                //    {
                //        // both lose
                //        if (time > Character2.GetComponent<Score>().time)
                //        {
                //            // P1 win
                //            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                //        }
                //        else
                //        {
                //            // P2 win
                //            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                //        }
                //    }
                //    else if (!win && Character2.GetComponent<Score>().win)
                //    {
                //        // P1 lose P2 win
                //        Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                //    }
                //    else if (win && !Character2.GetComponent<Score>().win)
                //    {
                //        //P1 win P2 lose
                //        Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                //    }

                //    if (Character2.GetComponent<Score>().counter > 6)
                //    {
                //        DontDestroyOnLoad(RemainInfo);
                //        SceneManager.LoadScene("TwoPlayerResult");
                //    }

                //}
                //else if (Character.name == "Character2")
                //{
                //    GameObject Character1 = GameObject.Find("Character1/Canvas/Score");

                //    if (!win && !Character1.GetComponent<Score>().win)
                //    {
                //        // both lose
                //        if (time > Character1.GetComponent<Score>().time)
                //        {
                //            // P2 win
                //            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                //        }
                //        else
                //        {
                //            // P1 win
                //            Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                //        }
                //    }
                //    else if (!win && Character1.GetComponent<Score>().win)
                //    {
                //        // P2 lose P1 win
                //        Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P1_name").GetComponent<Text>().text;
                //    }
                //    else if (win && !Character1.GetComponent<Score>().win)
                //    {
                //        //P2 win P1 lose
                //        Winner.GetComponent<Text>().text = RemainInfo.transform.Find("P2_name").GetComponent<Text>().text;
                //    }


                //    if (Character1.GetComponent<Score>().counter > 6) {
                //        DontDestroyOnLoad(RemainInfo);
                //        SceneManager.LoadScene("TwoPlayerResult");
                //    }

                //}




                //DontDestroyOnLoad(RemainInfo);
                //SceneManager.LoadScene("TwoPlayerResult");
            }





        }


    }

    private Vector3 GetGameOverPossition ()
    {
        Vector3 TextPosition = Vector3.zero;

        if (SceneManager.GetActiveScene().name == "OnePlayer")
        {
            TextPosition = Vector3.zero;
        }
        else if (SceneManager.GetActiveScene().name == "TwoPlayer")
        {
            if (GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "LR")
            {
                if (Character.name == "Character1")
                {
                    TextPosition = new Vector3(-mainCamera.pixelWidth / 2f, 0f, 0f);
                }
                else if (Character.name == "Character2")
                {
                    TextPosition = new Vector3(mainCamera.pixelWidth / 2f, 0f, 0f);
                }
            }
            else if (GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "UD")
            {
                if (Character.name == "Character1")
                {
                    TextPosition = new Vector3(0f, mainCamera.pixelHeight / 2f, 0f);
                }
                else if (Character.name == "Character2")
                {
                    TextPosition = new Vector3(0f, -mainCamera.pixelHeight / 2f, 0f);
                }
            }
        }

        return TextPosition;
    }


    private Vector3 GetScoreTextPosition ()
    {
        Vector3 TextPosition = Vector3.zero;

        if (SceneManager.GetActiveScene().name == "OnePlayer")
        {
            TextPosition = new Vector3(-0.92f * mainCamera.pixelWidth / 2f, 0.97f * mainCamera.pixelHeight / 2f, 0f);
        }
        else if (SceneManager.GetActiveScene().name == "TwoPlayer")
        {
            if (GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "LR")
            {
                if (Character.name == "Character1")
                {
                    TextPosition = new Vector3(-1.92f * mainCamera.pixelWidth / 2f, 0.97f * mainCamera.pixelHeight / 2f, 0f);
                }
                else if (Character.name == "Character2")
                {
                    TextPosition = new Vector3(0.08f * mainCamera.pixelWidth / 2f, 0.97f * mainCamera.pixelHeight / 2f, 0f);
                }
            }
            else if (GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "UD")
            {
                if (Character.name == "Character1")
                {
                    TextPosition = new Vector3(-0.92f * mainCamera.pixelWidth / 2f, 1.9f * mainCamera.pixelHeight / 2f, 0f);
                }
                else if (Character.name == "Character2")
                {
                    TextPosition = new Vector3(-0.92f * mainCamera.pixelWidth / 2f, -0.1f * mainCamera.pixelHeight / 2f, 0f);
                }
            }
        }

        return TextPosition;
    }



    private void WriteScoreToTxt (string difficulty)
    {
        string name = GameObject.Find("Remain_info/P1_name").GetComponent<Text>().text;
        string new_scoreboard = "";
        string saveName = "";

        if (difficulty == "easy") {
            saveName = "rank_easy.txt";
        }
        else if (difficulty == "normal") {
            saveName = "rank_normal.txt";
        }
        else if (difficulty == "hard") {
            saveName = "rank_hard.txt";
        }

        if (System.IO.File.Exists(saveName))
        {
            // if "rank.txt" exist
            string[] lines = System.IO.File.ReadAllLines(saveName); // array of line
            int i = 0;
            int count = 0;
            bool find_higher = false;

            while (true)
            {
                if (count == 5)
                {
                    break;
                }

                string[] line_list = lines[i].Split(' ');

                if (find_higher)
                {
                    // the new score has already been placed on the ranking board, just print out the rest
                    new_scoreboard += "n " + (System.Int64.Parse(line_list[1]) + 1).ToString() + " " + line_list[2] + " " + line_list[3];
                    i++;
                }
                else if ((System.Double.Parse(line_list[3]) == 0) || time < System.Double.Parse(line_list[3]))
                {
                    // found the place of new score on board
                    new_scoreboard += "t " + line_list[1] + " " + name + " " + time;
                    find_higher = true;
                }
                else
                {
                    // if the place of new score on board has not been found, print out the original text
                    //new_scoreboard += "n " + lines[i];
                    new_scoreboard += "n " + line_list[1] + " " + line_list[2] + " " + line_list[3];
                    i++;
                }

                if (i < 5)
                {
                    new_scoreboard += "\n";
                }

                count++;
            }
        }
        else
        {
            // create a new "rank.txt" if not exist
            new_scoreboard += "t " + 1 + " " + name + " " + time + "\n";
            for (int i =1;i < 5; i++)
            {
                new_scoreboard += "n " + (i + 1).ToString() + " " + "-" + " " + 0.ToString();
                if (i < 5)
                {
                    new_scoreboard += "\n";
                }
            }
        }


        // write new ranking score to rank.txt
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveName, false))
        {
            file.WriteLine(new_scoreboard);
        }
    }



    private void CountDown ()
    {
        countDown_time -= Time.deltaTime;


        if (countDown_time <= 3 && countDown_time > 2)
        {
            if (Count_down.GetComponent<Text>().text != "3")
            {
                Count_down.GetComponent<Text>().color = original_color;
            }
            Count_down.GetComponent<Text>().text = "3";
        }
        else if (countDown_time <= 2 && countDown_time > 1)
        {
            if (Count_down.GetComponent<Text>().text != "2")
            {
                Count_down.GetComponent<Text>().color = original_color;
            }
            Count_down.GetComponent<Text>().text = "2";
        }
        else if (countDown_time <= 1 && countDown_time > 0)
        {
            if (Count_down.GetComponent<Text>().text != "1")
            {
                Count_down.GetComponent<Text>().color = original_color;
            }
            Count_down.GetComponent<Text>().text = "1";
        }
        else
        {
            if (Count_down.GetComponent<Text>().text != "Go!")
            {
                Count_down.GetComponent<Text>().color = original_color;
            }
            start_timer = true;
            Count_down.GetComponent<Text>().text = "Go!";
        }
    }

    private void ColorFadded ()
    {
        Color c = Character.transform.Find("Canvas/Count_down").GetComponent<Text>().color;
        c.a -= 0.7f * Time.deltaTime;
        Character.transform.Find("Canvas/Count_down").GetComponent<Text>().color = c;
    }
}
