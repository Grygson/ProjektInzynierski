using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject playerPrefab;
    public int nodeCount = 20;

    private List<Node> nodes = new List<Node>();
    private Player player1;
    private Player player2;
    private Player currentPlayer;
    private bool isVsAI;
    private MCTS mcts = new MCTS();

    private int player1Moves = 0; // Licznik ruchów gracza 1
    private int player2Moves = 0; // Licznik ruchów gracza 2

    void Start()
    {
        Camera.main.transform.position = new Vector3(0, 20, -20);
        Camera.main.transform.LookAt(Vector3.zero);

        nodePrefab = Resources.Load<GameObject>("Node");
        playerPrefab = Resources.Load<GameObject>("Player");

        isVsAI = PlayerPrefs.GetInt("VsAI", 0) == 1;
        nodeCount = PlayerPrefs.GetInt("NodeCount", 20);
        int difficulty = PlayerPrefs.GetInt("Difficulty", 1); // Domyślnie Normal

        GenerateGraph();
        SpawnPlayers();
        SetCurrentPlayer(player1);
    }

    public void GenerateGraph()
    {
        float spread = 15f;
        float minDistance = 2f;

        for (int i = 0; i < nodeCount; i++)
        {
            Vector3 position;
            bool positionValid;

            do
            {
                position = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
                positionValid = true;

                foreach (Node existingNode in nodes)
                {
                    if (Vector3.Distance(position, existingNode.transform.position) < minDistance)
                    {
                        positionValid = false;
                        break;
                    }
                }
            } while (!positionValid);

            GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity);
            Node node = nodeObject.GetComponent<Node>();
            node.Initialize(i);
            nodes.Add(node);
        }

        HashSet<Node> connectedNodes = new HashSet<Node> { nodes[0] };
        while (connectedNodes.Count < nodes.Count)
        {
            Node nodeA = GetRandomNode(connectedNodes);
            Node nodeB = GetRandomNodeExcluding(connectedNodes);

            nodeA.AddNeighbor(nodeB);
            nodeB.AddNeighbor(nodeA);

            connectedNodes.Add(nodeB);
        }
    }

    public void SpawnPlayers()
    {
        player1 = Instantiate(playerPrefab, nodes[0].transform.position, Quaternion.identity).GetComponent<Player>();
        player1.currentNode = nodes[0];
        nodes[0].SetOccupyingPlayer(player1);

        player2 = Instantiate(playerPrefab, nodes[nodes.Count - 1].transform.position, Quaternion.identity).GetComponent<Player>();
        player2.currentNode = nodes[nodes.Count - 1];
        nodes[nodes.Count - 1].SetOccupyingPlayer(player2);
    }

    public void OnNodeClicked(Node clickedNode)
    {
        if (currentPlayer.MoveTo(clickedNode))
        {
            if (currentPlayer == player1) player1Moves++;
            else player2Moves++;

            CheckEndCondition();
            StartCoroutine(DelayedSwitchPlayer());
        }
    }

    private IEnumerator DelayedSwitchPlayer()
    {
        yield return new WaitForSeconds(1f);
        SwitchPlayer();
    }

public void SwitchPlayer()
{
    // Zmień obecnego gracza
    currentPlayer = (currentPlayer == player1) ? player2 : player1;

    // Sprawdź, czy nowy gracz może się poruszyć
    if (!currentPlayer.CanMove())
    {
        EndGame();
        return;
    }

    // Jeśli jest aktywny tryb AI i obecnym graczem jest AI, wykonaj ruch AI
    if (isVsAI && currentPlayer == player2)
    {
        StartCoroutine(DelayedAIMove());
    }
}


    private IEnumerator DelayedAIMove()
    {
        yield return new WaitForSeconds(1f);
        MakeAIMove();
    }

public void CheckEndCondition()
{
    // Sprawdź, czy obecny gracz może się poruszyć
    if (!currentPlayer.CanMove())
    {
        // Jeśli obecny gracz to AI, zakończ grę
        if (currentPlayer == player2 && isVsAI)
        {
            EndGame();
            return;
        }

        // Jeśli to gracz 1 lub gracz 2 w trybie PvP, zakończ grę
        if (!isVsAI || currentPlayer == player1)
        {
            EndGame();
            return;
        }
    }
}
public void MakeAIMove()
{
    Debug.Log("AI is making a move...");
    // Pobierz poziom trudności
    int difficulty = PlayerPrefs.GetInt("Difficulty", 1);

    // Ustaw liczbę iteracji w zależności od poziomu trudności
    int iterations = difficulty switch
    {
        0 => 10,   // Easy
        1 => 100,  // Normal
        2 => 1000, // Hard
        _ => 100   // Domyślnie Normal
    };
    // Utwórz węzeł MCTS i oblicz najlepszy ruch
    MCTS.MCTSNode root = new MCTS.MCTSNode(player2.currentNode, null);
    MCTS.MCTSNode bestMove = mcts.BestMove(root, iterations, player2);

 if (bestMove != null)
    {
        player2.MoveTo(bestMove.GameNode);
        Debug.Log("AI moved to node: " + bestMove.GameNode.id);

        CheckEndCondition();
        SwitchPlayer();
    }
    else
    {
        Debug.LogWarning("AI could not find a valid move!");
        EndGame();
    }
}



void EndGame()
    {
        string winner = (currentPlayer == player1)
            ? (isVsAI ? "AI" : "Player 2")
            : "Player 1";

        PlayerPrefs.SetString("Winner", winner);
        PlayerPrefs.SetInt("Player1Moves", player1Moves);
        PlayerPrefs.SetInt("Player2Moves", player2Moves);

    Debug.Log($"Game Over! Winner: {winner}");
    PlayerPrefs.SetString("Winner", winner);
    PlayerPrefs.SetInt("Player1Moves", player1Moves);
    PlayerPrefs.SetInt("Player2Moves", player2Moves);

    SceneManager.LoadScene("GameOverScene");
    }

    Node GetRandomNode(HashSet<Node> nodeSet)
    {
        int index = Random.Range(0, nodeSet.Count);
        foreach (Node node in nodeSet)
        {
            if (index == 0) return node;
            index--;
        }
        return null;
    }

    Node GetRandomNodeExcluding(HashSet<Node> excludedSet)
    {
        List<Node> availableNodes = new List<Node>();
        foreach (Node node in nodes)
        {
            if (!excludedSet.Contains(node))
            {
                availableNodes.Add(node);
            }
        }

        return availableNodes[Random.Range(0, availableNodes.Count)];
    }

    void SetCurrentPlayer(Player player)
    {
        if (currentPlayer != null)
        {
            currentPlayer.Highlight(false);
        }

        currentPlayer = player;
        currentPlayer.Highlight(true);
    }

}
