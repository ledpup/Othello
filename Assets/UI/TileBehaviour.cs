using Reversi.Model;
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
		_oldColour = renderer.material.color;
		renderer.material.color = Color.cyan;
    }
	
	void OnMouseExit()
	{
		renderer.material.color = _oldColour;
	}
	
    void OnLastPlay(short index)
    {
        if (Tile.Index == index)
        {
            _oldColour = renderer.material.color = Color.green;
        }
        else
        {
            _oldColour = renderer.material.color = Color.white;
        }
    }
}