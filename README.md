# Abbot Command Line

A CLI (Command Line Interface) for editing Abbot skills in a local directory.

## Installation

At the moment, installation is very manual.

1. Visit our [Releases page](https://github.com/aseriousbiz/abbot-cli/releases) and click on the latest release.
2. Grab the zip file that corresponds to your platform.
3. Unzip the file into a directory on your computer.
4. If that directory is not in your `PATH`, add it to your `PATH`.

## Usage

The first step is to set up an Abbot Skills folder. This is just a folder with some metadata created using the CLI. This folder will contain a folder for each skill that you're working on.

You can use the `abbot auth` command to set it up. For example,

```bash
$ abbot auth ./my-skills

            Please visit https://ab.bot/account/apikeys to generate an authentication token. I will attempt to open your browser for you.

Type in the API Key token and hit ENTER:
```

This launches a browser to a page where you can create an Abbot API Key. If you omit the directory argument, then it will set up the current directory as an Abbot Skills folder. Once you have this set up, you can download a skill to work on it.

### Download a skill

Use `abbot get` to download the code for a skill into an Abbot Skills folder.


```bash
$ cd my-skills
$ abbot get my-cool-skill
Created skill directory /Users/haacked/my-skills/my-cool-skill
Edit the code in the directory. When you are ready to publish it, run

    abbot publish my-cool-skill

```

Note that this command must be run in the root directory of an Abbot Skills folder __OR__ the path to the Abbot Skills folder must be supplied like so: `abbot get my-cool-skill ~/users/haacked/my-skills`

### Deploy a skill

```bash
$ abbot deploy my-cool-skill
Skill bot updated.
```

Note that this command must also be run in the __root__ directory of the Abbot Skills folder  __OR__ the path to the Abbot Skills Folder must be supplied like so: `abbot deploy my-skill ~/users/haacked/my-skills`.

### Get the status

You can always check on the status of an Abbot Skills folder with `abbot status`.

```bash
$ abbot status .
The directory /Users/haacked/my-skills is an authenticated Abbot Skill Development environment.
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

## Development

This is a .NET 5 Console project.

The `Abbot.Messages` project is in a [Git Subtree](https://www.atlassian.com/git/tutorials/git-subtree). For the most part, you shouldn't have to worry about it.
However, if you make changes to `Abbot.Messages` from here, you'll want to push those changes upstream via `script/push-subtree`.

If for some reason changes were made directly to `Abbot.Messages`, you can pull those changes via `script/pull-subtree`.
