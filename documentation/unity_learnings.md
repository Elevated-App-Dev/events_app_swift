# Unity Learnings

A collection of bugs encountered and their solutions to avoid repeating mistakes.

---

## 1. Cyclic Assembly Definition Dependencies

**Date:** January 2026

**Error Message:**
```
One or more cyclic dependencies detected between assemblies: 
Assets/Scripts/Core/EventPlannerSim.Core.asmdef, 
Assets/Scripts/Data/EventPlannerSim.Data.asmdef, 
Assets/Scripts/Tests/EditMode/EditModeTests.asmdef
```

**Root Cause:**
The `Core` assembly was configured to reference `Data`, while `Data` also referenced `Core`. This created a circular dependency that Unity cannot resolve.

**The Problem:**
```json
// Core.asmdef - WRONG
{
    "references": ["EventPlannerSim.Data"]  // Core should NOT reference Data
}

// Data.asmdef
{
    "references": ["EventPlannerSim.Core"]  // Data references Core - correct
}
```

**The Solution:**
Remove the reference from Core to Data. Core should be the foundation layer with no dependencies on other project assemblies.

```json
// Core.asmdef - CORRECT
{
    "references": []  // No project references
}
```

**Best Practice - Assembly Dependency Hierarchy:**
```
Tests (top layer)
  ↓ references
Systems / Managers (middle layer)
  ↓ references
Data (data structures layer)
  ↓ references
Core (foundation layer - enums, structs, interfaces)
  ↓ references
Nothing (no project dependencies)
```

**Prevention Rules:**
1. **Core assembly should have zero project references** - it contains only foundational types (enums, basic structs like GameDate, interfaces)
2. **Dependencies flow downward only** - higher layers reference lower layers, never the reverse
3. **Before adding a reference**, ask: "Does this create a cycle?" Check if the target assembly already references the source
4. **Keep Core minimal** - if a type needs to reference Data classes, it belongs in Data or a higher layer, not Core

---

## 2. Float Precision in Property Tests

**Date:** January 2026

**Error Message:**
```
Remaining calculation failed. Total: 66810.65, Spent: 21136.1
Expected: 45674.552734375 +/- 0.001
But was:  45674.5546875
```

**Root Cause:**
When testing float arithmetic with large numbers (tens of thousands), the precision loss in `float` type can exceed tight tolerances like `0.001`.

**The Problem:**
```csharp
// Tolerance too tight for float precision with large values
Assert.AreEqual(expected, actual, 0.001f, ...);
```

**The Solution:**
Use a more appropriate tolerance based on the magnitude of values being tested:
```csharp
// Use larger tolerance (0.01) due to float precision with large numbers
Assert.AreEqual(expected, actual, 0.01f, ...);
```

**Prevention Rules:**
1. **For float comparisons with values > 10,000**, use tolerance of at least `0.01f`
2. **For float comparisons with values > 100,000**, consider tolerance of `0.1f` or higher
3. **Alternative**: Use `decimal` type for financial calculations requiring exact precision
4. **Rule of thumb**: Float has ~7 significant digits of precision, so tolerance should be relative to value magnitude
