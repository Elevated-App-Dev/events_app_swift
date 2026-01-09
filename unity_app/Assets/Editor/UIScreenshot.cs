using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace EventPlannerSim.Editor
{
    public static class UIScreenshot
    {
        public static void CaptureUI()
        {
            int width = 540;
            int height = 960;
            string outputPath = "/tmp/unity_ui_preview.png";

            // Create texture to draw on
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            // Fill with dark background
            Color bgColor = new Color(0.1f, 0.1f, 0.15f, 1f);
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = bgColor;
            tex.SetPixels(pixels);

            // Draw HUD bar (top)
            DrawRect(tex, 0, height - 60, width, 60, new Color(0.12f, 0.12f, 0.18f, 1f));

            // Draw bottom nav bar
            DrawRect(tex, 0, 0, width, 70, new Color(0.12f, 0.12f, 0.18f, 1f));

            // Draw nav buttons
            int btnWidth = (width - 60) / 3;
            Color btnColor = new Color(0.25f, 0.5f, 0.85f, 1f);
            DrawRect(tex, 15, 10, btnWidth, 50, btnColor);
            DrawRect(tex, 15 + btnWidth + 15, 10, btnWidth, 50, btnColor);
            DrawRect(tex, 15 + (btnWidth + 15) * 2, 10, btnWidth, 50, btnColor);

            // Draw content area
            DrawRect(tex, 20, 90, width - 40, height - 170, new Color(0.08f, 0.08f, 0.12f, 0.95f));

            // Draw some placeholder elements in content
            // Title area
            DrawRect(tex, 40, height - 250, width - 80, 80, new Color(0.15f, 0.15f, 0.22f, 1f));

            // Content cards
            DrawRect(tex, 40, height - 350, width - 80, 60, new Color(0.18f, 0.18f, 0.25f, 1f));
            DrawRect(tex, 40, height - 420, width - 80, 60, new Color(0.18f, 0.18f, 0.25f, 1f));
            DrawRect(tex, 40, height - 490, width - 80, 60, new Color(0.18f, 0.18f, 0.25f, 1f));

            // Money indicator (green bar in HUD)
            DrawRect(tex, width - 150, height - 45, 130, 30, new Color(0.2f, 0.5f, 0.25f, 1f));

            // Reputation indicator (gold bar in HUD)
            DrawRect(tex, width - 290, height - 45, 130, 30, new Color(0.6f, 0.45f, 0.15f, 1f));

            tex.Apply();

            // Save to file
            byte[] pngData = tex.EncodeToPNG();
            File.WriteAllBytes(outputPath, pngData);

            Object.DestroyImmediate(tex);

            UnityEngine.Debug.Log($"[UIScreenshot] Saved preview to {outputPath}");
        }

        private static void DrawRect(Texture2D tex, int x, int y, int w, int h, Color color)
        {
            for (int px = x; px < x + w && px < tex.width; px++)
            {
                for (int py = y; py < y + h && py < tex.height; py++)
                {
                    if (px >= 0 && py >= 0)
                        tex.SetPixel(px, py, color);
                }
            }
        }

        public static void CaptureFromCommandLine()
        {
            CaptureUI();
            EditorApplication.Exit(0);
        }
    }
}
