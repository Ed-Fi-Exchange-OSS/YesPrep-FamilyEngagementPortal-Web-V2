use [v3.2.0_Production_Skyward_EdFi_Ods]
go

BEGIN
	BEGIN TRY
	BEGIN TRANSACTION
		declare @@SchoolId int = 56 --Hoffman; 
		-- ** Student Identification, Names and Demographics ** --
		declare @@StudentUniqueId nvarchar(50) = '989898';
		declare @@StudentFirstName nvarchar(10) = 'Maria';
		declare @@StudentMiddleName nvarchar(10) = 'Fernanda';
		declare @@StudentLastSurname nvarchar(10) = 'Perez';
		declare @BirthSexDescriptorId int = null;                  -- Female
		declare @SexDescriptorId int = null;                       -- Female
		declare @TelephoneNumberTypeDescriptorId int = null;       -- Home
		declare @RaceDescriptorId int = 1610;                      -- American Indian or Alaskan Native
		
		-- ** Parent Name and Demographics ** --
		declare @@ParentUniqueId nvarchar(50) = '979797';
		declare @@ParentFirstName nvarchar(10) = 'April';
		declare @@ParentLastSurname nvarchar(10) = 'Perez';
		declare @@EmailLogin nvarchar(50) = 'aprilperez910101@gmail.com';
		declare @ElectronicMailTypeDescriptorId int = null;         -- Home/Personal
		declare @RelationDescriptorId int = null;                   -- mother
		declare @AddressTypeDescriptorId int = null;                -- Home
		declare @StateAbbreviationDescriptorId int = null;          -- Texas
		
		-- ** Student Enrollment Courses **--
		declare @SessionName nvarchar(20) = '2020-2021 Year Round';-- Session Name (Fall Semester, Spring Semester, Year round)
		declare @@LocalCourseCode1 nvarchar(50) = 'E3010';	       -- Local Course Name
		declare @@LocalCourseCode2 nvarchar(50) = 'H3010';	       -- Local Course Name
		declare @@LocalCourseCode3 nvarchar(50) = 'M3010';         -- Local Course Name
		declare @@LocalCourseCode4 nvarchar(50) = 'S3010';         -- Local Course Name

		declare @@GradeLevelDescriptorId int = null;               -- 8th Grade
		declare @@EntryTypeDescriptorId int = null                 -- Promoted to Next Grade
		declare @@GraduationPlanTypeDescriptorId int = null        -- Graduation Plan (YES Foundation HS Prog w/Endorsements)
		declare @@CalendarCode nvarchar(5)                         -- Calendar Code
		declare @@StudentParticipationCodeDescriptorId int; -- Perpetrator
		declare @AttendanceEventCategoryDescriptorId int = null    -- Absent
		declare @@GradeTypeDescriptorId int = null;                -- Semester
		declare @GradingPeriodDescriptorId int = null              -- A1

		select top 1 @BirthSexDescriptorId =                     DescriptorId from edfi.Descriptor where [Description] = 'Female' and Namespace = 'uri://ed-fi.org/SexDescriptor';
	    select top 1 @SexDescriptorId      =                     DescriptorId from edfi.Descriptor where [Description] = 'Female' and Namespace = 'uri://ed-fi.org/SexDescriptor';
        select top 1 @TelephoneNumberTypeDescriptorId =          DescriptorId from edfi.Descriptor where [Description] = 'Home' and Namespace = 'uri://ed-fi.org/TelephoneNumberTypeDescriptor';
        select top 1 @RaceDescriptorId =                         DescriptorId from edfi.Descriptor where [Description] = 'American Indian - Alaska Native' and Namespace = 'uri://ed-fi.org/RaceDescriptor';
        select top 1 @ElectronicMailTypeDescriptorId =           DescriptorId from edfi.Descriptor where [Description] = 'Home/Personal' and Namespace = 'uri://ed-fi.org/ElectronicMailTypeDescriptor';
        select top 1 @RelationDescriptorId =                     DescriptorId from edfi.Descriptor where [Description] = 'Mother' and Namespace = 'uri://ed-fi.org/RelationDescriptor';
        select top 1 @AddressTypeDescriptorId =                  DescriptorId from edfi.Descriptor where [Description] = 'Home' and Namespace = 'uri://ed-fi.org/AddressTypeDescriptor';
        select top 1 @StateAbbreviationDescriptorId =            DescriptorId from edfi.Descriptor where [Description] = 'TX' and Namespace = 'uri://ed-fi.org/StateAbbreviationDescriptor';
		Select top 1  @@GradeLevelDescriptorId  =                DescriptorId FROM edfi.Descriptor where [Description] = 'Eighth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';                -- 8th Grade
        Select top 1  @@EntryTypeDescriptorId  =                 DescriptorId FROM edfi.Descriptor where [Description] = 'New to education system' AND [Namespace] = 'uri://ed-fi.org/EntryTypeDescriptor';      -- New to education system
        Select top 1  @@GraduationPlanTypeDescriptorId  =        DescriptorId FROM edfi.Descriptor where [Description] = 'Standard' AND [Namespace] = 'uri://ed-fi.org/GraduationPlanTypeDescriptor';            -- Standard
        Select top 1  @@CalendarCode = null                                                                                                                                                                      -- Calendar Code
        Select top 1  @@StudentParticipationCodeDescriptorId  =  DescriptorId FROM edfi.Descriptor where [Description] = 'Perpetrator' AND [Namespace] = 'uri://ed-fi.org/StudentParticipationCodeDescriptor';   -- Perpetrator
        Select top 1  @AttendanceEventCategoryDescriptorId  =    DescriptorId FROM edfi.Descriptor where [Description] = 'Excused Absence' AND [Namespace] = 'uri://ed-fi.org/AttendanceEventCategoryDescriptor';-- Absent
        Select top 1  @@GradeTypeDescriptorId  =                 DescriptorId FROM edfi.Descriptor where [Description] = 'Semester' AND [Namespace] = 'uri://ed-fi.org/GradeTypeDescriptor';                     -- Semester
        Select top 1  @GradingPeriodDescriptorId  =              DescriptorId FROM edfi.Descriptor where [Description] = 'Grading Period' AND [Namespace] = 'uri://ed-fi.org/GradeTypeDescriptor';               --grading period

		
		declare @@StudentUSI int = null;
		declare @@ParentUSI int = null;
		declare @@CurrentSchoolYear int = null;

		INSERT INTO [edfi].[Student]
		           ([PersonalTitlePrefix]
		           ,[FirstName]
		           ,[MiddleName]
		           ,[LastSurname]
		           ,[BirthDate]
		           ,[BirthSexDescriptorId]
		           ,[StudentUniqueId])
		     VALUES
		           ('Mrs'
		           ,@@StudentFirstName
		           ,@@StudentMiddleName
		           ,@@StudentLastSurname
		           ,'2014-07-29'
		           ,@BirthSexDescriptorId
		           ,@@StudentUniqueId)
		set @@StudentUSI = SCOPE_IDENTITY();
		
		select @@CurrentSchoolYear = CAST(SchoolYear AS int) from edfi.SchoolYearType where CurrentSchoolYear = 1
		
		declare @RegisterStudentInSchool nvarchar(10) = null;
		select @RegisterStudentInSchool = format(getDate(), 'yyyy-MM-dd');
		
		INSERT INTO [edfi].[StudentSchoolAssociation]
		           ([EntryDate]
		           ,[SchoolId]
		           ,[StudentUSI]
		           ,[EntryGradeLevelDescriptorId]
		           ,[EntryTypeDescriptorId]
		           ,[GraduationPlanTypeDescriptorId]
		           ,[EducationOrganizationId]
		           ,[CalendarCode]
		           ,[SchoolYear])
		     VALUES
		           (@RegisterStudentInSchool
		           ,@@SchoolId
		           ,@@StudentUSI
		           ,@@GradeLevelDescriptorId -- Grade Level
		           ,@@EntryTypeDescriptorId -- Promoted to Next Grade
		           ,@@GraduationPlanTypeDescriptorId -- Graduation Plan
		           ,@@SchoolId
			       ,@@CalendarCode -- Calendar Code
		           ,@@CurrentSchoolYear)
		
		INSERT INTO [edfi].[Parent]
		           ([PersonalTitlePrefix]
		           ,[FirstName]
		           ,[LastSurname]
		           ,[SexDescriptorId]
		           ,[ParentUniqueId])
		     VALUES
		          ('Ms'
		           ,@@ParentFirstName
		           ,@@ParentLastSurname
		           ,@SexDescriptorId
		           ,@@ParentUniqueId)
		
		set @@ParentUSI = SCOPE_IDENTITY();
				   
		INSERT INTO [edfi].[StudentParentAssociation] 
					([ParentUSI]
					,[StudentUSI]
					,[RelationDescriptorId]
					,[PrimaryContactStatus]
					,[LivesWith]
					,[EmergencyContactStatus]) 
			VALUES 
					(@@ParentUSI 
					,@@StudentUSI 
					,@RelationDescriptorId 
					,1
		            ,1
		            ,1)
		
		INSERT INTO [edfi].[ParentAddress]
		           ([AddressTypeDescriptorId]
		           ,[ParentUSI]
		           ,[StreetNumberName]
		           ,[City]
		           ,[StateAbbreviationDescriptorId]
		           ,[PostalCode]
		           ,[NameOfCounty])
		     VALUES
		           (@AddressTypeDescriptorId
		           ,@@ParentUSI
		           ,'123 Red Jay St'
		           ,'Houston'
		           ,@StateAbbreviationDescriptorId
		           ,12123
		           ,'United States')
		
		INSERT INTO [edfi].[ParentElectronicMail]
		           ([ElectronicMailTypeDescriptorId]
		           ,[ParentUSI]
		           ,[ElectronicMailAddress]
		           ,[PrimaryEmailAddressIndicator])
		     VALUES
		           (@ElectronicMailTypeDescriptorId
		           ,@@ParentUSI
		           ,@@EmailLogin
		           ,1)
		
		--course 1
		declare @SectionIdentifier1 nvarchar(50) = null;
		declare @BeginDate1 nvarchar(50) = null;
		declare @LocalCourseCode1 nvarchar(50) = null;
		declare @EndDate1 nvarchar(50) = null;
		declare @GradebookEntryTitle1 nvarchar(50) = null;
		
		--course 2
		declare @SectionIdentifier2 nvarchar(50) = null;
		declare @BeginDate2 nvarchar(50) = null;
		declare @LocalCourseCode2 nvarchar(50) = null;
		declare @EndDate2 nvarchar(50) = null;
		declare @GradebookEntryTitle2 nvarchar(50) = null;
		
		--course 3
		declare @SectionIdentifier3 nvarchar(50) = null;
		declare @BeginDate3 nvarchar(50) = null;
		declare @LocalCourseCode3 nvarchar(50) = null;
		declare @EndDate3 nvarchar(50) = null;
		declare @GradebookEntryTitle3 nvarchar(50) = null;
		
		--course 4
		declare @SectionIdentifier4 nvarchar(50) = null;
		declare @BeginDate4 nvarchar(50) = null;
		declare @LocalCourseCode4 nvarchar(50) = null;
		declare @EndDate4 nvarchar(50) = null;
		declare @GradebookEntryTitle4 nvarchar(50) = null;
		
		--course 5
		--declare @SectionIdentifier5 nvarchar(50) = null;
		--declare @BeginDate5 nvarchar(50) = null;
		--declare @LocalCourseCode5 nvarchar(50) = null;
		--declare @EndDate5 nvarchar(50) = null;
		--declare @GradebookEntryTitle5 nvarchar(50) = null;
		
		select top(1) @SectionIdentifier1 = SectionIdentifier, @BeginDate1 = DateAssigned, @GradebookEntryTitle1 = GradebookEntryTitle, @LocalCourseCode1 = LocalCourseCode from edfi.GradebookEntry where LocalCourseCode = @@LocalCourseCode1 and SchoolId = @@SchoolId and SessionName = @SessionName and SchoolYear = @@CurrentSchoolYear
		select top(1) @SectionIdentifier2 = SectionIdentifier, @BeginDate2 = DateAssigned, @GradebookEntryTitle2 = GradebookEntryTitle, @LocalCourseCode2 = LocalCourseCode from edfi.GradebookEntry where LocalCourseCode = @@LocalCourseCode2 and SchoolId = @@SchoolId and SessionName = @SessionName and SchoolYear = @@CurrentSchoolYear
		select top(1) @SectionIdentifier3 = SectionIdentifier, @BeginDate3 = DateAssigned, @GradebookEntryTitle3 = GradebookEntryTitle, @LocalCourseCode3 = LocalCourseCode from edfi.GradebookEntry where LocalCourseCode = @@LocalCourseCode3 and SchoolId = @@SchoolId and SessionName = @SessionName and SchoolYear = @@CurrentSchoolYear
		select top(1) @SectionIdentifier4 = SectionIdentifier, @BeginDate4 = DateAssigned, @GradebookEntryTitle4 = GradebookEntryTitle, @LocalCourseCode4 = LocalCourseCode from edfi.GradebookEntry where LocalCourseCode = @@LocalCourseCode4 and SchoolId = @@SchoolId and SessionName = @SessionName and SchoolYear = @@CurrentSchoolYear
		--select top(1) @SectionIdentifier5 = SectionIdentifier, @BeginDate5 = DateAssigned, @GradebookEntryTitle5 = GradebookEntryTitle, @LocalCourseCode5 = LocalCourseCode from edfi.GradebookEntry where LocalCourseCode = @@LocalCourseCode5 and SchoolId = @@SchoolId and SessionName = @SessionName and SchoolYear = @@CurrentSchoolYear
		
		select @EndDate1 = EndDate from edfi.StudentSectionAssociation where BeginDate = @BeginDate1
		select @EndDate2 = EndDate from edfi.StudentSectionAssociation where BeginDate = @BeginDate2 
		select @EndDate3 = EndDate from edfi.StudentSectionAssociation where BeginDate = @BeginDate3 
		select @EndDate4 = EndDate from edfi.StudentSectionAssociation where BeginDate = @BeginDate4 
		--select @EndDate5 = EndDate from edfi.StudentSectionAssociation where BeginDate = @BeginDate5 
		
		INSERT INTO [edfi].[StudentSectionAssociation]
		           ([BeginDate]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[EndDate]
		           ,[HomeroomIndicator])
		     VALUES
		           (@BeginDate1
		           ,@@LocalCourseCode1
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier1
		           ,@SessionName
		           ,@@StudentUSI
		           ,@EndDate1
		           ,0);
		
		INSERT INTO [edfi].[StudentSectionAssociation]
		           ([BeginDate]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[EndDate]
		           ,[HomeroomIndicator])
		     VALUES
		           (@BeginDate2
		           ,@@LocalCourseCode2
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier2
		           ,@SessionName
		           ,@@StudentUSI
		           ,@EndDate2
		           ,0);
		
		INSERT INTO [edfi].[StudentSectionAssociation]
		           ([BeginDate]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[EndDate]
		           ,[HomeroomIndicator])
		     VALUES
		           (@BeginDate3
		           ,@@LocalCourseCode3
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier3
		           ,@SessionName
		           ,@@StudentUSI
		           ,@EndDate3
		           ,0);
		
		INSERT INTO [edfi].[StudentSectionAssociation]
		           ([BeginDate]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[EndDate]
		           ,[HomeroomIndicator])
		     VALUES
		           (@BeginDate4
		           ,@@LocalCourseCode4
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier4
		           ,@SessionName
		           ,@@StudentUSI
		           ,@EndDate4
		           ,0);
		
		--INSERT INTO [edfi].[StudentSectionAssociation]
		--           ([BeginDate]
		--           ,[LocalCourseCode]
		--           ,[SchoolId]
		--           ,[SchoolYear]
		--           ,[SectionIdentifier]
		--           ,[SessionName]
		--           ,[StudentUSI]
		--           ,[EndDate]
		--           ,[HomeroomIndicator])
		--     VALUES
		--           (@BeginDate5
		--           ,@@LocalCourseCode5
		--           ,@@SchoolId
		--           ,@@CurrentSchoolYear
		--           ,@SectionIdentifier5
		--           ,@SessionName
		--           ,@@StudentUSI
		--           ,@EndDate5
		--           ,0)
		
		INSERT INTO [edfi].[StudentGradebookEntry]
		           ([BeginDate]
		           ,[DateAssigned]
		           ,[GradebookEntryTitle]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[LetterGradeEarned])
		     VALUES
		           (@BeginDate1
		           ,@BeginDate1
		           ,@GradebookEntryTitle1
		           ,@LocalCourseCode1
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier1
		           ,@SessionName
		           ,@@StudentUSI
		           ,'Missing')

		
		INSERT INTO [edfi].[DisciplineIncident]
				   ([IncidentIdentifier]
				   ,[SchoolId]
				   ,[IncidentDate]
				   ,[IncidentDescription])
			 VALUES
				   ('9999'
				   ,56
				   ,GETDATE()
				   ,'Test incident')

		INSERT INTO [edfi].[StudentDisciplineIncidentAssociation]
		           ([IncidentIdentifier]
		           ,[SchoolId]
		           ,[StudentUSI]
		           ,[StudentParticipationCodeDescriptorId])
		     VALUES
		           ('9999'
		           ,@@SchoolId
		           ,@@StudentUSI
		           ,@@StudentParticipationCodeDescriptorId)
		
		INSERT INTO [edfi].[StudentEducationOrganizationAssociation]
		           ([EducationOrganizationId]
		           ,[StudentUSI]
		           ,[SexDescriptorId]
		           ,[HispanicLatinoEthnicity])
		     VALUES
		           (@@SchoolId
		           ,@@StudentUSI
		           ,@SexDescriptorId
		           ,1)
		
		INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail]
		           ([EducationOrganizationId]
		           ,[ElectronicMailTypeDescriptorId]
		           ,[StudentUSI]
		           ,[ElectronicMailAddress])
		     VALUES
		           (@@SchoolId
		           ,@ElectronicMailTypeDescriptorId
		           ,@@StudentUSI
		           ,@@StudentFirstName + @@StudentLastSurname+ '@yesprep.com')
		
		INSERT INTO [edfi].[StudentEducationOrganizationAssociationTelephone]
		           ([EducationOrganizationId]
		           ,[StudentUSI]
		           ,[TelephoneNumberTypeDescriptorId]
		           ,[TelephoneNumber])
		     VALUES
		           (@@SchoolId
		           ,@@StudentUSI
		           ,@TelephoneNumberTypeDescriptorId
		           ,'(123) 456 7890')
		
		INSERT INTO [edfi].[StudentEducationOrganizationAssociationRace]
		           ([EducationOrganizationId]
				   ,[RaceDescriptorId]
				   ,[StudentUSI])
		    VALUES (@@SchoolId
					,@RaceDescriptorId
					,@@StudentUSI)
		
		INSERT INTO [edfi].[StudentSchoolAttendanceEvent]
		           ([AttendanceEventCategoryDescriptorId]
		           ,[EventDate]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[AttendanceEventReason])
		     VALUES
		           (@AttendanceEventCategoryDescriptorId
		           ,'2020-03-24' 
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SessionName
		           ,@@StudentUSI
		           ,'The student left the school.')
		
		INSERT INTO [edfi].[Grade]
		           ([BeginDate]
		           ,[GradeTypeDescriptorId]
		           ,[GradingPeriodDescriptorId]
		           ,[GradingPeriodSchoolYear]
		           ,[GradingPeriodSequence]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[NumericGradeEarned])
		     VALUES
		           (@BeginDate1
		           ,@@GradeTypeDescriptorId -- Semester
		           ,934 -- A1
		           ,@@CurrentSchoolYear-- year of period
		           ,5
		           ,@LocalCourseCode1
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier1
		           ,@SessionName
		           ,@@StudentUSI
		           ,90.00)
			
		INSERT INTO [edfi].[Grade]
		           ([BeginDate]
		           ,[GradeTypeDescriptorId]
		           ,[GradingPeriodDescriptorId]
		           ,[GradingPeriodSchoolYear]
		           ,[GradingPeriodSequence]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[NumericGradeEarned])
		     VALUES
		           (@BeginDate2
		           ,@@GradeTypeDescriptorId  -- Semester
		           ,934 --A2
		           ,@@CurrentSchoolYear
		           ,5
		           ,@LocalCourseCode2
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier2
		           ,@SessionName
		           ,@@StudentUSI
		           ,63.00)
				   
		INSERT INTO [edfi].[Grade]
		           ([BeginDate]
		           ,[GradeTypeDescriptorId]
		           ,[GradingPeriodDescriptorId]
		           ,[GradingPeriodSchoolYear]
		           ,[GradingPeriodSequence]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[NumericGradeEarned])
		     VALUES
		           (@BeginDate3
		           ,@@GradeTypeDescriptorId
		           ,934 --A4
		           ,@@CurrentSchoolYear -- year of period
		           ,5
		           ,@LocalCourseCode3
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier3
		           ,@SessionName
		           ,@@StudentUSI
		           ,72.00)
				   
		INSERT INTO [edfi].[Grade]
		           ([BeginDate]
		           ,[GradeTypeDescriptorId]
		           ,[GradingPeriodDescriptorId]
		           ,[GradingPeriodSchoolYear]
		           ,[GradingPeriodSequence]
		           ,[LocalCourseCode]
		           ,[SchoolId]
		           ,[SchoolYear]
		           ,[SectionIdentifier]
		           ,[SessionName]
		           ,[StudentUSI]
		           ,[NumericGradeEarned])
		     VALUES
		           (@BeginDate4
		           ,@@GradeTypeDescriptorId
		           ,934 --A5
		           ,@@CurrentSchoolYear -- year of period
		           ,5
		           ,@LocalCourseCode4
		           ,@@SchoolId
		           ,@@CurrentSchoolYear
		           ,@SectionIdentifier4
		           ,@SessionName
		           ,@@StudentUSI
		           ,82.00)
				  
