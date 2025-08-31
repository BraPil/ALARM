# ADAPT PROTOCOL
## External Dependency Isolation Through Clean Interfaces

**Purpose:** Create adapter interfaces that isolate external dependencies from domain logic  
**When Used:** Integrating AutoCAD, Oracle, Framework APIs, or any external dependency  
**Success Criteria:** Clean interfaces, isolated dependencies, comprehensive test doubles  

---

## **🔧 PROTOCOL STEPS**

### **STEP 1: DEPENDENCY ANALYSIS**
- Identify all external API touch points
- Document current coupling patterns
- Analyze dependency complexity and risk
- Define isolation boundaries

### **STEP 2: INTERFACE DESIGN**
- **MANDATORY:** Design domain-focused interfaces (not API-focused)
- **MANDATORY:** Use domain language, not external API terminology
- **MANDATORY:** Include comprehensive error handling patterns
- **MANDATORY:** Support both sync and async operations where needed

### **STEP 3: ADAPTER IMPLEMENTATION**
- Implement concrete adapters using external APIs
- **MANDATORY:** All external API calls contained within adapter
- **MANDATORY:** Convert external types to domain types
- **MANDATORY:** Comprehensive error handling and logging

### **STEP 4: TEST DOUBLE CREATION**
- Create test doubles for all adapter interfaces
- **MANDATORY:** Support all interface methods
- **MANDATORY:** Configurable behavior for different test scenarios
- **MANDATORY:** Failure simulation capabilities

### **STEP 5: INTEGRATION VERIFICATION**
- **MANDATORY:** Domain layer has zero direct external dependencies
- **MANDATORY:** All external calls go through adapters
- **MANDATORY:** Test doubles work in all test scenarios
- **MANDATORY:** Performance acceptable (within 10% of direct calls)

---

## **✅ COMPLETION CHECKLIST**

- [ ] Clean domain-focused interfaces designed
- [ ] Concrete adapters implemented with full API coverage
- [ ] Test doubles created for all interfaces
- [ ] Domain layer isolated from external dependencies
- [ ] Error handling comprehensive and tested
- [ ] Performance within acceptable limits
- [ ] Integration tests pass with real dependencies
- [ ] Unit tests pass with test doubles

---

## **🏗️ ADAPTER ARCHITECTURE**

```
Domain Layer (Pure C#)
    ↓ (Interfaces Only)
Adapter Layer (Interface Implementations)
    ↓ (External API Calls)
Interop Layer (P/Invoke, COM, Native)
```

---

## **🚨 ANTI-PATTERNS TO AVOID**

- ❌ Leaking external types into domain layer
- ❌ Direct external API calls from domain code
- ❌ Adapter interfaces that mirror external APIs exactly
- ❌ Missing error handling for external failures
- ❌ Test doubles that don't match real adapter behavior

---

## **📊 SUCCESS METRICS**

- **Isolation:** 0 direct external dependencies in domain layer
- **Coverage:** 100% external API calls through adapters
- **Testability:** 100% unit tests use test doubles
- **Performance:** <10% overhead vs direct calls

---

**Protocol Version:** 1.0  
**Last Updated:** August 30, 2025
