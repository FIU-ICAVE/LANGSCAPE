# Capstone II: LANGSCAPE

LANGSCAPE: Language-driven Architectural VR Scape is an innovative capstone project that aims to develop an immersive and interactive virtual reality (VR) experience, 
allowing users to control their environment using natural language exclusively. By creating an intuitive and seamless interface between the user and the VR environment, 
LANGSCAPE enables efficient communication with an AI assistant for designing and manipulating a 3D space. The project's primary goal is to enhance user engagement by bridging 
the gap between the digital and physical realms by exploring recent advances in Large Language Models (LLM), ultimately providing an innovative and accessible platform for 
users to explore and create architectural structures.

## Installation

### Unity Setup

#### 1. Install [Unity Hub](https://unity3d.com/get-unity/download).

#### 2. Open the Unity Hub application and go to the Installs menu on the left hand side.

#### 3. Install the 2021.3.25f1 Editor with no packages.

#### 4. Install [Visual Studio Community 2022](https://visualstudio.microsoft.com/vs/).


### Project Setup

#### 1. Download [GIT](https://git-scm.com/downloads).

#### 2. Open a new terminal and type the following command to build your repository.

```
git clone https://github.com/commanser-sar/LANGSCAPE.git
```

#### 3. Open the new folder through Unity Hub.

After this, Unity should have populated your directory with all the proper files required to open and execute the project using the Unity Editor. I would recommend sticking with the Visual Studio 2019 that comes with the download, as using VS Code can result in not being able to see the Unity packages with intellisense.

[Unity Documentation](https://docs.unity.com/)

## Quick GIT Guide

[GIT Documentation](https://git-scm.com/doc)

#### How to create a new branch locally (do this before your changes on someone else's branch):

```
git checkout -b <branch_name>
```

#### How to switch to another GitHub branch:
```
git checkout <existing_branch>
```

#### How to merge changes from another branch into yours:
There are two ways to do this, from the command line or GitHub. Using the command line, you have to
resolve conflicts manually and could become a hastle. If you make a pull request on GitHub, you get 
to see what the conflicts are (if any) and pick which files to merge.
```
git pull origin <branch>
```

#### How to push changes to Github:

Use this command to verify the branch you are pushing to is correct, and see your changes.
```
git status
```

Use this command to add the changes you want to add to the commit.
```
git add <path/to/filename>
```

Then, use this command to commit your changes to have them queued for pushing.
```
git commit -m "<short description of commit>"
```

Lasty, push your changes into the Github with this command.
```
git push
```

Alternatively, if you made changes on the wrong branch, use this command to push.
```
git push origin <current_branch>:<correct_branch>
```

If you are not sure how to do it right, or are worried you will break something, try googling it or just ask for help.

## Building and Exporting

To build the project and obtain an executable, you have to specify what kind of build you want. This depends on the operating system and what kind of computer architecture you intend to target.

#### 1. Go to **File > Build Settings**

#### 2. Select your Operating System or the target platform's Operating System.

#### 3. Select the appropriate build architecture, ideally 64x.

#### 4. Hit build and save it in a /Bin directory in your project folder.
