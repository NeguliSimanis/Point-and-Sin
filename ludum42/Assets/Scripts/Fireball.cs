using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    private float fireBallDamage;
    private float fireBallDuration = 0.54f;
    private float fireBallDeathTime;

    #region fireball MOVEMENT
    private float fireBallMoveSpeed = 2.5f;
    private bool allowAcceleration = true;
    private float moveAcceleration = 0.9f;
    private Vector2 fireballDirNormalized;
    private Vector2 targetPosition;

    private float explodeDistance = 0.35f; // if fireball is this close to target it will explode
    #endregion

    #region fireball ROTATION
    private Vector2 fireballStartLocation;
    #endregion

    #region CURRENT STATE
    bool fireBallStarted = false;
    bool flyingRight;
    bool isExploding = false;
    #endregion

    #region EXPLOSION
    float explosionDuration;
    [SerializeField] AnimationClip explosionAnimation;
    [SerializeField] Animator animator;
    #endregion

    #region AUDIO
    private float fireBallSFXVolume = 1.2f;
    private float fireBallExplodeSFXVolume = 1.5f;

    [SerializeField] AudioSource audioControl;
    [SerializeField] AudioClip fireballSFX;
    [SerializeField] AudioClip explosionSFX;
    #endregion

    private void Start()
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
    }

    public void StartFireball(bool isFlyingRight, Vector2 target)
    {
        // play sfx
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
        audioControl.PlayOneShot(fireballSFX, fireBallSFXVolume);

        // set default values
        fireBallStarted = true;
        fireBallDeathTime = fireBallDuration + Time.time;
        fireballStartLocation = transform.position;
        explosionDuration = explosionAnimation.length;

        // set variables that were passed to function
        flyingRight = isFlyingRight;
        GetDirNormalized(target);
        targetPosition = new Vector2(target.x, target.y);

        AdjustFireballRotation();

    }

    void GetDirNormalized(Vector2 sourceVector)
    {
        fireballDirNormalized = new Vector2(sourceVector.x - transform.position.x, sourceVector.y - transform.position.y);
        fireballDirNormalized = fireballDirNormalized.normalized;
    }

    private void Update()
    {
        if (Time.time > fireBallDuration + fireBallDeathTime)
            Explode();
    }

    private void FixedUpdate()
    {
        if (!fireBallStarted)
        {
            return;
        }
        //CheckIfTargetPositionReached();  
        MoveFireball();
    }

    private void Explode()
    {
        if (isExploding)
            return;
        isExploding = true;
        animator.SetTrigger("explode");
        audioControl.PlayOneShot(explosionSFX, fireBallExplodeSFXVolume);
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(PlayerData.current.fireballDamage, DamageSource.PlayerFireball);
            Explode();
        }
    }

    private void MoveFireball()
    {
        if (isExploding)
            return;

        if (allowAcceleration)
            IncreaseMoveSpeed();

        // move fireball
        transform.position = new Vector2(transform.position.x, transform.position.y)
            + fireballDirNormalized * fireBallMoveSpeed * Time.deltaTime;
    }

    private void IncreaseMoveSpeed()
    {
        fireBallMoveSpeed =  fireBallMoveSpeed + moveAcceleration * Time.deltaTime;
    }

    /// <summary>
    /// adjust the rotation of the fireball object to the trajectory of its flight
    /// </summary>
    void AdjustFireballRotation()
    {
        if (isExploding)
            return;

        /*            
         *                                      target location
         *                                   /| 
         *                                  / |
         *                   hypotenuse    /  |   cathetusA                 
         *                                /   |                
         *                               /    |                    
         *    fireball start location   /_____| 
         *           
         *                                cathetusB
         */
        // get triangle sides
        float cathetusA = targetPosition.y - fireballStartLocation.y;
        float cathetusB = targetPosition.x - fireballStartLocation.x;

        float rotationAngle = 90f;
        
        // calculate arctanget to find out the angle at which to rotate the fireball
        if (cathetusB != 0)
        {
            float arctangent = Mathf.Atan(cathetusA / cathetusB);
            //Debug.Log("x " + cathetusB);
            ///Debug.Log("y " + cathetusA);
            //Debug.Log("arctangent " + Mathf.Rad2Deg * arctangent);

            if (cathetusB > 0)
                rotationAngle = Mathf.Rad2Deg * arctangent;
            else
                rotationAngle = (Mathf.Rad2Deg * arctangent)*(-1);
        }
        // at 90 or 270 degrees arctangent is undefined (infinity)
        else
        {
            if (cathetusA > 0)
            {
                rotationAngle = 90;
            }
            else
            {
                rotationAngle = 270;
            }
        }
        
        // rotate
        transform.Rotate(new Vector3(0, 0, rotationAngle));
    }

    /// <summary>
    ///  check if fireball reached target (explode if yes)
    /// </summary>
    public void CheckIfTargetPositionReached()
    {
        if (isExploding)
            return;

        //Debug.Log("target: " + targetPosition + ". Current position: " + transform.position);
        //Debug.Log("distance to target: " + Vector2.Distance(targetPosition, transform.position));
        if (Vector2.Distance(targetPosition, transform.position) <= explodeDistance)
        {
           // Debug.Log("TARGET REACHED BABY " + Time.time);
            Explode();
        }
    }


}
