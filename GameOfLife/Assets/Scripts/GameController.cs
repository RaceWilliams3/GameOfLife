using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int cellCount_X, cellCount_y;
    public Cell[][] cells;
    public Cell cellPrefab;
    public Transform parent;

    public float updateTime;
    private float lastUpdated = 0;

    private bool running = false;
    private int stepCount = 0;

    public int spawnChance;


    float avgPop = 0f;
    int finalCount = 0;
    int stepsToDie = 0;

    bool displayedStats = true;

    private void Awake()
    {
        CreateCells();
    }


    private void Start()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                if ((int)Random.Range(0, 100) < spawnChance)
                {
                    cells[i][j].MarkAlive();
                    cells[i][j].UpdateCell();
                }

            }
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            running = !running;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBoard();
        }

        if (stepCount >= 1000)
        {
            stepsToDie = 0;
            avgPop = avgPop / (float)stepCount;
            running = false;
            displayedStats = false;
            finalCount = amountAlive();
            stepCount = 0;
            ScreenCapture.CaptureScreenshot("SpawnChance-" + spawnChance.ToString() + ".png");
        }

        if (running == false && displayedStats == false)
        {
            Debug.Log("Spawn Chance: " + spawnChance.ToString() + "\nAverage Population: " + avgPop.ToString() + "\nFinal Count: " + finalCount.ToString() + "\n Time to Die: " + stepsToDie.ToString());
            displayedStats = true;
        }

        if (Time.time - lastUpdated > updateTime && running == true)
        {
            UpdateCells();
            lastUpdated = Time.time;
            stepCount++;
            avgPop += amountAlive();


            if (checkForDead() == true)
            {
                stepsToDie = stepCount;
                finalCount = 0;
                avgPop = avgPop / (float)stepCount;
                running = false;
                displayedStats = false;
                ScreenCapture.CaptureScreenshot("SpawnChance-" + spawnChance.ToString() + ".png");
            }
        }
    }

    private void ResetBoard()
    {
        stepCount = 0;
        avgPop = 0f;
        finalCount = 0;
        stepsToDie = 0;

        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                cells[i][j].MarkDead();
                cells[i][j].UpdateCell();

                if ((int)Random.Range(0, 100) < spawnChance)
                {
                    cells[i][j].MarkAlive();
                    cells[i][j].UpdateCell();
                }

            }
        }
    }

    private bool checkForDead()
    {
        bool allDead = true;
        if (amountAlive() > 0)
        {
            allDead = false;
        }
        return allDead;
    }

    private int amountAlive()
    {
        int living = 0;

        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                if (cells[i][j].isCellAlive)
                {
                    living++;
                }
            }
        }
        return living;
    }

    


    private void CreateCells()
    {
        cells = new Cell[cellCount_X][];

        for (int i = 0; i < cellCount_X; i++)
        {
            cells[i] = new Cell[cellCount_y];
            for (int j = 0; j < cellCount_y; j++)
            {
                Cell cell = Instantiate(cellPrefab, new Vector2(i, j), Quaternion.identity, parent);
                cells[i][j] = cell;
                cells[i][j].name = "cell{i}{j}";
            }
        }
    }

    private void UpdateCells()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                int liveNeighbors = 0;

                //Check bottom left
                if (i > 0 && j > 0 && cells[i - 1][j - 1].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check bottom
                if (j > 0 && cells[i][j - 1].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check bottom right
                if (i < cells.Length - 1 && j > 0 && cells[i + 1][j - 1].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check right
                if (i > 0 && cells[i - 1][j].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check left
                if (i < cells.Length - 1 && cells[i + 1][j].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check top left
                if (i > 0 && j < cells[i].Length - 1 && cells[i - 1][j + 1].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check top
                if (j < cells[i].Length - 1 && cells[i][j + 1].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Check top right
                if (i < cells.Length - 1 && j < cells[i].Length - 1 && cells[i + 1][j + 1].isCellAlive)
                {
                    liveNeighbors++;
                }

                //Rule 1: A live cell with 2 or 3 alive neighbors will survive
                if (cells[i][j].isCellAlive && (liveNeighbors == 2 || liveNeighbors == 3))
                {
                    continue;
                }

                //Rule 2: A dead cell with 3 neighboring cells will get alive
                if (!cells[i][j].isCellAlive && liveNeighbors == 3)
                {
                    cells[i][j].MarkAlive();
                    continue;
                }

                //Rile 3: all other cells die
                cells[i][j].MarkDead();
            }
        }

        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                cells[i][j].UpdateCell();
            }
        }
    }


    private float checkAverageLiving()
    {
        float livingCellNum = 0;
        float cellNum = 0;

        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                if (cells[i][j].isCellAlive)
                {
                    livingCellNum++;
                }
                cellNum++;

            }
        }

        return livingCellNum / cellNum;
    }

}
