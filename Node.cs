using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int id; // ID wierzchołka
    private List<Node> neighbors = new List<Node>(); // Lista sąsiadów
    private LineRenderer lineRenderer; // LineRenderer do rysowania krawędzi
    private Player occupyingPlayer; // Gracz znajdujący się na tym wierzchołku (jeśli taki istnieje)

    public void Initialize(int id)
    {
        this.id = id;
        lineRenderer = GetComponent<LineRenderer>();

        // Ustawienia LineRenderer
        lineRenderer.startWidth = 0.5f; // Grubsza linia
        lineRenderer.endWidth = 0.5f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.grey;
    }

    public void AddNeighbor(Node node)
    {
        if (!neighbors.Contains(node))
        {
            neighbors.Add(node);
        }
        UpdateConnections();
    }

    public void RemoveNeighbor(Node node)
    {
        if (neighbors.Contains(node))
        {
            neighbors.Remove(node);
            UpdateConnections();
        }
    }

    public List<Node> GetNeighbors()
    {
        return neighbors;
    }

    private void UpdateConnections()
    {
        if (neighbors == null || neighbors.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = neighbors.Count * 2;
        int index = 0;

        foreach (Node neighbor in neighbors)
        {
            if (neighbor != null)
            {
                lineRenderer.SetPosition(index++, transform.position);
                lineRenderer.SetPosition(index++, neighbor.transform.position);
            }
        }
    }

    public bool IsOccupied()
    {
        return occupyingPlayer != null;
    }

    public void SetOccupyingPlayer(Player player)
    {
        occupyingPlayer = player;
    }

    public Player GetOccupyingPlayer()
    {
        return occupyingPlayer;
    }

    private void OnMouseDown()
    {
        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        manager.OnNodeClicked(this);
    }
}
