using System.Collections;
using DG.Tweening;
using Engine;
using Main;
using Template.CharSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagneticObject : MonoBehaviour,ILevelCompleted, ILevelFailed
{
    public bool CanBeMoved=true;
    public bool CanBeDraged=true;
    private float _speedRotation=2;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _gravityHolder;
    [SerializeField] private Damage _damage;
    private float maxOffsetx =3;
    private float maxOffsety =2;
    private float _pushForce=30f;
    private float _angel =60f;
    private Tween _moveTween;
    public bool InList;
    private float _radius;
    private bool _rotating;
    private bool _moving;
    [SerializeField]private Collider _collider;
    float t = 0f;
    bool nonInPos = false;
    float t2 = 0;
    private float y;
    private float offset;
    Vector3 m_EulerAngleVelocity = new Vector3(0, 100, 0);
    private float _rotationSpeedCorrecor;
    [SerializeField] private ParticleSystem _holdPartocals;
    public void OnEnable()
    {
        
        m_EulerAngleVelocity =
            new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        _rotationSpeedCorrecor = Random.Range(0.5f, 2f);
        _rigidbody = GetComponent<Rigidbody>();
        LevelStatueFailed.Subscribe(this);
        LevelStatueCompleted.Subscribe(this);
        _damage.value = 10;
    }
    public void OnDisable()
    {
        LevelStatueFailed.Unsubscribe(this); 
        LevelStatueCompleted.Unsubscribe(this);
    }

    public void StartRotate(Transform gravityHolderTransgrom,float radius)
    {
        _radius = radius;
        _gravityHolder = gravityHolderTransgrom;
        _rigidbody.isKinematic = true;
        Rotate();
    }
   

    public void Rotating(Transform gravityHolderTransgrom,float radius)
    {
        CanBeMoved = false;
        CanBeDraged = false;
        _collider.enabled = false;
        _gravityHolder = gravityHolderTransgrom;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _radius = radius;
        Rotate();
        if(_holdPartocals!=null)
            _holdPartocals.Play();
    }
    

    public void Stop()
    {
        _moving = false;
        _rotating = false;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        StartCoroutine(GrabDelay());
        CanBeMoved = true;
        _collider.enabled = true;
        if(_holdPartocals!=null)
            _holdPartocals.Stop();
    }
    public void StopMove(float radius)
    {
        _radius = radius;
        Move();
        StartRotate(_gravityHolder,_radius);
        CanBeDraged = false;

    }

    private void FixedUpdate()
    {

            if (CanBeDraged&&_moving)
            {
                t += 0.04f;
                Vector3 direction = (_gravityHolder.position -transform.position ).normalized;

                _rigidbody.MovePosition(transform.position+(direction*(_speedRotation*10+t)*Time.fixedDeltaTime));
                if(t>=4)
                    Stop();
            }
            
            if (!CanBeMoved && _rotating)
            {
                t2 += _speedRotation*_rotationSpeedCorrecor * Time.fixedDeltaTime;
                float x = Mathf.Cos(t2) * (_radius + offset);
                float z = Mathf.Sin(t2) * (_radius + offset);
                Vector3 pos = new Vector3(x, y, z);
                Vector3 newPos = ((pos + _gravityHolder.position) - transform.position).normalized;
                if (Vector3.Distance(transform.position, (pos + _gravityHolder.position)) > 0.3f && !nonInPos)
                {
                    _rigidbody.MovePosition(
                        transform.position + (newPos * ((_speedRotation * 8) * Time.fixedDeltaTime)));

                }
                else
                {
                    nonInPos = true;
                    _rigidbody.MovePosition(pos + _gravityHolder.position);
                }

                Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity *3 * Time.fixedDeltaTime);
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
                

            }
    }

    public void PushInConus(Transform playerTransform)
    {
        if(_holdPartocals!=null)
            _holdPartocals.Stop();
        _moving = false;
        _rotating = false;
        _collider.enabled = true;
        CanBeMoved = true;
        StartCoroutine(GrabDelay());
        StopMove(_radius);
        transform.SetParent(null);
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        
        var rot = Quaternion.AngleAxis(_angel, playerTransform.forward);
        var direction = (rot * playerTransform.forward).normalized;
        var mainDirection = direction + (_gravityHolder.position - transform.position).normalized;
        _rigidbody.AddForce(mainDirection*_pushForce,ForceMode.Impulse);
    }
    public void PushInRadius()
    {
        if(_holdPartocals!=null)
            _holdPartocals.Stop();
        _moving = false;
        _rotating = false;
        _collider.enabled = true;
        CanBeMoved = true;
        StartCoroutine(GrabDelay());
        StopMove(_radius);
        transform.SetParent(null);
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce((transform.position-_gravityHolder.position).normalized*_pushForce,ForceMode.Impulse);

    }
    protected void OnCollisionEnter(Collision collision)
    {
        
       
        if(_rigidbody.velocity.magnitude<10||_rigidbody.isKinematic)
            return;
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("HIT ENEMY");
            Fighter attacked = collision.transform.root.GetComponent<Fighter>();
            _damage = new Damage(attacked, 10f);
            _damage.SetFighter(attacked);
            attacked.TakeDamage(_damage);
            

        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9 && other.gameObject.layer != 6 && other.gameObject.layer != 10) return;
        if (!_moving && !_rotating) return;
        InList = false;
        Stop();
    }


    private IEnumerator GrabDelay()
    {
        yield return new WaitForSeconds(1f);
        CanBeDraged = true;
        InList = false;

    }



    private void Rotate()
    {
        _rigidbody.isKinematic = true;
        nonInPos = false;
        t2 = 0;
        y = Random.Range(0f, maxOffsety);
        offset = Random.Range(0f, maxOffsetx);
        t2 = 0;
        _moving = false;
        _rotating = true;
    }

    private void Move()
    {
        t = 0;
        _rotating = false;
        _moving = true;
    }
    

    public void LevelCompleted()
    {
        if(CanBeDraged && CanBeMoved)
            return;
        StopAllCoroutines();
        PushInRadius();
    }

    public void LevelFailed()
    {
        if(CanBeDraged && CanBeMoved)
            return;
        StopAllCoroutines();
        PushInRadius();
    }
    
}
