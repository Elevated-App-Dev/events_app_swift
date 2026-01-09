using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for MapSystem.
    /// Feature: event-planner-simulator, Property 2: Zone Visibility by Stage
    /// Validates: Requirements R3
    /// </summary>
    [TestFixture]
    public class MapSystemPropertyTests
    {
        private System.Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new System.Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 2: Zone Visibility by Stage
        /// For any stage (1-5), GetVisibleZones should return the correct zones.
        /// Stage 1: Neighborhood only
        /// Stage 2: Neighborhood + Downtown
        /// Stage 3: Neighborhood + Downtown
        /// Stage 4: Neighborhood + Downtown + Uptown
        /// Stage 5: Neighborhood + Downtown + Uptown + Waterfront
        /// **Validates: Requirements R3.5-R3.10**
        /// </summary>
        [Test]
        public void MapSystem_GetVisibleZones_ReturnsCorrectZonesByStage()
        {
            var mapSystem = new MapSystemImpl();

            // Expected zones for each stage
            var expectedZones = new Dictionary<int, List<MapZone>>
            {
                { 1, new List<MapZone> { MapZone.Neighborhood } },
                { 2, new List<MapZone> { MapZone.Neighborhood, MapZone.Downtown } },
                { 3, new List<MapZone> { MapZone.Neighborhood, MapZone.Downtown } },
                { 4, new List<MapZone> { MapZone.Neighborhood, MapZone.Downtown, MapZone.Uptown } },
                { 5, new List<MapZone> { MapZone.Neighborhood, MapZone.Downtown, MapZone.Uptown, MapZone.Waterfront } }
            };

            for (int stage = 1; stage <= 5; stage++)
            {
                var visibleZones = mapSystem.GetVisibleZones(stage);
                var expected = expectedZones[stage];

                Assert.AreEqual(expected.Count, visibleZones.Count,
                    $"Stage {stage} should have {expected.Count} visible zones, got {visibleZones.Count}");

                foreach (var zone in expected)
                {
                    Assert.Contains(zone, visibleZones,
                        $"Stage {stage} should include {zone}");
                }
            }
        }

        /// <summary>
        /// Property 2a: Neighborhood Always Visible
        /// For any valid stage, Neighborhood zone should always be visible.
        /// **Validates: Requirements R3.5**
        /// </summary>
        [Test]
        public void MapSystem_NeighborhoodZone_AlwaysVisible()
        {
            var mapSystem = new MapSystemImpl();

            for (int i = 0; i < 100; i++)
            {
                int stage = _random.Next(1, 6);
                var visibleZones = mapSystem.GetVisibleZones(stage);

                Assert.Contains(MapZone.Neighborhood, visibleZones,
                    $"Neighborhood should always be visible at Stage {stage}");
            }
        }

        /// <summary>
        /// Property 2b: Zone Unlock Progression
        /// For any two stages where stage1 < stage2, the visible zones for stage2
        /// should be a superset of the visible zones for stage1.
        /// **Validates: Requirements R3.5-R3.10**
        /// </summary>
        [Test]
        public void MapSystem_HigherStages_HaveMoreOrEqualZones()
        {
            var mapSystem = new MapSystemImpl();

            for (int i = 0; i < 100; i++)
            {
                int stage1 = _random.Next(1, 5);
                int stage2 = _random.Next(stage1 + 1, 6);

                var zones1 = mapSystem.GetVisibleZones(stage1);
                var zones2 = mapSystem.GetVisibleZones(stage2);

                // All zones visible at stage1 should be visible at stage2
                foreach (var zone in zones1)
                {
                    Assert.Contains(zone, zones2,
                        $"Zone {zone} visible at Stage {stage1} should also be visible at Stage {stage2}");
                }

                // Stage2 should have at least as many zones as stage1
                Assert.GreaterOrEqual(zones2.Count, zones1.Count,
                    $"Stage {stage2} should have >= zones than Stage {stage1}");
            }
        }

        /// <summary>
        /// Property 2c: IsZoneUnlocked Consistency
        /// For any stage and zone, IsZoneUnlocked should be consistent with GetVisibleZones.
        /// **Validates: Requirements R3.5-R3.10**
        /// </summary>
        [Test]
        public void MapSystem_IsZoneUnlocked_ConsistentWithGetVisibleZones()
        {
            var mapSystem = new MapSystemImpl();
            var allZones = Enum.GetValues(typeof(MapZone)).Cast<MapZone>().ToList();

            for (int i = 0; i < 100; i++)
            {
                int stage = _random.Next(1, 6);
                var visibleZones = mapSystem.GetVisibleZones(stage);

                foreach (var zone in allZones)
                {
                    bool isUnlocked = mapSystem.IsZoneUnlocked(zone, stage);
                    bool isVisible = visibleZones.Contains(zone);

                    Assert.AreEqual(isVisible, isUnlocked,
                        $"IsZoneUnlocked({zone}, {stage}) = {isUnlocked} should match visibility = {isVisible}");
                }
            }
        }

        /// <summary>
        /// Property 2d: Invalid Stage Clamping
        /// Invalid stage values should be clamped to valid range (1-5).
        /// **Validates: Requirements R3**
        /// </summary>
        [Test]
        public void MapSystem_InvalidStage_ClampedToValidRange()
        {
            var mapSystem = new MapSystemImpl();

            // Stage 0 should behave like Stage 1
            var zonesStage0 = mapSystem.GetVisibleZones(0);
            var zonesStage1 = mapSystem.GetVisibleZones(1);
            CollectionAssert.AreEquivalent(zonesStage1, zonesStage0,
                "Stage 0 should clamp to Stage 1");

            // Negative stage should behave like Stage 1
            var zonesNegative = mapSystem.GetVisibleZones(-5);
            CollectionAssert.AreEquivalent(zonesStage1, zonesNegative,
                "Negative stage should clamp to Stage 1");

            // Stage 6+ should behave like Stage 5
            var zonesStage6 = mapSystem.GetVisibleZones(6);
            var zonesStage5 = mapSystem.GetVisibleZones(5);
            CollectionAssert.AreEquivalent(zonesStage5, zonesStage6,
                "Stage 6 should clamp to Stage 5");

            // Stage 100 should behave like Stage 5
            var zonesStage100 = mapSystem.GetVisibleZones(100);
            CollectionAssert.AreEquivalent(zonesStage5, zonesStage100,
                "Stage 100 should clamp to Stage 5");
        }

        /// <summary>
        /// Property 2e: Downtown Unlocks at Stage 2
        /// Downtown zone should only be visible from Stage 2 onwards.
        /// **Validates: Requirements R3.6, R3.8**
        /// </summary>
        [Test]
        public void MapSystem_DowntownZone_UnlocksAtStage2()
        {
            var mapSystem = new MapSystemImpl();

            // Stage 1: Downtown should NOT be visible
            var zonesStage1 = mapSystem.GetVisibleZones(1);
            Assert.IsFalse(zonesStage1.Contains(MapZone.Downtown),
                "Downtown should NOT be visible at Stage 1");
            Assert.IsFalse(mapSystem.IsZoneUnlocked(MapZone.Downtown, 1),
                "Downtown should NOT be unlocked at Stage 1");

            // Stage 2+: Downtown should be visible
            for (int stage = 2; stage <= 5; stage++)
            {
                var zones = mapSystem.GetVisibleZones(stage);
                Assert.IsTrue(zones.Contains(MapZone.Downtown),
                    $"Downtown should be visible at Stage {stage}");
                Assert.IsTrue(mapSystem.IsZoneUnlocked(MapZone.Downtown, stage),
                    $"Downtown should be unlocked at Stage {stage}");
            }
        }

        /// <summary>
        /// Property 2f: Uptown Unlocks at Stage 4
        /// Uptown zone should only be visible from Stage 4 onwards.
        /// **Validates: Requirements R3.9**
        /// </summary>
        [Test]
        public void MapSystem_UptownZone_UnlocksAtStage4()
        {
            var mapSystem = new MapSystemImpl();

            // Stages 1-3: Uptown should NOT be visible
            for (int stage = 1; stage <= 3; stage++)
            {
                var zones = mapSystem.GetVisibleZones(stage);
                Assert.IsFalse(zones.Contains(MapZone.Uptown),
                    $"Uptown should NOT be visible at Stage {stage}");
                Assert.IsFalse(mapSystem.IsZoneUnlocked(MapZone.Uptown, stage),
                    $"Uptown should NOT be unlocked at Stage {stage}");
            }

            // Stage 4+: Uptown should be visible
            for (int stage = 4; stage <= 5; stage++)
            {
                var zones = mapSystem.GetVisibleZones(stage);
                Assert.IsTrue(zones.Contains(MapZone.Uptown),
                    $"Uptown should be visible at Stage {stage}");
                Assert.IsTrue(mapSystem.IsZoneUnlocked(MapZone.Uptown, stage),
                    $"Uptown should be unlocked at Stage {stage}");
            }
        }

        /// <summary>
        /// Property 2g: Waterfront Unlocks at Stage 5
        /// Waterfront zone should only be visible at Stage 5.
        /// **Validates: Requirements R3.10**
        /// </summary>
        [Test]
        public void MapSystem_WaterfrontZone_UnlocksAtStage5()
        {
            var mapSystem = new MapSystemImpl();

            // Stages 1-4: Waterfront should NOT be visible
            for (int stage = 1; stage <= 4; stage++)
            {
                var zones = mapSystem.GetVisibleZones(stage);
                Assert.IsFalse(zones.Contains(MapZone.Waterfront),
                    $"Waterfront should NOT be visible at Stage {stage}");
                Assert.IsFalse(mapSystem.IsZoneUnlocked(MapZone.Waterfront, stage),
                    $"Waterfront should NOT be unlocked at Stage {stage}");
            }

            // Stage 5: Waterfront should be visible
            var zonesStage5 = mapSystem.GetVisibleZones(5);
            Assert.IsTrue(zonesStage5.Contains(MapZone.Waterfront),
                "Waterfront should be visible at Stage 5");
            Assert.IsTrue(mapSystem.IsZoneUnlocked(MapZone.Waterfront, 5),
                "Waterfront should be unlocked at Stage 5");
        }

        /// <summary>
        /// Property 2h: Location Filtering by Zone
        /// For any zone, GetLocationsInZone should only return locations in that zone.
        /// **Validates: Requirements R3.1, R3.2**
        /// </summary>
        [Test]
        public void MapSystem_GetLocationsInZone_ReturnsOnlyLocationsInZone()
        {
            var mapSystem = new MapSystemImpl();

            // Register some test locations in different zones
            for (int i = 0; i < 20; i++)
            {
                var zone = (MapZone)_random.Next(0, 4);
                var location = new LocationData
                {
                    locationId = $"test_location_{i}",
                    displayName = $"Test Location {i}",
                    locationType = (LocationType)_random.Next(1, 5), // Exclude All
                    zone = zone,
                    mapPosition = new Vector2(_random.Next(0, 100) / 100f, _random.Next(0, 100) / 100f)
                };
                mapSystem.RegisterLocation(location);
            }

            // Test each zone
            foreach (MapZone zone in Enum.GetValues(typeof(MapZone)))
            {
                var locationsInZone = mapSystem.GetLocationsInZone(zone);

                foreach (var location in locationsInZone)
                {
                    Assert.AreEqual(zone, location.zone,
                        $"Location {location.locationId} should be in zone {zone}, but is in {location.zone}");
                }
            }
        }

        /// <summary>
        /// Property 2i: Location Type Filtering
        /// When a filter is set, GetLocationsInZone should only return locations of that type.
        /// **Validates: Requirements R3.2, R3.7**
        /// </summary>
        [Test]
        public void MapSystem_LocationFilter_FiltersCorrectly()
        {
            var mapSystem = new MapSystemImpl();

            // Register locations of different types
            var types = new[] { LocationType.Venue, LocationType.Vendor, LocationType.Office, LocationType.MeetingPoint };
            foreach (var type in types)
            {
                for (int i = 0; i < 5; i++)
                {
                    var location = new LocationData
                    {
                        locationId = $"{type}_{i}",
                        displayName = $"{type} {i}",
                        locationType = type,
                        zone = MapZone.Neighborhood,
                        mapPosition = new Vector2(i * 0.1f, i * 0.1f)
                    };
                    mapSystem.RegisterLocation(location);
                }
            }

            // Test filtering
            foreach (var filterType in types)
            {
                mapSystem.SetLocationFilter(filterType);
                var filteredLocations = mapSystem.GetLocationsInZone(MapZone.Neighborhood);

                foreach (var location in filteredLocations)
                {
                    Assert.AreEqual(filterType, location.locationType,
                        $"With filter {filterType}, location {location.locationId} should be of type {filterType}, but is {location.locationType}");
                }

                Assert.AreEqual(5, filteredLocations.Count,
                    $"Should have 5 locations of type {filterType}");
            }

            // Test All filter returns everything
            mapSystem.SetLocationFilter(LocationType.All);
            var allLocations = mapSystem.GetLocationsInZone(MapZone.Neighborhood);
            Assert.AreEqual(20, allLocations.Count,
                "All filter should return all 20 locations");
        }

        /// <summary>
        /// Property 2j: Location Preview Data Consistency
        /// For any registered location, GetLocationPreview should return consistent data.
        /// **Validates: Requirements R3.3**
        /// </summary>
        [Test]
        public void MapSystem_GetLocationPreview_ReturnsConsistentData()
        {
            var mapSystem = new MapSystemImpl();

            for (int i = 0; i < 100; i++)
            {
                var location = new LocationData
                {
                    locationId = $"test_{i}",
                    displayName = $"Test Location {i}",
                    description = $"Description for location {i}",
                    locationType = (LocationType)_random.Next(1, 5),
                    zone = (MapZone)_random.Next(0, 4),
                    mapPosition = new Vector2(_random.Next(0, 100) / 100f, _random.Next(0, 100) / 100f)
                };
                mapSystem.RegisterLocation(location);

                var preview = mapSystem.GetLocationPreview(location.locationId);

                Assert.IsNotNull(preview, $"Preview should not be null for {location.locationId}");
                Assert.AreEqual(location.locationId, preview.locationId,
                    "Preview locationId should match");
                Assert.AreEqual(location.displayName, preview.displayName,
                    "Preview displayName should match");
                Assert.AreEqual(location.locationType, preview.locationType,
                    "Preview locationType should match");
                Assert.AreEqual(location.zone, preview.zone,
                    "Preview zone should match");
            }
        }

        /// <summary>
        /// Property 2k: Null/Empty Location ID Returns Null Preview
        /// GetLocationPreview should return null for null or empty location IDs.
        /// **Validates: Requirements R3.3**
        /// </summary>
        [Test]
        public void MapSystem_GetLocationPreview_ReturnsNullForInvalidId()
        {
            var mapSystem = new MapSystemImpl();

            Assert.IsNull(mapSystem.GetLocationPreview(null),
                "Should return null for null locationId");
            Assert.IsNull(mapSystem.GetLocationPreview(""),
                "Should return null for empty locationId");
            Assert.IsNull(mapSystem.GetLocationPreview("nonexistent_id"),
                "Should return null for nonexistent locationId");
        }

        /// <summary>
        /// Property 2l: Zone Navigation Updates Current Zone
        /// NavigateToZone should update the CurrentZone property.
        /// **Validates: Requirements R3.2**
        /// </summary>
        [Test]
        public void MapSystem_NavigateToZone_UpdatesCurrentZone()
        {
            var mapSystem = new MapSystemImpl();

            Assert.IsNull(mapSystem.CurrentZone, "CurrentZone should be null initially");

            foreach (MapZone zone in Enum.GetValues(typeof(MapZone)))
            {
                mapSystem.NavigateToZone(zone);
                Assert.AreEqual(zone, mapSystem.CurrentZone,
                    $"CurrentZone should be {zone} after navigation");
            }
        }
    }
}
