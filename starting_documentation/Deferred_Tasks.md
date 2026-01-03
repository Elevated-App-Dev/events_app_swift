# Deferred Tasks

A running list of items to tackle later during development.

---

## Game Design Gaps

### Stage Transition Requirements - Incomplete

**Status:** Needs Definition  
**Priority:** Medium  
**Related Document:** Event_Planner_Sim_Game_Design_Document.md

The stage transition requirements are inconsistently defined across the 5 progression stages:

| Transition | Requirements | Status |
|------------|--------------|--------|
| Stage 1 → Stage 2 | Reputation 10, Save $2,000 | ✅ Defined |
| Stage 2 → Stage 3 | Employee level 5, Save $10,000, Personal reputation 20 | ✅ Defined |
| Stage 3 → Stage 4 | Reputation 30, Save $30,000 | ⚠️ Partially defined (no explicit unlock stated) |
| Stage 4 → Stage 5 | Not specified | ❌ Missing |

**Notes:**
- Stage 2→3 is unique: presented as a player *choice* to leave employment rather than automatic unlock. Consider if "stay for stability" path should have gameplay implications.
- Need to define Stage 4→5 requirements (reputation level? revenue milestone? number of successful high-complexity events?)
- Consider whether all transitions should follow the same pattern (reputation + money) or if later stages should have different criteria (e.g., complete X weddings, achieve Y average satisfaction)

---

## How to Use This Document

Add new deferred items using this format:

```
### [Task Title]

**Status:** [Needs Definition | In Progress | Blocked | Ready for Review]  
**Priority:** [High | Medium | Low]  
**Related Document:** [filename if applicable]

[Description of the task or issue]

**Notes:**
- [Additional context or considerations]
```
