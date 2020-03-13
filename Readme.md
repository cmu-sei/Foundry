# Foundry

This mono-repo contains a suite of applications to support the discoverability and curation of training content.

It primarily leverages `AspNetCore` and `Angular`.

## Overview

TODO: Coming soon.

More information can be found the `docs` folder.

## Developers

Requirements: dotnet-sdk 2.1
1. For each `api` app you want to run, use `dotnet run` from a terminal at that app's folder.
2. For each `ui` app you want to run, use `npm install` and `npm start` from a terminal at that app's folder.

TODO: discuss Identity integration

The Api apps support multiple database providers.  By default they are set to PostgreSQL.

Authentication to the apps is handled by a third party OpenIdentity Connect provider.  By default it expects an instance of [Identity](https://github.com/cmu-sei/Identity) running locally.

## Contributions
