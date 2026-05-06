using NUnit.Framework;
using UnityEngine;
using UBear.Inventory;
using UnityEditor.VersionControl;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UBear.Editor.Tests
{
public class UBearInventoryTests
{
  [Test]
  public void EquippedItemsData_DefaultSlots_HaveExpectedLayout()
  {
    var data = ScriptableObject.CreateInstance<EquippedItemsData>();

    Assert.AreEqual(16, data.equipmentSlots.Count);

    Assert.AreEqual(EquipmentCategory.MainHand, data.equipmentSlots[0].SlotType);
    Assert.AreEqual("MainHand", data.equipmentSlots[0].SlotName);

    Assert.AreEqual(EquipmentCategory.OffHand, data.equipmentSlots[1].SlotType);
    Assert.AreEqual("OffHand", data.equipmentSlots[1].SlotName);

    Assert.AreEqual(EquipmentCategory.Earring, data.equipmentSlots[12].SlotType);
    Assert.AreEqual("Earring Left", data.equipmentSlots[12].SlotName);

    Assert.AreEqual(EquipmentCategory.Earring, data.equipmentSlots[13].SlotType);
    Assert.AreEqual("Earring Right", data.equipmentSlots[13].SlotName);

    Assert.AreEqual(EquipmentCategory.Ring, data.equipmentSlots[14].SlotType);
    Assert.AreEqual("Ring Left", data.equipmentSlots[14].SlotName);

    Assert.AreEqual(EquipmentCategory.Ring, data.equipmentSlots[15].SlotType);
    Assert.AreEqual("Ring Right", data.equipmentSlots[15].SlotName);
  }
  [Test]
  public void EquipmentSlot_Equip_SwapsItems()
  {
    var slot = new EquipmentSlot(EquipmentCategory.MainHand);

    var firstBlueprint = CreateEquipmentBlueprint("Starter Sword", EquipmentCategory.MainHand, 100f);
    var secondBlueprint = CreateEquipmentBlueprint("Steel Sword", EquipmentCategory.MainHand, 120f);

    Item firstItem = firstBlueprint.CreateInstance();
    Item secondItem = secondBlueprint.CreateInstance();

    Assert.IsNull(slot.EquippedItem);

    slot.Equip(ref firstItem);
    Assert.NotNull(slot.EquippedItem);
    Assert.AreEqual("Starter Sword", slot.EquippedItem.Blueprint.ItemName);
    Assert.IsNull(firstItem);

    slot.Equip(ref secondItem);
    Assert.NotNull(slot.EquippedItem);
    Assert.AreEqual("Steel Sword", slot.EquippedItem.Blueprint.ItemName);
    Assert.NotNull(secondItem);
    Assert.AreEqual("Starter Sword", secondItem.Blueprint.ItemName);
  }
  [Test]
  public void Item_FromEquipmentBlueprint_UsesDurabilityRules()
  {
    var blueprint = CreateEquipmentBlueprint("Durable Blade", EquipmentCategory.MainHand, 7f);
    var item = blueprint.CreateInstance();

    Assert.AreEqual(7f, item.CurrentDurability);
    Assert.AreEqual(1f, item.DurabilityPercentage);

    item.ReduceDurability(2f);
    Assert.AreEqual(5f, item.CurrentDurability);

    item.RepairDurability(10f);
    Assert.AreEqual(7f, item.CurrentDurability);
  }

  [Test]
  public void TestEquipmentAsset_HasExpectedSerializedValues()
  {
#if UNITY_EDITOR
    //Create asset and initialize all relevant fields
    var asset = ScriptableObject.CreateInstance<EquipmentDefinition>();
    asset.ItemName = "TestEquipment";
    asset.ItemObjectType = ItemType.Equipment;
    asset.Description = "This item is used for unit testing and should never be seen.";
    asset.ID = 69420;
    asset.MaxStackSize = 1;
    asset.UniqueStacks = 0;
    asset.SellPrice = 2;
    asset.Tags.Add("TestItem");
    asset.EquipmentType = EquipmentCategory.MainHand;
    asset.ArmorValue = 3f;
    asset.DamageBonus = 4f;
    asset.Weight = 5f;
    asset.RequiredLevel = 6;
    asset.MaxDurability = 7f;

    //Check initialized values against expected values
    Assert.AreEqual("TestEquipment", asset.ItemName);
    Assert.AreEqual(ItemType.Equipment, asset.ItemObjectType);
    Assert.AreEqual("This item is used for unit testing and should never be seen.", asset.Description);
    Assert.AreEqual(69420, asset.ID);
    Assert.AreEqual(1, asset.MaxStackSize);
    Assert.AreEqual(0, asset.UniqueStacks);
    Assert.AreEqual(2, asset.SellPrice);
    Assert.AreEqual(true, asset.HasTag("TestItem"));
    Assert.AreEqual(false, asset.HasTag("TestItemasdf"));
    Assert.AreEqual(EquipmentCategory.MainHand, asset.EquipmentType);
    Assert.AreEqual(3f, asset.ArmorValue);
    Assert.AreEqual(4f, asset.DamageBonus);
    Assert.AreEqual(5f, asset.Weight);
    Assert.AreEqual(6, asset.RequiredLevel);
    Assert.AreEqual(7f, asset.MaxDurability);

    Object.DestroyImmediate(asset);
#else
    Assert.Ignore("AssetDatabase tests require UNITY_EDITOR.");
#endif
  }

  private static EquipmentDefinition CreateEquipmentBlueprint(string name, EquipmentCategory category, float maxDurability)
  {
    var equipment = ScriptableObject.CreateInstance<EquipmentDefinition>();
    equipment.ItemName = name;
    equipment.ItemObjectType = ItemType.Equipment;
    equipment.MaxStackSize = 1;
    equipment.EquipmentType = category;
    equipment.MaxDurability = maxDurability;
    return equipment;
  }
}
}