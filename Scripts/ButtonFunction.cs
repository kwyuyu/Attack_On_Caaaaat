using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour {

    private GameObject ButtonOnePlayer;
    private GameObject ButtonTwoPlayer;
    private GameObject ButtonStart;
    private GameObject ButtonIntruction;
    private GameObject InputPlayer1;
    private GameObject InputPlayer2;
    private GameObject RemainInfo;
    private GameObject ScreenSetting;
    private GameObject DifficultySetting;
    private Animator InstructionPanelAnimator;
    private int instruction_trigger;




    private void Start ()
    {
        //RemainInfo = GameObject.Find("Remain_info");
        RemainInfo = Instantiate(GameObject.Find("Template/Remain_info_template"));
        RemainInfo.name = "Remain_info";

        ScreenSetting = GameObject.Find("Canvas/Screen_setting");
        ButtonOnePlayer = GameObject.Find("Canvas/One_player");
        ButtonTwoPlayer = GameObject.Find("Canvas/Two_player");
        ButtonStart = GameObject.Find("Canvas/Start");
        ButtonIntruction = GameObject.Find("Canvas/Instruction");
        InputPlayer1 = GameObject.Find("Canvas/P1_name");
        InputPlayer2 = GameObject.Find("Canvas/P2_name");
        InstructionPanelAnimator = GameObject.Find("Canvas/Instruction").GetComponent<Animator>();
        DifficultySetting = GameObject.Find("Canvas/Difficulty");


        InputPlayer1.SetActive(false);
        InputPlayer2.SetActive(false);
        ScreenSetting.SetActive(false);
        ButtonStart.SetActive(false);
        DifficultySetting.SetActive(false);

        //UnableInputField(InputPlayer1);
        //UnableInputField(InputPlayer2);
        //UnableDropdown(ScreenSetting);
        //UnableButton(ButtonStart);
        //UnableDropdown(DifficultySetting);


        instruction_trigger = 0;

        RemainInfo.transform.Find("Difficulty_value").GetComponent<Text>().text = "easy";
    }


    public void OnClick_onePlayer()
    {
        InputPlayer1.SetActive(true);
        InputPlayer2.SetActive(false);
        ScreenSetting.SetActive(false);
        ButtonStart.SetActive(true);
        DifficultySetting.SetActive(true);

        //EnableInputField(InputPlayer1);
        //UnableInputField(InputPlayer2);
        //UnableDropdown(ScreenSetting);
        //EnableButton(ButtonStart);
        //EnableDropdown(DifficultySetting);



        RemainInfo.transform.Find("Split_method").GetComponent<Text>().text = "";
    }

    public void OnClick_twoPlayer()
    {
        InputPlayer1.SetActive(true);
        InputPlayer2.SetActive(true);
        ScreenSetting.SetActive(true);
        ButtonStart.SetActive(true);
        DifficultySetting.SetActive(true);

        //EnableInputField(InputPlayer1);
        //EnableInputField(InputPlayer2);
        //EnableDropdown(ScreenSetting);
        //EnableButton(ButtonStart);
        //EnableDropdown(DifficultySetting);


        RemainInfo.transform.Find("Split_method").GetComponent<Text>().text = "LR";
    }

    public void OnSelect ()
    {
        Dropdown dropdown = GameObject.Find("Screen_setting").GetComponent<Dropdown>();

        if (dropdown.value == 0)
        {
            RemainInfo.transform.Find("Split_method").GetComponent<Text>().text = "LR";
        }
        else if (dropdown.value == 1)
        {
            RemainInfo.transform.Find("Split_method").GetComponent<Text>().text = "UD";
        }
    }

    public void OnSelectDifficulty ()
    {
        Dropdown dropdown = GameObject.Find("Difficulty").GetComponent<Dropdown>();

        if (dropdown.value == 0)
        {
            RemainInfo.transform.Find("Difficulty_value").GetComponent<Text>().text = "easy";
        }
        else if (dropdown.value == 1)
        {
            RemainInfo.transform.Find("Difficulty_value").GetComponent<Text>().text = "normal";
        }
        else if (dropdown.value == 2)
        {
            RemainInfo.transform.Find("Difficulty_value").GetComponent<Text>().text = "hard";
        }
    }

    public void OnStart()
    {
        GameObject NameTemplate = GameObject.Find("Template/Name_template");
        GameObject WarningTemplate = GameObject.Find("Template/Warning_template");


        // one player
        if (InputPlayer1.activeSelf && !InputPlayer2.activeSelf)
        //if (InputPlayer1.GetComponent<InputField>().enabled && !InputPlayer2.GetComponent<InputField>().enabled)
        {
            string name_input1 = InputPlayer1.transform.Find("Text").GetComponent<Text>().text;
            if (StringExtensions.IsNullOrWhiteSpace(name_input1))
            {
                if (GameObject.Find("Canvas/Warning1") == null)
                {
                    GameObject Warning1 = Instantiate(WarningTemplate, GameObject.Find("Canvas").transform);
                    Warning1.GetComponent<Text>().enabled = true;
                    Warning1.transform.position = InputPlayer1.transform.position + Vector3.down * 35;
                    Warning1.name = "Warning1";
                }

                if (!StringExtensions.IsNullOrWhiteSpace(name_input1))
                {
                    Destroy(GameObject.Find("Canvas/Warning1"));
                }
            }
            else
            {
                GameObject P1_name = Instantiate(NameTemplate, RemainInfo.transform);
                P1_name.GetComponent<Text>().enabled = true;
                P1_name.GetComponent<Text>().text = InputPlayer1.transform.Find("Text").GetComponent<Text>().text;
                P1_name.name = "P1_name";


                ////InputPlayer1.SetActive(true);
                ////InputPlayer2.SetActive(true);
                ////ScreenSetting.SetActive(true);
                ////ButtonStart.SetActive(true);
                ////DifficultySetting.SetActive(true);

                //EnableInputField(InputPlayer1);
                //EnableInputField(InputPlayer2);
                //EnableDropdown(ScreenSetting);
                //EnableButton(ButtonStart);
                //EnableDropdown(DifficultySetting);



                //instruction_trigger = 0;

                //DontDestroyOnLoad(RemainInfo);
                //SceneManager.LoadScene("OnePlayer");




                //InputPlayer1.SetActive(true);
                //InputPlayer2.SetActive(true);
                //ScreenSetting.SetActive(true);
                //ButtonStart.SetActive(true);

                //instruction_trigger = 0;


                // reset object activeness
                InputPlayer1.SetActive(false);
                InputPlayer2.SetActive(false);
                ScreenSetting.SetActive(false);
                ButtonStart.SetActive(false);
                DifficultySetting.SetActive(false);

                instruction_trigger = 0;


                DontDestroyOnLoad(RemainInfo);
                SceneManager.LoadScene("OnePlayer");
            }

        }

        // two player
        if (InputPlayer1.activeSelf && InputPlayer2.activeSelf)
        //if (InputPlayer1.GetComponent<InputField>().enabled && InputPlayer2.GetComponent<InputField>().enabled)
        {

            string name_input1 = InputPlayer1.transform.Find("Text").GetComponent<Text>().text;
            string name_input2 = InputPlayer2.transform.Find("Text").GetComponent<Text>().text;

            if (StringExtensions.IsNullOrWhiteSpace(name_input1) || StringExtensions.IsNullOrWhiteSpace(name_input2))
            {
                if (GameObject.Find("Canvas/Warning1") == null)
                {
                    GameObject Warning1 = Instantiate(WarningTemplate, GameObject.Find("Canvas").transform);
                    Warning1.GetComponent<Text>().enabled = true;
                    Warning1.transform.position = InputPlayer1.transform.position + Vector3.down * 35;
                    Warning1.name = "Warning1";
                }

                if (GameObject.Find("Canvas/Warning2") == null)
                {
                    GameObject Warning2 = Instantiate(WarningTemplate, GameObject.Find("Canvas").transform);
                    Warning2.GetComponent<Text>().enabled = true;
                    Warning2.transform.position = InputPlayer2.transform.position + Vector3.down * 35;
                    Warning2.name = "Warning2";
                }

                if (!StringExtensions.IsNullOrWhiteSpace(name_input1))
                {
                    Destroy(GameObject.Find("Canvas/Warning1"));
                }
                if (!StringExtensions.IsNullOrWhiteSpace(name_input2))
                {
                    Destroy(GameObject.Find("Canvas/Warning2"));
                }

            }
            else
            {

                GameObject P1_name = Instantiate(NameTemplate, RemainInfo.transform);
                GameObject P2_name = Instantiate(NameTemplate, RemainInfo.transform);

                P1_name.GetComponent<Text>().enabled = true;
                P1_name.GetComponent<Text>().text = InputPlayer1.transform.Find("Text").GetComponent<Text>().text;
                P1_name.name = "P1_name";

                P2_name.GetComponent<Text>().enabled = true;
                P2_name.GetComponent<Text>().text = InputPlayer2.transform.Find("Text").GetComponent<Text>().text;
                P2_name.name = "P2_name";


                //// reset object activeness
                ////InputPlayer1.SetActive(true);
                ////InputPlayer2.SetActive(true);
                ////ScreenSetting.SetActive(true);
                ////ButtonStart.SetActive(true);
                ////DifficultySetting.SetActive(true);


                //EnableInputField(InputPlayer1);
                //EnableInputField(InputPlayer2);
                //EnableDropdown(ScreenSetting);
                //EnableButton(ButtonStart);
                //EnableDropdown(DifficultySetting);


                //instruction_trigger = 0;


                //DontDestroyOnLoad(RemainInfo);
                //SceneManager.LoadScene("TwoPlayer");





                // reset object activeness
                InputPlayer1.SetActive(false);
                InputPlayer2.SetActive(false);
                ScreenSetting.SetActive(false);
                ButtonStart.SetActive(false);
                DifficultySetting.SetActive(false);

                instruction_trigger = 0;


                DontDestroyOnLoad(RemainInfo);
                SceneManager.LoadScene("TwoPlayer");

                //InputPlayer1.SetActive(true);
                //InputPlayer2.SetActive(true);
                //ScreenSetting.SetActive(true);
                //ButtonStart.SetActive(true);

                //instruction_trigger = 0;

            }
        }

    }


    public void OnClick_BackToMenu ()
    {
        Destroy(GameObject.Find("Remain_info"));
        SceneManager.LoadScene("Menu");
    }

    public void OnClick_Instruction ()
    {
        if (instruction_trigger == 0)
        {
            InstructionPanelAnimator.SetBool("IsOpen", true);
            instruction_trigger = 1;
        }
        else
        {
            InstructionPanelAnimator.SetBool("IsOpen", false);
            instruction_trigger = 0;
        }
    }



    private void EnableButton (GameObject But) 
    {
        But.GetComponent<Image>().enabled = true;
        But.GetComponent<Button>().enabled = true;
        But.transform.Find("Text").gameObject.GetComponent<Text>().enabled = true;
    }

    private void UnableButton(GameObject But)
    {
        But.GetComponent<Image>().enabled = false;
        But.GetComponent<Button>().enabled = false;
        But.transform.Find("Text").gameObject.GetComponent<Text>().enabled = false;
    }

    private void EnableInputField(GameObject InF)
    {
        InF.GetComponent<Image>().enabled = true;
        InF.GetComponent<InputField>().enabled = true;
        InF.transform.Find("Text").gameObject.GetComponent<Text>().enabled = true;
        InF.transform.Find("Placeholder").gameObject.GetComponent<Text>().enabled = true;
    }

    private void UnableInputField(GameObject InF)
    {
        InF.GetComponent<Image>().enabled = false;
        InF.GetComponent<InputField>().enabled = false;
        InF.transform.Find("Text").gameObject.GetComponent<Text>().enabled = false;
        InF.transform.Find("Placeholder").gameObject.GetComponent<Text>().enabled = false;
    }

	private void EnableDropdown (GameObject Drop)
	{
        Drop.GetComponent<Image>().enabled = true;
        Drop.GetComponent<Dropdown>().enabled = true;
        Drop.transform.Find("Label").gameObject.GetComponent<Text>().enabled = true;
        Drop.transform.Find("Arrow").gameObject.GetComponent<Image>().enabled = true;
	}

    private void UnableDropdown(GameObject Drop)
    {
        Drop.GetComponent<Image>().enabled = false;
        Drop.GetComponent<Dropdown>().enabled = false;
        Drop.transform.Find("Label").gameObject.GetComponent<Text>().enabled = false;
        Drop.transform.Find("Arrow").gameObject.GetComponent<Image>().enabled = false;
    }




	// check if string contain only space or string is empty. 
	private static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }


}
