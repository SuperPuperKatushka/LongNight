//using NUnit.Framework; 
//using UnityEngine; 
//using UnityEngine.TestTools; 

//public class PickupTests
//{
//	private GameObject pickupObject;           
//	private Pickup pickupScript;               
//	private Inventory inventory;       

//	// Метод вызывается перед каждым тестом — подготавливает сцену
//	[SetUp]
//	public void Setup()
//	{
//		// Создаем объект игрока и добавляем компонент Inventory
//		var playerObject = new GameObject("Player");
//		inventory = playerObject.AddComponent<Inventory>();

//		// Инициализируем массивы слотов и флагов занятости
//		inventory.isFull = new bool[3]; // Три слота, все свободны по умолчанию (false)
//		inventory.slots = new GameObject[3]; // Создаем GameObject для каждого слота

//		for (int i = 0; i < 3; i++)
//		{
//			inventory.slots[i] = new GameObject($"Slot{i}"); // Назначаем пустые игровые объекты как слоты
//		}

//		pickupObject = new GameObject("Pickup");
//		pickupScript = pickupObject.AddComponent<Pickup>();

//		pickupScript.slotButton = new GameObject("SlotButton");

//		// Устанавливаем ссылку на инвентарь
//		pickupScript.GetType()
//			.GetField("inventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//			.SetValue(pickupScript, inventory);
//	}

//	// Тест: Проверяем, добавляется ли предмет в первый свободный слот
//	[Test]
//	public void TryPickup_AddsItemToFirstAvailableSlot()
//	{
//		// Act
//		bool result = pickupScript.TryPickup(); // Вызываем метод

//		// Assert
//		Assert.IsTrue(result); // Метод должен вернуть true (предмет подобран)
//		Assert.IsTrue(inventory.isFull[0]); // Первый слот должен стать занятым
//	}

//	// Тест: Проверяем, что предмет не подбирается, если все слоты заняты
//	[Test]
//	public void TryPickup_FailsWhenInventoryIsFull()
//	{
//		// Arrange — заполняем все слоты
//		for (int i = 0; i < inventory.isFull.Length; i++)
//		{
//			inventory.isFull[i] = true;
//		}

//		// Act
//		bool result = pickupScript.TryPickup();

//		// Assert
//		Assert.IsFalse(result); // Метод должен вернуть false (не было свободных слотов)
//	}

//	// Метод вызывается после каждого теста — очищает сцену
//	[TearDown]
//	public void Teardown()
//	{
//		Object.DestroyImmediate(pickupObject);
//		Object.DestroyImmediate(inventory.gameObject);
//	}
//}