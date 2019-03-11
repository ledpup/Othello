using System;
using Othello.Model;
using UnityEngine;


public class PieceBehaviour : MonoBehaviour
{
	public short Index;
	float _gameSpeed;
	bool _flip;
	int _flippingDirection;
	bool _drop;
	int _dropDirection;
	double _flipWait;
    public const float TileHeight = -0.2f;
    const float flipHeight = -1f;
	void Start()
    {
		Messenger<float>.AddListener("Game speed changed", OnGameSpeedChanged);
    }
	
	public void OnGameSpeedChanged(float gameSpeed)
	{
		_gameSpeed = gameSpeed;
	}
	
    void Update()
    {
		if (_drop)
		{
			var translation = Time.deltaTime * GameController.PieceStartingHeight * _gameSpeed * 4f;
			transform.Translate(0, 0, translation * (float)_dropDirection);
			if (transform.position.z + translation > TileHeight)
			{
				_drop = false;
				transform.position = new Vector3(transform.position.x, transform.position.y, TileHeight);
			}
		}
		else if (_flip)
		{
			if (_flipWait > 0)
			{
				_flipWait -= Time.deltaTime * _gameSpeed;
				return;
			}
			
			var rotation = Time.deltaTime * 500 * _gameSpeed * _flippingDirection;
            transform.position = new Vector3(transform.position.x, transform.position.y, flipHeight);

            if (transform.rotation.eulerAngles.y + rotation > 180)
			{
				rotation = 360;
				_flip = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, TileHeight);
            }
			else if (transform.rotation.eulerAngles.y + rotation < 0)
			{
				rotation = 0;
				_flip = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, TileHeight);
            }
			
			transform.Rotate(0, rotation , 0);
		}
    }
	
	public void Flip(int playerIndex, short placement)
	{
		_flip = true;
		_flippingDirection = playerIndex == 0 ? -1 : 1;

	    var dropWait = GameController.PieceStartingHeight * .01f;
	    var distanceWait = EuclideanDistance(Index.ToCartesianCoordinate(), placement.ToCartesianCoordinate());

        _flipWait = dropWait * distanceWait;
	}
	
	public static double EuclideanDistance(Point p1, Point p2)
	{
		return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
	}
	
	public void Drop(int colour)
	{
		_drop = true;
		_dropDirection = colour == 0 ? 1 : -1;
	}
}