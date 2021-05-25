using System;
using UnityEngine;

public class GameField
{
    public GameObject FieldObject { get; }

    private Cell[,] _grid;

    public int Height { get; }
    public int Width { get; }

    private Camera _sceneCamera;
    
    private SpriteLoader _loader;

    private int[,] _grid2D;
    
    
    public GameField(SpriteLoader loader, int[,] grid, Camera sceneCamera)
    {
        _loader = loader;
        _grid2D = grid;
        Height = grid.GetLength(0);
        Width = grid.GetLength(1);

        FieldObject = new GameObject("Game Field");

        _grid = new Cell[Height, Width];

        _sceneCamera = sceneCamera;
    }

    public void Initialize()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Cell.CType type = (Cell.CType) _grid2D[i, j];
                Vector2Int pos = new Vector2Int(j,  i);
                CellCreator creator = new CellCreator(this, _loader, pos, Quaternion.identity);
                creator.CreateCell(type);
            }
        }
        
        _sceneCamera.transform.position = new Vector3((Width - 1) / 2.0f, (Height - 1) / 2.0f, -1);
    }

    public static int[,] ConvertTo2DGrid(int[] array, int height, int width)
    {
        int[,] output = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output[i, j] = array[i * width + j];
            }
        }
        return output;
    }

    public static int[] ConvertTo1DGrid(int[,] grid)
    {
        int height = grid.GetLength(0);
        int width = grid.GetLength(1);
        int t = 0;
        int[] output = new int[width * height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output[t++] = grid[i, j];
            }
        }

        return output;
    }

    public int GetEmptyCount()
    {
        int res = 0;
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (!_grid[i, j].IsOccupied)
                {
                    res++;   
                }
            }
        }

        return res;
    }
    
    public Cell this [int i, int j]
    {
        get
        {
            try
            {
                Cell t = _grid[i, j];
                return t;
            }
            catch(Exception e)
            {
                Debug. LogError(e);
                return null;
            }
        }
        set
        {
            try
            {
                _grid[i, j] = value;
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.LogError(e);
            }
        }
    }
}