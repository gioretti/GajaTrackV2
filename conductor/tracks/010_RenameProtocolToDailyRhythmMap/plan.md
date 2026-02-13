# Implementation Plan: Rename Protocol to Daily Rhythm Map

## Phase 1: Application & Domain (The Core)
- [ ] Rename GajaTrack.Application/DTOs/Protocol folder to DailyRhythmMap.
- [ ] Rename ProtocolDtos.cs to DailyRhythmMapDtos.cs.
- [ ] Perform text replacement for DTO names and namespaces in DailyRhythmMapDtos.cs.
- [ ] Rename IProtocolService.cs to IDailyRhythmMapService.cs.
- [ ] Rename ProtocolDomainService.cs to DailyRhythmMapDomainService.cs.

## Phase 2: Infrastructure & API
- [ ] Rename ProtocolService.cs to DailyRhythmMapService.cs.
- [ ] Update Program.cs for DI and API mapping.
- [ ] Update BabyPlusImportService.cs or other consumers if they use the protocol service.

## Phase 3: Web UI
- [ ] Rename ProtocolPage.razor and its folder/dependencies.
- [ ] Rename ProtocolChart.razor.
- [ ] Rename ProtocolSummaryCell.razor.
- [ ] Update navigation in MainLayout.razor.

## Phase 4: Tests
- [ ] Rename ProtocolServiceTests.cs.
- [ ] Rename ProtocolApiTests.cs.
- [x] Update all test content.

## Phase 5: Verification
- [ ] Run dotnet build.
- [ ] Run dotnet test.
