//using NUnit.Framework; 
//using UnityEngine; 
//using UnityEngine.TestTools; 

//public class PickupTests
//{
//	private GameObject pickupObject;           
//	private Pickup pickupScript;               
//	private Inventory inventory;       

//	// ����� ���������� ����� ������ ������ � �������������� �����
//	[SetUp]
//	public void Setup()
//	{
//		// ������� ������ ������ � ��������� ��������� Inventory
//		var playerObject = new GameObject("Player");
//		inventory = playerObject.AddComponent<Inventory>();

//		// �������������� ������� ������ � ������ ���������
//		inventory.isFull = new bool[3]; // ��� �����, ��� �������� �� ��������� (false)
//		inventory.slots = new GameObject[3]; // ������� GameObject ��� ������� �����

//		for (int i = 0; i < 3; i++)
//		{
//			inventory.slots[i] = new GameObject($"Slot{i}"); // ��������� ������ ������� ������� ��� �����
//		}

//		pickupObject = new GameObject("Pickup");
//		pickupScript = pickupObject.AddComponent<Pickup>();

//		pickupScript.slotButton = new GameObject("SlotButton");

//		// ������������� ������ �� ���������
//		pickupScript.GetType()
//			.GetField("inventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//			.SetValue(pickupScript, inventory);
//	}

//	// ����: ���������, ����������� �� ������� � ������ ��������� ����
//	[Test]
//	public void TryPickup_AddsItemToFirstAvailableSlot()
//	{
//		// Act
//		bool result = pickupScript.TryPickup(); // �������� �����

//		// Assert
//		Assert.IsTrue(result); // ����� ������ ������� true (������� ��������)
//		Assert.IsTrue(inventory.isFull[0]); // ������ ���� ������ ����� �������
//	}

//	// ����: ���������, ��� ������� �� �����������, ���� ��� ����� ������
//	[Test]
//	public void TryPickup_FailsWhenInventoryIsFull()
//	{
//		// Arrange � ��������� ��� �����
//		for (int i = 0; i < inventory.isFull.Length; i++)
//		{
//			inventory.isFull[i] = true;
//		}

//		// Act
//		bool result = pickupScript.TryPickup();

//		// Assert
//		Assert.IsFalse(result); // ����� ������ ������� false (�� ���� ��������� ������)
//	}

//	// ����� ���������� ����� ������� ����� � ������� �����
//	[TearDown]
//	public void Teardown()
//	{
//		Object.DestroyImmediate(pickupObject);
//		Object.DestroyImmediate(inventory.gameObject);
//	}
//}