using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    Button[,] buttons; 
    Image[] images; 
    
    lines lines;
    Score Score;

    [SerializeField] private Text countTxt;
    public static float Oldscore = 0;
    public static float score = 0;

    // Start is called before the first frame update
    void Start()
    {
        lines = new lines(ShowBox, PlayCut);
        InitButtons();
        InitImages();
        //ShowBox(0, 0, 5);
        lines.Start();
    }



    public void ShowBox(int x, int y, int ball)
    {
        buttons[x, y].GetComponent<Image>().sprite = images[ball].sprite;
    }
    public void PlayCut()
    {

    }

    public void Click()
    {
        if (lines.IsGameOver())
        {
            if (Oldscore < score)
            {
                Oldscore = score;
            }
            score = 0f;
        }
        string name = EventSystem.current.currentSelectedGameObject.name;
        int nr = GetNumber(name);
        int x = nr % lines.SIZE;
        int y = nr / lines.SIZE;
        Debug.Log($"clicked {name} {x} {y}");
        lines.Click( x, y );
        score = lines.GetScore();
        countTxt.text = "–екорд:" + Oldscore + "\n" + "—чет:" + score;
    }

    private int GetNumber(string name)
    {
        Regex regex = new Regex("\\((\\d+)\\)");
        Match match = regex.Match(name);
        if (!match.Success) 
        {
            throw new Exception("Unrecognized object name");
        }
        Group group = match.Groups[1];
        string number = group.Value;
        return Convert.ToInt32(number);
    }

    private void InitButtons()
    {
        buttons = new Button[lines.SIZE, lines.SIZE];
        for(int nr = 0; nr < lines.SIZE* lines.SIZE; nr++)
        {
            buttons[nr%lines.SIZE, nr/lines.SIZE] = GameObject.Find($"Button ({nr})").GetComponent<Button>();
        }
    }

    private void InitImages()
    {
        images = new Image[lines.BALLS];
        for (int i = 0; i < lines.BALLS; i++)
        {
            images[i] = GameObject.Find($"Image ({i})").GetComponent<Image>();
        }
    }
}
