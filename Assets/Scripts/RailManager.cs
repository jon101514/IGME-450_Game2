using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour {

    // Fields

        
    // Initial Track
    RailInfo[] initialRails;

    // Queues
    Queue<RailInfo> currentRails;
    Queue<RailInfo> unused;
    Queue<RailInfo> unusedTurns;

    // Most recent rail to be added to access both sides of the track
    RailInfo newestRail;
    
    // Next position in which a rail is removed
    float nextPosZ = 7.8f;
    float nextPosX = 7.8f;
    GameObject player;

    // Is there a turn and all information about it
    float nextTurnX;
    float nextTurnZ;
    Queue<RailInfo> upcomingTurns;

    // Direction of the cart and direction of the end of the track
    [SerializeField]
    Direction currentDirection;
    [SerializeField]
    Direction futureDirection;





    // Methods

	// Initialization
	void Start ()
    {

        // Cart for information
        player = GameObject.FindGameObjectWithTag("Player");


        // Set up empty queues
        currentRails = new Queue<RailInfo>();
        unused = new Queue<RailInfo>();
        unusedTurns = new Queue<RailInfo>();
        upcomingTurns = new Queue<RailInfo>();


        // Change later to add more turns
        GameObject initialTurn = GameObject.FindGameObjectWithTag("Turn");
        unusedTurns.Enqueue(initialTurn.GetComponent<RailInfo>());


        // Takes in initial rails
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Rail");
        int leng = objects.Length;
        initialRails = new RailInfo[leng];
        for (int i = 0; i < leng; i++)
        {
            RailInfo temp = objects[i].GetComponent<RailInfo>();
            initialRails[temp.initialPosition] = temp;
        }

        // Then puts them in the starting queue
        for (int i = 0; i < leng; i++)
        {
            currentRails.Enqueue(initialRails[i]);

            if(i == leng - 1)
            {
                newestRail = initialRails[i];
                currentDirection = newestRail.direction;
                futureDirection = newestRail.direction;
            }
        }
	}




    void Update()
    {

        TurnNeeded();

        CleanUp();
        
    }



    // Method for easy access to turn condition
    bool TurnSucceeded()
    {
        // Edit this
        return ( GameManager.instance.speed < 1.1f );
    }


    // Method to see if a turn is needed
    void TurnNeeded()
    {

        // Checks for turning
        if (upcomingTurns.Count > 0)
        {

            // Check if the middle of the turn has passed

            RailInfo nextTurn = upcomingTurns.Peek();
            nextTurnX = nextTurn.Position.x;
            nextTurnZ = nextTurn.Position.z;
            bool bool1 = player.transform.position.z > nextTurnZ && nextTurn.direction == Direction.ZUp;
            bool bool2 = player.transform.position.z < nextTurnZ && nextTurn.direction == Direction.ZDown;
            bool bool3 = player.transform.position.x > nextTurnX && nextTurn.direction == Direction.XUp;
            bool bool4 = player.transform.position.x < nextTurnX && nextTurn.direction == Direction.XDown;



            if (bool1 || bool2 || bool3 || bool4)
            {

                //If the turn is passed take it out of the turning queue so that it isn't checked against
                upcomingTurns.Dequeue();

                // Check if cart turned
                if ( TurnSucceeded() )
                {

                    //Succeeded the turn

                    currentDirection = nextTurn.turnDirection;

                    //Puts the player on next track

                    if (currentDirection == Direction.XUp)
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    }

                    else if (currentDirection == Direction.XDown)
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                    }

                    else if (currentDirection == Direction.ZDown)
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    }

                    else
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }

                }
                else
                {
                    // Game Over condition
                    GameManager.instance.GameOver();
                }
            }
        }
    }
    


    void CleanUp ()
    {

        // Direction check of cart
        Direction direction = currentDirection;

        bool bool1 = player.transform.position.z > nextPosZ && direction == Direction.ZUp;
        bool bool2 = player.transform.position.z < nextPosZ && direction == Direction.ZDown;
        bool bool3 = player.transform.position.x > nextPosX && direction == Direction.XUp;
        bool bool4 = player.transform.position.x < nextPosX && direction == Direction.XDown;


        // Changes next deletion position
        if (bool1)
        {
            nextPosZ += 2.6f;
        }
        else if (bool2)
        {
            nextPosZ -= 2.6f;
        }
        else if (bool3)
        {
            nextPosX += 2.6f;
        }
        else if (bool4)
        {
            nextPosX -= 2.6f;
        }



        //Take out the farthest rail
        if (bool1 || bool2 || bool3 || bool4)
        {

            if (currentRails.Peek().turn)
            {
                unusedTurns.Enqueue(currentRails.Dequeue());
            }
            else
            {
                unused.Enqueue(currentRails.Dequeue());
            }
        }
    }


    void AddTrack()
    {

        // Conditional for turning being spawned
        // Edit this
        if (unusedTurns.Count > 0)
        {
            NewTurn();
        }
        else
        {
            NewStraight();
        }
    }


    void NewStraight()
    {

        Vector3 position = newestRail.Position;

        Direction direction = futureDirection;

        bool bool1 = direction == Direction.ZUp;
        bool bool2 = direction == Direction.ZDown;
        bool bool3 = direction == Direction.XUp;
        bool bool4 = direction == Direction.XDown;


        // Set position based on previous track
        if (bool1)
        {
            position.z += 2.6f;
        }
        else if (bool2)
        {
            position.z -= 2.6f;
        }
        else if (bool3)
        {
            position.x += 2.6f;
        }
        else if (bool4)
        {
            position.x -= 2.6f;
        }

        // Set new track and add it
        newestRail = unused.Dequeue();
        newestRail.direction = futureDirection;
        newestRail.NewPosition(position);
        currentRails.Enqueue(newestRail);

    }


    void NewTurn()
    {

        // Starting direction of turn
        Direction direction = futureDirection;

        Vector3 position = newestRail.Position;

        bool bool1 = direction == Direction.ZUp;
        bool bool2 = direction == Direction.ZDown;
        bool bool3 = direction == Direction.XUp;
        bool bool4 = direction == Direction.XDown;


        if (bool1)
        {
            position.z += 2.6f;
        }
        else if (bool2)
        {
            position.z -= 2.6f;
        }
        else if (bool3)
        {
            position.x += 2.6f;
        }
        else if (bool4)
        {
            position.x -= 2.6f;
        }


        newestRail = unusedTurns.Dequeue();



        // Setting turn type
        // Edit this
        if (currentDirection == Direction.ZUp)
        {
            newestRail.direction = Direction.ZUp;
            newestRail.turnDirection = Direction.XUp;
        }
        else if (currentDirection == Direction.XUp)
        {
            newestRail.direction = Direction.XUp;
            newestRail.turnDirection = Direction.ZDown;
        }
        else if (currentDirection == Direction.ZDown)
        {
            newestRail.direction = Direction.ZDown;
            newestRail.turnDirection = Direction.XDown;
        }
        else if (currentDirection == Direction.XDown)
        {
            newestRail.direction = Direction.XDown;
            newestRail.turnDirection = Direction.ZUp;
        }


        // Create rail and add it
        newestRail.NewPosition(position);

        upcomingTurns.Enqueue(newestRail);
        futureDirection = newestRail.turnDirection;

        currentRails.Enqueue(newestRail);
    }


}
