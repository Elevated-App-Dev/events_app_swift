using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System;
using System.Reflection;

namespace EventPlannerSim.Editor
{
    /// <summary>
    /// Automatically attaches controller scripts to prefabs and wires up SerializeField references.
    /// This script finds child objects by name and assigns them to matching fields.
    ///
    /// Run via: Tools > Wire Controllers to Prefabs
    /// Or command line: -executeMethod EventPlannerSim.Editor.PrefabControllerWirer.WireAll
    /// </summary>
    public static class PrefabControllerWirer
    {
        private const string PrefabRoot = "Assets/Prefabs/UI";

        [MenuItem("Tools/Wire Controllers to Prefabs")]
        public static void WireAll()
        {
            UnityEngine.Debug.Log("[PrefabControllerWirer] Starting controller wiring...");

            // HUD - exists
            WirePrefab<UI.HUD.HUDController>($"{PrefabRoot}/HUD/TopBar.prefab");

            // Phone - exists
            WirePrefab<UI.Phone.PhoneOverlayController>($"{PrefabRoot}/Phone/PhoneOverlay.prefab");
            WirePrefab<UI.Phone.AppIconController>($"{PrefabRoot}/Phone/AppIcon.prefab");
            WirePrefab<UI.Phone.Apps.CalendarAppController>($"{PrefabRoot}/Phone/CalendarApp.prefab");
            WirePrefab<UI.Phone.Apps.MessagesAppController>($"{PrefabRoot}/Phone/MessagesApp.prefab");
            WirePrefab<UI.Phone.Apps.BankAppController>($"{PrefabRoot}/Phone/BankApp.prefab");
            WirePrefab<UI.Phone.Apps.ContactsAppController>($"{PrefabRoot}/Phone/ContactsApp.prefab");
            WirePrefab<UI.Phone.Apps.ReviewsAppController>($"{PrefabRoot}/Phone/ReviewsApp.prefab");
            WirePrefab<UI.Phone.Apps.TasksAppController>($"{PrefabRoot}/Phone/TasksApp.prefab");
            WirePrefab<UI.Phone.Apps.ClientsAppController>($"{PrefabRoot}/Phone/ClientsApp.prefab");

            // Map - exists
            WirePrefab<UI.Map.MapOverlayController>($"{PrefabRoot}/Map/MapOverlay.prefab");

            // Map
            WirePrefab<UI.Map.PreviewCardController>($"{PrefabRoot}/Map/PreviewCard.prefab");

            // Event Planning
            WirePrefab<UI.EventPlanning.ClientInquiryController>($"{PrefabRoot}/EventPlanning/ClientInquiryPanel.prefab");
            WirePrefab<UI.EventPlanning.BudgetAllocationController>($"{PrefabRoot}/EventPlanning/BudgetAllocationPanel.prefab");
            WirePrefab<UI.EventPlanning.VenueSelectionController>($"{PrefabRoot}/EventPlanning/VenueSelectionPanel.prefab");
            WirePrefab<UI.EventPlanning.VendorSelectionController>($"{PrefabRoot}/EventPlanning/VendorSelectionPanel.prefab");

            // Event Execution
            WirePrefab<UI.EventExecution.EventExecutionController>($"{PrefabRoot}/EventExecution/EventExecutionPanel.prefab");

            // Results
            WirePrefab<UI.Results.ResultsController>($"{PrefabRoot}/Results/ResultsPanel.prefab");

            // Tutorial
            WirePrefab<UI.Tutorial.TutorialOverlayController>($"{PrefabRoot}/Tutorial/TutorialOverlay.prefab");

            // Notifications
            WirePrefab<UI.Notifications.NotificationController>($"{PrefabRoot}/Notifications/NotificationPopup.prefab");

            // Settings
            WirePrefab<UI.Settings.PauseMenuController>($"{PrefabRoot}/Settings/PauseMenu.prefab");
            WirePrefab<UI.Settings.SettingsController>($"{PrefabRoot}/Settings/SettingsPanel.prefab");

            // Milestone
            WirePrefab<UI.Milestone.MilestoneOverlayController>($"{PrefabRoot}/Milestone/MilestoneOverlay.prefab");

            // Main Menu
            WirePrefab<UI.MainMenu.MainMenuController>($"{PrefabRoot}/MainMenu/MainMenuPanel.prefab");

            // Loading
            WirePrefab<UI.Loading.LoadingController>($"{PrefabRoot}/Loading/LoadingScreen.prefab");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("[PrefabControllerWirer] Controller wiring complete!");
        }

