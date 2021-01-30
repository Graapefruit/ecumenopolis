using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODOS:
// 1: Separate the source/dest from the graphs
// 2: Keep copies of the graphs in memory
// 3: Terminate Dijkstra's early if shortest path found

public class BoardManager : MonoBehaviour
{
    public class Vector3Path {
        // TODO: Better Encapsulation
        public List<Vector3> _path;
        public float _dist;

        public Vector3Path(Vector3 firstWaypoint, float distToSource) {
            _path = new List<Vector3>();
            _path.Add(firstWaypoint);
            _dist = distToSource;
        }

        public Vector3Path(Vector3Path source) {
            _path = new List<Vector3>();
            foreach(Vector3 vector in source.getPath()) {
                _path.Add(vector);
            }
            _dist = source.getDistance();
        }

        public void addWaypoint(Vector3 waypoint) {
            _dist += (waypoint - _path[_path.Count-1]).magnitude;
            _path.Add(waypoint);
        }

        public List<Vector3> getPath() {
            return _path;
        }

        public float getDistance() {
            return _dist;
        }

        public string printPath() {
            string r = "";
            for(int i = 0; i < _path.Count-1; i++) {
                r += string.Format("({0}, {1}) => ", _path[i].x, _path[i].z);
            }
            r += string.Format("({0}, {1})", _path[_path.Count-1].x, _path[_path.Count-1].z);
            return r;
        }
    }

    public GameObject floor;
    private const int TERRAIN_LAYER_MASK = 1 << 9;
    private List<GameObject> blockers;
    private List<Vector3> visibilityVerticies;
    // Implements an adjacency list
    private List<List<float>> visibilityGraph;
    private static BoardManager bm;
    
    void Awake() {
        if (bm != null) {
            Debug.LogWarning("Static bm object already found! Deleting and making another");
            GameObject.Destroy(bm);
        }
        bm = this;
        loadMap();
    }

