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

    public int spawnChance;

    private void Awake()
    {
        CreateCells();
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            running = true;
        }

        if (Time.time - lastUpdated > updateTime && running == true)
        {
            UpdateCells();
            lastUpdated = Time.time;
        }
    }

    private void Start()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                if ((int)Random.Range(0,100) < spawnChance)
                {
                    cells[i][j].MarkAlive();
                    cells[i][j].UpdateCell();
                }
                
            }
        }
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
}
