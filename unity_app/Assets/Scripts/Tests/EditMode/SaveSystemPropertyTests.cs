using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for Save/Load round-trip.
    /// Feature: event-planner-simulator, Property 1: Save/Load Round Trip
    /// Validates: Requirements R2, R27
    /// </summary>
    [TestFixture]
    public class SaveSystemPropertyTests
    {
        private System.Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new System.Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 1: Save/Load Round Trip
        /// For any valid SaveData object S, serializing to JSON and deserializing back
        /// should produce a semantically equivalent object.
        /// **Validates: Requirements R2, R27**
        /// </summary>
        [Test]
        public void SaveData_JsonRoundTrip_PreservesAllFields()
        {
            // Run 100 iterations as per testing strategy
            for (int i = 0; i < 100; i++)
            {
                // Generate random SaveData
                SaveData original = GenerateRandomSaveData();

                // Serialize to JSON
                string json = JsonUtility.ToJson(original);

                // Deserialize back
                SaveData reconstructed = JsonUtility.FromJson<SaveData>(json);

                // Assert semantic equivalence
                AssertSaveDataEquivalent(original, reconstructed, $"Iteration {i}");
            }
        }


        /// <summary>
        /// Property 1: Save/Load Round Trip - PlayerData preservation
        /// All PlayerData fields should be preserved through serialization.
        /// **Validates: Requirements R2, R27**
        /// </summary>
        [Test]
        public void SaveData_JsonRoundTrip_PreservesPlayerData()
        {
            for (int i = 0; i < 100; i++)
            {
                SaveData original = GenerateRandomSaveData();
                string json = JsonUtility.ToJson(original);
                SaveData reconstructed = JsonUtility.FromJson<SaveData>(json);

                AssertPlayerDataEquivalent(original.playerData, reconstructed.playerData, $"Iteration {i}");
            }
        }

        /// <summary>
        /// Property 1: Save/Load Round Trip - EventData preservation
        /// All EventData in activeEvents and eventHistory should be preserved.
        /// **Validates: Requirements R2, R27**
        /// </summary>
        [Test]
        public void SaveData_JsonRoundTrip_PreservesEventData()
        {
            for (int i = 0; i < 100; i++)
            {
                SaveData original = GenerateRandomSaveData();
                string json = JsonUtility.ToJson(original);
                SaveData reconstructed = JsonUtility.FromJson<SaveData>(json);

                Assert.AreEqual(original.activeEvents.Count, reconstructed.activeEvents.Count,
                    $"Active events count mismatch at iteration {i}");
                Assert.AreEqual(original.eventHistory.Count, reconstructed.eventHistory.Count,
                    $"Event history count mismatch at iteration {i}");

                for (int j = 0; j < original.activeEvents.Count; j++)
                {
                    AssertEventDataEquivalent(original.activeEvents[j], reconstructed.activeEvents[j],
                        $"Iteration {i}, ActiveEvent {j}");
                }
            }
        }

        /// <summary>
        /// Property 1: Save/Load Round Trip - Booking calendars preservation
        /// All vendor and venue bookings should be preserved.
        /// **Validates: Requirements R2, R27**
        /// </summary>
        [Test]
        public void SaveData_JsonRoundTrip_PreservesBookingCalendars()
        {
            for (int i = 0; i < 100; i++)
            {
                SaveData original = GenerateRandomSaveData();
                string json = JsonUtility.ToJson(original);
                SaveData reconstructed = JsonUtility.FromJson<SaveData>(json);

                Assert.AreEqual(original.vendorBookings.Count, reconstructed.vendorBookings.Count,
                    $"Vendor bookings count mismatch at iteration {i}");
                Assert.AreEqual(original.venueBookings.Count, reconstructed.venueBookings.Count,
                    $"Venue bookings count mismatch at iteration {i}");

                for (int j = 0; j < original.vendorBookings.Count; j++)
                {
                    AssertBookingEntryEquivalent(original.vendorBookings[j], reconstructed.vendorBookings[j],
                        $"Iteration {i}, VendorBooking {j}");
                }

                for (int j = 0; j < original.venueBookings.Count; j++)
                {
                    AssertBookingEntryEquivalent(original.venueBookings[j], reconstructed.venueBookings[j],
                        $"Iteration {i}, VenueBooking {j}");
                }
            }
        }

        /// <summary>
        /// Property 1: Save/Load Round Trip - Settings preservation
        /// All GameSettings should be preserved.
        /// **Validates: Requirements R2, R27**
        /// </summary>
        [Test]
        public void SaveData_JsonRoundTrip_PreservesSettings()
        {
            for (int i = 0; i < 100; i++)
            {
                SaveData original = GenerateRandomSaveData();
                string json = JsonUtility.ToJson(original);
                SaveData reconstructed = JsonUtility.FromJson<SaveData>(json);

                AssertGameSettingsEquivalent(original.settings, reconstructed.settings, $"Iteration {i}");
            }
        }

        /// <summary>
        /// Property 1: Save/Load Round Trip - Edge cases
        /// Empty lists, special characters, and boundary values should be preserved.
        /// **Validates: Requirements R2, R27**
        /// </summary>
        [Test]
        public void SaveData_JsonRoundTrip_HandlesEdgeCases()
        {
            // Test with empty lists
            var emptyData = new SaveData();
            string emptyJson = JsonUtility.ToJson(emptyData);
            SaveData emptyReconstructed = JsonUtility.FromJson<SaveData>(emptyJson);
            Assert.IsNotNull(emptyReconstructed);
            Assert.AreEqual(0, emptyReconstructed.activeEvents.Count);
            Assert.AreEqual(0, emptyReconstructed.eventHistory.Count);

            // Test with special characters in names
            var specialData = new SaveData();
            specialData.playerData.playerName = "Test'Player\"Name<>&";
            string specialJson = JsonUtility.ToJson(specialData);
            SaveData specialReconstructed = JsonUtility.FromJson<SaveData>(specialJson);
            Assert.AreEqual(specialData.playerData.playerName, specialReconstructed.playerData.playerName);

            // Test with max values
            var maxData = new SaveData();
            maxData.playerData.money = float.MaxValue;
            maxData.playerData.reputation = int.MaxValue;
            maxData.excellenceStreak = int.MaxValue;
            string maxJson = JsonUtility.ToJson(maxData);
            SaveData maxReconstructed = JsonUtility.FromJson<SaveData>(maxJson);
            Assert.AreEqual(maxData.playerData.money, maxReconstructed.playerData.money);
            Assert.AreEqual(maxData.playerData.reputation, maxReconstructed.playerData.reputation);

            // Test with zero money (player broke, needs loan)
            var zeroMoneyData = new SaveData();
            zeroMoneyData.playerData.money = 0f;
            string zeroMoneyJson = JsonUtility.ToJson(zeroMoneyData);
            SaveData zeroMoneyReconstructed = JsonUtility.FromJson<SaveData>(zeroMoneyJson);
            Assert.AreEqual(0f, zeroMoneyReconstructed.playerData.money, "Zero money should be preserved");

            // Test with negative money (debt/loan scenario)
            var negativeMoneyData = new SaveData();
            negativeMoneyData.playerData.money = -5000f;
            string negativeMoneyJson = JsonUtility.ToJson(negativeMoneyData);
            SaveData negativeMoneyReconstructed = JsonUtility.FromJson<SaveData>(negativeMoneyJson);
            Assert.AreEqual(-5000f, negativeMoneyReconstructed.playerData.money, "Negative money (debt) should be preserved");

            // Test with minimum float value for extreme debt
            var minMoneyData = new SaveData();
            minMoneyData.playerData.money = float.MinValue;
            string minMoneyJson = JsonUtility.ToJson(minMoneyData);
            SaveData minMoneyReconstructed = JsonUtility.FromJson<SaveData>(minMoneyJson);
            Assert.AreEqual(float.MinValue, minMoneyReconstructed.playerData.money, "Min float money should be preserved");

            // Test with zero and negative reputation
            var zeroRepData = new SaveData();
            zeroRepData.playerData.reputation = 0;
            string zeroRepJson = JsonUtility.ToJson(zeroRepData);
            SaveData zeroRepReconstructed = JsonUtility.FromJson<SaveData>(zeroRepJson);
            Assert.AreEqual(0, zeroRepReconstructed.playerData.reputation, "Zero reputation should be preserved");

            var negativeRepData = new SaveData();
            negativeRepData.playerData.reputation = -50;
            string negativeRepJson = JsonUtility.ToJson(negativeRepData);
            SaveData negativeRepReconstructed = JsonUtility.FromJson<SaveData>(negativeRepJson);
            Assert.AreEqual(-50, negativeRepReconstructed.playerData.reputation, "Negative reputation should be preserved");

            // Test with boundary dates (day 1, day 30, month 1, month 12)
            var boundaryDateData = new SaveData();
            boundaryDateData.currentDate = new GameDate(1, 1, 1); // First possible date
            string boundaryDateJson = JsonUtility.ToJson(boundaryDateData);
            SaveData boundaryDateReconstructed = JsonUtility.FromJson<SaveData>(boundaryDateJson);
            Assert.AreEqual(boundaryDateData.currentDate, boundaryDateReconstructed.currentDate, "First date should be preserved");

            var lastDayData = new SaveData();
            lastDayData.currentDate = new GameDate(30, 12, 100); // Last day of year 100
            string lastDayJson = JsonUtility.ToJson(lastDayData);
            SaveData lastDayReconstructed = JsonUtility.FromJson<SaveData>(lastDayJson);
            Assert.AreEqual(lastDayData.currentDate, lastDayReconstructed.currentDate, "Last day of month should be preserved");

            // Test with empty string player name
            var emptyNameData = new SaveData();
            emptyNameData.playerData.playerName = "";
            string emptyNameJson = JsonUtility.ToJson(emptyNameData);
            SaveData emptyNameReconstructed = JsonUtility.FromJson<SaveData>(emptyNameJson);
            Assert.AreEqual("", emptyNameReconstructed.playerData.playerName, "Empty player name should be preserved");

            // Test with Unicode characters in names
            var unicodeData = new SaveData();
            unicodeData.playerData.playerName = "プレイヤー名前🎉";
            string unicodeJson = JsonUtility.ToJson(unicodeData);
            SaveData unicodeReconstructed = JsonUtility.FromJson<SaveData>(unicodeJson);
            Assert.AreEqual(unicodeData.playerData.playerName, unicodeReconstructed.playerData.playerName, "Unicode characters should be preserved");

            // Test with very long string
            var longNameData = new SaveData();
            longNameData.playerData.playerName = new string('A', 1000);
            string longNameJson = JsonUtility.ToJson(longNameData);
            SaveData longNameReconstructed = JsonUtility.FromJson<SaveData>(longNameJson);
            Assert.AreEqual(1000, longNameReconstructed.playerData.playerName.Length, "Long string should be preserved");

            // Test with all family help used (max 3)
            var maxFamilyHelpData = new SaveData();
            maxFamilyHelpData.playerData.familyHelpUsed = 3;
            string maxFamilyHelpJson = JsonUtility.ToJson(maxFamilyHelpData);
            SaveData maxFamilyHelpReconstructed = JsonUtility.FromJson<SaveData>(maxFamilyHelpJson);
            Assert.AreEqual(3, maxFamilyHelpReconstructed.playerData.familyHelpUsed, "Max family help should be preserved");
        }


        #region Helper Methods - Random Data Generation

        private SaveData GenerateRandomSaveData()
        {
            var data = new SaveData
            {
                saveVersion = "1.0",
                lastSavedTimestamp = DateTime.UtcNow.Ticks,
                playerData = GenerateRandomPlayerData(),
                currentDate = GenerateRandomGameDate(),
                excellenceStreak = _random.Next(0, 20),
                workHours = GenerateRandomWorkHoursData()
            };

            // Add random active events (0-5)
            int activeEventCount = _random.Next(0, 6);
            for (int i = 0; i < activeEventCount; i++)
            {
                data.activeEvents.Add(GenerateRandomEventData());
            }

            // Add random event history (0-10)
            int historyCount = _random.Next(0, 11);
            for (int i = 0; i < historyCount; i++)
            {
                data.eventHistory.Add(GenerateRandomEventData());
            }

            // Add random vendor bookings (0-5)
            int vendorBookingCount = _random.Next(0, 6);
            for (int i = 0; i < vendorBookingCount; i++)
            {
                data.vendorBookings.Add(GenerateRandomBookingEntry($"vendor_{i}"));
            }

            // Add random venue bookings (0-3)
            int venueBookingCount = _random.Next(0, 4);
            for (int i = 0; i < venueBookingCount; i++)
            {
                data.venueBookings.Add(GenerateRandomBookingEntry($"venue_{i}"));
            }

            // Generate random settings
            data.settings = GenerateRandomGameSettings();

            return data;
        }

        private PlayerData GenerateRandomPlayerData()
        {
            var stages = (BusinessStage[])Enum.GetValues(typeof(BusinessStage));
            var paths = (CareerPath[])Enum.GetValues(typeof(CareerPath));
            var zones = (MapZone[])Enum.GetValues(typeof(MapZone));

            var player = new PlayerData
            {
                playerName = $"Player_{_random.Next(1000)}",
                stage = stages[_random.Next(stages.Length)],
                careerPath = paths[_random.Next(paths.Length)],
                reputation = _random.Next(-50, 300),
                money = (float)(_random.NextDouble() * 100000),
                familyHelpUsed = _random.Next(0, 4),
                hiddenAboveAndBeyondCount = _random.Next(0, 20),
                hiddenExploitationCount = _random.Next(0, 10)
            };

            // Add random unlocked zones
            int zoneCount = _random.Next(1, zones.Length + 1);
            player.unlockedZones.Clear();
            for (int i = 0; i < zoneCount; i++)
            {
                var zone = zones[_random.Next(zones.Length)];
                if (!player.unlockedZones.Contains(zone))
                    player.unlockedZones.Add(zone);
            }

            // Add random vendor relationships
            int relationshipCount = _random.Next(0, 6);
            for (int i = 0; i < relationshipCount; i++)
            {
                player.vendorRelationships.Add(new VendorRelationship
                {
                    vendorId = $"vendor_{i}",
                    timesHired = _random.Next(0, 10),
                    reliabilityRevealed = _random.Next(2) == 1,
                    flexibilityRevealed = _random.Next(2) == 1,
                    playerNote = $"Note_{_random.Next(100)}"
                });
            }

            // Add employee data if in Stage 2
            if (player.stage == BusinessStage.Employee)
            {
                player.employeeData = new EmployeeData
                {
                    companyName = "Premier Events Co.",
                    employeeLevel = _random.Next(1, 6),
                    performanceScore = _random.Next(0, 100),
                    consecutiveNegativeReviews = _random.Next(0, 4),
                    eventsCompletedSinceReview = _random.Next(0, 4),
                    activeSideGigs = _random.Next(0, 3)
                };
            }

            return player;
        }

        private EventData GenerateRandomEventData()
        {
            var statuses = (EventStatus[])Enum.GetValues(typeof(EventStatus));
            var phases = (EventPhase[])Enum.GetValues(typeof(EventPhase));
            var personalities = (ClientPersonality[])Enum.GetValues(typeof(ClientPersonality));

            return new EventData
            {
                id = Guid.NewGuid().ToString(),
                clientId = $"client_{_random.Next(1000)}",
                clientName = $"Client_{_random.Next(1000)}",
                eventTitle = $"Event_{_random.Next(1000)}",
                eventTypeId = $"type_{_random.Next(10)}",
                subCategory = $"SubCategory_{_random.Next(20)}",
                status = statuses[_random.Next(statuses.Length)],
                phase = phases[_random.Next(phases.Length)],
                eventDate = GenerateRandomGameDate(),
                acceptedDate = GenerateRandomGameDate(),
                personality = personalities[_random.Next(personalities.Length)],
                guestCount = _random.Next(10, 500),
                budget = GenerateRandomEventBudget(),
                isCompanyEvent = _random.Next(2) == 1,
                isReferral = _random.Next(2) == 1
            };
        }

        private EventBudget GenerateRandomEventBudget()
        {
            float total = (float)(_random.NextDouble() * 50000) + 500;
            return new EventBudget
            {
                total = total,
                venueAllocation = total * 0.3f,
                cateringAllocation = total * 0.3f,
                entertainmentAllocation = total * 0.15f,
                decorationsAllocation = total * 0.15f,
                staffingAllocation = total * 0.05f,
                contingencyAllocation = total * 0.05f,
                spent = (float)(_random.NextDouble() * total * 1.2f)
            };
        }

        private GameDate GenerateRandomGameDate()
        {
            return new GameDate(
                _random.Next(1, 31),
                _random.Next(1, 13),
                _random.Next(1, 10)
            );
        }

        private WorkHoursData GenerateRandomWorkHoursData()
        {
            return new WorkHoursData
            {
                dailyCapacity = 8,
                hoursUsedToday = _random.Next(0, 12),
                overtimeUsedToday = _random.Next(0, 3)
            };
        }

        private BookingEntry GenerateRandomBookingEntry(string entityId)
        {
            var entry = new BookingEntry(entityId);
            int bookingCount = _random.Next(1, 6);
            for (int i = 0; i < bookingCount; i++)
            {
                entry.bookedDates.Add(GenerateRandomGameDate());
            }
            return entry;
        }

        private GameSettings GenerateRandomGameSettings()
        {
            return new GameSettings
            {
                musicVolume = (float)_random.NextDouble(),
                sfxVolume = (float)_random.NextDouble(),
                muteAll = _random.Next(2) == 1,
                showTutorialTips = _random.Next(2) == 1,
                notifications = new NotificationSettings
                {
                    eventDeadlines = _random.Next(2) == 1,
                    taskDeadlines = _random.Next(2) == 1,
                    newInquiries = _random.Next(2) == 1,
                    referrals = _random.Next(2) == 1,
                    financialWarnings = _random.Next(2) == 1
                },
                privacy = new PrivacySettings
                {
                    analyticsEnabled = _random.Next(2) == 1,
                    crashReportingEnabled = _random.Next(2) == 1,
                    hasGivenConsent = _random.Next(2) == 1
                },
                accessibility = new AccessibilitySettings
                {
                    textSize = (TextSize)_random.Next(3),
                    reducedMotion = _random.Next(2) == 1,
                    colorblindMode = (ColorblindMode)_random.Next(4)
                }
            };
        }

        #endregion


        #region Helper Methods - Assertion

        private void AssertSaveDataEquivalent(SaveData expected, SaveData actual, string context)
        {
            Assert.AreEqual(expected.saveVersion, actual.saveVersion, $"{context}: saveVersion mismatch");
            Assert.AreEqual(expected.lastSavedTimestamp, actual.lastSavedTimestamp, $"{context}: lastSavedTimestamp mismatch");
            Assert.AreEqual(expected.currentDate, actual.currentDate, $"{context}: currentDate mismatch");
            Assert.AreEqual(expected.excellenceStreak, actual.excellenceStreak, $"{context}: excellenceStreak mismatch");

            AssertPlayerDataEquivalent(expected.playerData, actual.playerData, context);
            AssertWorkHoursDataEquivalent(expected.workHours, actual.workHours, context);
            AssertGameSettingsEquivalent(expected.settings, actual.settings, context);
        }

        private void AssertPlayerDataEquivalent(PlayerData expected, PlayerData actual, string context)
        {
            Assert.AreEqual(expected.playerName, actual.playerName, $"{context}: playerName mismatch");
            Assert.AreEqual(expected.stage, actual.stage, $"{context}: stage mismatch");
            Assert.AreEqual(expected.careerPath, actual.careerPath, $"{context}: careerPath mismatch");
            Assert.AreEqual(expected.reputation, actual.reputation, $"{context}: reputation mismatch");
            Assert.AreEqual(expected.money, actual.money, 0.001f, $"{context}: money mismatch");
            Assert.AreEqual(expected.familyHelpUsed, actual.familyHelpUsed, $"{context}: familyHelpUsed mismatch");
            Assert.AreEqual(expected.hiddenAboveAndBeyondCount, actual.hiddenAboveAndBeyondCount, $"{context}: hiddenAboveAndBeyondCount mismatch");
            Assert.AreEqual(expected.hiddenExploitationCount, actual.hiddenExploitationCount, $"{context}: hiddenExploitationCount mismatch");

            Assert.AreEqual(expected.unlockedZones.Count, actual.unlockedZones.Count, $"{context}: unlockedZones count mismatch");
            Assert.AreEqual(expected.vendorRelationships.Count, actual.vendorRelationships.Count, $"{context}: vendorRelationships count mismatch");
        }

        private void AssertEventDataEquivalent(EventData expected, EventData actual, string context)
        {
            Assert.AreEqual(expected.id, actual.id, $"{context}: id mismatch");
            Assert.AreEqual(expected.clientId, actual.clientId, $"{context}: clientId mismatch");
            Assert.AreEqual(expected.clientName, actual.clientName, $"{context}: clientName mismatch");
            Assert.AreEqual(expected.eventTitle, actual.eventTitle, $"{context}: eventTitle mismatch");
            Assert.AreEqual(expected.status, actual.status, $"{context}: status mismatch");
            Assert.AreEqual(expected.phase, actual.phase, $"{context}: phase mismatch");
            Assert.AreEqual(expected.eventDate, actual.eventDate, $"{context}: eventDate mismatch");
            Assert.AreEqual(expected.personality, actual.personality, $"{context}: personality mismatch");
            Assert.AreEqual(expected.guestCount, actual.guestCount, $"{context}: guestCount mismatch");
            Assert.AreEqual(expected.isCompanyEvent, actual.isCompanyEvent, $"{context}: isCompanyEvent mismatch");
            Assert.AreEqual(expected.isReferral, actual.isReferral, $"{context}: isReferral mismatch");

            // Budget assertions
            Assert.AreEqual(expected.budget.total, actual.budget.total, 0.001f, $"{context}: budget.total mismatch");
            Assert.AreEqual(expected.budget.spent, actual.budget.spent, 0.001f, $"{context}: budget.spent mismatch");
        }

        private void AssertBookingEntryEquivalent(BookingEntry expected, BookingEntry actual, string context)
        {
            Assert.AreEqual(expected.entityId, actual.entityId, $"{context}: entityId mismatch");
            Assert.AreEqual(expected.bookedDates.Count, actual.bookedDates.Count, $"{context}: bookedDates count mismatch");

            for (int i = 0; i < expected.bookedDates.Count; i++)
            {
                Assert.AreEqual(expected.bookedDates[i], actual.bookedDates[i], $"{context}: bookedDates[{i}] mismatch");
            }
        }

        private void AssertWorkHoursDataEquivalent(WorkHoursData expected, WorkHoursData actual, string context)
        {
            Assert.AreEqual(expected.dailyCapacity, actual.dailyCapacity, $"{context}: dailyCapacity mismatch");
            Assert.AreEqual(expected.hoursUsedToday, actual.hoursUsedToday, $"{context}: hoursUsedToday mismatch");
            Assert.AreEqual(expected.overtimeUsedToday, actual.overtimeUsedToday, $"{context}: overtimeUsedToday mismatch");
        }

        private void AssertGameSettingsEquivalent(GameSettings expected, GameSettings actual, string context)
        {
            Assert.AreEqual(expected.musicVolume, actual.musicVolume, 0.001f, $"{context}: musicVolume mismatch");
            Assert.AreEqual(expected.sfxVolume, actual.sfxVolume, 0.001f, $"{context}: sfxVolume mismatch");
            Assert.AreEqual(expected.muteAll, actual.muteAll, $"{context}: muteAll mismatch");
            Assert.AreEqual(expected.showTutorialTips, actual.showTutorialTips, $"{context}: showTutorialTips mismatch");

            // Notification settings
            Assert.AreEqual(expected.notifications.eventDeadlines, actual.notifications.eventDeadlines, $"{context}: notifications.eventDeadlines mismatch");
            Assert.AreEqual(expected.notifications.taskDeadlines, actual.notifications.taskDeadlines, $"{context}: notifications.taskDeadlines mismatch");
            Assert.AreEqual(expected.notifications.newInquiries, actual.notifications.newInquiries, $"{context}: notifications.newInquiries mismatch");
            Assert.AreEqual(expected.notifications.referrals, actual.notifications.referrals, $"{context}: notifications.referrals mismatch");
            Assert.AreEqual(expected.notifications.financialWarnings, actual.notifications.financialWarnings, $"{context}: notifications.financialWarnings mismatch");

            // Privacy settings
            Assert.AreEqual(expected.privacy.analyticsEnabled, actual.privacy.analyticsEnabled, $"{context}: privacy.analyticsEnabled mismatch");
            Assert.AreEqual(expected.privacy.crashReportingEnabled, actual.privacy.crashReportingEnabled, $"{context}: privacy.crashReportingEnabled mismatch");
            Assert.AreEqual(expected.privacy.hasGivenConsent, actual.privacy.hasGivenConsent, $"{context}: privacy.hasGivenConsent mismatch");

            // Accessibility settings
            Assert.AreEqual(expected.accessibility.textSize, actual.accessibility.textSize, $"{context}: accessibility.textSize mismatch");
            Assert.AreEqual(expected.accessibility.reducedMotion, actual.accessibility.reducedMotion, $"{context}: accessibility.reducedMotion mismatch");
            Assert.AreEqual(expected.accessibility.colorblindMode, actual.accessibility.colorblindMode, $"{context}: accessibility.colorblindMode mismatch");
        }

        #endregion
    }
}
