Parent Portal provided by YesPrep and Student1 
============

This parent portal was made possible thanks to Yes Prep Public Schools and Student1.

Description
------------
The Parent Engagement Portal provides an easy-to-use view of student information, attendance, discipline, grades, and assessment scores with links to parent views in other applications. The Portal enables communication between members of the student’s “success team” by supporting text communications with automatic language translation.

Live Demo
------------

URL: http://parentportal.toolwise.net/

**Credentials:**

~~~
Parent:
   Email: perry.savage@toolwise.onmicrosoft.com
   Password: Parent123
~~~

~~~
Teacher:
   Email: alexander.kim@toolwise.onmicrosoft.com
   Password: Teacher123
~~~

~~~
Teacher:
   Email: fred.lloyd@toolwise.onmicrosoft.com
   Password: Teacher1234
~~~

End Of Year Rollover Checklist
To do this we recommend you follow this check list:

1. Backup your current ODS - Go into your database management tool and create a backup of the current year ODS. Name that backup with the suffix of the year end. For example: Bakup_Ed-Fi_ODS_2021
2. Use an Ed-Fi "Empty template" or "Minimal template" and restore it with the destination name of your ODS with the year. Something like: v3.4.0_Production_EdFi_ODS_2021
3. Point your Admin App and EdFi ODS API connection strings to your new ODS (If you are not using Year Specific Config)
4. Ensure you copy over at least your Local Education Agencies so that your API security still works.
5. Ensure you sync Descriptors and child tables with previous year.
6. Ensure you update the edfi.SchoolYearType and mark the appropriate school year as the current one.
7. Ensure data that is required to rollover gets rolled over like:
	1. Staff and parent Profiles that are in the parent portal schema
	2. Chat log and queue messages
	3. Admins have to be moved over also.
8. Ensure data that all chatlogs records has been marked as read before to start the new scholar year.
9. Please verify in your SQL server properties for the Db you have implemented compatibility with sql2014.
