using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

namespace Gameplay {

class Cell {
  public bool visited = false;
  public bool[] status = new bool[4];
}

public class DungeonGenerator : MonoBehaviour {

  public Vector2Int size;
  [SerializeField]
  GameObject[] roomsPrefab;
  [SerializeField]
  GameObject corridorPrefab;
  [SerializeField]
  GameObject finishPrefab;
  [SerializeField]
  GameObject fpsPlayerPrefab;
  [SerializeField]
  GameObject enemyPrefab;
  [SerializeField]
  int numEnemy = 10;

  GameObject playerObj;
  Vector2 offset;
  Vector2 corridorOffset;
  Cell[,] board;
  int[] startPos;
  int[] endPos;
  Vector2Int playerPos;
  Vector3 worldBottomLeft;
  GameObject world;
  GameObject enemyParent;
  internal List<GameObject> enemyList = new List<GameObject>();

  internal void GenerateWorld() {
    startPos = new int[] { Random.Range(0, size.x), Random.Range(0, size.y) };
    offset = new Vector2Int(12, 12);
    corridorOffset = new Vector2Int(2, 6);

    worldBottomLeft =
        transform.position - Vector3.right * size.x * offset.x / 2 -
        Vector3.forward * size.y * offset.y / 2 + new Vector3(2f, 0f, 2f);

    MazeGenerator();
    DrawDungeon();
    GameObject.Find("A*").GetComponent<PathfindingGrid>().CreateGrid();
    StartCoroutine("SpawnEnemy");
  }

  // Update is called once per frame
  void Update() {
    if (playerObj != null) {
      Vector3 worldPosition = playerObj.transform.position - worldBottomLeft;
      int x = Mathf.FloorToInt(worldPosition.x / offset.x);
      int y = Mathf.FloorToInt(worldPosition.z / offset.y);
      playerPos = new Vector2Int(x, y);
    }
  }

  // Get all neighbors without checking for path
  List<int[]> GetNeighbors(int[] cell) {
    List<int[]> neighbors = new List<int[]>();

    // check up neighbor
    if (cell[1] + 1 < size.y && !board[cell[0], cell[1] + 1].visited) {
      neighbors.Add(new int[] { cell[0], cell[1] + 1 });
    }

    // check down neighbor
    if (cell[1] - 1 >= 0 && !board[cell[0], cell[1] - 1].visited) {
      neighbors.Add(new int[] { cell[0], cell[1] - 1 });
    }

    // check right neighbor
    if (cell[0] + 1 < size.x && !board[cell[0] + 1, cell[1]].visited) {
      neighbors.Add(new int[] { cell[0] + 1, cell[1] });
    }

    // check left neighbor
    if (cell[0] - 1 >= 0 && !board[cell[0] - 1, cell[1]].visited) {
      neighbors.Add(new int[] { cell[0] - 1, cell[1] });
    }

    return neighbors;
  }

  // Get neighbors that have a path from current cell
  List<int[]> CheckNeighbors(int[] cell) {
    List<int[]> neighbors = new List<int[]>();

    // check up neighbor
    if (board[cell[0], cell[1]].status[0]) {
      neighbors.Add(new int[] { cell[0], cell[1] + 1 });
    }

    // check down neighbor
    if (board[cell[0], cell[1]].status[2]) {
      neighbors.Add(new int[] { cell[0], cell[1] - 1 });
    }

    // check right neighbor
    if (board[cell[0], cell[1]].status[1]) {
      neighbors.Add(new int[] { cell[0] + 1, cell[1] });
    }

    // check left neighbor
    if (board[cell[0], cell[1]].status[3]) {
      neighbors.Add(new int[] { cell[0] - 1, cell[1] });
    }

    return neighbors;
  }

  int[] GetEndPos() {
    Stack<int[]> path = new Stack<int[]>();
    bool[,] visited = new bool[size.x, size.y];
    int[,] distance = new int[size.x, size.y];

    int[] cur, res = startPos;
    int maxDist = 0;
    visited[startPos[0], startPos[1]] = true;
    distance[startPos[0], startPos[1]] = 0;
    path.Push(startPos);
    while (path.Count != 0) {
      cur = path.Pop();
      visited[cur[0], cur[1]] = true;
      if (distance[cur[0], cur[1]] >= maxDist) {
        res = cur;
      }
      List<int[]> neighbors = CheckNeighbors(cur);
      foreach (int[] cell in neighbors) {
        if (!visited[cell[0], cell[1]]) {
          path.Push(cell);
          distance[cell[0], cell[1]] = distance[cur[0], cur[1]] + 1;
        }
      }
    }
    return res;
  }

  IEnumerator SpawnEnemy() {
    while (true) {
      if (GameObject.Find("Player") != null && enemyList.Count < numEnemy) {
        int x, y;
        while (true) {
          x = Random.Range(0, size.x);
          y = Random.Range(0, size.y);

          float dist = (playerPos - new Vector2Int(x, y)).magnitude;
          if (dist != 0f) {
            break;
          }
        }
        GameObject enemy =
            Instantiate(enemyPrefab,
                        worldBottomLeft + Vector3.right * (x * offset.x + 2f) +
                            Vector3.forward * (y * offset.y + 2f),
                        Quaternion.identity, enemyParent.transform);
        enemy.name = "Enemy" + enemyList.Count;
        enemyList.Add(enemy);
      }
      yield return new WaitForSeconds(1f);
    }
  }

