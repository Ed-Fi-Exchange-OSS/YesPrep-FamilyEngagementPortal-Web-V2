angular.module('app.languages')
    .config(['$translateProvider', function ($translateProvider) {
        $translateProvider.translations('en-us', {
            // Global level translations
            'Add new address': 'Add new address',
            'Add new electronic mail': 'Add new electronic mail',
            'Add new telephone': 'Add new telephone',
            'Address Type': 'Address Type',
            'Addresses': 'Addresses',
            'Alerts Config': 'Alerts Config',
            'App Browser': 'This app works better with the Google Chrome Browser',
            'Apt/room/suite number': 'Apt/room/suite number',
            'Assesment Disclaimer': "Data may not be reflective of student's assessment scores. If you have questions about data posted here, please contact your campus.",
            'Biographical Information': 'Biographical Information',
            'Biography': 'Biography',
            'City': 'City',
            'Click Me': 'Click for more information',
            'Country': 'Country',
            'Course': 'Course',
            'Communications': 'Communications',
            'Communication': 'Communication',
            'Confirm delete': 'Confirm delete',
            'Contact Information': 'Contact Information',
            'Confirm': 'Confirm',
            'Confirmation': 'Confirmation',
            'Current Age': 'Current Age',
            'Check if this is the primary method of contact': 'Check if this is the primary method of contact',
            'Check if this number supports text messages or SMS': 'Check if this number supports text messages or SMS',
            'Check if this is your primary email': 'Check if this is your primary email',
            'Date': 'Date',
            'Description': 'Description',
            'detail': 'detail',
            'Electronic Email Address': 'Electronic Email Address',
            'Email': 'Email',
            'Emails': 'Emails',
            'Family': 'Family',
            'Fun fact': 'Fun fact',
            'First Name': 'First Name',
            'General Alerts': 'Alerts',
            'General Alerts Info': "Alerts keep you informed about your student's performance. You can choose which alerts you wish to receive. Disabling the alerts will pause all communication from the family engagement portal.",
            "Empty Alerts": "You don't have any alerts pending",
            "Alert Absences Exceeded": "Absences Threshold Exceeded",
            "Alert Course Exceeded": "Course Threshold Exceeded",
            "Alert Behavior Exceeded": "Behavior Threshold Exceeded",
            "new absences": "new absences",
            "grades below threshold": "grades below threshold",
            "new behavior incident": "new behavior incident",
            'HelpTooltip': 'Help / Resources',
            'Help': 'Resources',
            'HelpTitle': 'Help',
            'Home': 'Home',
            'Feedback': 'Get Help',
            'Information saved': 'Information saved',
            'Last Name': 'Last Name',
            'Log out': 'Log out',
            'Message': 'Message',
            'Middle Name': 'Middle Name',
            'Name': 'Name',
            'Nickname': 'Nickname',
            'No': 'No',
            'NoData': 'No Data',
            'Postal Code': 'Postal Code',
            'Phone': 'Phone',
            'Primary': 'Primary',
            'Privacy Policy': 'Privacy Policy',
            'Profile': 'Profile',
            'Programs': 'Programs',
            'Save': 'Save',
            'Send': 'Send',
            'Short Biography': 'Short Biography',
            'SMS Capable': 'SMS Capable',
            'State': 'State',
            'Student Name': 'Student Name',
            'Teacher': 'Teacher',
            'Parent': 'Parent',
            'Street Number and Name': 'Street Number and Name',
            'Telephone number': 'Telephone number',
            'Telephone numbers': 'Telephone numbers',
            'Yes': 'Yes',
            'Highly Qualified': 'Highly Qualified',
            'In app chat': 'In app chat',
            'Email message': 'Email message',
            'SMS text message': 'SMS text message',
            'Years of Teaching Experience': 'Years of Teaching Experience',
            'Education': 'Education',
            'Qualifications': 'Qualifications',
            'Languages': 'Languages',
            'Credentials': 'Credentials',
            'Choose a language': 'Choose a language',
            'Indicator': 'Indicator',
            'Category': 'Category',
            'Missing Assignments': 'Missing Assignments',
            'Abv Missing Assignments': 'Assignments',
            'Abv Course Average': 'Courses',
            'Course Average': 'Course Average',
            'Emergency Contact': 'Emergency Contact',
            'User Configuration': 'User Configuration',
            'Portal Notifications': 'Portal Notifications',
            'User Preferences': 'User Preferences',
            'Preferred Method of Contact': 'Preferred Method of Contact',
            'Preferred Method for Portal Notifications': 'Preferred Method for Portal Notifications',
            'Preferred Language': 'Preferred Language',
            'Reply Expectations': 'Reply Expectations',
            'Classroom Number': 'Classroom Number',
            'UnreadMessageTooltip': 'Unread Messages',
            'New Messages': 'Unread Messages',
            'See all': 'See all',
            'Hide': 'Hide',
            'No unread Messages': 'You are up to date. No messages.',
            'No missing assignments': 'No missing assignments',
            'Todays Schedule': "Today's Schedule",
            'OnTrackMessage': 'Reach out to your College Counselor for more information.',
            'Grading Scale': 'Grading Scale',
            'Number Grade': 'Number Grade',
            'Letter Grade': 'Letter Grade',
            'Grade Points': 'Grade Points (For high school credit classes only)',
            'Below': 'and Below',
            'Search': 'Search',
            'Welcome': 'Welcome',
            'Quick Messaging Title': 'Group Messaging to Families',
            'Message Sent': 'Success! Messages have been queued and will be sent shortly.',
            'Message Teacher Sent': 'Success! Your messages were sent.',
            'Nodata': 'There is no data for the filters selected.',
            "Audience Message part 2 singular": "family member connected to",
            "student at": "student at",
            'Quick Messaging Description': 'Send messages to the families of students in the selected classes.',
            'Quick Messaging Note': 'This message will be sent to ',
            'Quick Messaging Note 2': 'family members connected to',
            'Quick Messaging Note 3': 'students enrolled in',
            'Quick Messaging Note 2 singular': 'family member connected to',
            'Quick Messaging Note 3 singular': 'student enrolled in',
            'Quick Messaging Note 4': 'Please ensure you are following your campus communications guidelines.',
            'Quick Messaging Note 5': 'enrolled in {{CourseTitle}}',
            'Quick Messaging Subject Placeholder': 'Please type subject here.',
            'Quick Messaging Message Placeholder': 'Please type your message here.',
            'Quick Messaging Modal Body Message': 'Are you sure you want to send this message to the families of students in {{SectionName}}?',
            'class': 'class',
            'Classes': 'Classes',
            'Group Messaging': 'Group Messaging',
            'Individual Messaging': 'Individual Messaging',
            'Unread Messages': 'Unread Messages',
            'Audience': 'Audience',
            'Audience Message part 1': 'This message will be sent to',
            'Audience Message part 2': 'family members connected to',
            'students at': 'students at',
            'in': 'in',
            'and': 'and',
            'Send to': 'Send to',
            'people': 'people',
            'Send to people': 'Send to {{count}} people',
            'Send to one person': 'Send to 1 person',
            'Principals Modal Message Body': 'Are you sure you want to send this message to {{familyCount}} people?',
            'Principals Modal Button Label': 'Send to {{familyCount}} people',
            'Principals Modal Message Body One Person': 'Are you sure you want to send this message to 1 person?',
            'Principals Modal Button Label One Person': 'Send to 1 person',
            'The form is invalid': 'The form is invalid',
            'Ready to submit': 'Ready to submit',
            'Principals Individual Message Audience': 'This message will be sent to: {{audience}}',
            'Principals Individual Message Audience without audience': 'This message will be sent to',
            'Search by Student or Families Name': "Search by student or family member",
            'Search Results': 'Search Results',
            'No result found': 'No results found',
            'Start by running a search': 'Start by running a search',
            'Message Log': 'Message Log',
            'Group Log': 'Group Log',
            'Individual Log': 'Individual Log',
            'Advanced Search': 'Advanced Search',
            'Search group messages': 'Search group messages',
            'Unread Messages Only': 'Unread Messages Only',
            'No pending messages': 'No unread messages',
            'Please select a chat': 'Please select a conversation',
            'Results': 'Results',
            'to': 'to',
            'of': 'of',
            'Displayed as Family Member - Student Name': 'Displayed as Family Member - Student, ordered by latest message',
            'Preferred method of contact and language alert': 'Enhance your portal experience by setting your [preferred] language and [notifications].',
            'Search Individual Messages': 'Search by student name and parent name',

            'Grade Level': 'Grade Level',
            'First grade': 'First grade',
            'Second grade': 'Second grade',
            'Third grade': 'Third grade',
            'Fourth grade': 'Fourth grade',
            'Fifth grade': 'Fifth grade',
            'Sixth grade': 'Sixth grade',
            'Seventh grade': 'Seventh grade',
            'Eight grade': 'Eight grade',
            'Ninth grade': 'Ninth grade',
            'Tenth grade': 'Tenth grade',
            'Eleventh grade': 'Eleventh grade',
            'Twelfth grade': 'Twelfth grade',

            'Student with no Parents': "This student doesn't have parents associated on the system.",
            'Student Last Name': 'Student Last Name',
            'a to z': 'A-Z',
            'z to a': 'Z-A',
            'leastAbsences': 'fewest first',
            'mostAbsences': 'most first',
            'leastDisciplineIncidents': 'fewest first',
            'mostDisciplineIncidents': 'most first',
            'lowestGPA': 'lowest first',
            'highestGPA': 'highest first',

            'Attendance': 'Attendance',
            'Absences': 'Absences',
            'Excused Absences': 'Excused Absences',
            'Unexcused Absences': 'Unexcused Absences',
            "Unexcused Absence": "Unexcused Absence",
            "Attendance Descriptors": "These are the Ed-fi standard Attendance Descriptors",
            "Early departure": "Early departure",
            "Excused Absence": "Excused Absence",
            "In Attendance": "In Attendance",
            "Partial": "Partial",
            "Tardy": "Tardy",

            'Tardy Absences': 'Tardy Absences',
            'Abv Tardy Absences': 'Tardy',
            'Abv Excused Absences': 'Excused',
            'Abv Unexcused Absences': 'Unexcused',
            'Absence ADA': 'Absences are ADA (Average Daily Attendance)',
            'Behavior': 'Behavior',
            'Behavior Tooltip': 'Removal from Class to Expulsions',
            'Discipline': 'Discipline',
            'GPA': 'GPA',
            'Filters': 'Filters',
            'Sections': 'Sections',
            'Order by': 'Order by',
            'We had successfully received your feedback': 'We had successfully received your feedback',
            'View Student Detail': 'View Student Detail',

            /* View level translations */
            // Login
            'view.login.app_online_status': 'We are updating student data. Please come back in a few.',
            'view.login.app_pilot': "Thank you for helping us during this Pilot. Our team is working very hard every day to bring relevant data to improve your student's education. Please let us know if you have any feedback.",
            'view.login.app_name': 'Family Portal',
            'view.login.log_into_account': 'Log into Your Account',
            'view.login.email': 'Email',
            'view.login.password': 'Password',
            'view.login.remember_me': 'Remember me',
            'view.login.forgot_password': 'Forgot Password?',
            'view.login.sign_in': 'Sign in',
            'view.login.sign_out': 'Log out',
            'view.login.or_login_with': 'Or Login With',
            'view.login.dont_have_account': "Don't have an account?",
            'view.login.currentlyt_loged_in_as': "You are currently logged is as",
            'view.login.register_now': 'Register Now',

            // Landing
            'view.landing.title': 'Home',
            'view.landing.subtitle': 'currently enrolled kids',

            // Teacher Landing
            'view.teacher.landing.title': 'Home',
            'view.teacher.landing.studentsTab': 'Students',
            'view.teacher.landing.groupsTab': 'Groups',
            'view.teacher.landing.subtitle': 'Sections and students',
            'view.teacher.landing.viewingSection': 'Viewing Section',
            'view.teacher.landing.createMessageTab': 'Create Message',
            'view.teacher.landing.sentMessageTab': 'Sent Messages',
            'view.teacher.createMessagesTitle': 'Create Group Message',
            'view.teacher.sentMessagesTitle': 'Sent Group Message',

            // Student Detail
            'view.studentDetail.title': 'Student',
            'view.studentDetail.subtitle': 'detail',
            'view.studentDetail.viewingStudent': 'Viewing Student',
            'view.studentDetail.of': 'out of',

            // User Profile
            'view.userProfile.title': 'User Profile',
            'view.userProfile.subtitle': ' Update your information',
            'Configure User Alerts': 'Configure User Alerts',
            'view.userProfile.profilePicture': 'Profile Picture',
            'view.userProfile.changeProfilePicture': 'Click to change profile picture',
            'view.userProfile.expectationsParentTooltip': 'For example - I am available most evenings after 7.',
            'view.userProfile.expectationsTooltip': 'For example - My normal office hours are M-F from 1.00pm to 2.00pm. I will usually get back to you within 24 to 48 hrs.',

            // Alert View
            "view.alert.generalAlerts": "Enable Alerts?",
            "view.alert.absence": "Attendance Threshold Alert",
            "view.alert.behaviorAlert": "Discipline Threshold Alert",
            "view.alert.assignmentAlert": "Assignment Threshold Alert",
            "view.alert.courseAlert": "Course Average Threshold Alert",

            // Communications view
            'view.communications.title': 'Communications',
            'view.communications.subtitle': 'initiate and view threads',
            'view.communications.simultaneousTranslation': 'Simultaneous Translation',
            'view.communications.loadMore': 'Load More',

            //Admin view
            'view.admin.title': 'Administrator',
            'view.admin.view.as': 'Login As',
            'view.admin.view.input.email': 'Impersonate email',
            'view.admin.impersonate.section': 'Impersonate User',
            'view.admin.impersonate.description': 'Enter the email of the user you wish to impersonate.',

            //impersonate
            'view.impersonate.view.as': 'Viewing as:',
            'view.impersonate.view.as.exit': 'Exit View As',

            // Directives and Modules
            'view.studentCardGrid.noResultsWithFilters': 'No results with current filters',

            'view.studentParentInfo.title': 'Family',
            'view.studentParentInfo.subtitle': 'Contact Information',

            'view.currentGrades.title': 'Current',
            'view.currentGrades.subtitle': 'Grades',
            'view.currentGrades.gp1': 'Grading Period One',
            'view.currentGrades.gp1.abbr': 'GP1',
            'view.currentGrades.gp2': 'Grading Period One Two',
            'view.currentGrades.gp2.abbr': 'GP2',
            'view.currentGrades.gp3': 'Grading Period Three',
            'view.currentGrades.gp3.abbr': 'GP3',
            'view.currentGrades.gp4': 'Grading Period Four',
            'view.currentGrades.gp4.abbr': 'GP4',
            'view.currentGrades.gp5': 'Grading Period Five',
            'view.currentGrades.gp5.abbr': 'GP5',
            'view.currentGrades.gp6': 'Grading Period Six',
            'view.currentGrades.gp6.abbr': 'GP6',
            'view.currentGrades.s1': 'Semester One Final Marking Grade',
            'view.currentGrades.s1.abbr': 'S1',
            'view.currentGrades.s2': 'Semester Two Final Marking Grade',
            'view.currentGrades.s2.abbr': 'S2',
            'view.currentGrades.ex1': 'Exam One',
            'view.currentGrades.ex1.abbr': 'EX1',
            'view.currentGrades.ex2': 'Exam Two',
            'view.currentGrades.ex2.abbr': 'EX2',
            'view.currentGrades.finalGrade': 'Year Final Marking Grade',
            'view.currentGrades.finalGrade.abbr': 'Y',

            'view.missingAssignments.title': 'Missing',
            'view.missingAssignments.subtitle': 'Assignments',
            'view.missingAssignments.assigned': 'Assigned',
            'view.missingAssignments.days': 'days ago',

            'view.studentCalendar.title': 'Schedule',
            'view.studentCalendar.subtitle': 'Detail',

            'view.disciplineIncidents.title': 'Behavior',
            'view.disciplineIncidents.subtitle': 'Incidents Detail',
            'view.disciplineIncidents.actionsTaken': 'Actions Taken',
            'view.disciplineIncidents.moreDetail': "For more detailed information about your student's daily behavior, please visit",
            'view.disciplineIncidents.moreDetail.staff': "For more detailed information about this student's daily behavior, please visit",

            'view.studentSuccessTeam.title': "Student's Success Team",
            'view.studentSuccessTeam.subtitle': '',

            'view.studentAssesment.performanceTooltip': 'This performance level means that your student is in the Masters range...',
            'view.studentAssesment.score': 'Score',
            'view.studentAssesment.totalScore': 'Total Score',
            'view.studentAssesment.performanceLevel.Did Not Meet Grade Level': 'STAAR Performance Levels Did Not Meet - Students in this category do not demonstrate a sufficient understanding of the assessed knowledge and skills for their current grade level.  Does Not Approach indicates that students are unlikely to succeed in the next grade or course without significant, ongoing academic intervention.',
            'view.studentAssesment.performanceLevel.Approaches Grade Level': 'STAAR Performance Levels Approaches - Students in this category generally demonstrate the ability to apply the assessed knowledge and skills in familiar contexts. Approaches performance level indicates that students are likely to succeed in the next grade or course with targeted academic intervention.',
            'view.studentAssesment.performanceLevel.Meets Grade Level': 'STAAR Performance Levels Meets - Students in this category generally demonstrate the ability to think critically and apply the assessed knowledge and skills in familiar contexts. Meets performance level indicates that students have a high likelihood of success in the next grade or course but may still need some short-term, targeted academic intervention."',
            'view.studentAssesment.performanceLevel.Masters Grade Level': 'STAAR Performance Levels Masters - Students in this category demonstrate the ability to think critically and apply the assessed knowledge and skills in varied contexts, both familiar and unfamiliar. Masters performance level indicates that students are expected to succeed in the next grade or course with little or no academic intervention.',
            'view.studentAssesment.performanceLevel.Not yet Taken': 'This assesment has not been taken.',

            'view.studentStaarAssessment.title': 'STAAR',
            'view.studentStaarAssessment.subtitle': 'State of Texas Assessments of Academic Readines (3rd - 8th)',
            'view.studentStaarAssessment.gradeLevel': 'Grade Level',
            'view.studentStaarAssessment.subject': 'Subject',
            'view.studentStaarAssessment.scaleScore': 'Scale Score',
            'view.studentStaarAssessment.performanceLevel': 'Performance Level',

            'view.studentAttendance.title': 'Absences',
            'view.studentAttendance.subtitle': 'Detail',

            'view.telephone.telephoneNumber': 'Telephone Number',
            'view.telephone.telephoneNumberType': 'Telephone Number Type',
            'view.telephone.telephoneCarrierType': 'Telephone Carrier Type',

            'view.emails.electronicMailAddress': 'E-mail Address',
            'view.emails.electronicMailType': 'E-mail Type',

            'view.feedback.name': 'Name',
            'view.feedback.email': 'Email',
            'view.feedback.subject': 'Subject',
            'view.feedback.type': 'Type',
            'view.feedback.detailedDescription': 'Detailed Description',
            'view.feedback.bug': 'Bug/System Issue',
            'view.feedback.comment': 'Comment',
            'view.feedback.dataProblem': 'Data Problem',
            'view.feeback.featureRequest': 'Feature Request',
            'view.feedback.question': 'Question',
            'view.feedback.privacyIssue': 'Privacy Issue',
            'view.feedback.login': 'Unable to Login',

            'view.studentDetail.grades.interpretation.bad': 'Failing - Grade Below 70',
            'view.studentDetail.grades.interpretation.warning': 'At risk of failing - Grade Below 72',
            'view.studentDetail.grades.interpretation.good': 'Average Grade - Grade Above 73',
            'view.studentDetail.grades.interpretation.great': 'Good Grade - Grade Above 90',




            //ReportView
            'view.reportlogin.title': 'Reports',
            'view.reportlogin.subTitle': 'Teacher Indicators',
            'view.reportlogin.subTitleAdmin': 'Indicators',
            'view.reportlogin.fromDate': 'From:',
            'view.reportlogin.toDate': 'To:',

            // -> Indicators
            //    -> Absences Indicator
            'view.studentDetail.indicators.absences.title': 'Absences',
            'view.studentDetail.indicators.absences.tooltip': 'Year to date absences',
            'view.studentDetail.indicators.absences.interpretation.great': "Your student has no absences. This is really good! Being at school and attending class positively impacts their performance.",
            'view.studentDetail.indicators.absences.interpretation.good': "Your student has a few absences. Being at school and attending class positively impacts their performance.",
            'view.studentDetail.indicators.absences.interpretation.warning': "Your student has some absences. If they go over the limit, they will lose credit for a subject.",
            'view.studentDetail.indicators.absences.interpretation.bad': "Your student has many absences. If they go over the limit, they will lose credit for a subject.",
            'view.studentDetail.indicators.absences.interpretation.staff.great': "This student has no absences. This is really good! Being at school and attending class positively impacts their performance.",
            'view.studentDetail.indicators.absences.interpretation.staff.good': "This student has a few absences. Being at school and attending class positively impacts their performance.",
            'view.studentDetail.indicators.absences.interpretation.staff.warning': "This student has some absences. If they go over the limit, they will lose credit for a subject.",
            'view.studentDetail.indicators.absences.interpretation.staff.bad': "This student has many absences. If they go over the limit, they will lose credit for a subject.",
            //    -> Behavior Indicator
            'view.studentDetail.indicators.behavior.title': 'Behavior',
            'view.studentDetail.indicators.behavior.tooltip': 'Year to date behavior incidents',
            'view.studentDetail.indicators.behavior.interpretation.great': "Your student has {{incidentCount}} discipline incidents. This is really good! Not having discipline incidents helps the student concentrate better and positively impacts their performance.",
            'view.studentDetail.indicators.behavior.interpretation.good': "Your student has {{incidentCount}} discipline incidents. This is really good! Not having discipline incidents helps the student concentrate better and positively impacts their performance.",
            'view.studentDetail.indicators.behavior.interpretation.warning': "Your student has {{incidentCount}} discipline incidents. The higher the count the higher the chance that it will impact their performance negatively.",
            'view.studentDetail.indicators.behavior.interpretation.bad': "Your student has {{incidentCount}} discipline incidents. The higher the count the higher the chance that it will impact their performance negatively.",
            //    -> GPA Indicator
            'view.studentDetail.indicators.gpa.title': 'GPA',
            'view.studentDetail.indicators.gpa.tooltip': 'Cumulative Grade Point Average',
            'view.studentDetail.indicators.gpa.interpretation.': "No Data",
            'view.studentDetail.indicators.gpa.interpretation.good': "The national GPA average is {{nationalGPA}}. A GPA closer to 4.0 will be beneficial in a competitive world",
            'view.studentDetail.indicators.gpa.interpretation.bad': "The national GPA average is {{nationalGPA}}. A GPA closer to 4.0 will be beneficial in a competitive world",
            'view.studentDetail.indicators.gpa.interpretation.warning': 'The national GPA average is missing, please contact your registrar',

            //    -> Assignments Indicator
            'view.studentDetail.indicators.missingAssignments.title': 'Missing Assignments',
            'view.studentDetail.indicators.missingAssignments.tooltip': 'Count of Missing Assignments',
            'view.studentDetail.indicators.missingAssignments.interpretation.great': "Your student is not missing any assignments. This is really good. Continue motivating them to turn in assignments on time.",
            'view.studentDetail.indicators.missingAssignments.interpretation.good': "Your student is missing very few assignments. This is good. Continue motivating them to turn in assignments on time.",
            'view.studentDetail.indicators.missingAssignments.interpretation.warning': "Your student is missing {{count}} assignments. This is a warning sign. You should motivate them to turn in assignments on time.",
            'view.studentDetail.indicators.missingAssignments.interpretation.bad': "Your student is missing {{count}} assignments. This is bad, and might lose credits for courses having to retake them. Get in touch with the teacher ASAP",
            'view.studentDetail.indicators.missingAssignments.interpretation.staff.great': "This student is not missing any assignments. This is really good. Continue motivating them to turn in assignments on time.",
            'view.studentDetail.indicators.missingAssignments.interpretation.staff.good': "This student is missing very few assignments. This is good. Continue motivating them to turn in assignments on time.",
            'view.studentDetail.indicators.missingAssignments.interpretation.staff.warning': "This student is missing {{count}} assignments. This is a warning sign. You should motivate them to turn in assignments on time.",
            'view.studentDetail.indicators.missingAssignments.interpretation.staff.bad': "This student is missing {{count}} assignments. This is bad, and might lose credits for courses having to retake them. Get in touch with the teacher ASAP",
            'view.studentDetail.indicators.missingAssignments.interpretation.null': "No Data",
            //    -> Course Average Indicator
            'view.studentDetail.indicators.courseAverage.title': 'Course Average',
            'view.studentDetail.indicators.courseAverage.tooltip': 'Current Course Average',
            'view.studentDetail.indicators.courseAverage.interpretation.excellent': "Your student's current course average is excellent.",
            'view.studentDetail.indicators.courseAverage.interpretation.great': "Your student's current course average is very good.",
            'view.studentDetail.indicators.courseAverage.interpretation.good': "Your student's current course average is good.",
            'view.studentDetail.indicators.courseAverage.interpretation.warning': "Your student's current course average is fair.",
            'view.studentDetail.indicators.courseAverage.interpretation.bad': "Your student's current course average is bad.",

            'view.studentDetail.indicators.courseAverage.interpretation.staff.excellent': "This student's current course average is excellent.",
            'view.studentDetail.indicators.courseAverage.interpretation.staff.great': "This student's current course average is very good.",
            'view.studentDetail.indicators.courseAverage.interpretation.staff.good': "This student's current course average is good.",
            'view.studentDetail.indicators.courseAverage.interpretation.staff.warning': "This student's current course average is fair.",
            'view.studentDetail.indicators.courseAverage.interpretation.staff.bad': "This student's current course average is bad.",
            'view.studentDetail.indicators.courseAverage.interpretation.': "No Data",
            //    -> On Track To Graduate Indicator
            'view.studentDetail.indicators.onTrackToGraduate.title': 'On Track to Graduate',
            'view.studentDetail.indicators.onTrackToGraduate.tooltip': 'On Track to Graduate',
            'view.studentDetail.indicators.onTrackToGraduate.interpretation.good': "Your student is on track to graduate. This is really good. This means he will graduate with his cohort and be ready for what is next.",
            'view.studentDetail.indicators.onTrackToGraduate.interpretation.bad': "Your student is off track to graduate. Help your student get back on track. Arrange a meeting with your student's counselor to make an action plan.",
            'view.studentDetail.indicators.onTrackToGraduate.interpretation.null': "There is no data to be able to interpret.",

            "formLabels.subject": "Subject",
            "formLabels.section": "Section",
            "formLabels.gradeLevel": "Grade",
            "formLabels.program": "Program",
            "formLabels.message": "Message",
            "formLabels.school": "School",

            "Black - African American": "Black - African American",
            "American Indian - Alaska Native": "American Indian - Alaska Native",
            "Asian": "Asian",
            "Native Hawaiian - Pacific Islander": "Native Hawaiian - Pacific Islander",
            "White": "White",
            "Home Access Center": "Home Access Center",
            "Familiy Portal": "Familiy Portal",
            "Ready to Save": "Ready to Save",
            "Home/Personal": "Home/Personal",
            "Organization": "Organization",
            "Other": "Other",
            "Work": "Work",
            "Emergency 1": "Emergency 1",
            "Emergency 2": "Emergency 2",
            "Mobile": "Mobile",
            "Unlisted": "Unlisted",
            "Billing": "Billing",
            "Doubled - up (i.e., living with another family)": "Doubled - up (i.e., living with another family)",
            "Father Address": "Father Address",
            "Guardian Address": "Guardian Address",
            "Hotels/Motels": "Hotels/Motels",
            "Mailing": "Mailing",
            "Mother Address": "Mother Address",
            "Physical": "Physical",
            "Shelters, Transitional housing, Awaiting Foster Care": "Shelters, Transitional housing, Awaiting Foster Care",
            "Temporary": "Temporary",
            "Shipping": "Shipping",
            "Unshelte (cars, parks, temporary trailers, or abandoned buildings)": "Unshelte (cars, parks, temporary trailers, or abandoned buildings)",
            "Language": "Language",
            "Approaches Grade Level": "Approaches Grade Level",
            "Commended Performance": "Commended Performance",
            "Dit Not Meet Grade Level": "Dit Not Meet Grade Level",
            "Fail": "Fail",
            "Masters Grade Level": "Masters Grade Level",
            "Meets Grade Level": "Meets Grade Level",
            "Met Standard": "Met Standard",
            "Minimum": "Minimum",
            "Pass": "Pass",
            "Proficient": "Proficient",
            "Satisfactory": "Satisfactory",
            "Unsatisfactory": "Unsatisfactory",
            "Well Below Basic": "Well Below Basic",
            "Settings": "Settings",
            "Your session has expired. Please login again": "Your session has expired. Please login again",
            "Type your message here.": "Type your message here.",
            "Expand": "Expand",
            "Collapse": "Collapse",
            "Hispanic or Latino": "Hispanic or Latino",
            "No Suspendable Behaviors.": "No Suspendable Behaviors.",
            "Detail Alerts": "Detail Alerts",
            "Alert": "Alert",
            "Threshold": "Threshold",
            "Enable": "Enable",
            "Absence Alert": "Absence Alert",
            "Behavior Alert": "Behavior Alert",
            "Missing Assignment Alert": "Missing Assignment Alert",
            "Course Grade Alert": "Course Grade Alert",
            "Unread Message Alert": "Unread Message Alert",
            "College Initiative Corner": "College Initiative Corner",
            "Mon": "Mon",
            "Tue": "Tue",
            "Wed": "Wed",
            "Thr": "Thr",
            "Fri": "Fri",
            "Time": "Time",
            "New App Title": "Update is available!",
            "New App Description": "Good news! A new version of the application is available.",
            "Update": "Update",
            "Please review the info and try again": "Please review the info and try again",
            "Message and data rates may apply.": "Message and data rates may apply.",
            "Click to Login": "Click to Login",
            "Please use": "Please use",
            "for all communications about elementary students.": "for all communications about elementary students.",
            "Your session is about to expire, do you wish to continue?": "Your session is about to expire. Do you wish to continue?",
            "Inactivity Alert": "Inactivity Alert",
            "Click here": "Click here",
            "State of Texas Assessments of Academic Readiness": "State of Texas Assessments of Academic Readiness",
            "Scholastic Aptitude Test": "Scholastic Aptitude Test",
            "Preliminary Scholastic Aptitude Test": "Preliminary Scholastic Aptitude Test",
            "Log Out": "Log Out",
            "Welcome to": "Welcome to",
            "Take a moment to setup the portal": "Take a moment to setup the portal",
            "Telephone": "Telephone",
            "Skip": "Skip",
            "Text Message (SMS)": "Text Message (SMS)",
            "App Notifications": "App Notifications",
            "Select a telephone": "Select a telephone",
            "Done": "Done",
            "This application uses the same": "This application uses the same",
            "email registered in Skyward.": "email registered in Skyward.",
            "email registered in your student information system.": "email registered in your student information system.",
            "Questions?": "Questions?",
            "Contact your registrar or see the": "Contact your registrar or see the",
            "Email Not Found": "Email Not Found",
            //wizard
            "Take a moment to set up the Portal": "Take a moment to set up the Portal",
            "You can always change this in your profile": "You can always change this in your profile",
            "Select your preferred language": "Select your preferred language",
            "Welcome to the": "Welcome to the",
            "Select your preferred method of contact": "Select your preferred method of contact",
        });
    }]);