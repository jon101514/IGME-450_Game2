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
	GameObject player;
	GameObject top;
	PlayerMovement playerScript;

	// Is there a turn and all information about it
	float nextTurnX;
	float nextTurnZ;
	Queue<RailInfo> upcomingTurns;

	//something
	int totalRailsPassed;
	int straightsSpawnedInRow;

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
		top = GameObject.FindGameObjectWithTag("Cart_Top");
		playerScript = player.GetComponent<PlayerMovement>();

		// Set up empty queues
		currentRails = new Queue<RailInfo>();
		unused = new Queue<RailInfo>();
		unusedTurns = new Queue<RailInfo>();
		upcomingTurns = new Queue<RailInfo>();


		// Change later to add more turns
		GameObject[] initialTurns = GameObject.FindGameObjectsWithTag("Turn");
		for (int i = 0; i < initialTurns.Length; i++)
		{
			unusedTurns.Enqueue(initialTurns[i].GetComponent<RailInfo>());
		}


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

		// Set default ints
		totalRailsPassed = 0;
		straightsSpawnedInRow = 18;

	}




	void Update()
	{

		Vector3 rotation = top.transform.rotation.eulerAngles;
		top.transform.rotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, -25 * playerScript.leanState));

		TurnNeeded();
		//checking if player passes the checkpoint
		/*
		if(player.transform.position.z>=railCheckpoint.transform.position.z)
		{
			float oldTime = GameManager.instance.GetTime();
			float newTime = 15.0f;
			oldTime += newTime;
			GameManager.instance.SetTime(oldTime);
		}
		*/
		CleanUp();

	}



	// Method for easy access to turn condition
	bool TurnSucceeded(RailInfo turnTaken)
	{
		// Edit this
		return ( GameManager.instance.speed < 1.1f || turnTaken.Lean == playerScript.leanState);
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
			bool bigEnoughZ = player.transform.position.z > nextTurnZ && nextTurn.direction == Direction.ZUp;
			bool smallEnoughZ = player.transform.position.z < nextTurnZ && nextTurn.direction == Direction.ZDown;
			bool bigEnoughX = player.transform.position.x > nextTurnX && nextTurn.direction == Direction.XUp;
			bool smallEnoughX = player.transform.position.x < nextTurnX && nextTurn.direction == Direction.XDown;



			if (bigEnoughZ || smallEnoughZ || bigEnoughX || smallEnoughX)
			{

				//If the turn is passed take it out of the turning queue so that it isn't checked against
				RailInfo turnTaken = upcomingTurns.Dequeue();

				// Check if cart turned
				if ( TurnSucceeded(turnTaken) )
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

					// Set the position to align with the track
					if (currentDirection == Direction.ZDown || currentDirection == Direction.ZUp)
					{
						player.transform.position = new Vector3( turnTaken.nextPosition.x, player.transform.position.y, player.transform.position.z );
					}
					else
					{
						player.transform.position = new Vector3( player.transform.position.x, player.transform.position.y, turnTaken.nextPosition.z );
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

		bool tracksToCheck = true;

		do
		{

			// Direction check of cart
			Direction direction = currentDirection;

			bool bigEnoughZ = player.transform.position.z > currentRails.Peek().Position.z + 7.8 && direction == Direction.ZUp;
			bool smallEnoughZ = player.transform.position.z < currentRails.Peek().Position.z - 7.8 && direction == Direction.ZDown;
			bool bigEnoughX = player.transform.position.x > currentRails.Peek().Position.x + 7.8 && direction == Direction.XUp;
			bool smallEnoughX = player.transform.position.x < currentRails.Peek().Position.x - 7.8 && direction == Direction.XDown;



			// Take out the farthest rail
			if (bigEnoughZ || smallEnoughZ || bigEnoughX || smallEnoughX)
			{

				if (currentRails.Peek().turn)
				{
					RailInfo dequeued = currentRails.Dequeue();
					dequeued.NewPosition(new Vector3(0,100,0));
					unusedTurns.Enqueue(dequeued);
				}
				else
				{
					RailInfo dequeued = currentRails.Dequeue();
					dequeued.NewPosition(new Vector3(0, 100, 0));
					unused.Enqueue(dequeued);
				}

				totalRailsPassed++;

				AddTrack();

				tracksToCheck = true;

			}
			else
			{
				tracksToCheck = false;
			}


		} while (tracksToCheck);
	}


	void AddTrack()
	{

		// Random track length with a max of 25  and min of 3
		// Not true random gets more likely to end every time a track is added
		bool newTurn = true;
		if(straightsSpawnedInRow < 25)
		{
			newTurn = (Random.Range(0, 25 - straightsSpawnedInRow) == 0);
		}
		if(straightsSpawnedInRow < 3)
		{
			newTurn = false;
		}


		// Conditional for turning being spawned
		// Edit this
		if (unusedTurns.Count > 0 && newTurn)
		{
			straightsSpawnedInRow = 0;
			NewTurn();
		}
		else if (unused.Count > 0)
		{
			straightsSpawnedInRow++;
			NewStraight();
		}
	}


	void NewStraight()
	{

		Vector3 position = newestRail.Position;

		Direction direction = futureDirection;

		bool zUp = direction == Direction.ZUp;
		bool zDown = direction == Direction.ZDown;
		bool xUp = direction == Direction.XUp;
		bool xDown = direction == Direction.XDown;


		// Set position based on previous track
		if (zUp)
		{
			position.z += 2.6f;
		}
		else if (zDown)
		{
			position.z -= 2.6f;
		}
		else if (xUp)
		{
			position.x += 2.6f;
		}
		else if (xDown)
		{
			position.x -= 2.6f;
		}

		newestRail.nextPosition = position;

		// Set new track and add it
		newestRail = unused.Dequeue();
		newestRail.direction = futureDirection;
		newestRail.turnDirection = futureDirection;
		newestRail.NewPosition(position);
		currentRails.Enqueue(newestRail);

	}


	void NewTurn()
	{

		// Starting direction of turn
		Direction direction = futureDirection;

		Vector3 position = newestRail.Position;

		bool zUp = direction == Direction.ZUp;
		bool zDown = direction == Direction.ZDown;
		bool xUp = direction == Direction.XUp;
		bool xDown = direction == Direction.XDown;


		// Set position based on previous track
		if (zUp)
		{
			position.z += 2.6f;
		}
		else if (zDown)
		{
			position.z -= 2.6f;
		}
		else if (xUp)
		{
			position.x += 2.6f;
		}
		else if (xDown)
		{
			position.x -= 2.6f;
		}

		newestRail.nextPosition = position;

		newestRail = unusedTurns.Dequeue();


		// Setting turn type
		// Edit this
		newestRail.direction = futureDirection;

		bool leftOrRight = (1 == Random.Range(0,2));
		if(leftOrRight)
		{
			newestRail.turnDirection = (Direction)((int)futureDirection * 2);
		}
		else
		{
			newestRail.turnDirection = (Direction)((int)futureDirection / 2);
		}

		if((int) newestRail.turnDirection > 8)
		{
			newestRail.turnDirection = (Direction) 1;
		}
		if ((int)newestRail.turnDirection < 1)
		{
			newestRail.turnDirection = (Direction) 8;
		}


		// Create rail and add it
		newestRail.NewPosition(position);

		upcomingTurns.Enqueue(newestRail);
		futureDirection = newestRail.turnDirection;

		currentRails.Enqueue(newestRail);
	}


}