    private void loadMap() {
        blockers = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++) {
            blockers.Add(transform.GetChild(i).gameObject);
        }
        initializeVisibilityGraph();
    }

    private void initializeVisibilityGraph() {
        bm.visibilityVerticies = new List<Vector3>();
        bm.visibilityGraph = new List<List<float>>();
        generateVisibilityEdges();
        removeUnreachableEdges();
        generateVisibilityAdjacencyList();
    }

    private void generateVisibilityEdges() {
        foreach(GameObject blocker in blockers) {
            // TODO: Handle Multiple Shapes
            // TODO: Handle Redundant/Unreachable Edges
            float xpos = blocker.transform.position.x;
            float zpos = blocker.transform.position.z;
            float xscale = blocker.transform.localScale.x;
            float zscale = blocker.transform.localScale.z;
            float size = 0.51f;
            // TODO: Accomodate Size? Instead of Static +/- 0.5f
            bm.visibilityVerticies.Add(new Vector3(xpos - (xscale / 2.0f) - size, 0.5f, zpos + (zscale / 2.0f) + size));
            bm.visibilityVerticies.Add(new Vector3(xpos + (xscale / 2.0f) + size, 0.5f, zpos + (zscale / 2.0f) + size));
            bm.visibilityVerticies.Add(new Vector3(xpos - (xscale / 2.0f) - size, 0.5f, zpos - (zscale / 2.0f) - size));
            bm.visibilityVerticies.Add(new Vector3(xpos + (xscale / 2.0f) + size, 0.5f, zpos - (zscale / 2.0f) - size));
        } 
    }

    private void removeUnreachableEdges() {
        // Case 1: Edge inside an object
        // Case 2: Spherecasting? Is this even an issue?
    }

    private void generateVisibilityAdjacencyList() {
        // TODO: Since adj. list is mirrored, cut down on half the iterations
        for (int i = 0; i < bm.visibilityVerticies.Count; i++) {
            bm.visibilityGraph.Add(new List<float>());
            for (int j = 0; j < bm.visibilityVerticies.Count; j++) {
                if (i == j) {
                    bm.visibilityGraph[i].Add(0);
                } else if (somethingBetweenTwoPoints(bm.visibilityVerticies[i], bm.visibilityVerticies[j])) {
                    bm.visibilityGraph[i].Add(Mathf.Infinity);
                } else {
                    bm.visibilityGraph[i].Add((bm.visibilityVerticies[i] - bm.visibilityVerticies[j]).magnitude);
                }
            }
        }
    }

    private bool somethingBetweenTwoPoints(Vector3 source, Vector3 destination) {
        Vector3 direction = (destination-source).normalized;
        float distance = (destination-source).magnitude;
        RaycastHit r;
        return Physics.SphereCast(source, 0.5f, direction, out r, distance, TERRAIN_LAYER_MASK);
    }

    // TODO: Make path subscribable(observer pattern) in case of updates
    // TODO: Dijkstra's!

    public static List<Vector3> getPath(Vector3 source, Vector3 dest) {
        int verticies = bm.visibilityVerticies.Count;
        Vector3Path[] pathsToI = new Vector3Path[verticies];
        DijkstraPriorityQueue shortestPaths = new DijkstraPriorityQueue(verticies);
        float shortestEndingPathLength = Mathf.Infinity;
        int shortestEndingPathIndex = -1;

        for(int i = 0; i < verticies; i++) {
            if (!bm.somethingBetweenTwoPoints(source, bm.visibilityVerticies[i])) {
                float distToI = (bm.visibilityVerticies[i] - source).magnitude;
                pathsToI[i] = new Vector3Path(bm.visibilityVerticies[i], distToI);
                shortestPaths.decreaseValue(i, distToI);
            }
        }

        for(int iterations = 0; iterations < verticies; iterations++) {
            int i = shortestPaths.popSmallest();
            if (i == Mathf.Infinity) {
                break;
            }
            for(int j = 0; j < verticies; j++) {
                float IToJDistance = bm.visibilityGraph[i][j];
                if(bm.connectionExists(IToJDistance)) {
                    float newPossibleDistance = pathsToI[i].getDistance() + IToJDistance;
                    bool waypointPreviouslyUnreachable = (pathsToI[j] == null);
                    if (waypointPreviouslyUnreachable || newPossibleDistance < pathsToI[j].getDistance()) {
                        pathsToI[j] = new Vector3Path(pathsToI[i]);
                        pathsToI[j].addWaypoint(bm.visibilityVerticies[j]);
                        shortestPaths.decreaseValue(j, newPossibleDistance);
                    }
                }
            }
        }

        for(int i = 0; i < verticies; i++) {
            if (!bm.somethingBetweenTwoPoints(bm.visibilityVerticies[i], dest)) {
                float newPossibleSmallestDistance = pathsToI[i].getDistance() + (dest - bm.visibilityVerticies[i]).magnitude;
                if (newPossibleSmallestDistance < shortestEndingPathLength) {
                    shortestEndingPathLength = newPossibleSmallestDistance;
                    shortestEndingPathIndex = i;
                }
            } 
        }
        return pathsToI[shortestEndingPathIndex].getPath();
    }

    private bool connectionExists(float i) {
        return i > 0.0f && i != Mathf.Infinity;
    }

    public static Vector3 move(float radius, Vector3 source, Vector3 direction, float distance) {
        RaycastHit hit;
        Vector3 endingPoint;
        // TODO: Switch to Spherecast. Beware: if the spherecast starts inside the wall even a smidge, it will ignore the wall
        if (Physics.Raycast(source, direction, out hit, distance, TERRAIN_LAYER_MASK)) {
            endingPoint = new Vector3(hit.point.x + (hit.normal.x * radius), hit.point.y, hit.point.z + (hit.normal.z * radius));
        } else {
            // TODO: Better direction vector
            endingPoint = new Vector3(source.x + (direction.x * distance), 0.5f, source.z + (direction.z * distance));
        }
        if (bm.isOverEdge(endingPoint, radius)) {
            endingPoint = source;
        }
        return endingPoint;
    }

    public static Vector3 getRandomLocation(float height) {
        float randX = Random.Range(bm.floor.transform.position.x - (bm.floor.transform.localScale.x * 5), bm.floor.transform.position.x + (bm.floor.transform.localScale.x * 5));
        float randZ = Random.Range(bm.floor.transform.position.z - (bm.floor.transform.localScale.z * 5), bm.floor.transform.position.z + (bm.floor.transform.localScale.z * 5));
        return new Vector3(randX, height, randZ);
    }

    private bool isOverEdge(Vector3 location, float radius) {
        return (location.x - radius < bm.floor.transform.position.x - (bm.floor.transform.localScale.x * 5)) ||
               (location.x + radius > bm.floor.transform.position.x + (bm.floor.transform.localScale.x * 5)) ||
               (location.z - radius < bm.floor.transform.position.z - (bm.floor.transform.localScale.z * 5)) ||
               (location.z + radius > bm.floor.transform.position.z + (bm.floor.transform.localScale.z * 5));
    }
}
