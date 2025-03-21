using System;
using System.Collections.Generic;
using UnityEngine;

public class Shape
{
    public List<Pixel.Coords> shapeCoords = new List<Pixel.Coords>();
}

public class GameManager : MonoBehaviour
{
    /*
     * (0,0) (1,0) (2,0) (3,0)
     *
     * (0,0)
     * (0,1)
     * (0,2) (1,2)
     *
     * (0,0) (1,0)
     * (0,1) (1,1)
     *
     * (0,0)
     * (0,1) (1,1)
     *       (1,2)
     *
     * (0,0) (1,0) (2,0)
     *       (1,1)
     */


    private List<Shape> _shapes;
    
    private void GenerateShapes()
    {
        _shapes = new List<Shape>();
        
        Shape shape = new Shape();
        shape.shapeCoords = new List<Pixel.Coords>();
            
        for (int i = 0; i < 4; i++)
        {
            shape.shapeCoords.Add(new Pixel.Coords(i, 0));
        }

        Shape shape2 = new Shape();
        shape2.shapeCoords = new List<Pixel.Coords>();
        
        shape.shapeCoords.Add(new Pixel.Coords(0, 0));
        shape.shapeCoords.Add(new Pixel.Coords(0, 1));
        shape.shapeCoords.Add(new Pixel.Coords(0, 2));
        shape.shapeCoords.Add(new Pixel.Coords(1, 2));

        Shape shape3 = new Shape();
        shape3.shapeCoords = new List<Pixel.Coords>();
        
        shape3.shapeCoords.Add(new Pixel.Coords(0, 0));
        shape3.shapeCoords.Add(new Pixel.Coords(1, 0));
        shape3.shapeCoords.Add(new Pixel.Coords(0, 1));
        shape3.shapeCoords.Add(new Pixel.Coords(1, 1));

        Shape shape4 = new Shape();
        shape4.shapeCoords = new List<Pixel.Coords>();
        
        shape4.shapeCoords.Add(new Pixel.Coords(0, 0));
        shape4.shapeCoords.Add(new Pixel.Coords(0, 1));
        shape4.shapeCoords.Add(new Pixel.Coords(1, 1));
        shape4.shapeCoords.Add(new Pixel.Coords(1, 2));

        Shape shape5 = new Shape();
        shape5.shapeCoords = new List<Pixel.Coords>();
        
        shape5.shapeCoords.Add(new Pixel.Coords(0, 0));
        shape5.shapeCoords.Add(new Pixel.Coords(1, 0));
        shape5.shapeCoords.Add(new Pixel.Coords(2, 0));
        shape5.shapeCoords.Add(new Pixel.Coords(1, 1));
        
        _shapes.Add(shape);
        _shapes.Add(shape2);
        _shapes.Add(shape3);
        _shapes.Add(shape4);
        _shapes.Add(shape5);
    }

    public GameObject cellPrefab; // Assign this in the Inspector
    private Pixel[,] grid;
    
    public Transform gridParent;
    void Start()
    {
        GeneratePixelBoard();
        GenerateShapes();
    }

    private bool _gameStarted;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_gameStarted)
            {
                Debug.Log($"Game Started");
                _gameStarted = true;
                SpawnShape();
            }
        }
        
        if (_gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                
            }
        }
    }

    private void MoveRight()
    {
        if (_currentStartingPointX + 1 >= WIDTH)
        {
            return;
        }
        
        
        //OccupyPixel()
    }

    private void MoveLeft()
    {
        
    }

    private Shape _currentlyControlledShape;
    private (int, int, int, int) _currentlyControlledShapeCoords;
    private List<Pixel> _currentControlledPixels;
    private int _currentStartingPointX;
    private int _currentStartingPointY;
    
    private void SpawnShape()
    {
        int count = _shapes.Count;

        int spawnRandomShapeId = UnityEngine.Random.Range(0, count);

        _currentlyControlledShape = new Shape();
        _currentlyControlledShape.shapeCoords = new List<Pixel.Coords>();
        _currentlyControlledShape.shapeCoords.AddRange(_shapes[spawnRandomShapeId].shapeCoords);
        
        _currentlyControlledShapeCoords = GetMinMaxOfShape(_shapes[spawnRandomShapeId]);

        var spawnXStartPoint = UnityEngine.Random.Range(0, WIDTH - 1 - _currentlyControlledShapeCoords.Item3);
        var spawnYStartPoint = 0;

        _currentStartingPointX = spawnXStartPoint;
        _currentStartingPointY = spawnYStartPoint;
        
        count = _currentlyControlledShape.shapeCoords.Count;

        bool gameOver = false;
        for (int i = 0; i < count; i++)
        {
            if (grid[spawnXStartPoint + _currentlyControlledShape.shapeCoords[i].x,
                    spawnYStartPoint + _currentlyControlledShape.shapeCoords[i].y].isOccupied)
            {
                gameOver = true;
            }
        }

        if (!gameOver)
        {
            OccupyPixel(spawnXStartPoint, spawnYStartPoint);
        }
    }

    private void OccupyPixel(int spawnXStartPoint, int spawnYStartPoint)
    {
        int count = _currentlyControlledShape.shapeCoords.Count;
        _currentControlledPixels = new List<Pixel>();
        for (int i = 0; i < count; i++)
        {
            grid[spawnXStartPoint + _currentlyControlledShape.shapeCoords[i].x,
                    spawnYStartPoint + _currentlyControlledShape.shapeCoords[i].y].TurnOn();
        }
    }
    
    private (int, int, int, int) GetMinMaxOfShape(Shape shape)
    {
        int count = shape.shapeCoords.Count;

        int minX = 100;
        int minY = 100;
        int maxX = -1;
        int maxY = -1;
        
        for (int i = 0; i < count; i++)
        {
            if (minX > shape.shapeCoords[i].x)
            {
                minX = shape.shapeCoords[i].x;
            }

            if (minY > shape.shapeCoords[i].y)
            {
                minY = shape.shapeCoords[i].y;
            }

            if (maxX < shape.shapeCoords[i].x)
            {
                maxX = shape.shapeCoords[i].x;
            }

            if (maxY < shape.shapeCoords[i].y)
            {
                maxY = shape.shapeCoords[i].y;
            }
        }

        return (minX, minY, maxX, maxY);
    }

    private const int WIDTH = 10;
    private const int HEIGHT = 20;

    private void GeneratePixelBoard()
    {
        grid = new Pixel[WIDTH,HEIGHT];    

        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                Pixel cell = Instantiate(cellPrefab, gridParent).GetComponent<Pixel>();
                
                grid[i, j] = cell;
                cell.SetProperties(i, j);
            }
        }
    }
}