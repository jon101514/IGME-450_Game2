﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    ZUp = 1,
    XUp = 2,
    ZDown = 4,
    XDown = 8
}

public class RailInfo : MonoBehaviour {

    //All fields will be accessed by the Rail Manager

    //Set in editor only
    //Initial placement in the starting rail
    public int initialPosition;

    //Set in the editor originally
    //If the rail is a turn or is straight
    public bool turn;

    //Set in the editor originally
    //Direction the rail is facing
    public Direction direction;

    //Second direction only necessary for turns
    public Direction turnDirection;

    //Position for RailManager to access
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    //Lean of rail to check against the cart
    public int Lean
    {
        get
        {
            if((int) direction * 2 == ((int) turnDirection) || (int)direction / 8 == ((int)turnDirection))
            {
                return 1;
            }
            else if((int)direction / 2 == ((int)turnDirection) || (int) direction * 8 == ((int)turnDirection))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    //Position of the following track
    public Vector3 nextPosition
    {
        get; set;
    }

    //Moves the rail without GameObject
    /// <summary>
    /// Move added to the current position
    /// </summary>
    /// <param name="movement">Vector 3 to add</param>
    public void Move(Vector3 movement)
    {
        transform.position += movement;
    }

    //Moves the rail without GameObject
    /// <summary>
    /// Position changed directly
    /// </summary>
    /// <param name="position">New position</param>
    public void NewPosition(Vector3 position)
    {
        transform.position = position;

        if (direction == Direction.XUp)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if (direction == Direction.XDown)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
        else if (direction == Direction.ZDown)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        if(Lean == -1)
        {
            transform.Rotate(new Vector3(0, 90, 0));
        }

    }
}
