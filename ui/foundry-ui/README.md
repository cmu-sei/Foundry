# Step Browser

### Required Software
1. Node.js SDK which includes NPM
2. Recommended to Update NPM ( npm update -g npm ).
3. Install latest Angular-CLI

### Run the App
1. install the NPM dependencies `npm install`
2. run the server `ng serve`

#### Settings

All configurable values (urls, etc) should be made to use the SettingsService. The SettingsService loads it's values from configuration files located in /assets/config/. There are two files used for this, as follows:

1. settings.json

    This file is committed to source control, and holds default values for all settings. Changes should only be made to this file to add new settings, or change the default value of a setting that will affect everyone who pulls down the project.

2. settings.env.json

    This file is NOT committed to source control, and will differ for each environment. Settings can be placed into this file and they will override settings found in settings.json. Any settings not found in this file will default to the values in settings.json.

In a production environment, settings.env.json should contain only the settings that need to be changed for that environment, and settings.json serves as a reference for the default values as well as any unchanged settings. settings.json should NOT be altered in a production environment for any reason.
