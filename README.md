# Angora

#### RAIK 383H Software Engineering Project

---

## Building and running

To run Angora locally, clone/download the repo and open the solution `angora.sln` in Visual Studio 2013. Make sure that NuGet is allowed to download missing packages *(Tools > Options > Package Manager >>> Check both check boxes)*. Make sure that `Angora.Web` is set as your startup project. You can now build and run the solution.

A few notes on building and running:
- The web app is currently configured to use localdb. It is managed and configured using Entity Framework's code first approach. Occasionally the local `.mdf` file gets in a wonky state and the program will crash and burn with lots of exceptions. If this happens, delete the `.mdf` and `.ldf` files in `Angora.Web\App_Data`.
- LESS compilation is currently handled by Web Essentials 2013. If the styling for things looks wonky, the LESS probably needs to be recompiled, since the minified CSS is not in the repository.

Android:
Made with Android Studio ver 0.5.4 (using Gradle 0.9).
Please download Android studio and import the project with that.
Building the project manually with Gradle currently requires you create your own settings.gradle file, not to mention it is untested.

HELP! I'm getting an error! "Failed to refresh Gradle project 'Angora': The project is using an unsupported version of the Android Gradle plug-in (0.7.3). Version 0.9.0 introduced incompatible changes in the build language. Please read the migration guide to learn how to update your project. Open migration guide, fix plug-in version and re-import project."

Give it a minute. I mean actually let Android Studio sit and think for about 60 seconds. Things will (should) turn green and you will be able to build.


## Changelog

### Beta Release (3 March 2014)

- Fixing and improving architecture (proper inversion of control, database contexts)
- Pulling basic Facebook and Twitter account information at login.
- Basic account management, including linking account to both Facebook and Twitter
- Service for interacting with Foo CDN
- Event service
- Simple proof-of-concept for event feed
- Addition of CSS library and establishing consistent styles for the site
- Initial work on Android app (see below paragraph)

### Alpha Release (20 Feb 2014)

- Created initial projects
- Set up initial architecture and groundwork
- Added ability to log in using Facebook and/or Twitter
