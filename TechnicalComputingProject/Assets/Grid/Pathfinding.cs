using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public NodeGrid nodeGrid;
    public Transform AISeekerTf;
    public Transform targetTf;
    bool once = true;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //FindPath(AISeekerTf.position, targetTf.position);
    }
    // Update is called once per frame
    void Update()
    {
        //if (once)
        //{
        if (nodeGrid.gridLSize.x > 0 && nodeGrid.gridLSize.y > 0)
        {
            //once = false;
            FindPath(AISeekerTf.position, targetTf.position);
        }
        //}
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = nodeGrid.NodeFromWorldPoint(startPos);
        startNode.gCost = 0;
        Node endNode = nodeGrid.NodeFromWorldPoint(endPos);
        Debug.Log("Start node = " + startNode.nodeWPosition);
        Debug.Log("End node = " + endNode.id);

        List<Node> openSet = new List<Node>();                                                                              // Creating list to store open nodes
        List<Node> closedSet = new List<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)                                                                          // Looping through open set and checking f costs to decide which node to
            {
                Debug.Log("Added node fCost: " + openSet[i].fCost);

                if (openSet[i].fCost < currentNode.fCost ||                                                                 // (Will be node with lowest F cost in open list)
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))                          // If F cost is the same, compare h cost and pick
                {                                                                                                           // one closest (lowest) to end node
                    currentNode = openSet[i];
                    Debug.Log("assigning current node");
                }
            }

            openSet.Remove(currentNode);

            closedSet.Add(currentNode);

            if (currentNode.id == endNode.id)                                                                                     // Have found the target node
            {
                
                Debug.Log("RETRACE PATH");
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Node neighbourNode in nodeGrid.GetNeighbours(currentNode))
            {
                Debug.Log("Neighbour node ID = " + neighbourNode.id);

                if (!neighbourNode.walkable || closedSet.Exists(x => x.id == neighbourNode.id))                                           // Checking if neighbour node is not walkable or is in the closed set
                {                                                                                                           // so we can just skip to the next neighbor in list
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode);               // Distance/cost from current node to neighbour node to
                                                                                                                            // calculate costs for new neighbour node
                Debug.Log("curr node: " + currentNode.id);
                Debug.Log("Open set Count: " + openSet.Count);

                if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Exists(x => x.id == neighbourNode.id))                   // Checking if new neighbour G cost is less than its current cost to see if it needs
                {                                                                                                           // to be reduced or is not in the open list (not already going to be checked)
                    neighbourNode.gCost = newMovementCostToNeighbour;                                                       // Set current G cost to new G cost (better path from start node found)
                    neighbourNode.hCost = GetDistance(neighbourNode, endNode);                                              // Set H cost as distance to end node
                    neighbourNode.parentNode = currentNode;                                                                 // Setting parent node to look back on

                    if (!openSet.Exists(x => x.id == neighbourNode.id))
                    {
                        Debug.Log("Added node fCost: " + neighbourNode.fCost);
                        openSet.Add(neighbourNode);                                                                         // Adding chosen valid neighbours node to open list
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        Debug.Log("RETRACE CALLED");
        List<Node> path = new List<Node>();                                                                                 // Creating new list for the path backwards from the end node

        Node currentNode = endNode;                                                                                         // Starting node for path will be the end node
        //Debug.Log("Start node path"+ startNode.id);
        while (currentNode.id != startNode.id)
        {
            path.Add(currentNode);                                                                                          // Add the current node to the path
            Debug.Log(path.Count);
            currentNode = currentNode.parentNode;                                                                           // Make the new current node, the old current nodes parent going back
        }                                                                                                                   // until the current node is == the start node
        path.Add(currentNode);
        path.Reverse();                                                                                                     // Reverse path to get path from start node to end node

        Debug.Log("Path count: " + path.Count);
        nodeGrid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridXPos - nodeB.gridXPos);
        int dstY = Mathf.Abs(nodeA.gridYPos - nodeB.gridYPos);

        // Diagonal distance to get on same horizontal/vertical line as end node always = lower distance
        // so then rest of the distance == highest distance - lowest distance
        // (Multiplying by 14 for diagonal and 10 for non diagonal (1.4*10 , 1.0*10)
        if(dstX > dstY)                                                                                                 // If Y distance to end node < X distance,
        {
            //Debug.Log(14 * dstY + 10 * (dstX - dstY));
            return 14 * dstY + 10 * (dstX - dstY);
        }
        //Debug.Log(14 * dstX + 10 * (dstY - dstX));
        return 14 * dstX + 10 * (dstY - dstX);
        
    }
}
