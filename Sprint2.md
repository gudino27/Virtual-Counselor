# Sprint x Report 
Video Link: 
## What's New (User Facing)
 * made the courses load in hour time through parallel as well as making it work with the UI
 * Made the Transcript page as a premade excel sheet so that the user can have it as well as as see there GPA and how many credits they need for graduation 
 * worked on loading in the Degrees from all majors and the requirments of the current year
 * sorting the degree data to work fluently so that we can read the header text and take out key words so we can see what classes can be taken instead of other classes as well as parsing the footnotes to see adequate replacments that satisfy the degree

## Work Summary (Developer Facing)
throught our sprint we primarily worked on the backend data so we have everything we should need for the UI we also worked on some components of the UI funcationality primarily the linking of the site as we want it on a upper nav bar not a side nav bar like the template is given. we also worked on one of the pages for now which was the transcript page and getting an autocomplete with this so that it works with the UI so as your are typing what courses are on your Transcript they are also showing what courses match it and giving the full name of it for the transcript and also proof it can be done so we can work on the other backend things for the UI to display.

## Unfinished Work
some of the work we did not finish is on the courses we could not get the descr descriptions due to it causing the scraper to break and it would cause more strain to the system so we are currently working on a way to make it open more instances of a driver so that we can load everything while also not compromising the system speed so that when it first spins up we can run all the main tasks without crashing the system and also on the refreshes it works the same without a huge  processing gap.

## Completed Issues/User Stories
Here are links to the issues that we completed in this sprint:

 * [ courses scrape](https://github.com/gudino27/Virtual-Counselor/issues/19)
 * [Transcript](https://github.com/gudino27/Virtual-Counselor/issues/8)
 * [degree Requirments](https://github.com/gudino27/Virtual-Counselor/issues/20)
 
 ## Incomplete Issues/User Stories
 Here are links to issues we worked on but did not complete in this sprint:
 
 * URL of issue 1 <<One sentence explanation of why issue was not completed>>
 * URL of issue 2 <<One sentence explanation of why issue was not completed>>
 * URL of issue n <<One sentence explanation of why issue was not completed>>
 
 Examples of explanations (Remove this section when you save the file):
  * "We ran into a complication we did not anticipate (explain briefly)." 
  * "We decided that the feature did not add sufficient value for us to work on it in this sprint (explain briefly)."
  * "We could not reproduce the bug" (explain briefly).
  * "We did not get to this issue because..." (explain briefly)

## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
 * [ryandegreescraper.cs](https://github.com/gudino27/Virtual-Counselor/blob/Ryan's_Work/VirtualCounselor/Backend/RyansDegreeScraper.cs)
 * [webscrapecourses.cs](https://github.com/gudino27/Virtual-Counselor/blob/main/VirtualCounselor/Backend/webscrapecourses.cs)
 * [Transcript.razor](https://github.com/gudino27/Virtual-Counselor/blob/main/VirtualCounselor/Components/Pages/Transcript.razor)
 
## Retrospective Summary
Here's what went well:
  * WebScrapeCourses
  * Transcript
  * Degree scrape
 
Here's what we'd like to improve:
   * Item Linkage
  
Here are changes we plan to implement in the next sprint:
   * Course Scheduler
   * UI build out
   * Integrations
