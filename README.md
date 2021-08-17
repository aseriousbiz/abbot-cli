# Abbot Command Line

A CLI (Command Line Interface) for editing Abbot skills in a local directory.

## Usage

The first step is to set up an Abbot Skills Development Environment (ASDE). This is just a folder with some metadata created using the CLI. This folder will contain a folder for each skill that you're working on.

You can use the `abbot auth` command to set it up. For example,

```bash
$ abbot auth ./my-skills

            Please visit https://ab.bot/account/apikeys to generate an authentication token. I will attempt to open your browser for you.

Type in the API Key token and hit ENTER:
```

This launches a browser to a page where you can create an Abbot API Key. If you omit the directory argument, then it will set up the current directory as an ASDE. Once you have this set up, you can download a skill to work on it.


```bash
$ cd my-skills
$ abbot download my-cool-skill
Created skill directory /Users/haacked/my-skills/my-cool-skill
Edit the code in the directory. When you are ready to publish it, run

    abbot publish my-cool-skill

```

Note that this command must be run in the root directory of the ASDE __OR__ the path to the ASDE must be supplied like so: `abbot download my-cool-skill ~/users/haacked/my-skills`

## Deploy a skill

```bash
$ abbot deploy my-cool-skill
Skill bot updated.
```

Note that this command must also be run in the __root__ directory of the ASDE  __OR__ the path to the ASDE must be supplied like so: `abbot deploy my-skill ~/users/haacked/my-skills`.

You can always check on the status of an ASDE directory with `abbot status`.

```bash
$ abbot status .
The directory /Users/haacked/my-skills is an authenticated Abbot Skill Development environment.
Organization: Serious Business (Slack T0123456)
User: Phil Haack (U0123456)
```
