using System.Collections.Generic;
using UnityEngine;
using UBear.Inventory;

namespace UBear.Combat
{
/// <summary>
/// Determines how the firing points on a weapon,
/// or the projectiles on a firing point, fire together
/// </summary>
public enum FireCoordinationPattern
{
  Sequential,
  AllTogether,
  Random,
  Combo
}

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "UBear/Combat/Weapons/Ranged Weapon")]
//Inherits from EquipmentItem, which inherits from Item, so that it can be handled by the inventory system, be equipped, and have stats
public class RangedWeaponObject : EquipmentDefinition
{
  public List<FiringPoint> FiringPoints = new List<FiringPoint> {
    new FiringPoint()
  };
  public FireCoordinationPattern FirepointPattern = FireCoordinationPattern.Sequential;
  public float ComboTime = 0.1f;
  public float BaseAttackCooldown = 1f;
  public float ShotSpread = 0f;
  public bool ContinuousFireAllowed = true;
}

[System.Serializable]
public class FiringPoint
{
  public Vector3 Position = Vector3.zero;
  public List<ProjectileObject> Projectiles = new List<ProjectileObject>();
  public FireCoordinationPattern FirePattern = FireCoordinationPattern.Sequential;
  public float ShotSpread = 0f;
  [System.NonSerialized]
  public int nextProjectileIndex = 0;
}
}