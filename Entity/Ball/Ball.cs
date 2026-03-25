using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static Dictionary<Transform, Ball> ballMap = new();

    public static Vector3 scale = new Vector3(0.5f, 0.5f, 1f);

    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private MaterialPropertyBlock _mpb;

    [Header("Ball Properties")]
    public bool isMerging = false;
    public SpriteRenderer bodySprite;
    public BallEyes ballEyes;
    public Sprite imageLive;
    public Sprite imageLose;
    public eBallType type = eBallType.None;
    public eBallType nextType = eBallType.None;
    public SpriteRenderer aim;
    public Vector3 aimScale;
    public Collider2D ballCollider;
    public float ballUnit = 1f;
    public bool isAimed = false;
    public Rigidbody2D rb;
    public bool isFalling = false;
    public LightFX lightFX;

    [Header("Trail Settings")]
    public TrailRenderer trail;
    [SerializeField] private float trailDuration = 0.3f;
    [Range(0f, 1f)]
    [SerializeField] private float widthScaleFactor = 0.3f;
    private Coroutine trailRoutine;
    private float baseTrailWidth;
    private WaitForSeconds _trailWait;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();

        if (trail != null)
        {
            baseTrailWidth = trail.widthMultiplier;
            _trailWait = new WaitForSeconds(trailDuration);
        }

        if (aim != null)
            aimScale = aim.transform.localScale;
    }

    private void OnEnable()
    {
        ballMap[transform] = this;

        //rb.gravityScale = 0f;
        //rb.linearVelocity = Vector2.down * 4f;

        materialReset();
        isFalling = false;
        trail.emitting = false;
        bodySprite.sprite = imageLive;
        ballEyes.gameObject.SetActive(true);
        if (aim != null) aim.gameObject.SetActive(false);

        rb.constraints = RigidbodyConstraints2D.None;
        rb.bodyType = RigidbodyType2D.Dynamic;
        isMerging = false;
    }

    private void OnDisable()
    {
        ballMap.Remove(transform);

        if (isAimed)
        {
            MyAnimation.StopAimAnimation(this);
            isAimed = false;
        }
    }
    // ------------------------- Trail Methods -------------------------  
    public void ActivateTrail(float duration = -1f)
    {
        if (bodySprite != null)
            trail.widthMultiplier = bodySprite.bounds.size.x * widthScaleFactor;

        if (trailRoutine != null) StopCoroutine(trailRoutine);

        trail.Clear();
        trail.enabled = true;
        trail.emitting = true;

        WaitForSeconds wait = duration > 0 ? new WaitForSeconds(duration) : _trailWait;
        trailRoutine = StartCoroutine(DisableTrailAfterSeconds(wait));
    }
    private IEnumerator DisableTrailAfterSeconds(WaitForSeconds wait)
    {
        yield return wait;
        DisableTrail();
    }
    public void DisableTrail()
    {
        if (trailRoutine != null)
        {
            StopCoroutine(trailRoutine);
            trailRoutine = null;
        }

        trail.emitting = false;
        trail.Clear();
        trail.enabled = false;
    }

    // ------------------------- Aim Methods -------------------------  
    public void OnHoverEnter() => MyAnimation.AimAnimation(this);
    public void OnHoverExit() => MyAnimation.StopAimAnimation(this);

    // ------------------------- Helper Methods -------------------------  
    public bool IsSameType(Ball other) => other.type == type;
    public bool IsMaxType() => this.type == eBallType.Ball11;

    public void ChangeState(bool isDead)
    {
        if (isDead)
        {
            bodySprite.sprite = imageLose;
            ballEyes.gameObject.SetActive(false);
        }
        else
        {
            bodySprite.sprite = imageLive;
            ballEyes.gameObject.SetActive(true);
        }
    }

    public void SetIntensity(float value)
    {
        _mpb.SetFloat(IntensityID, value);
        bodySprite.SetPropertyBlock(_mpb);
    }

    private void materialReset() => SetIntensity(0f);

}