INSERT INTO [edfi].[Assessment]
           ([AssessmentIdentifier]
           ,[Namespace]
           ,[AssessmentTitle]
           ,[AssessmentCategoryDescriptorId]
           ,[CreateDate])
     VALUES
           ('STAAR MATHEMATICS','uri://STAAR.gov/Assessment','State of Texas Assessments of Academic Readiness (STAAR) - 3rd Through 8th',145,GETUTCDATE()),
           ('STAAR READING','uri://STAAR.gov/Assessment','State of Texas Assessments of Academic Readiness (STAAR) - 3rd Through 8th',145,GETUTCDATE()),
           ('STAAR SCIENCE','uri://STAAR.gov/Assessment','State of Texas Assessments of Academic Readiness (STAAR) - 3rd Through 8th',145,GETUTCDATE())


INSERT INTO [edfi].[AssessmentScore]
           ([AssessmentIdentifier]
           ,[AssessmentReportingMethodDescriptorId]
           ,[Namespace]
           ,[MinimumScore]
           ,[MaximumScore]
           ,[ResultDatatypeTypeDescriptorId]
           ,[CreateDate])
     VALUES
          ('STAAR MATHEMATICS',218,'uri://STAAR.gov/Assessment',0,100	,1926,GETUTCDATE()),
           ('STAAR READING',218,'uri://STAAR.gov/Assessment',0,100	,1926,GETUTCDATE()),
           ('STAAR SCIENCE',218,'uri://STAAR.gov/Assessment',0,100	,1926,GETUTCDATE())



