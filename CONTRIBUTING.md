<h1 align="center"><img src="code/assets/logos/ttt_reborn_logo.png" alt="TTTReborn logo" height="200"/></h1>

## Contributing to TTTReborn

## Table of contents

- [Creating Issues](#creating-issues)
- [Creating Pull Requests](#creating-pull-requests)
- [Continuous Integration](#continuous-integration)
- [Communication](#communication)

## Creating Issues

Before creating an issue, check [open issues](https://github.com/TTTReborn/ttt-reborn/issues?q=is%3Aissue+is%3Aopen) to see if someone has already reported it. If that's the case, feel free to add additional information to it. You can also use GitHub's 'subscribe' feature to get notified on updates.

If you have found a new issue we'd like to hear about it! Make sure to use the provided issue template to provide us with the necessary information so we can reproduce the issue.

Please make sure you only include non-private/non-sensitive information.

## Creating Pull Requests

No pull request is too small! Fixing typos, adding meaningful comments or documentation, fixing bugs, creating new features, ... everything is welcome.

Please keep in mind that we need to understand your changes before we can merge them, so be sure to express what goal you are trying to achieve and give your reasoning for it.

If you are familiar with coding and contributing on GitHub you can skip to [Adhere to Codestyle](#adhere-to-codestyle) otherwise please read on.

If you are only planning on working on a 'small' thing e.g. only changing something in a single file, you don't need to setup everything below. Simply go to the file you want to edit on GitHub and click the pencil icon in the top right. After you edited the file how you wanted simply open a new [pull request](https://github.com/TTTReborn/ttt-reborn/compare) with your changes.

### Setup your workspace

TTTReborn being a gamemode for S&Box means it is a .NET application. Therefore you need to install the .NET SDK and a code editor / IDE of your choice.

Please refer to the [.NET Installation Instructions](https://docs.microsoft.com/en-us/dotnet/core/install/) on how to install the SDK on your operating system.

If you don't already have an IDE or code editor in mind: [Visual Studio (IDE)](https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio) or [Visual Studio Code (code editor)](https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio-code) are relatively safe choices.

#### Adhere to Codestyle

We use the `.editorconfig` file to document our codestyle as is endorsed by Microsoft for .NET applications.
Most IDEs will pick up the editorconfig automatically and act accordingly. For simple code editors you might want to check out their documentation on how use the editorconfig. For example if you happen to use VSCode simply install the 'EditorConfig for VS Code
' Extension and you should be set.

##### (optionally) Install and use tools

If you want to make sure you adhere to our codestyle you can run the following .NET commands in the repository's root directory.
You don't have to use these as our [continuous integration](#continuous-integration) will inform you about problems as soon as you submit your pull request.

Install `dotnet-format` tooling:

```shell
dotnet tool install -g dotnet-format
```

Use `dotnet-format` for linting only:

```shell
dotnet-format --check -f code
```

Use `dotnet-format` to automatically format your `.cs` files:

```shell
dotnet-format -f code
```

## Continuous Integration

As of now our continuous integration only enforces codestyle. If you submit a pull request it will check for errors and create GitHub annotations in places where your code needs fixing.
Refer to [Adhere to Codestyle](#adhere-to-codestyle) for more information.

## Communication

If you want to chat with us and the community, join the [discord](https://discord.gg/Npcbb4W).

If you want to discuss something in more depth, feel free to [open a discussion](https://github.com/TTTReborn/ttt-reborn/discussions/new).
