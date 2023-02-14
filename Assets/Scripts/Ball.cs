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

    void Start()
    {
        //ballLocation = transform.position;
        rb = GetComponent<Rigidbody>();
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
        Destroy(arrow);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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
                rb.isKinematic = true;
                activeBall = false;
                enteredCollision = true;

                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 collisionForward = collision.gameObject.transform.TransformDirection(Vector3.forward);
                var result = Vector3.Dot(forward, collisionForward);

                if (result < -0.5f && result > -1.5f)
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
        yield return new WaitForSeconds(0.3f);

        while (transform.position != targetPos)
        {
            var curPosition = transform.position;
            transform.position = Vector3.MoveTowards(curPosition, targetPos, 0.0045f);
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);
        enteredCollision = false;
        activeBall = false;
        rb.isKinematic = true;
        SpawnRandomArrow();
    }
}
