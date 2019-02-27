using Othello.Model;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Tile Tile;
	Color _oldColour;
    
    void Start()
    {
        Messenger<short>.AddListener("Notify tile", OnLastPlay);
    }

    void Update()
    {

    }

    void OnMouseUp()
    {
        Messenger<short>.Broadcast("Tile clicked", Tile.Index);
    }    
	
    void OnMouseEnter()
    {
        Messenger<short>.Broadcast("Tile hover", Tile.Index);
		_oldColour = GetComponent<Renderer>().material.color;
		GetComponent<Renderer>().material.color = new Color(0.5f, 0.25f, 0.75f); // mouse over colour
    }
	
	void OnMouseExit()
	{
		GetComponent<Renderer>().material.color = _oldColour;
	}
	
    void OnLastPlay(short index)
    {
        if (Tile.Index == index)
        {
            _oldColour = GetComponent<Renderer>().material.color = new Color(0.3f, 1, 0.3f); // last played
        }
        else
        {
            _oldColour = GetComponent<Renderer>().material.color = new Color(0, .625f, 0); // base board colour - green
        }
    }
}