# How to Contribute

We love Pull Requests! Your contributions help make this project great.

## Getting Started

So you want to contribute? Great! Contributions take many forms from submitting issues, writing docs, to making code changes. We welcome it all.

But first things first...

* Make sure you have a [GitHub account](https://github.com/signup/free)
* Create an issue describing your requested change, assuming one does not already exist.
  * Clearly describe the issue including steps to reproduce when it is a bug.
  * Make sure you fill in the earliest version that you know has the issue.
* Fork the repository on GitHub by clicking on the "Clone in Windows" button or run the following command in a git shell.
```
git clone https://github.com/aseriousbiz/abbot-api-messages
```
* To build the project you will need to have the [.net 5 sdk](https://dotnet.microsoft.com/download/dotnet/5.0) installed.

## Making Changes

* Create a topic branch off `main` (don't work directly on `main`).
* Make commits of logical units.
* Provide descriptive commit messages.
* Make sure you have added the necessary tests for your changes.
* Run _all_ the tests to assure nothing else was accidentally broken.

## Submitting Changes

* Push your changes to a topic branch in your fork of the repository.
* Submit a pull request. Note what issue/issues your patch fixes.

Some things that will increase the chance that your pull request is accepted.

* Follow existing code conventions. Most of what we do follows standard .NET conventions except in a few places.
* Include unit tests that would otherwise fail without your code, but pass with it.
* Update the documentation, the surrounding one, examples elsewhere, guides, whatever is affected by your contribution


# Additional Resources

* [General GitHub documentation](http://help.github.com/)
* [GitHub pull request documentation](https://help.github.com/articles/creating-a-pull-request/)
