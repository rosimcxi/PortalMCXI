# RFUTIL release promotion contract

## Canonical branches

- development/release work happens on an explicit version branch
- `main` is the canonical released source

## Required release sequence

1. source/basic CI gates pass
2. complete package build passes
3. fresh install package smoke passes
4. release evidence is produced
5. package is published as an immutable artifact
6. release source is promoted to `main`
7. `main` identity is verified against the promoted release

A package artifact without step 6 is not a complete source release.

## Promotion safety

Promotion to `main` must fail closed when:
- required CI is not successful
- release branch is behind/diverged unexpectedly
- VERSION / PACKAGE_ID identity is inconsistent
- package/evidence checksums are missing
- prohibited runtime SQLite/secrets are found

`ACCEPTED` is never changed by source promotion. `TESTED_TARGET` is never inferred from CI or package build.

## Current correction

10.0.52 and 10.0.53 were built successfully on version branches, while `main` remained behind because this final promotion step was absent. 10.1.1 introduces this contract and is the point at which the release process must be corrected.

