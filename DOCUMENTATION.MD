# EasyFlow Documentation

Welcome to the EasyFlow documentation.

## Table of Contents

- [EasyFlow Documentation](#easyflow-documentation)
  - [Table of Contents](#table-of-contents)
  - [Usage](#usage)
    - [Starting EasyFlow](#starting-easyflow)
    - [Data Handling](#data-handling)

## Usage

### Starting EasyFlow

**Launch the Application:**

- On **Windows**, to install, just run the `EasyFlow-win-Setup`. After that, run the `EasyFlow.exe`.
- On **Linux**, execute the `EasyFlow` binary from the build directory.

Downloads are available in the official [page](dpieve.github.io/EasyFlow/).

**Creating and Managing Tags:**

- Go to the **Settings** section to create new tags for your focus sessions.
- Click the Add button and write a name for the new tag to categorize your activities (e.g., Work, Study).
- You can also edit or delete existing tags.

**Configuring Timers:**

- Navigate to the **Focus** section.
- Select a tag and set the duration for Pomodoro sessions and breaks. Also, how many sessions before the long break.
- Adjust the settings according to your preferences (e.g., work duration, short break, long break).

**Starting a Pomodoro Session:**

- Go to **Focus**, choose a tag and start a Pomodoro session.
- The timer will begin, and notifications will alert you when the session or break ends.

**Adding Session Descriptions:**

- After completing a session, add a description to note what you accomplished during the Pomodoro.
- Descriptions help you track progress and review your work later.

**Viewing Progress:**

- Visit the **Dashboard** to see an overview of your productivity.
- The dashboard displays a visual representation of your sessions, including completed tasks and total time spent on each tag.

### Data Handling

EasyFlow respects your privacy. It does not collect or store any personal data. All data is kept local to your device, ensuring that your productivity metrics are secure and confidential.

**Generating Reports:**

- Navigate to the **Dashboard** section to generate a detailed report on your productivity.
- The report is exported as a `.csv` file, which can be easily imported into tools like Microsoft Excel or LibreOffice Calc for deeper analysis.
- These reports offer valuable insights into your work patterns and progress over time. The session descriptions serve as reminders of what you accomplished, making it easier to reflect on your activities. Over time, these reports can become crucial records—whether you’re seeking a raise at work by showcasing your productivity or reviewing your study habits to better understand which topics you’ve focused on most. Having a clear, documented history of your efforts can be a powerful tool for both personal and professional growth.

**Backup data:**

- Go to **Settings** and click on the button **Backup data**.
- Select the path where you want to save your backup.

**Import backup data:**

- To import a backup previously saved, it just requires you to put the `EasyFlow.db` file inside the `Documents` directory (windows) or `Home` on Linux.

**Delete data:**

- Go to **Settings** and click on the button **Delete data**. This will delete the database and restart the software.
- If you prefer to do it manually, go to `Documents` directory (windows) or `Home` on Linux and delete the `EasyFlow.db` file.