INSERT INTO [edfi].[StudentAssessment]
           ([AssessmentIdentifier]
           ,[Namespace]
           ,[StudentAssessmentIdentifier]
           ,[StudentUSI]
		   ,[WhenAssessedGradeLevelDescriptorId]
           ,[AdministrationDate]
           ,[SchoolYear]
           ,[CreateDate])
     VALUES
          ('STAAR MATHEMATICS','uri://STAAR.gov/Assessment','13/05/2021',@@StudentUSI,918 ,'2019-04-15 00:00:00.0000000',2019,GETUTCDATE()),
           ('STAAR READING','uri://STAAR.gov/Assessment','13/05/2021',@@StudentUSI,904 ,'2020-04-15 00:00:00.0000000',2020,GETUTCDATE()),
           ('STAAR SCIENCE','uri://STAAR.gov/Assessment','13/05/2021',@@StudentUSI,919 ,'2018-04-15 00:00:00.0000000',2018,GETUTCDATE())


 
INSERT INTO [edfi].[StudentAssessmentScoreResult]
           ([AssessmentIdentifier]
           ,[AssessmentReportingMethodDescriptorId]
           ,[Namespace]
           ,[StudentAssessmentIdentifier]
           ,[StudentUSI]
           ,[Result]
           ,[ResultDatatypeTypeDescriptorId]
           ,[CreateDate])
     VALUES
           ('STAAR MATHEMATICS',218,'uri://STAAR.gov/Assessment','13/05/2021',@@StudentUSI,89,1926,GETUTCDATE()),
           ('STAAR READING',218,'uri://STAAR.gov/Assessment','13/05/2021',@@StudentUSI,92,1926,GETUTCDATE()),
           ('STAAR SCIENCE',218,'uri://STAAR.gov/Assessment','13/05/2021',@@StudentUSI,80,1926,GETUTCDATE())



