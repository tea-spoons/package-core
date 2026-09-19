# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.package-core` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- The editor menu entry that opened the old internal package registry now opens `github.com/tea-spoons`.

## Planned changes

- [x] Tag and publish `v1.5.0` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
<!-- review-items:start -->
- [ ] **P1** Write per-type documentation with a minimal example each. It is the package everything else builds on.
- [ ] **P1** Reduce the duplication: decide which helpers are truly shared, and make the stand-ins in logging, static-data and stacking-dialogs either thin or unnecessary.
- [ ] **P1** Declares `unity: 2022.3`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P2** In `PlayerLoopUtility` throw `InvalidOperationException` (or return a `bool`) with a clear message, and test insertion `Before`, `After` and `Append` on a fake loop.
- [ ] **P2** Mention in the README that `ReadOnlyAttribute` overlaps with NaughtyAttributes and MyBox.
- [ ] **P2** The README is only 24 lines. Add a short example for each public type.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [dbrizov/NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) | not checked | Inspector attributes such as ReadOnly and ShowIf without custom editors. |
| [Deadcows/MyBox](https://github.com/Deadcows/MyBox) | not checked | `ReadOnly`, `ConditionalField` and many other attributes and tools. |

### Findings from reading the code

- **[Docs]** The README is 24 lines and says only "Core functionality for other packages". There are 22 public types with no description: `PlayerLoopUtility` (with `Path`, `Before`, `After`, `Append`), `PlayModeEditable`, `ObjectAmount`, `ReadOnlyAttribute` and its drawer, the `GUIColor`/`GizmosColor`/`HandlesColor` scopes, `EditorWebRequest` and `SerializedPropertyExtensions`.
- **[Duplication]** Other packages now carry stand-ins for pieces of it (`GUIColor` in logging, `TryGetTargetObject` in static-data, `PlayModeEditable` and its drawer in stacking-dialogs). That is the same code in three places.
- **[Overlap]** `ReadOnlyAttribute` does what NaughtyAttributes and MyBox already do.
- **[Robustness]** `PlayerLoopUtility` throws a plain `Exception` when the system to insert before or after is not found.
<!-- review:end -->

## Notes and ideas

_Add your own here._
