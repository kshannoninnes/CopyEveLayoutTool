# Copy Eve Layout Tool
This tool allows a user to copy their account and character settings between different accounts and characters. With this tool you can set up 1 character on 1 account, then copy the layout and game settings over to any other account.

## Usage
- Download the EXE file from the releases page
- After running the file, you'll see the below window

	![Image1](Images/image1.png)

- Once you've selected a profile directory, a master core_user and/or master core_char file, and added at least 1 slave, you're good to hit copy!

	![Image2](Images/image2.png)

#### Explanation
- **Profile Directory**
  - This field dictates where your copied settings files will be copied to. If it's set to something other than the directory of the profile you intend to use, you'll just be copying files into a random directory.
- **Core User and Core Char**
  - This is where you specify the master files to be used as a base for the slave files. This should be set to the core_user and core_char files of the character who's profile you want to copy to other characters.
- **Slave Profiles**
  - This is where you select the core_user and core_char files which will be overwritten by the master files. These will be the core_user and core_char files of the characters you want to have the same settings as the master files. Only characters and accounts which have been logged into at least one will have core_char and core_user files.

## FAQ
- **Isn't this what profiles are for?**
  - No. Profiles are groups of settings files. If you set up one account within a profile, then make another account use that same profile, it will still have default settings, as you haven't set that account up within that profile yet. Think of profiles as an art gallery, and settings as the individual pieces. My tool copies one piece of art into another frame within the same gallery, allowing you two difference pieces of art with the same artwork.
- **How do I figure out which files are my Master Profile and which are my Slave Profiles?**
  - The most commonly used method is to log into the character who's profile you want to serve as the Master Profile, then log back out, and sort your profile directory by "Last Modified". The core_user and core_char files with a timestamp matching the time you logged out will be your master profile.
- **What if I only want to copy across user or account settings, but not both?**
  - Account settings are stored in the core_user files, while character settings are stored in the core_char files. You don't need to copy from both at the same time, and the tool will only copy settings from a selected master file to the same type of slave file. So if you only select a core_user file, only other core_user files will be overwritten, even if you selected one or more core_char slave files.
- **Will this tool break my EVE profile?**
  - As with any tool that plays around with any files, regardless of the context, you should always make a backup.
- **Will this tool steal my passwords?**
  - You're probably reading this on github so you can just take a look at the source code and see for yourself. CELT simply automates the tedious process of copying filenames to copies of your main character's files.
