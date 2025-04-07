# Sprint x Report 
Video Link: https://youtu.be/jpjnIRihn4k
## What's New (User Facing)
 * Made the courses load in hour time through parallel as well as making it work with the UI
 * Made the Transcript page as a premade excel sheet so that the user can have it as well as as see there GPA and how many credits they need for graduation 
 * Worked on loading in the Degrees from all majors and the requirments of the current year
 * Sorting the degree data to work fluently so that we can read the header text and take out key words so we can see what classes can be taken instead of other classes as well as parsing the footnotes to see adequate replacments that satisfy the degree

## Work Summary (Developer Facing)
Throught our sprint, we primarily worked on the backend data so we have everything we should need for the UI. We also worked on some components of the UI funcationality primarily the linking of the site as we want it on a upper nav bar not a side nav bar like the template is given. We also worked on one of the pages for now which was the transcript page and getting an autocomplete with this so that it works with the UI so as you're are typing what courses are on your Transcript they are also showing what courses match it and giving their full names for the transcript. This not only enhances the UI but also serves a proof of concept, allowing us to focus more of our work on the other backend things for the UI display.

## Unfinished Work
Some of the work we did not finish is on the courses we could not get the descriptions due to it causing the scraper to break and it would cause more strain to the system so we are currently working on a way to make it open more instances of a driver so that we can load everything while also not compromising the system speed so that when it first spins up we can run all the main tasks without crashing the system and also on the refreshes it works the same without a huge  processing gap.

## Completed Issues/User Stories
Here are links to the issues that we completed in this sprint:

 * [courses scrape](https://github.com/gudino27/Virtual-Counselor/issues/19)
 * [Transcript](https://github.com/gudino27/Virtual-Counselor/issues/8)
 * [degree Requirments](https://github.com/gudino27/Virtual-Counselor/issues/20)
 * [Smart Search](https://github.com/gudino27/Virtual-Counselor/issues/11) #Enhancements has been made to this feature 
 
 ## Incomplete Issues/User Stories
 Here are links to issues we worked on but did not complete in this sprint:
 
 * [URL of issue 1](https://github.com/gudino27/Virtual-Counselor/issues/13#issue-2896211655) <<As explained above, trying to get the description causes the scraper to break and this partly has to do with the sheer amount of courses that need to be naviguated through to find the one whose description we want.>>
 * [URL of issue 2](https://github.com/gudino27/Virtual-Counselor/issues/16#issue-2923784727) <<This feature has been started for a bit now and it is incomplete in the sense that an in-depth testing needs to be done to ensure it is ready for merging.>>
 * [URL of issue 3](https://github.com/gudino27/Virtual-Counselor/issues/15#issue-2923571837) <<This feature has been started for a bit now and it is incomplete in the sense that an in-depth testing needs to be done to ensure it is ready for merging.>>

## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
 * [ryandegreescraper.cs](https://github.com/gudino27/Virtual-Counselor/blob/Ryan's_Work/VirtualCounselor/Backend/RyansDegreeScraper.cs)
 * [webscrapecourses.cs](https://github.com/gudino27/Virtual-Counselor/blob/main/VirtualCounselor/Backend/webscrapecourses.cs)
 * [Transcript.razor](https://github.com/gudino27/Virtual-Counselor/blob/main/VirtualCounselor/Components/Pages/Transcript.razor)
 * [WebScraper.cs] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Backend/WebScraper.cs)
 * [SmartSearch.cs] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Backend/SmartSearch.cs)
 * [CentralBackend.cs] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Backend/CentralBackend.cs)
 * [Degree.cs] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Backend/DegreeClasses/Degree.cs)
 * [Major.cs] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Backend/DegreeClasses/Major.cs)
 * [Minor.cs] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Backend/DegreeClasses/Minor.cs)
 * [CourseSearch.razor] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Components/Pages/CourseSearch.razor)
 * [NavMenu.razor] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/VirtualCounselor/Components/Layout/NavMenu.razor)
 * [.gitignore] (https://github.com/gudino27/Virtual-Counselor/blob/modeste01-smart_search/gitignore)

 
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
