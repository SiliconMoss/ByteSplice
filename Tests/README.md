# ByteSplice Tests

```cmd
cd Tests
run_tests.bat
```

## Test Cases

The automated test suite includes 8 comprehensive tests:

1. **Basic Exact Match** - Simple pattern replacement
2. **Wildcard Matching** - Pattern with `??` wildcards
3. **Offset Patching** - Patching at relative offset from match
4. **Expected Matches Validation (Fail)** - Verifies safety abort on mismatch count
5. **Expected Matches Validation (Pass)** - Verifies correct match count handling
6. **Standalone Mode** - Command-line pattern/replace without config file
7. **Backup Collision Detection** - Ensures tool aborts if `.bak` exists
8. **Dry Run Mode** - Verifies no changes are written in dry-run

All tests validate ByteSplice functionality and safety features.

## Notes

- All test files are automatically cleaned up after each test run
- Tests use small fixture files (< 1KB) to keep the repository lightweight
