# Angora

#### RAIK 383H Software Engineering Project

---

## Building and running

To run Angora locally, clone/download the repo and open the solution `angora.sln` in Visual Studio 2013. Make sure that NuGet is allowed to download missing packages *(Tools > Options > Package Manager >>> Check both check boxes)*. Make sure that `Angora.Web` is set as your startup project. You can now build and run the solution.

A few notes on building and running:
- The web app is currently configured to use localdb. It is managed and configured using Entity Framework's code first approach. Occasionally the local `.mdf` file gets in a wonky state and the program will crash and burn with lots of exceptions. If this happens, delete the `.mdf` and `.ldf` files in `Angora.Web\App_Data`.
- LESS compilation is currently handled by Web Essentials 2013. If the styling for things looks wonky, the LESS probably needs to be recompiled, since the minified CSS is not in the repository.

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
