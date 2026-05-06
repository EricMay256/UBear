using UnityEngine;
using  UBear.Input;

namespace UBear.Combat
{
public class TestFiring : MonoBehaviour
{
  public GameEvent OnFirePressed;
  RangedWeapon _testWeapon; 
  public void HandleFirePressed()
  {
    if (_testWeapon != null)
    {
      _testWeapon.Attack(Vector3.right);
    } 
    Debug.Log("Fire button pressed - TestFiring component on " + gameObject.name + " received the event.");
  }
  void Awake()
  {
    _testWeapon = GetComponent<RangedWeapon>();
    if (_testWeapon == null)
    {
      Debug.LogError("TestFiring component on " + gameObject.name + " requires a RangedWeapon component on the same GameObject.");
    }
    if (OnFirePressed == null)
    {
      Debug.LogError("TestFiring component on " + gameObject.name + " is missing a reference to an OnFirePressed GameEvent.");
    }
    else
    {
      OnFirePressed.RegisterListener(HandleFirePressed);
    }
  }
  void OnDestroy()
  {
    if (OnFirePressed != null)
    {
      OnFirePressed.UnregisterListener(HandleFirePressed);
    }
  }
}
}

