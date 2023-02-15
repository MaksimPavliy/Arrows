using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [SerializeField] private List<GameObject> arrows;
    [SerializeField] private List<Material> materials;
    [SerializeField] private float ballSpeed = 4;

    public Vector3 ballLocation;
    private GameObject arrow;
    private Rigidbody rb;
    private bool enteredCollision = false;
    private bool activeBall = false;
    private Vector3 faceDir;
    private float ballStartMass;

    void Start()
    {
        ballLocation = transform.position;
        rb = GetComponent<Rigidbody>();
        ballStartMass = rb.mass;
        SpawnRandomArrow();
    }

    private void SpawnRandomArrow()
    {
        var random = Random.Range(0, arrows.Count);

        GetComponent<MeshRenderer>().material = materials[random];
        arrow = Instantiate(arrows[random], transform.position + new Vector3(0, 2), arrows[random].transform.rotation);
        transform.rotation *= Quaternion.Euler(0, arrow.transform.localEulerAngles.y + 90, 0);
        arrow.transform.SetParent(transform, true);
    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (GameManager.instance.isPlayerTurn)
        {
            Move();
        }
    }

    public void Move()
    {
        faceDir = transform.forward;
        Destroy(arrow);
        rb.mass = 0.01f;
        rb.isKinematic = false;
        activeBall = true;
        rb.velocity = (transform.rotation * Vector3.forward).normalized * ballSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ball otherBall = null;

        if (collision.gameObject.tag == "Ball" && activeBall)
        {
            otherBall = collision.gameObject.GetComponent<Ball>();

            if (!otherBall.enteredCollision && !enteredCollision)
            {
                activeBall = false;
                enteredCollision = true;

                float dot = Vector3.Dot(faceDir, otherBall.transform.forward);

                if (dot < -0.75f && dot > -1.25f)
                {
                    otherBall.rb.isKinematic = true;
                    Destroy(gameObject);
                }
                else
                {
                    otherBall.Move();
                    StartCoroutine(SetNewPositionAfterCollision(otherBall.ballLocation));
                }
            }
        }
    }

    private IEnumerator SetNewPositionAfterCollision(Vector3 targetPos)
    {
        rb.isKinematic = false;

        while (Vector3.Distance(transform.position, targetPos) > 0.2f)
        {
            rb.velocity = faceDir * ballSpeed;
            yield return new WaitForEndOfFrame();
        }

        rb.MovePosition(targetPos);
        ResetBall();
    }

    private void ResetBall()
    {
        rb.isKinematic = false;
        enteredCollision = false;
        activeBall = false;
        SpawnRandomArrow();
    }
}
