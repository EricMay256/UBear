using System;
using UBear.Input;
using UnityEngine;

namespace UBear.Combat
{
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Collider2D))]
  public class TargetDummyController : MonoBehaviour
  {
    TargetDummy _targetDummy;
    [SerializeField] GameEvent _damageEvent;

    void Awake()
    {
      _targetDummy = new TargetDummy(invulnerable: false, health: 100);
    }

    void OnEnable()
    {
      if (_damageEvent != null)
      {
        _damageEvent.RegisterListener(InflictDamage);
      }
      else
      {
        Debug.LogError("TargetDummyController on " + gameObject.name + " is missing a reference to a DamageEvent GameEvent.");
      }
    }

    void OnDisable()
    {      
      if (_damageEvent != null)
      {
        _damageEvent.UnregisterListener(InflictDamage);
      }
    }

    void InflictDamage()
    {
      _targetDummy.TakeDamage(10);
      Debug.Log($"TargetDummy Health: {_targetDummy.CurHealth}/{_targetDummy.MaxHealth} ({_targetDummy.HealthRatio * 100}%)");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      // Add logic for what happens when the target dummy collides with something
      // For example, you might want to apply damage to the dummy or log the collision
      if(collision.gameObject.GetComponent<IDamaging>() != null)
      {
        IDamaging damagingComponent = collision.gameObject.GetComponent<IDamaging>();
        var dmg = damagingComponent.Damage;
        _targetDummy.TakeDamage(dmg);
        Debug.Log($"TargetDummy took {dmg} damage from {collision.gameObject.name}. Current Health: {_targetDummy.CurHealth}/{_targetDummy.MaxHealth} ({_targetDummy.HealthRatio * 100}%)");
      }
      else
      {
        Debug.Log($"TargetDummy collided with {collision.gameObject.name} but it does not have a damaging component.");
      }
    }
  }
}
