using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Gameplay;
using Pathfinding;

namespace Objects {

public class Enemy : MonoBehaviour {

  const float minPathUpdateTime = .2f;
  const float pathUpdateMoveThreshold = .5f;

  [SerializeField]
  float speed = 20;
  [SerializeField]
  float turnSpeed = 3;
  [SerializeField]
  float turnDst = 5;
  [SerializeField]
  float stoppingDst = 10;
  [SerializeField]
  GameObject healthBar;
  [SerializeField]
  GameObject pillPrefab;
  [SerializeField]
  AudioClip slashAudio;
  [SerializeField]
  AudioClip playerLoseHealthAudio;
  [SerializeField]
  float attackDmg = 0.17f;

  internal float health = 1.0f;

  Transform target;
  Animator animation_controller;
  Path path;
  GameObject playerObj;
  float timeSinceLastAttack;
  DungeonGenerator generator;
  AudioSource audioSource;
  bool attacking = false;
  bool hitPlayer = false;

  void Start() {
    playerObj = GameObject.Find("Player");
    target = playerObj.transform;
    animation_controller = GetComponent<Animator>();
    animation_controller.SetInteger("state", 0);
    timeSinceLastAttack = Time.time;
    generator = GameObject.Find("Game").GetComponent<DungeonGenerator>();
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(UpdatePath());
  }

  void Update() {
    health = Mathf.Max(0f, health);
    if (health <= 0f) {
      GameObject pill = Instantiate(
          pillPrefab,
          new Vector3(transform.position.x, 1f, transform.position.z),
          Quaternion.identity, GameObject.Find("EnemyParent").transform);
      pill.name = "Pill";
      generator.enemyList.RemoveAll(e => e.name == transform.gameObject.name);
      Destroy(transform.gameObject);
    }
    playerObj = GameObject.Find("Player");
    if (playerObj != null) {
      target = playerObj.transform;
      float dist =
          (transform.position - playerObj.transform.position).magnitude;
      if (dist < 3f) {
        Vector3 directionToPlayer =
            playerObj.GetComponent<CapsuleCollider>().bounds.center -
            GetComponent<Collider>().bounds.center;
        directionToPlayer.Normalize();
        float angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(directionToPlayer.x,
                                                          directionToPlayer.z);
        transform.eulerAngles = new Vector3(0.0f, angleToRotate, 0.0f);
        animation_controller.SetInteger("state", 2);
      } else {
        animation_controller.SetInteger("state", 0);
      }
    }
    healthBar.GetComponent<Slider>().value = health;
  }
  // State: 0 Idle, 1 WalkForward, 2 Attack

  public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
    if (pathSuccessful) {
      path = new Path(waypoints, transform.position, turnDst, stoppingDst);

      StopCoroutine("FollowPath");
      StartCoroutine("FollowPath");
    }
  }

  public void OnTriggerStay(Collider other) {
    if (attacking && other.gameObject.name == "Player") {
      hitPlayer = true;
      attacking = false;
    }
  }

  public void PlaySlashAudio() {
    attacking = true;
    audioSource.PlayOneShot(slashAudio, 1f);
  }

  public void EndSlashMotion() {
    if (hitPlayer) {
      Game game = GameObject.Find("Game").GetComponent<Game>();
      game.playerHealth -= attackDmg;
      audioSource.PlayOneShot(playerLoseHealthAudio, 1f);
      hitPlayer = false;
    }
    attacking = false;
  }

  IEnumerator UpdatePath() {

    if (Time.timeSinceLevelLoad < .3f) {
      yield return new WaitForSeconds(.3f);
    }
    PathRequestManager.RequestPath(
        new PathRequest(transform.position, target.position, OnPathFound));

    float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
    Vector3 targetPosOld = target.position;

    while (true) {
      yield return new WaitForSeconds(minPathUpdateTime);
      if (playerObj != null) {
        if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
          PathRequestManager.RequestPath(new PathRequest(
              transform.position, target.position, OnPathFound));
          targetPosOld = target.position;
        }
      }
    }
  }

  IEnumerator FollowPath() {

    bool followingPath = true;
    int pathIndex = 0;
    transform.LookAt(path.lookPoints[0]);

    float speedPercent = 1;

    while (followingPath) {
      Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
      while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
        if (pathIndex == path.finishLineIndex) {
          followingPath = false;
          break;
        } else {
          pathIndex++;
        }
      }

      if (followingPath) {

        if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
          speedPercent = Mathf.Clamp01(
              path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(
                  pos2D) /
              stoppingDst);
          if (speedPercent < 0.01f) {
            followingPath = false;
          }
        }

        Quaternion targetRotation = Quaternion.LookRotation(
            path.lookPoints[pathIndex] - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
                                             Time.deltaTime * turnSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed *
                                speedPercent,
                            Space.Self);
        animation_controller.SetInteger("state", 1);
      }

      yield return null;
    }
  }
}
}
