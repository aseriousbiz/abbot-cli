# Abbot Command Line

A CLI (Command Line Interface) for editing Abbot skills in a local directory (Abbot Workspace).

## Installation

At the moment, installation is very manual.

1. Visit our [Releases page](https://github.com/aseriousbiz/abbot-cli/releases) and click on the latest release.
2. Grab the zip file that corresponds to your platform.
3. Unzip the file into a directory on your computer.
4. If that directory is not in your `PATH`, add it to your `PATH`.

## Usage

_Note that each of these commands assumes that you are running the command from an Abbot Workspace. An Abbot Workspace is a directory that contains your Abbot Skills. If you're running the command from a different directory, use the `--directory` (or `-d`) flag to specify the Abbot Workspace directory._

The first step is to set up an Abbot Workspace. This is just a folder with some metadata created using the CLI. This folder will contain a folder for each skill that you're working on.

You can use the `abbot auth` command to set it up. For example,

```bash
$ abbot auth

    Please visit https://ab.bot/account/apikeys to generate an authentication token. I will attempt to open your browser for you.

Type in the API Key token and hit ENTER:
```

This launches a browser to a page where you can create an Abbot API Key. If you omit the directory argument, then it will set up the current directory as an Abbot Workspace. Once you have this set up, you can download a skill to work on it.

### Download a skill

Use `abbot get` to download the code for a skill into a subfolder of the Abbot Workspace. This subfolder is named after the skill and called an Abbot Skill Workspace.


```bash
$ cd my-skills
$ abbot get my-cool-skill
Created skill directory /Users/haacked/my-skills/my-cool-skill
Edit the code in the directory. When you are ready to publish it, run

    abbot publish my-cool-skill

```

Note that this command must be run in the root directory of an Abbot Workspace __OR__ the path to the Abbot Workspace directory must be supplied like so: `abbot get my-cool-skill --directory ~/users/haacked/my-skills`

### Deploy a skill

```bash
$ abbot deploy my-cool-skill
Skill bot updated.
```

Note that this command must be run in the root directory of an Abbot Workspace __OR__ the path to the Abbot Workspace directory must be supplied like so: `abbot deploy my-skill --directory ~/users/haacked/my-skills`.

### Get the status

You can always check on the status of an Abbot Workspace with `abbot status`.

```bash
$ abbot status
The directory /Users/haacked/my-skills is an authenticated Abbot Workspace.
Organization: Serious Business (Slack T0123456)
User: Phil Haack (U0123456)
```

### Run a skill

If you want to test your local changes, you can call `abbot run` to run your local changes in the Abbot Skill Runner.

```bash
$ abbot run my-cool-skill "The arguments to the skill"
Hello The arguments to the skill
```

If there are more than one argument to the skill, the arguments must be quoted.

### Run a skill in a REPL

Sometimes you want to continuously test a skill. You can run a skill in a REPL (Read-eval-print-loop) using `abbot repl`.

## Development

This is a .NET 5 Console project.

The `Abbot.Messages` project is in a [Git Subtree](https://www.atlassian.com/git/tutorials/git-subtree). For the most part, you shouldn't have to worry about it.
However, if you make changes to `Abbot.Messages` from here, you'll want to push those changes upstream via `script/push-subtree`.

If for some reason changes were made directly to `Abbot.Messages`, you can pull those changes via `script/pull-subtree`.