INSERT INTO [edfi].[StudentAssessmentPerformanceLevel]
           ([AssessmentIdentifier]
           ,[AssessmentReportingMethodDescriptorId]
           ,[Namespace]
           ,[PerformanceLevelDescriptorId]
           ,[StudentAssessmentIdentifier]
           ,[StudentUSI]
           ,[PerformanceLevelMet]
           ,[CreateDate])
     VALUES ('STAAR MATHEMATICS',218,'uri://STAAR.gov/Assessment',2852,'13/05/2021',15480,1,GETUTCDATE()),
      ('STAAR READING',218,'uri://STAAR.gov/Assessment',2852,'13/05/2021',15480,1,GETUTCDATE()),
      ('STAAR SCIENCE',218,'uri://STAAR.gov/Assessment',2852,'13/05/2021',15480,1,GETUTCDATE()) 
		--INSERT INTO [edfi].[Grade]
		--           ([BeginDate]
		--           ,[GradeTypeDescriptorId]
		--           ,[GradingPeriodDescriptorId]
		--           ,[GradingPeriodSchoolYear]
		--           ,[GradingPeriodSequence]
		--           ,[LocalCourseCode]
		--           ,[SchoolId]
		--           ,[SchoolYear]
		--           ,[SectionIdentifier]
		--           ,[SessionName]
		--           ,[StudentUSI]
		--           ,[NumericGradeEarned])
		--     VALUES
		--           (@BeginDate5
		--           ,@@GradeTypeDescriptorId
		--           ,1361 --A6
		--           ,@@CurrentSchoolYear -- year of period
		--           ,6
		--           ,@LocalCourseCode5
		--           ,@@SchoolId
		--           ,@@CurrentSchoolYear
		--           ,@SectionIdentifier5
		--           ,@SessionName
		--           ,@@StudentUSI
		--           ,75.00);

	COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

		DECLARE @Message nvarchar(2048) = ERROR_MESSAGE() + '/Line:' + ERROR_LINE();
        DECLARE @Severity integer = ERROR_SEVERITY();
        DECLARE @State integer = ERROR_STATE();

        RAISERROR(@Message, @Severity, @State);
	END CATCH;
END;
