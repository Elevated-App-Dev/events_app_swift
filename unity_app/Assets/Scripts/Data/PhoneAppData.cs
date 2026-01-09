using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Data structure for Calendar app entries.
    /// Requirements: R4.2
    /// </summary>
    [Serializable]
    public class CalendarEntry
    {
        public string id;
        public string title;
        public string description;
        public GameDate date;
        public CalendarEntryType entryType;
        public string relatedEventId;
        public string relatedTaskId;
        public bool isCompleted;
        public bool isUrgent;
        
        /// <summary>
        /// Check if this entry is for today or past due.
        /// </summary>
        public bool IsDueOrPast(GameDate currentDate)
        {
            return date.TotalDays <= currentDate.TotalDays;
        }
        
        /// <summary>
        /// Get days until this entry's date.
        /// </summary>
        public int DaysUntil(GameDate currentDate)
        {
            return GameDate.DaysBetween(currentDate, date);
        }
    }
    
    /// <summary>
    /// Types of calendar entries.
    /// </summary>
    public enum CalendarEntryType
    {
        Event,          // Scheduled event
        TaskDeadline,   // Task deadline
        PrepTask,       // Preparation task
        Meeting,        // Client/vendor meeting
        Reminder        // General reminder
    }
    
    /// <summary>
    /// Data structure for Messages app entries.
    /// Requirements: R4.3, R4.6, R4.12, R4.13
    /// </summary>
    [Serializable]
    public class MessageThread
    {
        public string threadId;
        public string contactName;
        public string contactId;
        public MessageContactType contactType;
        public List<Message> messages = new();
        public bool hasUnread;
        public GameDate lastMessageDate;
        public bool isCompanyRelated; // Stage 2: company vs personal
        
        /// <summary>
        /// Get the most recent message in the thread.
        /// </summary>
        public Message GetLatestMessage()
        {
            if (messages == null || messages.Count == 0)
                return null;
            return messages[messages.Count - 1];
        }
        
        /// <summary>
        /// Get count of unread messages.
        /// </summary>
        public int GetUnreadCount()
        {
            int count = 0;
            foreach (var msg in messages)
            {
                if (!msg.isRead) count++;
            }
            return count;
        }
    }
    
    /// <summary>
    /// Individual message within a thread.
    /// </summary>
    [Serializable]
    public class Message
    {
        public string messageId;
        public string content;
        public GameDate sentDate;
        public bool isFromPlayer;
        public bool isRead;
        public MessagePriority priority;
    }
    
    /// <summary>
    /// Types of message contacts.
    /// </summary>
    public enum MessageContactType
    {
        Client,
        Vendor,
        Company,    // Stage 2: employer
        System      // Game notifications
    }
    
    /// <summary>
    /// Message priority levels.
    /// </summary>
    public enum MessagePriority
    {
        Normal,
        Important,
        Urgent
    }
    
    /// <summary>
    /// Data structure for Bank app entries.
    /// Requirements: R4.4, R4.14
    /// </summary>
    [Serializable]
    public class BankTransaction
    {
        public string transactionId;
        public string description;
        public float amount;
        public GameDate date;
        public TransactionType transactionType;
        public TransactionCategory category;
        public string relatedEventId;
        public bool isPending;
        public bool isCompanyRelated; // Stage 2: company vs side gig
    }
    
    /// <summary>
    /// Types of bank transactions.
    /// </summary>
    public enum TransactionType
    {
        Income,
        Expense,
        Pending
    }
    
    /// <summary>
    /// Categories for bank transactions.
    /// </summary>
    public enum TransactionCategory
    {
        EventPayment,       // Payment from client
        VendorPayment,      // Payment to vendor
        VenuePayment,       // Payment to venue
        Salary,             // Stage 2: company salary
        Commission,         // Stage 2: company commission
        SideGigIncome,      // Stage 2: personal side gig income
        FamilyHelp,         // Emergency family funding
        Contingency,        // Contingency fund usage
        Other
    }
    
    /// <summary>
    /// Bank account summary data.
    /// </summary>
    [Serializable]
    public class BankAccountSummary
    {
        public float currentBalance;
        public float pendingIncome;
        public float pendingExpenses;
        
        // Stage 2 separation
        public float salaryEarnings;
        public float commissionEarnings;
        public float sideGigEarnings;
        
        /// <summary>
        /// Get total available funds (balance + pending income - pending expenses).
        /// </summary>
        public float GetAvailableFunds()
        {
            return currentBalance + pendingIncome - pendingExpenses;
        }
    }
    
    /// <summary>
    /// Data structure for Contacts app entries (vendor rolodex).
    /// Requirements: R4.5
    /// </summary>
    [Serializable]
    public class ContactEntry
    {
        public string contactId;
        public string name;
        public ContactType contactType;
        public VendorCategory vendorCategory; // Only for vendor contacts
        public VendorTier vendorTier;         // Only for vendor contacts
        public float rating;
        public string playerNotes;
        public int relationshipLevel;
        public bool reliabilityRevealed;
        public bool flexibilityRevealed;
        public int timesHired;
        public GameDate lastHiredDate;
        public bool isFavorite;
    }
    
    /// <summary>
    /// Types of contacts.
    /// </summary>
    public enum ContactType
    {
        Vendor,
        Client,
        Venue,
        Company     // Stage 2: employer contacts
    }
    
    /// <summary>
    /// Data structure for Reviews app entries.
    /// Requirements: R4.9
    /// </summary>
    [Serializable]
    public class ReviewEntry
    {
        public string reviewId;
        public string clientName;
        public string eventTitle;
        public string eventId;
        public GameDate eventDate;
        public float satisfactionScore;
        public string testimonialText;
        public bool isPublic;
        public bool isHighlighted;
    }
    
    /// <summary>
    /// Reputation summary for Reviews app.
    /// </summary>
    [Serializable]
    public class ReputationSummary
    {
        public int currentReputation;
        public int totalEventsCompleted;
        public float averageSatisfaction;
        public int excellentEvents;     // 90%+ satisfaction
        public int goodEvents;          // 75-89% satisfaction
        public int averageEvents;       // 50-74% satisfaction
        public int poorEvents;          // <50% satisfaction
        public int currentExcellenceStreak;
        public int bestExcellenceStreak;
    }
    
    /// <summary>
    /// Data structure for Tasks app entries.
    /// Requirements: R4.10
    /// </summary>
    [Serializable]
    public class TaskListEntry
    {
        public string eventId;
        public string eventTitle;
        public GameDate eventDate;
        public List<TaskItem> tasks = new();
        public int completedCount;
        public int totalCount;
        public int pendingCount;
        public int failedCount;
        
        /// <summary>
        /// Get completion percentage.
        /// </summary>
        public float GetCompletionPercent()
        {
            if (totalCount == 0) return 0f;
            return (float)completedCount / totalCount * 100f;
        }
        
        /// <summary>
        /// Check if all tasks are complete.
        /// </summary>
        public bool IsComplete()
        {
            return completedCount == totalCount && totalCount > 0;
        }
    }
    
    /// <summary>
    /// Individual task item for Tasks app.
    /// </summary>
    [Serializable]
    public class TaskItem
    {
        public string taskId;
        public string taskName;
        public string description;
        public TaskStatus status;
        public GameDate deadline;
        public int hoursRequired;
        public bool isOverdue;
        public bool usedCompanyHelp;
    }
    
    /// <summary>
    /// Data structure for Clients app entries.
    /// Requirements: R4.11
    /// </summary>
    [Serializable]
    public class ClientRecord
    {
        public string clientId;
        public string clientName;
        public ClientPersonality personality;
        public List<ClientEventHistory> eventHistory = new();
        public int totalEventsPlanned;
        public float averageSatisfaction;
        public bool isReferralSource;
        public int referralsGenerated;
        public string playerNotes;
        public GameDate firstEventDate;
        public GameDate lastEventDate;
    }
    
    /// <summary>
    /// Event history entry for a client.
    /// </summary>
    [Serializable]
    public class ClientEventHistory
    {
        public string eventId;
        public string eventTitle;
        public string eventTypeId; // References EventTypeData.eventTypeId
        public GameDate eventDate;
        public float satisfactionScore;
        public float budgetTotal;
        public bool wasReferral;
    }
}
