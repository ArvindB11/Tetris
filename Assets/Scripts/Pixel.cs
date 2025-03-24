using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pixel : MonoBehaviour
{
    public Image pixelColor;
    public TextMeshProUGUI pixelCoords;

    public struct Coords
    {
        public int x;
        public int y;

        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    private Coords _coords;
    public Coords coords => _coords;
    public void SetProperties(int i, int j)
    {
        gameObject.name = $"({i}, {j})";
        _coords = new Coords(i, j);
        pixelCoords.SetText($"{_coords.x},{_coords.y}");
    }

    private bool _isOccupied;
    public bool isOccupied => _isOccupied;

    private int _shapeId;
    public int shapeId => _shapeId;
    public Pixel TurnOn(int shapeIdCount)
    {
        _shapeId = shapeIdCount;
        
        pixelColor.color = Color.black;
        _isOccupied = true;

        return this;
    }

    public void TurnOff()
    {
        _shapeId = -1;
        pixelColor.color = Color.white;
        _isOccupied = false;
    }
}
