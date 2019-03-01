using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour {

    //Rails
    RailInfo[] initialRails;
    Queue<RailInfo> currentRails;
    RailInfo lastRail;
    float nextPosZ = 7.8f;
    float nextPosX = 7.8f;
    GameObject player;
    public GameObject railCheckpoint;

    Queue<RailInfo> unused;
    Queue<RailInfo> unusedTurns;

	[SerializeField] 
    bool turn;
    float nextTurnX;
    float nextTurnZ;
    RailInfo nextTurn;
    [SerializeField]
    Direction currentDirection;
    [SerializeField]
    Direction futureDirection;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        currentRails = new Queue<RailInfo>();

        unused = new Queue<RailInfo>();
        unusedTurns = new Queue<RailInfo>();

        GameObject initialTurn = GameObject.FindGameObjectWithTag("Turn");
        unusedTurns.Enqueue(initialTurn.GetComponent<RailInfo>());

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Rail");
        int leng = objects.Length;
        initialRails = new RailInfo[leng];
        for (int i = 0; i < leng; i++)
        {
            RailInfo temp = objects[i].GetComponent<RailInfo>();
            initialRails[temp.initialPosition] = temp;
        }

        for (int i = 0; i < leng; i++)
        {
            currentRails.Enqueue(initialRails[i]);
            if(i == leng - 1)
            {
                lastRail = initialRails[i];
                currentDirection = lastRail.direction;
                futureDirection = lastRail.direction;
            }
        }

        turn = false;
	}

    void Update()
    {
        if(turn)
        {
            //Check if the middle of the turn has passed
            bool bool1 = player.transform.position.z > nextTurnZ && nextTurn.direction == Direction.ZUp;
            bool bool2 = player.transform.position.z < nextTurnZ && nextTurn.direction == Direction.ZDown;
            bool bool3 = player.transform.position.x > nextTurnX && nextTurn.direction == Direction.XUp;
            bool bool4 = player.transform.position.x < nextTurnX && nextTurn.direction == Direction.XDown;

            if(bool1 || bool2 || bool3 || bool4)
            {
                Debug.Log("Next t d: " + nextTurn.direction);

                //Check speed or turn
                //Needs to be edited later
                //Change speed value when game manager is correct
                //Add tilt value when added
				turn = false;
                if(GameManager.instance.speed < 1.1f)
                {
                    currentDirection = nextTurn.turnDirection;
                    if(currentDirection == Direction.XUp)
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
                    }
                    else if (currentDirection == Direction.XDown)
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                    }
                    else if(currentDirection == Direction.ZDown)
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    }
                    else
                    {
                        player.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
                    }
                }
                else
                {
                    //Edit to be the game over state
                    Debug.Log("Game Over");
					GameManager.instance.GameOver();
                }
            }
        }
        //checking if player passes the checkpoint
        if(player.transform.position.z>=railCheckpoint.transform.position.z)
        {
            float oldTime = GameManager.instance.GetTime();
            float newTime = 15.0f;
            oldTime += newTime;
            // GameManager.instance.SetTime(oldTime);
        }

        CleanUp();

        /*
            nextPosZ += 2.6f;
            Vector3 position = lastRail.Position;
            position.z += 2.6f;
            lastRail = currentRails.Dequeue();
            lastRail.direction = direction;
            lastRail.NewPosition(position);
            currentRails.Enqueue(lastRail);
         */
    }

    // Update is called once per frame
    void CleanUp ()
    {

        Direction direction = currentDirection;

        bool bool1 = player.transform.position.z > nextPosZ && direction == Direction.ZUp;

        bool bool2 = player.transform.position.z < nextPosZ && direction == Direction.ZDown;

        bool bool3 = player.transform.position.x > nextPosX && direction == Direction.XUp;

        bool bool4 = player.transform.position.x < nextPosX && direction == Direction.XDown;


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

        if (bool1 || bool2 || bool3 || bool4)
        {
            //Debug.Log("Last Rail position: " + lastRail.Position);

            if (currentRails.Peek().turn)
            {
                unusedTurns.Enqueue(currentRails.Dequeue());
                turn = false;
            }
            else
            {
                unused.Enqueue(currentRails.Dequeue());
            }


			if (unusedTurns.Count > 0)
            {

                Vector3 position = lastRail.Position;

                bool1 = direction == Direction.ZUp;

                bool2 = direction == Direction.ZDown;

                bool3 = direction == Direction.XUp;

                bool4 = direction == Direction.XDown;


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


                lastRail = unusedTurns.Dequeue();



                if (currentDirection == Direction.ZUp)
                {
                    lastRail.direction = Direction.ZUp;
                    lastRail.turnDirection = Direction.XUp;
                }
                else if (currentDirection == Direction.XUp)
                {
                    lastRail.direction = Direction.XUp;
                    lastRail.turnDirection = Direction.ZDown;
                }
                else if (currentDirection == Direction.ZDown)
                {
                    lastRail.direction = Direction.ZDown;
                    lastRail.turnDirection = Direction.XDown;
                }
                else if (currentDirection == Direction.XDown)
                {
                    lastRail.direction = Direction.XDown;
                    lastRail.turnDirection = Direction.ZUp;
                }



                lastRail.NewPosition(position);
                
                turn = true;
                nextTurnX = position.x;
                nextTurnZ = position.z;
                nextTurn = lastRail;
                futureDirection = lastRail.turnDirection;

                currentRails.Enqueue(lastRail);
            }
            else
            {
                Vector3 position = lastRail.Position;

                direction = futureDirection;

                bool1 = direction == Direction.ZUp;

                bool2 = direction == Direction.ZDown;

                bool3 = direction == Direction.XUp;

                bool4 = direction == Direction.XDown;


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

                lastRail = unused.Dequeue();
                lastRail.direction = futureDirection;
                lastRail.NewPosition(position);
                currentRails.Enqueue(lastRail);
            }
        }


    }
}
