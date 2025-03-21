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

    public void TurnOn()
    {
        pixelColor.color = Color.black;
        _isOccupied = true;
    }

    public void TurnOff()
    {
        pixelColor.color = Color.white;
        _isOccupied = false;
    }
}
