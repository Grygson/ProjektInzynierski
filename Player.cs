using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Node currentNode; // Aktualny wierzchołek
    private Renderer playerRenderer;

    private void Awake()
    {
        playerRenderer = GetComponent<Renderer>();
    }

    public bool MoveTo(Node targetNode)
    {
        // Sprawdź, czy targetNode jest sąsiadem
        if (!currentNode.GetNeighbors().Contains(targetNode))
        {
            Debug.LogWarning("Invalid move: Target node is not a neighbor.");
            return false;
        }

        // Sprawdź, czy wierzchołek jest zajęty
        if (targetNode.IsOccupied())
        {
            Debug.LogWarning("Invalid move: Target node is occupied.");
            return false;
        }

        // Usuń gracza z bieżącego wierzchołka
        currentNode.SetOccupyingPlayer(null);

        // Przenieś gracza na nowy wierzchołek
        StartCoroutine(MoveToPosition(targetNode));
        return true;
    }

    private IEnumerator MoveToPosition(Node targetNode)
    {
        Vector3 start = transform.position;
        Vector3 end = targetNode.transform.position;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        targetNode.SetOccupyingPlayer(this);
        currentNode.RemoveNeighbor(targetNode);
        targetNode.RemoveNeighbor(currentNode);
        currentNode = targetNode;
    }

public bool CanMove()
{
    // Pobierz listę wszystkich sąsiadów bieżącego wierzchołka
    var neighbors = currentNode.GetNeighbors();

    // Usuń sąsiadów, którzy są zajęci
    var availableNeighbors = neighbors.FindAll(neighbor => !neighbor.IsOccupied());

    // Jeśli brak dostępnych sąsiadów, jedyny możliwy ruch prowadzi na zajęty wierzchołek
    return availableNeighbors.Count > 0;
}


    public void Highlight(bool isActive)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = isActive ? Color.green : Color.white;
        }
    }
}