        private static void WirePrefab<T>(string prefabPath) where T : MonoBehaviour
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                UnityEngine.Debug.LogWarning($"[PrefabControllerWirer] Prefab not found: {prefabPath}");
                return;
            }

            // Load prefab for editing
            var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

            try
            {
                // Add or get the controller component
                var controller = prefabRoot.GetComponent<T>();
                if (controller == null)
                {
                    controller = prefabRoot.AddComponent<T>();
                    UnityEngine.Debug.Log($"[PrefabControllerWirer] Added {typeof(T).Name} to {prefabPath}");
                }

                // Auto-wire serialized fields
                WireSerializedFields(controller, prefabRoot);

                // Save the prefab
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                UnityEngine.Debug.Log($"[PrefabControllerWirer] Wired: {prefabPath}");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[PrefabControllerWirer] Error wiring {prefabPath}: {e.Message}");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static void WireSerializedFields(MonoBehaviour controller, GameObject root)
        {
            var type = controller.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var field in fields)
            {
                // Check if it has SerializeField attribute or is public
                var hasSerializeField = field.GetCustomAttribute<SerializeField>() != null;
                var isPublicField = field.IsPublic && field.GetCustomAttribute<NonSerializedAttribute>() == null;

                if (!hasSerializeField && !isPublicField)
                    continue;

                // Skip if already assigned
                var currentValue = field.GetValue(controller);
                if (currentValue != null && !(currentValue is UnityEngine.Object obj && obj == null))
                    continue;

                // Try to find matching child by field name
                var fieldName = field.Name.TrimStart('_');
                var target = FindChildByName(root.transform, fieldName, field.FieldType);

                if (target != null)
                {
                    field.SetValue(controller, target);
                    UnityEngine.Debug.Log($"  -> Wired {field.Name} to {target}");
                }
            }
        }

        private static object FindChildByName(Transform root, string fieldName, Type fieldType)
        {
            // Common naming patterns to try
            string[] namesToTry = {
                fieldName,
                ToPascalCase(fieldName),
                fieldName + "Text",
                fieldName + "Button",
                fieldName + "Image",
                fieldName.Replace("Text", ""),
                fieldName.Replace("Button", ""),
                fieldName.Replace("Btn", "Button"),
                StripPrefix(fieldName)
            };

            foreach (var name in namesToTry)
            {
                var child = FindChildRecursive(root, name);
                if (child != null)
                {
                    // Return the component if field expects a component type
                    if (typeof(Component).IsAssignableFrom(fieldType))
                    {
                        var component = child.GetComponent(fieldType);
                        if (component != null)
                            return component;
                    }
                    // Return GameObject if that's what's expected
                    else if (fieldType == typeof(GameObject))
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }

        private static Transform FindChildRecursive(Transform parent, string name)
        {
            // Case-insensitive comparison
            if (parent.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return parent;

            foreach (Transform child in parent)
            {
                var found = FindChildRecursive(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        private static string StripPrefix(string input)
        {
            // Remove common prefixes like "txt", "btn", "img"
            string[] prefixes = { "txt", "btn", "img", "lbl", "pnl" };
            foreach (var prefix in prefixes)
            {
                if (input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && input.Length > prefix.Length)
                {
                    return input.Substring(prefix.Length);
                }
            }
            return input;
        }

        // Command line entry point
        public static void WireFromCommandLine()
        {
            WireAll();
            EditorApplication.Exit(0);
        }
    }
}
