using UnityEngine;

namespace UBear.Combat
{
public interface IWeapon
{
  void Attack();
  void Attack(Vector3 direction);
}
}