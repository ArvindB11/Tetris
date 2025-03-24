using System;
using System.Collections.Generic;
using UnityEngine;

public class Shape
{
    public int shapeId;
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
        
        shape2.shapeCoords.Add(new Pixel.Coords(0, 0));
        shape2.shapeCoords.Add(new Pixel.Coords(0, 1));
        shape2.shapeCoords.Add(new Pixel.Coords(0, 2));
        shape2.shapeCoords.Add(new Pixel.Coords(1, 2));

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
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveDown();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Rotate();
            }
        }
    }
private void Rotate()
{
    List<Pixel.Coords> rotatedCoords = new List<Pixel.Coords>();

    foreach (var coord in _currentlyControlledShape.shapeCoords)
    {
        int newX = -coord.y;
        int newY = coord.x;
        rotatedCoords.Add(new Pixel.Coords(newX, newY));
    }

    int minX = int.MaxValue, minY = int.MaxValue;
    foreach (var coord in rotatedCoords)
    {
        if (coord.x < minX) minX = coord.x;
        if (coord.y < minY) minY = coord.y;
    }

    List<Pixel.Coords> adjustedCoords = new List<Pixel.Coords>();
    foreach (var coord in rotatedCoords)
    {
        adjustedCoords.Add(new Pixel.Coords(coord.x - minX, coord.y - minY));
    }

    Shape rotatedShape = new Shape
    {
        shapeId = _currentlyControlledShape.shapeId,
        shapeCoords = adjustedCoords
    };

    if (!DoesAnyShapeOrFloorOverlap(rotatedShape, _currentStartingPointX, _currentStartingPointY))
    {
        _currentlyControlledShape.shapeCoords = adjustedCoords;
        OccupyPixel(_currentStartingPointX, _currentStartingPointY, _currentlyControlledShape.shapeId);
    }
}

private void MoveRight()
{
    int newX = _currentStartingPointX + 1;
    if (!DoesAnyShapeOrFloorOverlap(_currentlyControlledShape, newX, _currentStartingPointY))
    {
        _currentStartingPointX = newX;
        OccupyPixel(_currentStartingPointX, _currentStartingPointY, _currentlyControlledShape.shapeId);
    }
}

private void MoveLeft()
{
    int newX = _currentStartingPointX - 1;
    if (!DoesAnyShapeOrFloorOverlap(_currentlyControlledShape, newX, _currentStartingPointY))
    {
        _currentStartingPointX = newX;
        OccupyPixel(_currentStartingPointX, _currentStartingPointY, _currentlyControlledShape.shapeId);
    }
}

private void MoveDown()
{
    int newY = _currentStartingPointY + 1;
    if (!DoesAnyShapeOrFloorOverlap(_currentlyControlledShape, _currentStartingPointX, newY))
    {
        _currentStartingPointY = newY;
        OccupyPixel(_currentStartingPointX, _currentStartingPointY, _currentlyControlledShape.shapeId);
    }
    else
    {
        _gameStarted = false;
        _currentlyControlledShape = null;
    }
}

