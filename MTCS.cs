using System.Collections.Generic;
using UnityEngine;

public class MCTS
{
    public class MCTSNode
    {
        public Node GameNode;
        public MCTSNode Parent;
        public List<MCTSNode> Children = new List<MCTSNode>();
        public int Visits = 0;
        public float Score = 0f;

        public MCTSNode(Node gameNode, MCTSNode parent)
        {
            GameNode = gameNode;
            Parent = parent;
        }
    }

    public MCTSNode BestMove(MCTSNode root, int iterations, Player player)
    {
        for (int i = 0; i < iterations; i++)
        {
            MCTSNode selected = Select(root);
            Expand(selected);
            float result = Simulate(selected, player);
            Backpropagate(selected, result);
        }

        MCTSNode bestChild = null;
        float bestScore = float.MinValue;

        foreach (MCTSNode child in root.Children)
        {
            if (!child.GameNode.IsOccupied() && child.Score > bestScore)
            {
                bestScore = child.Score;
                bestChild = child;
            }
        }

        return bestChild;
    }

    private MCTSNode Select(MCTSNode node)
    {
        while (node.Children.Count > 0)
        {
            node = BestUCT(node);
        }
        return node;
    }

    private MCTSNode BestUCT(MCTSNode node)
    {
        MCTSNode best = null;
        float bestValue = float.MinValue;

        foreach (MCTSNode child in node.Children)
        {
            float uctValue = (child.Score / (child.Visits + 1)) +
                             Mathf.Sqrt(2 * Mathf.Log(node.Visits + 1) / (child.Visits + 1));
            if (uctValue > bestValue)
            {
                bestValue = uctValue;
                best = child;
            }
        }

        return best;
    }

    private void Expand(MCTSNode node)
    {
        foreach (Node neighbor in node.GameNode.GetNeighbors())
        {
            if (!neighbor.IsOccupied())
            {
                MCTSNode childNode = new MCTSNode(neighbor, node);
                node.Children.Add(childNode);
            }
        }
    }

private float Simulate(MCTSNode node, Player aiPlayer)
{
    Node currentNode = node.GameNode;

    for (int steps = 0; steps < 10; steps++) // Maksymalna liczba kroków w symulacji
    {
        List<Node> neighbors = currentNode.GetNeighbors();
        neighbors.RemoveAll(n => n.IsOccupied());

        if (neighbors.Count == 0)
        {
            // Jeśli brak możliwych ruchów, AI przegrywa
            return aiPlayer == null ? 1f : -1f;
        }

        // Wybierz losowego sąsiada
        currentNode = neighbors[Random.Range(0, neighbors.Count)];
    }

    // Zwróć losową wartość jako wynik symulacji
    return Random.Range(0.5f, 1f);
}


    private void Backpropagate(MCTSNode node, float result)
    {
        while (node != null)
        {
            node.Visits++;
            node.Score += result;
            node = node.Parent;
        }
    }
}
