# ![Auderus logo](/Angora.Web/Images/AuderusLogos/LogoFull.png)

#### RAIK 383H Software Engineering Project

####The final release (v0.3) is (or at least was at some point in the past) [available here](http://seteam4.azurewebsites.net/).

---

## Building and running

To run Auderus locally, clone/download the repo and open the solution `angora.sln` in Visual Studio 2013. Make sure that NuGet is allowed to download missing packages *(Tools > Options > Package Manager >>> Check both check boxes)*. Make sure that `Angora.Web` is set as your startup project. You can now build and run the solution.

A few notes on building and running locally:
- The web app is currently configured to use localdb. It is managed and configured using Entity Framework's code first approach. Occasionally the local `.mdf` file gets in a wonky state and the program will crash and burn with lots of exceptions. If this happens, delete the `.mdf` and `.ldf` files in `Angora.Web\App_Data`.
- LESS compilation is currently handled by Web Essentials 2013. While the entirety of the compiled CSS is in the git repo, any changes made to any .less files will need to be recompiled using Web Essentials.
- The simplex solver depends on Microsoft.Solver.Foundation, which is self contained in the repo at `/Microsoft.Sover.Foundation/Microsoft.Solver.Foundation.dll`. Occasionally, the reference to this dll will be reset and it cannot be found. If this happens, remove the reference in the `Angora.Services` project and manually re-add it.

### Android:
Made with Android Studio ver 0.5.4 (using Gradle 0.9).
Please download Android studio and import the project with that.
Building the project manually with Gradle currently requires you create your own settings.gradle file, not to mention it is generally untested.

*HELP! I'm getting an error!*

> "Failed to refresh Gradle project 'Angora': The project is using an unsupported version of the Android Gradle plug-in (0.7.3). Version 0.9.0 introduced incompatible changes in the build language. Please read the migration guide to learn how to update your project. Open migration guide, fix plug-in version and re-import project."

Give it a minute. I mean actually let Android Studio sit and think for about 60 seconds. Things will (should) turn green and you will be able to build. Software sucks sometimes.


## Changelog

### Final Release (1 May 2014)

- Fleshed out events (proper properties)
- Location search for events (on creation)
- "Find a Time" - Doodle-esque scheduling
- Editing events
- Full event feed (events are currently all globally visible)
- Event details and posts page
- Post comments and photos to events online
- Automatically post to Facebook as well
- Simplex solver for FooCDN usage optimization
- About page
- Improved styling

#### Android

- Log in directly with Facebook or Twitter
- Snap & Go - take photos and upload them directly to an event.
- Event list - see all of your events
- Very primative profile view


### Beta Release (3 March 2014)

- Fixing and improving architecture (proper inversion of control, database contexts)
- Pulling basic Facebook and Twitter account information at login.
- Basic account management, including linking account to both Facebook and Twitter
- Service for interacting with Foo CDN
- Event service
- Simple proof-of-concept for event feed
- Addition of CSS library and establishing consistent styles for the site
- Initial work on Android app

### Alpha Release (20 Feb 2014)

- Created initial projects
- Set up initial architecture and groundwork
- Added ability to log in using Facebook and/or Twitter