private bool DoesAnyShapeOrFloorOverlap(Shape shape, int startX, int startY)
{
    foreach (var coord in shape.shapeCoords)
    {
        int newX = startX + coord.x;
        int newY = startY + coord.y;

        if (newY >= HEIGHT || 
            newX < 0 || newX >= WIDTH || 
            (grid[newX, newY].isOccupied && grid[newX, newY].shapeId != shape.shapeId))
        {
            return true;
        }
    }
    return false;
}
    // private void Rotate()
    // {
    //     List<Pixel.Coords> rotatedCoords = new List<Pixel.Coords>();
    //
    //     foreach (var coord in _currentlyControlledShape.shapeCoords)
    //     {
    //         int newX = -coord.y;
    //         int newY = coord.x;
    //         rotatedCoords.Add(new Pixel.Coords(newX, newY));
    //     }
    //
    //     int minX = int.MaxValue, minY = int.MaxValue;
    //     foreach (var coord in rotatedCoords)
    //     {
    //         if (coord.x < minX) minX = coord.x;
    //         if (coord.y < minY) minY = coord.y;
    //     }
    //
    //     List<Pixel.Coords> adjustedCoords = new List<Pixel.Coords>();
    //     foreach (var coord in rotatedCoords)
    //     {
    //         adjustedCoords.Add(new Pixel.Coords(coord.x - minX, coord.y - minY));
    //     }
    //
    //     foreach (var coord in adjustedCoords)
    //     {
    //         int newGridX = _currentStartingPointX + coord.x;
    //         int newGridY = _currentStartingPointY + coord.y;
    //
    //         if (newGridX < 0 || newGridX >= WIDTH || newGridY < 0 || newGridY >= HEIGHT ||
    //             (grid[newGridX, newGridY].isOccupied && grid[newGridX, newGridY].shapeId != _currentlyControlledShape.shapeId))
    //         {
    //             return;
    //         }
    //     }
    //
    //     _currentlyControlledShape.shapeCoords = adjustedCoords;
    //     OccupyPixel(_currentStartingPointX, _currentStartingPointY, _currentlyControlledShape.shapeId);
    // }
    //
    // private void MoveRight()
    // {
    //     if (_currentStartingPointX + _currentlyControlledShapeCoords.Item3 + 1 >= WIDTH)
    //     {
    //         return;
    //     }
    //
    //     OccupyPixel(_currentStartingPointX + 1, _currentStartingPointY, _currentlyControlledShape.shapeId);
    //     
    //     if (DoesAnyShapeOrFloorOverlap())
    //     {
    //         _gameStarted = false;
    //         _currentlyControlledShape = null;
    //     }
    // }
    //
    // private void MoveLeft()
    // {
    //     if (_currentStartingPointX - _currentlyControlledShapeCoords.Item1 - 1 < 0)
    //     {
    //         return;
    //     }
    //     
    //     OccupyPixel(_currentStartingPointX - 1, _currentStartingPointY, _currentlyControlledShape.shapeId);
    //     
    //     if (DoesAnyShapeOrFloorOverlap())
    //     {
    //         _gameStarted = false;
    //         _currentlyControlledShape = null;
    //     }
    // }
    //
    // private void MoveDown()
    // {
    //     OccupyPixel(_currentStartingPointX, _currentStartingPointY + 1, _currentlyControlledShape.shapeId);
    //     
    //     if (DoesAnyShapeOrFloorOverlap())
    //     {
    //         _gameStarted = false;
    //         _currentlyControlledShape = null;
    //     }
    // }

    private bool DoesAnyShapeOrFloorOverlap()
    {
        if (_currentStartingPointY + 1 + _currentlyControlledShapeCoords.Item4 >= HEIGHT)
        {
            return true;
        }
        
        int count = _currentlyControlledShape.shapeCoords.Count;
        
        for (int i = 0; i < count; i++)
        {
            if (grid[_currentStartingPointX + _currentlyControlledShape.shapeCoords[i].x,
                         _currentStartingPointY + 1 + _currentlyControlledShape.shapeCoords[i].y].isOccupied && 
                grid[_currentStartingPointX + _currentlyControlledShape.shapeCoords[i].x,
                         _currentStartingPointY + 1 + _currentlyControlledShape.shapeCoords[i].y].shapeId != _currentlyControlledShape.shapeId)
            {
                return true;
            }
        }
        
        return false;
    }

    private Shape _currentlyControlledShape;
    private (int, int, int, int) _currentlyControlledShapeCoords; // (minX, minY, maxX, maxY)
    private List<Pixel> _currentControlledPixels;
    private int _currentStartingPointX;
    private int _currentStartingPointY;

    private int _shapeIdCount;
    private void SpawnShape()
    {
        int count = _shapes.Count;

        int spawnRandomShapeId = UnityEngine.Random.Range(0, count);

        _currentlyControlledShape = new Shape();
        _currentlyControlledShape.shapeId = _shapeIdCount++;
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

        _currentControlledPixels = new List<Pixel>();
        if (!gameOver)
        {
            OccupyPixel(spawnXStartPoint, spawnYStartPoint, _currentlyControlledShape.shapeId);
        }
        else
        {
            Debug.Log($"Game Over");
        }
    }

    private void OccupyPixel(int spawnXStartPoint, int spawnYStartPoint, int shapeIdCount)
    {
        _currentStartingPointX = spawnXStartPoint;
        _currentStartingPointY = spawnYStartPoint;
        
        int controlledPixelsCount = _currentControlledPixels.Count;
        
        for (int i = 0; i < controlledPixelsCount; i++)
        {
            _currentControlledPixels[i].TurnOff();
        }

        _currentControlledPixels.Clear();
        
        int count = _currentlyControlledShape.shapeCoords.Count;
        
        for (int i = 0; i < count; i++)
        {
            _currentControlledPixels.Add(grid[spawnXStartPoint + _currentlyControlledShape.shapeCoords[i].x,
                    spawnYStartPoint + _currentlyControlledShape.shapeCoords[i].y].TurnOn(shapeIdCount));
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