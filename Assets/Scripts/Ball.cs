using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [SerializeField] private List<GameObject> arrows;
    [SerializeField] private List<Material> materials;
    [SerializeField] private float ballSpeed = 6;
    [SerializeField] private ParticleSystem deathParticles;

    private GameObject arrow;
    private Rigidbody rb;
    private bool enteredCollision = false;
    private bool activeBall = false;
    private Vector3 faceDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
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
        enteredCollision = true;
        activeBall = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = (transform.rotation * Vector3.forward).normalized * ballSpeed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Ball otherBall = null;

        if (collider.gameObject.tag == "Ball" && activeBall)
        {
            otherBall = collider.gameObject.GetComponent<Ball>();

            if (!otherBall.enteredCollision)
            {
                activeBall = false;
                otherBall.enteredCollision = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;

                float dot = Vector3.Dot(faceDir, otherBall.transform.forward);

                if (dot < -0.75f && dot > -1.25f)
                {
                    Destroy(gameObject);
                    PlayDeathAnimation();
                    otherBall.enteredCollision = false;
                }
                else
                {
                    otherBall.rb.velocity = Vector3.zero;
                    otherBall.rb.angularVelocity = Vector3.zero;
                    otherBall.Move();
                    StartCoroutine(SetNewPositionAfterCollision(otherBall.transform.position));
                }
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Playground")
        {
            PlayDeathAnimation();
        }
    }

    private void PlayDeathAnimation()
    {
        gameObject.SetActive(false);
        deathParticles = Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator SetNewPositionAfterCollision(Vector3 targetPos)
    {
        yield return new WaitForSeconds(0.1f);

        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, ballSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        ResetBall();
    }

    private void ResetBall()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        enteredCollision = false;
        activeBall = false;
        SpawnRandomArrow();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
