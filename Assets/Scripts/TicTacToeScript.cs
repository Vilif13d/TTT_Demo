using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeScript : MonoBehaviour
{
    public bool isTwoPlayer;
    public bool isPlaying;
    public int playerNum = 1;
    public List<int> grid;

    //player selection points
    public GameObject p1Grid;
    public List<GameObject> player1;
    public GameObject p2Grid;
    public List<GameObject> player2;

    public GameObject player1Win;
    public GameObject player2Win;
    public GameObject draw;
    public GameObject returnHome;
    public ParticleSystem winExp;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void GameStart()
    {
        p2Grid.SetActive(false);
        //sets the game to playing, which could be useful with future functions to prevent them from firing.
        isPlaying = true;
    }

    public void PlayerCount()
    {
        //swap which player is currently able to place tokens
        if (isTwoPlayer)
        {
            isTwoPlayer = false;
            return;
        }
        else
            isTwoPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool GridFull()
    {
        foreach (int i in grid)
        {
            //check if any number in the grid is still set to 0, if true the grid is not full
            if (i == 0)
                return false;
        }

        return true;
    }

    private void CheckForWin()
    {
        // Winning combinations for standard game. This could be expanded to larger spaces for a secondary game with a larger space,
        // the combinations would be longer. This pattern recognition works best with straight lines, as shape based patterns
        // would require a larger library of possible wins.
        int[,] winningCombinations = new int[,]
        {
        {0, 1, 2}, // Top row
        {3, 4, 5}, // Middle row
        {6, 7, 8}, // Bottom row
        {0, 3, 6}, // Left column
        {1, 4, 7}, // Middle column
        {2, 5, 8}, // Right column
        {0, 4, 8}, // Diagonal from top-left
        {2, 4, 6}  // Diagonal from top-right
        };

        // Check all winning combinations for matching numbers, but only grants a win if grid numbers are greater than 0 and match
        for (int i = 0; i < winningCombinations.GetLength(0); i++)
        {
            int a = winningCombinations[i, 0];
            int b = winningCombinations[i, 1];
            int c = winningCombinations[i, 2];

            if (grid[a] != 0 && grid[a] == grid[b] && grid[b] == grid[c])
            {
                StartCoroutine(GameEnd(grid[a])); //check winner, 1 for X, and 2 for O
                return;
            }
        }
        // No winner
        if (GridFull())
        {
            Debug.Log("Grid Full");
            StartCoroutine(GameEnd(0));
        }
        else
            NextPlayer();
    }

    public void NextPlayer()
    {
        //deactivate current player selection options and activate next player. If single player, call on the computer to choose a location
        if (p1Grid.activeInHierarchy)
        {
            p1Grid.SetActive(false);
            p2Grid.SetActive(true);

            if (!isTwoPlayer)
                ComputerChoice();
            return;
        }
        else
        {
            p1Grid.SetActive(true);
            p2Grid.SetActive(false);
        }
    }

    public void GridFill(int gLoc)
    {
        //fill in the number for the player that just marked their move, then deactivate that location for each player.
        if (p1Grid.activeInHierarchy)
            grid[gLoc] = 1;
        else
            grid[gLoc] = 2;

        player1[gLoc].SetActive(false);
        player2[gLoc].SetActive(false);

        CheckForWin();
    }

    IEnumerator GameEnd(int player)
    {
        //disable the grids so no more pieces can be dropped
        p1Grid.SetActive(false);
        p2Grid.SetActive(false);
        isPlaying = false;
        //play explosion
        winExp.Play();

        yield return new WaitForSeconds(1);

        if (player == 0)
        {
            draw.SetActive(true);
            returnHome.SetActive(true);
            Debug.Log("No Winner!");
        }
        else
        {
            returnHome.SetActive(true);

            if (player == 1)
                player1Win.SetActive(true);
            else
                player2Win.SetActive(true);

            Debug.Log("Player " + player + " Wins!");
        }
    }

    public void ComputerChoice()
    {
        //count all the positions of the player2 grid
        int totalObjects = player2.Count;
        int currentIndex = Random.Range(0, totalObjects);
        
        if (totalObjects == 0)
        {
            Debug.LogWarning("The list of game objects is empty.");
            return;
        }

        GameObject selectedObject = null;
        //find a random number and check if it is available on the grid. If not, iterate forward by one count on the grid list until an empty space is found.
        for (int i = 0; i < totalObjects; i++)
        {
            if (currentIndex >= totalObjects)
                currentIndex = 0;

            if (player2[currentIndex].activeInHierarchy)
            {
                selectedObject = player2[currentIndex];
                break;
            }

            currentIndex++;
        }
        //if grid point is available, marke it by calling the OnMouseUp function from that grid space.
        //OnMouseUp will call CheckForWin which in turn will change the current player. 
        if (selectedObject != null)
        {
            selectedObject.GetComponent<NewButton>().OnMouseUp();
        }
        else
        {
            Debug.LogWarning("No active game objects found in the list.");
        }
    }

    public void ResetGame()
    {
        //reset game by activating both grids, activating all grid spaces they have, and resetting each location marker.
        p1Grid.SetActive(true);
        p2Grid.SetActive(true);

        foreach (GameObject go in player1)
        {
            go.SetActive(true);
            go.GetComponent<NewButton>().ResetButton();
        }

        foreach (GameObject go in player2)
        {
            go.SetActive(true);
            go.GetComponent<NewButton>().ResetButton();
        }

        for (int i = 0; i < grid.Count; i++)
        {
            grid[i] = 0;
        }
        //delay clearing the win/draw screen
        StartCoroutine(DelayCleanup());
    }

    IEnumerator DelayCleanup()
    {
        //after waiting for the camera to move, deactivate the win/draw screen and the return home button.
        yield return new WaitForSeconds(1);
        player1Win.SetActive(false);
        player2Win.SetActive(false);
        draw.SetActive(false);
        returnHome.SetActive(false);
    }
}
