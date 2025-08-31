# VERIFY PROTOCOL
## Systematic Validation of All Changes

**Purpose:** Comprehensive verification to ensure changes meet requirements and maintain quality  
**When Used:** After any modification, before merging, before deployment  
**Success Criteria:** Complete verification with evidence, all requirements validated  

---

## **🔍 PROTOCOL STEPS**

### **STEP 1: CHANGE INVENTORY**
- Document all files modified, added, or deleted
- **MANDATORY:** Complete before/after file inventory
- **MANDATORY:** Line count changes documented
- **MANDATORY:** Dependency changes identified

### **STEP 2: REQUIREMENTS VERIFICATION**
- Verify all original requirements still met
- **MANDATORY:** Functional requirements validated
- **MANDATORY:** Non-functional requirements checked
- **MANDATORY:** No regression in existing functionality

### **STEP 3: CODE VERIFICATION**
- **Files < 1,000 lines:** Line-by-line verification
- **Files 1,000+ lines:** Function-by-function verification
- **MANDATORY:** All changes reviewed for correctness
- **MANDATORY:** Code quality standards maintained

### **STEP 4: TEST VERIFICATION**
- **MANDATORY:** All tests pass (unit, integration, smoke)
- **MANDATORY:** Test coverage >80% for changed code
- **MANDATORY:** New tests added for new functionality
- **MANDATORY:** No test quality degradation

### **STEP 5: INTEGRATION VERIFICATION**
- **MANDATORY:** Changes integrate properly with existing system
- **MANDATORY:** No breaking changes to public interfaces
- **MANDATORY:** Dependencies still resolve correctly
- **MANDATORY:** Performance within acceptable limits

### **STEP 6: EVIDENCE DOCUMENTATION**
- Document all verification results with evidence
- **MANDATORY:** Screenshots, logs, test results included
- **MANDATORY:** Verification confidence levels assigned
- **MANDATORY:** Any issues or concerns documented

---

## **✅ COMPLETION CHECKLIST**

- [ ] Complete file inventory documented
- [ ] All requirements verified and validated
- [ ] Code changes reviewed line-by-line or function-by-function
- [ ] All tests pass with >80% coverage
- [ ] Integration verified with no breaking changes
- [ ] Performance within acceptable limits
- [ ] Evidence documented with confidence levels
- [ ] Verification report completed

---

## **📋 VERIFICATION LEVELS**

### **LEVEL 1: BASIC VERIFICATION**
- Files changed documented
- Tests pass
- Basic functionality verified

### **LEVEL 2: COMPREHENSIVE VERIFICATION**
- Line-by-line code review
- Requirements traceability
- Integration testing

### **LEVEL 3: EXHAUSTIVE VERIFICATION**
- Multiple verification methods
- Independent verification
- Performance testing
- Security review

---

## **🚨 ANTI-PATTERNS TO AVOID**

- ❌ Skipping verification steps to save time
- ❌ Accepting failing tests "temporarily"
- ❌ Verification without documented evidence
- ❌ Assuming changes work without testing
- ❌ Incomplete requirements verification

---

## **📊 SUCCESS METRICS**

- **Completeness:** 100% of changes verified
- **Quality:** All verification evidence documented
- **Coverage:** >80% test coverage maintained
- **Integration:** No breaking changes introduced

---

**Protocol Version:** 1.0  
**Last Updated:** August 30, 2025
