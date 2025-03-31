using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    Rigidbody rb;

    public Slider healthBar;
    public Slider staminaBar;

    public AnimationCurve jumpCurve;

    float curvePosition;
    float stamina;
    bool speedChanged = false;
    bool running = false;
    bool grounded;
    bool canPlunge;
    bool plunging;

    public float health;
    public bool canChange = true;

    private Vector3 lastPos;

    [Header("Jump & Gravity Settings")]
    public float fallMultiplier = 2.5f; 
    public float plungeSpeed = 20f; 
    public float plungeDamage = 30f; 


    private void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();

        lastPos = new Vector3(transform.position.x, 0, transform.position.z);

        SkillStats.Instance.Health();
        health = SkillStats.Instance.maxHealth;

        stamina = SkillStats.Instance.maxStamina;
    }

    private void Update()
    {
        healthBar.value = health / SkillStats.Instance.maxHealth;
        SkillStats.Instance.Health();
        DistanceToGround();

        ApplyFallingGravity();

        #region Movement
        StartCoroutine(StaminaControl());
        staminaBar.value = stamina / SkillStats.Instance.maxStamina;

        if (!plunging)
        {
            float inputX = Input.GetAxisRaw("Horizontal") * SkillStats.Instance.speed;
            float inputY = Input.GetAxisRaw("Vertical") * SkillStats.Instance.speed;

            rb.linearVelocity = new Vector3(inputX, rb.linearVelocity.y, inputY);
        }

        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        float distanceThisFrame = Vector3.Distance(lastPos, currentPosition);

        if (distanceThisFrame > 0.1f)
        {
            SkillStats.Instance.distanceWalked += distanceThisFrame;
            lastPos = currentPosition;
            SkillStats.Instance.Walking();
        }

        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            curvePosition = 0;
            rb.AddForce(new Vector2(rb.linearVelocity.x, SkillStats.Instance.jumpForce * 10));
        }

        if (!grounded)
        {
            curvePosition += Time.deltaTime;
        }

        // Plunge Attack Trigger
        if (Input.GetMouseButtonDown(0))
        {
           // StartCoroutine(BaseAttack(Attack(transform.position, 10, 1.5f, 5f, fixthis)));
            if (canPlunge)
            {
                StartPlunge();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SkillStats.Instance.speed += 5;
            canChange = false;
            running = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (speedChanged)
            {
                canChange = true;
                running = false;
            }
            else if (!speedChanged)
            {
                SkillStats.Instance.speed -= 5;
                canChange = true;
                running = false;
                speedChanged = false;
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (!grounded)
        {
            rb.AddForce(Vector3.up * jumpCurve.Evaluate(curvePosition)*100);
        }
    }
    void ApplyFallingGravity()
    {
        if (rb.linearVelocity.y < 0 && !plunging) 
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    void StartPlunge()
    {
        plunging = true;
        rb.linearVelocity = Vector3.down * plungeSpeed;
    }

    void DistanceToGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = Vector3.down;
        float maxDistance = 10f;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance))
        {
            float distanceToGround = hit.distance;
            grounded = distanceToGround <= 1.5;
            canPlunge = distanceToGround >= 4;

            if (plunging && grounded)
            {
                OnPlungeImpact(hit.point);
            }
        }
        else
        {
            canPlunge = false;
            grounded = false;
        }
    }

    void OnPlungeImpact(Vector3 impactPoint)
    {
        plunging = false;

        // Damage enemies in an area (Example: Add sphere damage)
        Collider[] hitEnemies = Physics.OverlapSphere(impactPoint, 3f);
        foreach (Collider enemy in hitEnemies)
        {
            //do damage
        }
        // Stop downward velocity to avoid clipping into the ground
        rb.linearVelocity = Vector3.zero;
    }

    #region Health System
    public void TakeDamage(int amount)
    {
        health -= amount;
        SkillStats.Instance.Health();
        if (health < 1)
        {
            // Die logic
        }
        healthBar.value = health / SkillStats.Instance.maxHealth;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health >= SkillStats.Instance.maxHealth)
        {
            health = SkillStats.Instance.maxHealth;
        }
        healthBar.value = health / SkillStats.Instance.maxHealth;
    }
    #endregion

    IEnumerator StaminaControl()
    {
        if (running)
        {
            stamina -= Time.deltaTime;
            if (stamina <= 0)
            {
                running = false;
                SkillStats.Instance.speed -= 5;
                speedChanged = true;
            }
        }
        else if (!running && stamina < SkillStats.Instance.maxStamina)
        {
            yield return new WaitForSeconds(0.5f);
            stamina += Time.deltaTime;
        }
        staminaBar.value = stamina / SkillStats.Instance.maxStamina;
    }
}
