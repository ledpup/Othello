using Reversi.Model;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Tile Tile;
    
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

    void OnMouseOver()
    {
        Messenger<short>.Broadcast("Tile hover", Tile.Index);
    }

    void OnLastPlay(short index)
    {
        if (Tile.Index == index)
        {
            renderer.material.color = Color.green;
        }
        else
        {
            renderer.material.color = Color.white;
        }
    }
}