using UnityEngine;

namespace UBear.Combat
{
[CreateAssetMenu(fileName = "New Projectile", menuName = "UBear/Combat/Weapons/Projectile")]
public class ProjectileObject : ScriptableObject
{
  public GameObject Prefab;
  public GameObject ImpactEffect;
  public AudioClip ShootSound;
  public AudioClip ImpactSound;
  public Color Color = Color.white;
  public int Damage = 1;
  public float Scale = 1f;
  public float Speed = 10f;
  public float Lifetime = 5f;
  public float ComboTime = .1f;
}
}