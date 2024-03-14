# Hagglehaul README

[![Build Status](https://app.travis-ci.com/melaasar/cs130-template.svg?branch=master)](https://app.travis-ci.com/github/melaasar/cs130-template)
[![Release](https://img.shields.io/github/v/release/melaasar/cs130-template?label=release)](https://github.com/melaasar/cs130-template/releases/latest)

## Hagglehaul Live Version

<https://hagglehaul.azurewebsites.net/>

## What is Hagglehaul

Hagglehaul is a web app built to democratize ridesharing and schedule rides in advance within an era of surge pricing and ride deserts. Users make either a Rider or Driver account to post and place bids respectively on prescheduled trips, where Riders select the winner from the bids they've received to decide on who will be driving and how much it will cost.

## Hagglehaul Wiki

Guide for Hagglehaul users:
<https://github.com/RuralBrick/hagglehaul/wiki/User-Guide>

Installation/Deployment Guide for Devs:
<https://github.com/RuralBrick/hagglehaul/wiki/Install-and-Deploy-Guide>

## Hagglehaul Documentation

Documentation Website:
<https://ruralbrick.github.io/hagglehaul/>

## CI/CD Script Invocation

Whenever a developer **pushes a commit(s)**, the nunit-tests.yml script will be invoked. This performs tests on the code and uploads a testing artifact documenting the results to the directory "Hagglehaul-testResults".

Whenever a developer **pushes a branch to "main"**, the azure-webapp-publish.yml script will be invoked. This compiles, packages, and deploys the most current version of the app in "main" to Azure, adding the branch's features to Hagglehaul's live version.

## How to Install/Deploy the Project

Run these commands:
`git clone https://github.com/RuralBrick/hagglehaul.git`
`cd [into the Hagglehaul directory, wherever it is]`
`cd hagglehaul.client`
`npm install`
`cd ..`
`dotnet restore`
`dotnet build`
`dotnet run --no-build --launch-profile https`

Then visit <https://localhost:5173/> to see your version of the web app.
