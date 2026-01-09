using System;
using System.Collections.Generic;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Tests for MilestoneSystem.
    /// Feature: event-planner-simulator
    /// Requirements: R17.1-R17.10
    /// </summary>
    [TestFixture]
    public class MilestoneSystemTests
    {
        private MilestoneSystemImpl _milestoneSystem;

        [SetUp]
        public void Setup()
        {
            _milestoneSystem = new MilestoneSystemImpl();
        }

        #region ShouldTriggerMilestone Tests (R17.1)

        [Test]
        public void ShouldTriggerMilestone_Stage3NotSeen_ReturnsTrue()
        {
            var player = new PlayerData { stage = BusinessStage.SmallCompany };
            var progress = new MilestoneProgress { hasSeenStage3Milestone = false, hasChosenPath = false };

            bool result = _milestoneSystem.ShouldTriggerMilestone(player, progress);

            Assert.IsTrue(result, "Should trigger milestone when player reaches Stage 3 and hasn't seen it");
        }

        [Test]
        public void ShouldTriggerMilestone_Stage3AlreadySeen_ReturnsFalse()
        {
            var player = new PlayerData { stage = BusinessStage.SmallCompany };
            var progress = new MilestoneProgress { hasSeenStage3Milestone = true, hasChosenPath = true };

            bool result = _milestoneSystem.ShouldTriggerMilestone(player, progress);

            Assert.IsFalse(result, "Should not trigger milestone when already seen and path chosen");
        }

        [Test]
        public void ShouldTriggerMilestone_NotStage3_ReturnsFalse()
        {
            var stages = new[] { BusinessStage.Solo, BusinessStage.Employee, BusinessStage.Established, BusinessStage.Premier };
            var progress = new MilestoneProgress { hasSeenStage3Milestone = false, hasChosenPath = false };

            foreach (var stage in stages)
            {
                var player = new PlayerData { stage = stage };
                bool result = _milestoneSystem.ShouldTriggerMilestone(player, progress);

                Assert.IsFalse(result, $"Should not trigger milestone at stage {stage}");
            }
        }

        #endregion

        #region GenerateCareerSummary Tests (R17.2)

        [Test]
        public void GenerateCareerSummary_WithEvents_ReturnsCorrectStats()
        {
            var player = new PlayerData { reputation = 75 };
            var journeyStart = DateTime.Now.AddDays(-30);
            var events = CreateTestEventHistory();

            var summary = _milestoneSystem.GenerateCareerSummary(player, events, journeyStart);

            Assert.AreEqual(3, summary.totalEventsCompleted, "Should count all completed events");
            Assert.AreEqual(75, summary.currentReputation, "Should reflect current reputation");
            Assert.AreEqual("First Event", summary.firstEventName, "Should identify first event");
            Assert.AreEqual("Best Event", summary.highestSatisfactionEventName, "Should identify highest satisfaction event");
            Assert.AreEqual(95f, summary.highestSatisfactionScore, "Should record highest satisfaction score");
        }

        [Test]
        public void GenerateCareerSummary_NoEvents_ReturnsDefaultValues()
        {
            var player = new PlayerData { reputation = 50 };
            var journeyStart = DateTime.Now.AddDays(-10);

            var summary = _milestoneSystem.GenerateCareerSummary(player, new List<EventData>(), journeyStart);

            Assert.AreEqual(0, summary.totalEventsCompleted);
            Assert.AreEqual("N/A", summary.firstEventName);
            Assert.AreEqual("N/A", summary.highestSatisfactionEventName);
            Assert.AreEqual(0f, summary.highestSatisfactionScore);
        }

        [Test]
        public void GenerateCareerSummary_NullEvents_ReturnsDefaultValues()
        {
            var player = new PlayerData { reputation = 50 };
            var journeyStart = DateTime.Now.AddDays(-10);

            var summary = _milestoneSystem.GenerateCareerSummary(player, null, journeyStart);

            Assert.AreEqual(0, summary.totalEventsCompleted);
            Assert.AreEqual("N/A", summary.firstEventName);
        }

        [Test]
        public void GenerateCareerSummary_CalculatesTotalMoneyEarned()
        {
            var player = new PlayerData { reputation = 50 };
            var journeyStart = DateTime.Now.AddDays(-30);
            var events = new List<EventData>
            {
                CreateEventWithProfit("Event 1", 500f, 1),
                CreateEventWithProfit("Event 2", 750f, 2),
                CreateEventWithProfit("Event 3", 1000f, 3)
            };

            var summary = _milestoneSystem.GenerateCareerSummary(player, events, journeyStart);

            Assert.AreEqual(2250f, summary.totalMoneyEarned, "Should sum all event profits");
        }

        #endregion

        #region TriggerMilestoneSequence Tests (R17.1-R17.2)

        [Test]
        public void TriggerMilestoneSequence_ValidTrigger_ReturnsSequenceData()
        {
            var player = new PlayerData { stage = BusinessStage.SmallCompany, reputation = 60 };
            var progress = new MilestoneProgress();
            var events = CreateTestEventHistory();
            var journeyStart = DateTime.Now.AddDays(-30);

            var result = _milestoneSystem.TriggerMilestoneSequence(player, events, progress, journeyStart);

            Assert.IsTrue(result.ShouldShowSequence, "Should show sequence when conditions met");
            Assert.IsNotNull(result.CareerSummary, "Should include career summary");
            Assert.AreEqual(3, result.CareerSummary.totalEventsCompleted);
        }

        [Test]
        public void TriggerMilestoneSequence_AlreadyCompleted_DoesNotShow()
        {
            var player = new PlayerData { stage = BusinessStage.SmallCompany };
            var progress = new MilestoneProgress { hasSeenStage3Milestone = true, hasChosenPath = true };
            var events = CreateTestEventHistory();
            var journeyStart = DateTime.Now.AddDays(-30);

            var result = _milestoneSystem.TriggerMilestoneSequence(player, events, progress, journeyStart);

            Assert.IsFalse(result.ShouldShowSequence, "Should not show sequence when already completed");
        }

        #endregion

        #region ProcessPathChoice Tests (R17.3-R17.5)

        [Test]
        public void ProcessPathChoice_Entrepreneur_ReturnsEntrepreneurNarrative()
        {
            var progress = new MilestoneProgress();

            var result = _milestoneSystem.ProcessPathChoice(CareerPath.Entrepreneur, progress);

            Assert.AreEqual(CareerPath.Entrepreneur, result.ChosenPath);
            Assert.IsTrue(progress.hasChosenPath, "Should mark path as chosen");
            Assert.AreEqual(CareerPath.Entrepreneur, progress.chosenPath);
            Assert.IsTrue(result.NarrativeElements.Exists(e => e.Type == NarrativeElementType.EntrepreneurNarrative),
                "Should include entrepreneur narrative elements");
        }

        [Test]
        public void ProcessPathChoice_Corporate_ReturnsCorporateNarrative()
        {
            var progress = new MilestoneProgress();

            var result = _milestoneSystem.ProcessPathChoice(CareerPath.Corporate, progress);

            Assert.AreEqual(CareerPath.Corporate, result.ChosenPath);
            Assert.IsTrue(progress.hasChosenPath);
            Assert.AreEqual(CareerPath.Corporate, progress.chosenPath);
            Assert.IsTrue(result.NarrativeElements.Exists(e => e.Type == NarrativeElementType.CorporateNarrative),
                "Should include corporate narrative elements");
        }

        [Test]
        public void ProcessPathChoice_FirstTime_ShouldAwardAchievement()
        {
            var progress = new MilestoneProgress { hasSeenStage3Milestone = false };

            var result = _milestoneSystem.ProcessPathChoice(CareerPath.Entrepreneur, progress);

            Assert.IsTrue(result.ShouldAwardAchievement, "Should award achievement on first completion");
        }

        [Test]
        public void ProcessPathChoice_SubsequentTime_ShouldNotAwardAchievement()
        {
            var progress = new MilestoneProgress { hasSeenStage3Milestone = true };

            var result = _milestoneSystem.ProcessPathChoice(CareerPath.Entrepreneur, progress);

            Assert.IsFalse(result.ShouldAwardAchievement, "Should not award achievement on subsequent completions");
        }

        [Test]
        public void ProcessPathChoice_IncludesCreditsSequence()
        {
            var progress = new MilestoneProgress();

            var result = _milestoneSystem.ProcessPathChoice(CareerPath.Entrepreneur, progress);

            Assert.IsTrue(result.NarrativeElements.Exists(e => e.Type == NarrativeElementType.Credits),
                "Should include credits sequence");
            Assert.IsTrue(result.NarrativeElements.Exists(e => e.Type == NarrativeElementType.StoryContines),
                "Should include 'Your Story Continues' element");
        }

        #endregion

        #region Narrative Elements Tests (R17.4-R17.7)

        [Test]
        public void GetEntrepreneurNarrative_ReturnsFourElements()
        {
            var elements = _milestoneSystem.GetEntrepreneurNarrative();

            Assert.AreEqual(4, elements.Count, "Entrepreneur narrative should have 4 elements");
            Assert.IsTrue(elements.TrueForAll(e => e.Type == NarrativeElementType.EntrepreneurNarrative));
        }

        [Test]
        public void GetEntrepreneurNarrative_ContainsRequiredScenes()
        {
            var elements = _milestoneSystem.GetEntrepreneurNarrative();

            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Lease")), "Should include office lease signing");
            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Gesture") || e.Description.Contains("flowers")), 
                "Should include congratulatory flowers");
            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Family") || e.Description.Contains("family")), 
                "Should include family phone call");
            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Recognition") || e.Description.Contains("newspaper")), 
                "Should include newspaper feature");
        }

        [Test]
        public void GetCorporateNarrative_ReturnsFourElements()
        {
            var elements = _milestoneSystem.GetCorporateNarrative();

            Assert.AreEqual(4, elements.Count, "Corporate narrative should have 4 elements");
            Assert.IsTrue(elements.TrueForAll(e => e.Type == NarrativeElementType.CorporateNarrative));
        }

        [Test]
        public void GetCorporateNarrative_ContainsRequiredScenes()
        {
            var elements = _milestoneSystem.GetCorporateNarrative();

            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Announcement")), "Should include company announcement");
            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Mentor")), "Should include mentor speech");
            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Parking")), "Should include reserved parking");
            Assert.IsTrue(elements.Exists(e => e.Title.Contains("Office") || e.Description.Contains("nameplate")), 
                "Should include office nameplate");
        }

        [Test]
        public void GetCreditsSequence_ContainsCreditsAndContinuation()
        {
            var elements = _milestoneSystem.GetCreditsSequence();

            Assert.IsTrue(elements.Exists(e => e.Type == NarrativeElementType.Credits), 
                "Should include credits element");
            Assert.IsTrue(elements.Exists(e => e.Type == NarrativeElementType.StoryContines), 
                "Should include story continues element");
            Assert.IsTrue(elements.Exists(e => e.Description.Contains("Your story continues") || 
                                               e.Title.Contains("Your Story Continues")),
                "Should include 'Your story continues' messaging");
        }

        #endregion

        #region CompleteMilestoneSequence Tests (R17.10)

        [Test]
        public void CompleteMilestoneSequence_UpdatesProgress()
        {
            var progress = new MilestoneProgress();

            _milestoneSystem.CompleteMilestoneSequence(progress);

            Assert.IsTrue(progress.hasSeenStage3Milestone, "Should mark milestone as seen");
            Assert.IsTrue(progress.canSkipMilestoneSequence, "Should enable skip for future playthroughs");
        }

        [Test]
        public void CanSkipSequence_FirstPlaythrough_ReturnsFalse()
        {
            var progress = new MilestoneProgress { canSkipMilestoneSequence = false };

            bool canSkip = _milestoneSystem.CanSkipSequence(progress);

            Assert.IsFalse(canSkip, "Should not be able to skip on first playthrough");
        }

        [Test]
        public void CanSkipSequence_SubsequentPlaythrough_ReturnsTrue()
        {
            var progress = new MilestoneProgress { canSkipMilestoneSequence = true };

            bool canSkip = _milestoneSystem.CanSkipSequence(progress);

            Assert.IsTrue(canSkip, "Should be able to skip on subsequent playthroughs");
        }

        #endregion

        #region Helper Methods

        private List<EventData> CreateTestEventHistory()
        {
            return new List<EventData>
            {
                new EventData
                {
                    id = "1",
                    eventTitle = "First Event",
                    acceptedDate = new GameDate(1, 1, 1),
                    results = new EventResults { finalSatisfaction = 80f, profit = 500f }
                },
                new EventData
                {
                    id = "2",
                    eventTitle = "Second Event",
                    acceptedDate = new GameDate(15, 1, 1),
                    results = new EventResults { finalSatisfaction = 85f, profit = 750f }
                },
                new EventData
                {
                    id = "3",
                    eventTitle = "Best Event",
                    acceptedDate = new GameDate(1, 2, 1),
                    results = new EventResults { finalSatisfaction = 95f, profit = 1000f }
                }
            };
        }

        private EventData CreateEventWithProfit(string title, float profit, int dayOffset)
        {
            return new EventData
            {
                id = Guid.NewGuid().ToString(),
                eventTitle = title,
                acceptedDate = new GameDate(dayOffset, 1, 1),
                results = new EventResults { finalSatisfaction = 80f, profit = profit }
            };
        }

        #endregion
    }
}
