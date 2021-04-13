using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeInARow : MonoBehaviour
{
    public BoardPiece[,] board;
    [SerializeField] float pieceOffset = 1.3f;
    [SerializeField] BoardPiece bObject;
    [SerializeField] GameObject pipeConnector;
    [SerializeField] GameObject selectedMarker;
    [SerializeField] Text text;
    bool won = false;
    int selectedPieces = 0;

    Vector2Int activePiece;

    // Start is called before the first frame update
    void Start()
    {
        activePiece = new Vector2Int(1, 1);

        board = new BoardPiece[3, 3];
        // Make board matrix
        for (int x = 0; x < 3; x++)
        {
            //Pipes
            Instantiate(pipeConnector, transform.position + new Vector3(x * pieceOffset, 0), transform.rotation);
            for (int y = 0; y < 3; y++)
            {
                board[x, y] = Instantiate(bObject, transform.position +new Vector3(x*pieceOffset, -y*pieceOffset), transform.rotation);
            }
        }
        selectedMarker.transform.position = board[activePiece.x, activePiece.y].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!won)
        {
            if (selectedPieces > 8)
            {
                DoDraw();
                return;
            }
            DoInput();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetGame();
            }
        }
        // Move selector towards position
        selectedMarker.transform.position = Vector3.Lerp(selectedMarker.transform.position, board[activePiece.x, activePiece.y].transform.position, 5 * Time.deltaTime);
    }

    private void DoInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            activePiece.x--;
            if (activePiece.x < 0) activePiece.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            activePiece.x++;
            if (activePiece.x > 2) activePiece.x = 2;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            activePiece.y--;
            if (activePiece.y < 0) activePiece.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            activePiece.y++;
            if (activePiece.y > 2) activePiece.y = 2;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            // Player Move
            if (TrySelectPiece(activePiece))
            {                                
                board[activePiece.x, activePiece.y].SetPiece(1);
                selectedPieces++;
                if (CheckForEnd()) return;

                //AI Move
                AIMove();
                selectedPieces++;
                if (CheckForEnd()) return;
            }
        }
    }

    private bool CheckForEnd()
    {
        int i = CheckForWin();
        if (i != 0)
        {
            if (i == 1) DoWin(true);
            if (i == 2) DoWin(false);
            return true;
        }
        if (selectedPieces > 8)
        {
            DoDraw();
            return true;
        }
        return false;
    }

    private bool TrySelectPiece(Vector2Int p)
    {
        if (board[p.x, p.y].status != 0)
        {
            board[p.x, p.y].DoError();
            return false;
        }        
        return true;
    }

    private void DoWin(bool winner)
    {
        won = true;
        if (winner)
        {
            text.text = "You won!";
        }
        else
        {
            text.text = "You lost!";
        }
    }

    private void DoDraw()
    {
        won = true;
        text.text = "Draw!";
    }

    private void AIMove()
    {
        if (selectedPieces > 8)
        {
            DoDraw();
            return;
        }
        int counter = 0;

        while(true)
        {
            Vector2Int r = new Vector2Int( Random.Range(0, 3),Random.Range(0, 3));

            if (TrySelectPiece(r))
            {
                board[r.x, r.y].SetPiece(2);
                break;
            }
            counter++;
            if (counter > 5000)
            {
                Debug.LogError("Too many tries for AI!");
                return;
            }
        }
    }

    private int CheckForWin()
    {
        // Check y
        for (int x = 0; x < 3; x++)
        {
            int l1 = 0;
            int l2 = 0;
            for (int y = 0; y < 3; y++)
            {
                if (board[x, y].status == 1) l1++;
                if (board[x, y].status == 2) l2++;
            }
            if (l1 == 3) return 1;
            if (l2 == 3) return 2;
        }

        // Check x
        for (int y = 0; y < 3; y++)
        {
            int l1 = 0;
            int l2 = 0;
            for (int x = 0; x < 3; x++)
            {
                if (board[x, y].status == 1) l1++;
                if (board[x, y].status == 2) l2++;
            }
            if (l1 == 3) return 1;
            if (l2 == 3) return 2;
        }

        //Sideways LUp to RDown
        if (board[0, 0].status == 1 &&
            board[1, 1].status == 1 &&
            board[2, 2].status == 1) return 1;
        //Sideways RUp to LDown
        if (board[2, 0].status == 1 &&
            board[1, 1].status == 1 &&
            board[0, 2].status == 1) return 1;

        //Sideways LUp to RDown
        if (board[0, 0].status == 2 &&
            board[1, 1].status == 2 &&
            board[2, 2].status == 2) return 2;

        //Sideways RUp to LDown
        if (board[2, 0].status == 2 &&
            board[1, 1].status == 2 &&
            board[0, 2].status == 2) return 2;
        return 0;
    }

    private void ResetGame()
    {
        won = false;
        selectedPieces = 0;
        activePiece = new Vector2Int(1, 1);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                board[x, y].SetPiece(0);
            }
        }

        text.text = "New Game!";
    }
}
