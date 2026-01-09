using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Data class representing an in-app purchase product.
    /// Requirements: R29.1-R29.10
    /// 
    /// Note: In a full Unity implementation, this would be a ScriptableObject.
    /// For testability and pure C# logic, we use a serializable class.
    /// </summary>
    [Serializable]
    public class IAPProductData
    {
        /// <summary>
        /// Unique identifier for the product (matches store product ID).
        /// </summary>
        public string productId;

        /// <summary>
        /// Display name shown to the player.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Description of what the product provides.
        /// </summary>
        public string description;

        /// <summary>
        /// Base price in USD (actual price comes from store).
        /// Requirements: R29.5
        /// </summary>
        public float priceUSD;

        /// <summary>
        /// Type of product (consumable, non-consumable, subscription).
        /// Requirements: R28.8
        /// </summary>
        public IAPProductType productType;

        /// <summary>
        /// Amount of in-game currency granted (for currency packs).
        /// Requirements: R29.1
        /// </summary>
        public int currencyReward;

        /// <summary>
        /// List of unlock IDs granted by this product.
        /// Requirements: R29.2, R29.3
        /// </summary>
        public List<string> unlockIds = new List<string>();

        /// <summary>
        /// Whether this product grants ad-free experience.
        /// Requirements: R29.3, R29.4
        /// </summary>
        public bool grantsAdFree;

        /// <summary>
        /// Whether this product grants VIP status.
        /// Requirements: R29.4
        /// </summary>
        public bool grantsVIP;

        /// <summary>
        /// Whether this product grants Premium Idle Mode.
        /// Requirements: R29.7-R29.8
        /// </summary>
        public bool grantsPremiumIdleMode;

        /// <summary>
        /// Icon identifier for UI display.
        /// </summary>
        public string iconId;

        /// <summary>
        /// Whether this product is currently available for purchase.
        /// </summary>
        public bool isAvailable = true;

        /// <summary>
        /// Sort order for display in store UI.
        /// </summary>
        public int sortOrder;

        /// <summary>
        /// Category for grouping in store UI.
        /// </summary>
        public IAPProductCategory category;

        /// <summary>
        /// Whether this is a limited-time offer.
        /// </summary>
        public bool isLimitedTime;

        /// <summary>
        /// Discount percentage if on sale (0-100).
        /// </summary>
        public float discountPercent;

        /// <summary>
        /// Get the effective price after discount.
        /// </summary>
        public float EffectivePrice => priceUSD * (1f - discountPercent / 100f);

        /// <summary>
        /// Check if this product is on sale.
        /// </summary>
        public bool IsOnSale => discountPercent > 0;

        /// <summary>
        /// Check if this product grants any permanent unlocks.
        /// </summary>
        public bool HasUnlocks => unlockIds != null && unlockIds.Count > 0;

        /// <summary>
        /// Check if this product is a one-time purchase.
        /// </summary>
        public bool IsOneTimePurchase => productType == IAPProductType.NonConsumable;

        /// <summary>
        /// Check if this product can be purchased multiple times.
        /// </summary>
        public bool IsRepurchasable => productType == IAPProductType.Consumable;

        /// <summary>
        /// Create a default currency pack product.
        /// </summary>
        public static IAPProductData CreateCurrencyPack(string id, string name, float price, int currency)
        {
            return new IAPProductData
            {
                productId = id,
                displayName = name,
                description = $"Get {currency:N0} in-game currency",
                priceUSD = price,
                productType = IAPProductType.Consumable,
                currencyReward = currency,
                category = IAPProductCategory.Currency
            };
        }

        /// <summary>
        /// Create a default unlock product.
        /// </summary>
        public static IAPProductData CreateUnlock(string id, string name, string description, float price, params string[] unlocks)
        {
            return new IAPProductData
            {
                productId = id,
                displayName = name,
                description = description,
                priceUSD = price,
                productType = IAPProductType.NonConsumable,
                unlockIds = new List<string>(unlocks),
                category = IAPProductCategory.Unlock
            };
        }
    }

    /// <summary>
    /// Categories for organizing IAP products in the store UI.
    /// </summary>
    public enum IAPProductCategory
    {
        Currency,
        StarterPack,
        Unlock,
        Subscription,
        Bundle,
        Special
    }
}
