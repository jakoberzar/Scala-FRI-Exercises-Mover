# Scala-FRI-Exercises-Mover
A small project to help peer review code

## Folder structure
```
Root folder
- teden (the project / working folder)
- my (the folder containing your files)
- review (the folder for peer reviews)
  - 1st (1st guy's code)
  - 2nd (2nd guy's code)
  - 3rd (3rd guy's code)
- ScalaExercisesMover.exe (it should be in this directory or called from this directory)
```
The my and review folders are created automatically.

## Build
At the moment, all you have to do is clone the project, open solution with visual studio and build it.

## Usage
You can either always copy the program to the root folder or add it to path so you can call it from the root folder directly.

Run it when you're done with your code and it'll copy it to "my" folder, ready to upload to web.
When downloading peer review code, copy 1st one to review/1st folder that was created, and so on.
Afterwards, run it with paramater "1" to copy the 1st one's files to the project folder.