using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.IO;
using UnityEngine.UI;

/*
 * Services/Purchaser.cs
 *
 * @author Nathan "Nathcat" Baines
 */

public class Purchaser : MonoBehaviour, IStoreListener
{
  private static IStoreController storeController;
  private static IExtensionProvider storeExtensionProvider;
  public GameObject gemsText;
  public GameObject noadsBuyButton;

  void Start() {
    if (storeController == null) {
      InitializePurchasing();
    }
  }

  void Update() {
    noadsBuyButton.SetActive(!(read(Path.Combine(Application.persistentDataPath, "noads.txt")) == "true\n"));
  }

  public void InitializePurchasing() {
    if (IsInitialized()) {
      return;
    }

    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

    builder.AddProduct("25gempack", ProductType.Consumable);
    builder.AddProduct("50gempack", ProductType.Consumable);
    builder.AddProduct("75gempack", ProductType.Consumable);
    builder.AddProduct("100gempack", ProductType.Consumable);
    builder.AddProduct("noads", ProductType.NonConsumable);

    UnityPurchasing.Initialize(this, builder);
  }

  private bool IsInitialized() {
    return storeController != null && storeExtensionProvider != null;
  }

  public void BuyConsumable(string productId) {
    BuyProductId(productId);
  }

  void BuyProductId(string productId) {
    if (IsInitialized()) {
      Product product = storeController.products.WithID(productId);

      if (product != null && product.availableToPurchase) {
        Debug.Log(string.Format("Purchasing product asynchronously: '{0}'", product.definition.id));

        storeController.InitiatePurchase(product);

      } else {
        Debug.LogError("BuyProductId: FAILED. product either cannot be found or is not available to purchase");
      }

    } else {
      Debug.LogError("BuyProductId: FAILED. Not initialized");
    }
  }

  public void RestorePurchases() {
    if (!IsInitialized()) {
      Debug.LogError("RestorePurchases: FAILED. Not Initialized");
      return;

    }

    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
      Debug.Log("RestorePurchases: Starting...");

      var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();

      apple.RestoreTransactions((result) => {
        Debug.Log("RestorePurchases: Continuing " + result + ". If no further messages, no purchases available to restore.");
      });

    } else {
      Debug.LogError("RestorePurchases: FAILED. Not supported on this platform (" + Application.platform + ")");
    }
  }

  public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
    Debug.Log("OnInitialized: PASS");

    storeController = controller;
    storeExtensionProvider = extensions;
  }

  public void OnInitializeFailed(InitializationFailureReason error) {
    Debug.Log("OnIntialize Failed with error " + error);
  }

  public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
    Debug.LogError(string.Format("OnPurchaseFailed: FAILED, Product: '{0}', PurchaseFailureReason: '{1}'", product.definition.storeSpecificId, failureReason));
  }

  public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {

    if (String.Equals(args.purchasedProduct.definition.id, "25gempack", StringComparison.Ordinal)) {
      Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

      write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), (int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"))) + 25).ToString());
      gemsText.GetComponent<Text>().text = read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"));

    } else if (String.Equals(args.purchasedProduct.definition.id, "50gempack", StringComparison.Ordinal)) {
      Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

      write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), (int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"))) + 50).ToString());
      gemsText.GetComponent<Text>().text = read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"));

    } else if (String.Equals(args.purchasedProduct.definition.id, "75gempack", StringComparison.Ordinal)) {
      Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

      write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), (int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"))) + 75).ToString());
      gemsText.GetComponent<Text>().text = read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"));

    } else if (String.Equals(args.purchasedProduct.definition.id, "100gempack", StringComparison.Ordinal)) {
      Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

      write(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"), (int.Parse(read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"))) + 100).ToString());
      gemsText.GetComponent<Text>().text = read(Path.Combine(Application.persistentDataPath, "number_of_gems.txt"));


    } else if (String.Equals(args.purchasedProduct.definition.id, "noads", StringComparison.Ordinal)) {
      Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

      write(Path.Combine(Application.persistentDataPath, "noads.txt"), "true");

    } else {
      Debug.LogError("ProcessPurchase: FAIL, product not recoginized");
    }

    return PurchaseProcessingResult.Complete;
  }

  string read(string path) {
    StreamReader reader = new StreamReader(path);
    string content = reader.ReadToEnd();
    reader.Close();
    return content;
  }

  void write(string path, string content) {
    StreamWriter writer = new StreamWriter(path);
    writer.WriteLine(content);
    writer.Close();
  }
}
