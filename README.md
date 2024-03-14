# Hagglehaul README

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

You may also manually re-run a script invocation(s) for the most recent commit or branch. To do so:

1. Open the Hagglehaul GitHub page <https://github.com/RuralBrick/hagglehaul>
2. Click on the "Actions" tab
3. Select the most recent workflow run, located at the top of the presented list
4. Click on the "Re-run all jobs" button in the upper right

## How to Install/Deploy the Project

Run these commands:

```
git clone https://github.com/RuralBrick/hagglehaul.git
cd hagglehaul
cd hagglehaul.client
npm install
cd ..
dotnet restore
dotnet build
dotnet run --no-build --launch-profile https
```

Then visit <https://localhost:5173/> to see your version of the web app.
