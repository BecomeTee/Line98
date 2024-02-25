using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public delegate void ShowBox(int x, int y, int ball);
public delegate void PlayCut();

public class lines
{
    public const int SIZE = 9;
    public const int BALLS = 7;
    const int ADD_BALLS = 3;

    System.Random random = new System.Random();

    ShowBox showBox;
    PlayCut playCut;

    int[,] map;
    int fromX, fromY;
    bool isBallSelected;
    private bool[,] mark;
    private bool[,] used;

    /*[SerializeField] private Text countTxt;
    public static float Oldscore = 0;*/
    public static float score = 0;
    //public Score Score = new Score();

    public lines(ShowBox showBox, PlayCut playCut) 
    { 
        this.showBox = showBox;
        this.playCut = playCut;
        map = new int[SIZE, SIZE];
    }


    // Start is called before the first frame update
    public void Start()
    {
        ClearMap();
        AddRandomBalls();
        isBallSelected = false;
        mark = new bool[SIZE, SIZE];
        used = new bool[SIZE, SIZE];
    }

    public void Click(int x, int y)
    {
        if (IsGameOver())
        {
            Start();
        }
        else
        {
            if (map[x, y] > 0)
            {
                TakeBall(x, y);
            }
            else
            {
                MoveBall(x, y);
            }
        }
    }

    private void TakeBall(int x,int y)
    {
        fromX = x; 
        fromY = y;
        isBallSelected = true;
    }
    private void MoveBall(int x, int y)
    {
        if (!isBallSelected) return;
        if (!CanMove(x, y)) return;
        ClearUsed();
        SetMap(x, y, map[fromX, fromY]);
        SetMap(fromX, fromY, 0);
        isBallSelected=false;
        if (!CutLines())
        {
            AddRandomBalls();
            CutLines();
        }
        
    }

    public bool IsGameOver()
    {
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
                if (map[x, y] == 0)
                    return false;
        score = 0f;
        return true;
    }

    private void ClearUsed()
    {
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
                used[x,y] = false;
    }
    private void ClearMap()
    {
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
                SetMap(x, y, 0);
    }
    private bool OnMap(int x, int y)
    {
        return (x>=0 && x<SIZE && y >= 0 && y < SIZE);
    }
    private int GetMap(int x, int y)
    {
        if (!OnMap(x, y)) return 0;
        return map[x, y];
    }

    private void SetMap(int x , int y, int ball)
    {
        map[x, y] = ball;
        showBox(x, y, ball);
    }

    private void AddRandomBalls()
    {
        for (int i = 0; i < ADD_BALLS; i++)
            AddRandomBall();
    }

    private void AddRandomBall()
    {
        int x, y;
        int loop = SIZE * SIZE;
        do
        {
            x = random.Next(SIZE);
            y = random.Next(SIZE);
            if (--loop <= 0) return;
        } while (map[x, y] > 0);
        int ball = 1 + random.Next(BALLS - 1);
        SetMap(x, y, ball);
    }

    private bool CutLines()
    {
        int balls = 0;
        for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
            {
                balls += CutLine(x, y, 1, 0);
                balls += CutLine(x, y, 0, 1);
                balls += CutLine(x, y, 1, 1);
                balls += CutLine(x, y,-1, 1);
            }
        if(balls> 0)
        {
            score += balls * 100;
            //Debug.Log(score);
            //Score.addScore(score);*/
            for (int x = 0; x < SIZE; x++)
                for (int y = 0; y < SIZE; y++)
                {
                    if (mark[x, y])
                    {
                        SetMap(x, y, 0);
                        mark[x, y] = false;
                    }
                }   
            return true;
        }
        else
        {
            return false;
        }
    }

    private int CutLine(int x0, int y0, int sx, int sy)
    {
        int ball = map[x0, y0];
        if (ball == 0) return 0;
        int count = 0;
        for(int x = x0, y = y0; GetMap(x,y) == ball; x+=sx, y += sy)
        {
            count++;
        }
        if (count < 5)
        {
            return 0;
        }
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
        {
            mark[x,y] = true;
        }
        return count;
    }
    private bool CanMove(int toX, int toY)
    {
        Walk(fromX, fromY, true);
        return used[toX, toY];
    }

    private void Walk(int x, int y, bool start = false)
    {
        if (!start)
        {
            if (!OnMap(x, y)) return;
            if (map[x, y] > 0) return;
            if (used[x,y]) return;
        }
        used[x, y] = true;  
        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
        
    }

    public float GetScore()
    {
        return score;
    }
    
}
