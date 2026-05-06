using UnityEngine;

namespace UBear.Combat
{
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour, IDamaging
{
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private float _speed = 5f;
    /// <summary>
    /// Use a value of 0 for infinite lifetime or to destroy only via other means
    /// </summary>
    private float _lifetime = 10f;
    [SerializeField]
    private int _damage = 10;
    public int Damage { get
      {
        DamageApplied();
        return _damage;
      }
    }
    public float Speed => _speed;

    void Awake()
    {
      _rigidbody2D = GetComponent<Rigidbody2D>();
      if(_lifetime > 0)
      {
        Destroy(gameObject, _lifetime);
      }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      _rigidbody2D.MovePosition(transform.position + transform.right * _speed * Time.fixedDeltaTime);
    }


    public void DamageApplied()
    {
      Destroy(gameObject);
    }
}
}