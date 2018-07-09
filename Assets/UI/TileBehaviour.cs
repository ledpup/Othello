using Othello.Model;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Tile Tile;
	Color _oldColour;
    
    void Start()
    {
        Messenger<short>.AddListener("Last play", OnLastPlay);
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
		GetComponent<Renderer>().material.color = Color.cyan;
    }
	
	void OnMouseExit()
	{
		GetComponent<Renderer>().material.color = _oldColour;
	}
	
    void OnLastPlay(short index)
    {
        if (Tile.Index == index)
        {
            _oldColour = GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            _oldColour = GetComponent<Renderer>().material.color = Color.white;
        }
    }
}