  internal void CleanGameWorld() {
    StopCoroutine("SpawnEnemy");
    Destroy(GameObject.Find("World"));
    playerObj = null;
  }

  internal void ResetWorld() {
    StopCoroutine("SpawnEnemy");
    DestroyAllEnemy();
    Destroy(playerObj);
    enemyParent = new GameObject("EnemyParent");
    enemyParent.transform.parent = world.transform;
    playerObj = GameObject.Instantiate(
        fpsPlayerPrefab,
        worldBottomLeft + Vector3.right * (startPos[0] * offset.x + 1f) +
            Vector3.forward * (startPos[1] * offset.y + 1f) + Vector3.up * 1,
        Quaternion.identity, world.transform);
    playerObj.name = "Player";
    StartCoroutine("SpawnEnemy");
  }

  internal void DestroyAllEnemy() {
    Destroy(enemyParent);
    enemyParent = null;
    enemyList.Clear();
  }

  void DrawDungeon() {
    endPos = GetEndPos();
    world = new GameObject("World");
    enemyParent = new GameObject("EnemyParent");
    enemyParent.transform.parent = world.transform;

    playerObj = GameObject.Instantiate(
        fpsPlayerPrefab,
        worldBottomLeft + Vector3.right * (startPos[0] * offset.x + 1f) +
            Vector3.forward * (startPos[1] * offset.y + 1f) + Vector3.up * 1,
        Quaternion.identity, world.transform);
    playerObj.name = "Player";

    for (int i = 0; i < size.x; i++)
      for (int j = 0; j < size.y; j++) {
        Cell cur = board[i, j];
        if (cur.visited) {
          if (cur.status[0] && cur.status[2] && !cur.status[1] &&
              !cur.status[3]) {
            // Corridor vertically
            GameObject corridor = Instantiate(
                corridorPrefab,
                worldBottomLeft +
                    Vector3.right * (i * offset.x + corridorOffset.x) +
                    Vector3.forward * (j * offset.y),
                Quaternion.identity, world.transform);
            corridor.name = "Corridor";
          } else if (!cur.status[0] && !cur.status[2] && cur.status[1] &&
                     cur.status[3]) {
            // Corridor horizontally
            GameObject corridor = Instantiate(
                corridorPrefab,
                worldBottomLeft + Vector3.right * (i * offset.x) +
                    Vector3.forward * (j * offset.y + corridorOffset.y),
                Quaternion.Euler(0, 90, 0), world.transform);
            corridor.name = "Corridor";
          } else if (endPos.SequenceEqual(new int[] { i, j })) {
            GameObject finish =
                Instantiate(finishPrefab,
                            worldBottomLeft + Vector3.right * (i * offset.x) +
                                Vector3.forward * (j * offset.y),
                            Quaternion.identity, world.transform);
            finish.GetComponent<Objects.RoomBehavior>().UpdateRoom(cur.status);
            finish.name = "Finish";
          } else {
            GameObject room =
                Instantiate(roomsPrefab[Random.Range(0, roomsPrefab.Length)],
                            worldBottomLeft + Vector3.right * (i * offset.x) +
                                Vector3.forward * (j * offset.y),
                            Quaternion.identity, world.transform);
            room.GetComponent<Objects.RoomBehavior>().UpdateRoom(cur.status);
            room.name = "Room";
          }
        }
      }
  }

  void MazeGenerator() {
    board = new Cell[size.x, size.y];
    for (int i = 0; i < size.x; i++)
      for (int j = 0; j < size.y; j++) {
        board[i, j] = new Cell();
      }

    int[] cur = startPos;
    Stack<int[]> path = new Stack<int[]>();
    int k = 0;
    while (k++ <= 100000) {
      board[cur[0], cur[1]].visited = true;

      // Linear dungeon level design
      // if (cur.SequenceEqual(endPos))
      //   break;

      List<int[]> neighbors = GetNeighbors(cur);
      if (neighbors.Count == 0) {
        if (path.Count == 0) {
          break;
        } else {
          cur = path.Pop();
        }
      } else {
        path.Push(cur);
        int[] cell = neighbors[Random.Range(0, neighbors.Count)];
        if (cell[0] == cur[0]) {
          // Up or down
          if (cell[1] == cur[1] + 1) {
            board[cur[0], cur[1]].status[0] = true;
            cur = cell;
            board[cur[0], cur[1]].status[2] = true;
          } else {
            board[cur[0], cur[1]].status[2] = true;
            cur = cell;
            board[cur[0], cur[1]].status[0] = true;
          }
        } else {
          // Right or Left
          if (cell[0] == cur[0] + 1) {
            board[cur[0], cur[1]].status[1] = true;
            cur = cell;
            board[cur[0], cur[1]].status[3] = true;
          } else {
            board[cur[0], cur[1]].status[3] = true;
            cur = cell;
            board[cur[0], cur[1]].status[1] = true;
          }
        }
      }
    }
  }
}
}
