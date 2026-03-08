# Fast Release

Fast Release is a Dalamud plugin for FFXIV that makes the revive/release hold-button effectively instant.

## Scope

This plugin only changes the `SelectYesno` popup while the `Revive` agent is active. It does not alter every hold-to-confirm dialog in the game.

## Build requirements

- XIVLauncher / Dalamud dev build installed locally
- .NET 10 SDK

## Third-party repo

Users can add this custom repo URL in Dalamud:

`https://raw.githubusercontent.com/ashandarei/fast-release/main/repo.json`

## Release flow

Push a tag such as `v0.1.0` and GitHub Actions will:

1. download the current Dalamud dev runtime
2. build the plugin
3. upload `latest.zip` to the GitHub release
4. update `repo.json` on `main`

## Notes

The implementation shrinks the revive hold-button duration instead of automatically releasing the player. The user still clicks the native button, but the long hold is removed.
