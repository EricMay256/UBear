using UnityEngine;
using System.Collections.Generic;
using System;
using UBear.Input;

namespace UBear.Inventory {
public class PlayerController : MonoBehaviour
{
  public InventoryData inventory;
  [SerializeField] private float _moveSpeed = 5f;
  [SerializeField] private Vector2Event _onMove;
  Vector2 _curDirection;

  void OnEnable()
  {
    _onMove.RegisterListener(HandleMove);
    
  }

  void Start()
    {
      inventory.Load();
    }

  void OnDisable()
  {
    _onMove.UnregisterListener(HandleMove);
    inventory.Save();
  }

  void FixedUpdate()
  {
    transform.position += _moveSpeed * Time.fixedDeltaTime * (Vector3)_curDirection;
  }

    void HandleMove(Vector2 moveInput)
  {
    _curDirection = moveInput;
  }

  void OnTriggerEnter2D(Collider2D collision)
  {
    PickupOnTouch pickup = collision.GetComponent<PickupOnTouch>();
    if (pickup != null)
    {
      pickup.containedAmount -= inventory.AddItem(pickup.ContainedItemObject.ID, pickup.containedAmount);
      if (pickup.containedAmount <= 0)
        Destroy(collision.gameObject);
    }
  }

  void OnDestroy()
  {
    inventory.Container = new List<InventorySlot>();
  }

  [ContextMenu("Clear Inventory")]
  public void ClearInventory()
  {
    inventory.Container = new List<InventorySlot>();
  }
}}
