using System;
using System.Collections.Generic;
using System.Linq;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the milestone system.
    /// Manages Stage 3 milestone sequence and career path choice.
    /// Requirements: R17.1-R17.10
    /// </summary>
    public class MilestoneSystemImpl : IMilestoneSystem
    {
        /// <inheritdoc/>
        public bool ShouldTriggerMilestone(PlayerData player, MilestoneProgress milestoneProgress)
        {
            // Don't trigger if already seen and path chosen
            if (milestoneProgress.hasSeenStage3Milestone && milestoneProgress.hasChosenPath)
            {
                return false;
            }

            // Trigger when player reaches Stage 3 (SmallCompany)
            return player.stage == BusinessStage.SmallCompany;
        }

        /// <inheritdoc/>
        public CareerSummaryData GenerateCareerSummary(
            PlayerData player,
            List<EventData> eventHistory,
            DateTime journeyStartDate)
        {
            var summary = new CareerSummaryData
            {
                totalEventsCompleted = eventHistory?.Count ?? 0,
                currentReputation = player.reputation,
                JourneyStartDate = journeyStartDate,
                Stage3ReachedDate = DateTime.Now
            };

            if (eventHistory != null && eventHistory.Count > 0)
            {
                // Get first event name
                var firstEvent = eventHistory.OrderBy(e => e.acceptedDate.TotalDays).FirstOrDefault();
                summary.firstEventName = firstEvent?.eventTitle ?? "Unknown Event";

                // Get highest satisfaction event
                var highestSatisfactionEvent = eventHistory
                    .Where(e => e.results != null)
                    .OrderByDescending(e => e.results.finalSatisfaction)
                    .FirstOrDefault();

                if (highestSatisfactionEvent != null)
                {
                    summary.highestSatisfactionEventName = highestSatisfactionEvent.eventTitle;
                    summary.highestSatisfactionScore = highestSatisfactionEvent.results.finalSatisfaction;
                }
                else
                {
                    summary.highestSatisfactionEventName = "N/A";
                    summary.highestSatisfactionScore = 0f;
                }

                // Calculate total money earned from events
                summary.totalMoneyEarned = eventHistory
                    .Where(e => e.results != null)
                    .Sum(e => e.results.profit);
            }
            else
            {
                summary.firstEventName = "N/A";
                summary.highestSatisfactionEventName = "N/A";
                summary.highestSatisfactionScore = 0f;
                summary.totalMoneyEarned = 0f;
            }

            return summary;
        }

        /// <inheritdoc/>
        public MilestoneSequenceResult TriggerMilestoneSequence(
            PlayerData player,
            List<EventData> eventHistory,
            MilestoneProgress milestoneProgress,
            DateTime journeyStartDate)
        {
            var result = new MilestoneSequenceResult();

            // Check if milestone should be triggered
            if (!ShouldTriggerMilestone(player, milestoneProgress))
            {
                result.ShouldShowSequence = false;
                result.Message = milestoneProgress.hasChosenPath
                    ? "Milestone already completed."
                    : "Player has not reached Stage 3 yet.";
                return result;
            }

            // Generate career summary
            result.CareerSummary = GenerateCareerSummary(player, eventHistory, journeyStartDate);
            result.ShouldShowSequence = true;
            result.CanSkip = CanSkipSequence(milestoneProgress);
            result.Message = "Stage 3 Milestone: Time to choose your path!";

            return result;
        }

        /// <inheritdoc/>
        public PathChoiceResult ProcessPathChoice(CareerPath chosenPath, MilestoneProgress milestoneProgress)
        {
            var result = new PathChoiceResult
            {
                ChosenPath = chosenPath,
                ShouldAwardAchievement = !milestoneProgress.hasSeenStage3Milestone
            };

            // Update milestone progress
            milestoneProgress.hasChosenPath = true;
            milestoneProgress.chosenPath = chosenPath;

            // Get narrative elements based on chosen path
            result.NarrativeElements = chosenPath == CareerPath.Entrepreneur
                ? GetEntrepreneurNarrative()
                : GetCorporateNarrative();

            // Add credits sequence
            result.NarrativeElements.AddRange(GetCreditsSequence());

            return result;
        }

        /// <inheritdoc/>
        public List<NarrativeElement> GetEntrepreneurNarrative()
        {
            // Requirements: R17.4
            return new List<NarrativeElement>
            {
                new NarrativeElement
                {
                    Type = NarrativeElementType.EntrepreneurNarrative,
                    Title = "Signing the Lease",
                    Description = "You sign the lease on your very first office space. It's small, but it's yours. The beginning of something great.",
                    ImageId = "office_lease_signing"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.EntrepreneurNarrative,
                    Title = "A Thoughtful Gesture",
                    Description = "A beautiful bouquet arrives at your new office. The card reads: 'Congratulations on your new venture! - Your first client'",
                    ImageId = "congratulatory_flowers"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.EntrepreneurNarrative,
                    Title = "Family Pride",
                    Description = "Your phone rings. It's your family, calling to say how proud they are of everything you've accomplished.",
                    ImageId = "family_phone_call"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.EntrepreneurNarrative,
                    Title = "Local Recognition",
                    Description = "The local newspaper runs a small feature about your new event planning business. 'Local Entrepreneur Makes Dreams Come True'",
                    ImageId = "newspaper_feature"
                }
            };
        }

        /// <inheritdoc/>
        public List<NarrativeElement> GetCorporateNarrative()
        {
            // Requirements: R17.5
            return new List<NarrativeElement>
            {
                new NarrativeElement
                {
                    Type = NarrativeElementType.CorporateNarrative,
                    Title = "The Announcement",
                    Description = "The entire company gathers in the conference room. Your promotion to Director is announced to applause.",
                    ImageId = "company_announcement"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.CorporateNarrative,
                    Title = "Mentor's Words",
                    Description = "Your mentor takes the podium. 'I've watched this person grow from a junior planner to one of our finest. Today, they become a Director.'",
                    ImageId = "mentor_speech"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.CorporateNarrative,
                    Title = "Reserved Parking",
                    Description = "You're led to the parking garage where a spot with your name on it awaits. A small perk, but it feels significant.",
                    ImageId = "reserved_parking"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.CorporateNarrative,
                    Title = "Your New Office",
                    Description = "The door to your new office has a nameplate: your name, followed by 'Director'. You've made it.",
                    ImageId = "office_nameplate"
                }
            };
        }

        /// <inheritdoc/>
        public List<NarrativeElement> GetCreditsSequence()
        {
            // Requirements: R17.6-R17.7
            return new List<NarrativeElement>
            {
                new NarrativeElement
                {
                    Type = NarrativeElementType.Credits,
                    Title = "Congratulations!",
                    Description = "You've completed the main story of Event Planning Simulator. Thank you for playing!",
                    ImageId = "credits_header"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.Credits,
                    Title = "Game Credits",
                    Description = "Event Planning Simulator\n\nDeveloped with passion for event planners everywhere.",
                    ImageId = "credits_main"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.StoryContines,
                    Title = "Your Story Continues...",
                    Description = "But this isn't the end. Stages 4 and 5 await with new challenges, bigger events, and greater rewards. Your journey to becoming a Premier Event Planner has just begun!",
                    ImageId = "story_continues"
                },
                new NarrativeElement
                {
                    Type = NarrativeElementType.StoryContines,
                    Title = "Expansion Mode Unlocked",
                    Description = "You now have access to Expansion Mode content. Continue building your empire in Stages 4 and 5!",
                    ImageId = "expansion_unlocked"
                }
            };
        }

        /// <inheritdoc/>
        public void CompleteMilestoneSequence(MilestoneProgress milestoneProgress)
        {
            milestoneProgress.hasSeenStage3Milestone = true;
            // Enable skip for subsequent playthroughs
            milestoneProgress.canSkipMilestoneSequence = true;
        }

        /// <inheritdoc/>
        public bool CanSkipSequence(MilestoneProgress milestoneProgress)
        {
            // Requirements: R17.10 - Can skip on subsequent playthroughs
            return milestoneProgress.canSkipMilestoneSequence;
        }
    }
}
