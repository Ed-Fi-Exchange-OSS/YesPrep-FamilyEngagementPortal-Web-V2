BEGIN
	BEGIN TRY
	BEGIN TRANSACTION
	-- NOTE: Following the Ed-Fi dependency graph we should add the Education Organizations first.

	-- ************************************************************** --
	-- *** Adding the Education Organizations 1 LEA and 2 Schools *** --
	-- ************************************************************** --
	declare @LocalEducationAgencyId int = 61111;
	declare @LocalEducationAgencyName nvarchar(75)= 'Loyd ISD';
	declare @DiscriminatorLocalEdAgency nvarchar(50)= 'edfi.LocalEducationAgency';

	declare @HighSchoolId int = 62222;
	declare @HighSchoolName nvarchar(25) = 'Harris High School';
	declare @DiscriminatorHighSchool nvarchar(50)= 'edfi.School';
	
	declare @MiddleSchoolId int = 63333;
	declare @MiddleSchoolName nvarchar(25) = 'Chirris Middle School';
	declare @MiddleSchoolDiscriminator nvarchar(50)= 'edfi.School';

	-- *** Get the Descriptors we need to add the Education Organization entities. *** --
	declare @OperationalStatusActiveDescriptorId int;
	Select top 1 @OperationalStatusActiveDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Active' AND [Namespace] = 'uri://ed-fi.org/OperationalStatusDescriptor';
	declare @LocalEducationAgencyCategoryDescriptorId int;
	Select top 1 @LocalEducationAgencyCategoryDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Independent' AND [Namespace] = 'uri://ed-fi.org/LocalEducationAgencyCategoryDescriptor';

	-- Adding the LEA
	INSERT INTO [edfi].[EducationOrganization] ([EducationOrganizationId],[NameOfInstitution],[OperationalStatusDescriptorId],[Discriminator])
										VALUES (@LocalEducationAgencyId,@LocalEducationAgencyName,@OperationalStatusActiveDescriptorId,@DiscriminatorLocalEdAgency);
	INSERT INTO [edfi].[LocalEducationAgency] ([LocalEducationAgencyId],[LocalEducationAgencyCategoryDescriptorId])
										VALUES (@LocalEducationAgencyId,@LocalEducationAgencyCategoryDescriptorId);
	-- Adding the Schools
	INSERT INTO [edfi].[EducationOrganization] ([EducationOrganizationId],[NameOfInstitution],[OperationalStatusDescriptorId],[Discriminator])
										VALUES (@HighSchoolId,@HighSchoolName,@OperationalStatusActiveDescriptorId,@DiscriminatorHighSchool);
	INSERT INTO [edfi].[School] ([SchoolId],[LocalEducationAgencyId]) VALUES (@HighSchoolId,@LocalEducationAgencyId);

	INSERT INTO [edfi].[EducationOrganization] ([EducationOrganizationId],[NameOfInstitution],[OperationalStatusDescriptorId],[Discriminator])
										VALUES (@MiddleSchoolId,@MiddleSchoolName,@OperationalStatusActiveDescriptorId,@MiddleSchoolDiscriminator);
	INSERT INTO [edfi].[School] ([SchoolId],[LocalEducationAgencyId]) VALUES (@MiddleSchoolId,@LocalEducationAgencyId);

	-- ******************************************************************* --
	-- *** Adding People: Students, Parents, Teachers & Campus Leaders *** --
	-- ******************************************************************* --
	
	declare @BirthFemaleSexDescriptorId int; 
	Select top 1 @BirthFemaleSexDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Female' AND [Namespace] = 'uri://ed-fi.org/SexDescriptor';

	declare @BirthMaleSexDescriptorId int ;   
	Select top 1 @BirthMaleSexDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Male' AND [Namespace] = 'uri://ed-fi.org/SexDescriptor';

	declare @RelationDescriptorIdFather int;   
	Select top 1 @RelationDescriptorIdFather  = DescriptorId FROM edfi.Descriptor where [Description] = 'Father' AND [Namespace] = 'uri://ed-fi.org/RelationDescriptor';
	declare @RelationdescriptorIdMother int;
	Select top 1 @RelationdescriptorIdMother  = DescriptorId FROM edfi.Descriptor where [Description] = 'Mother' AND [Namespace] = 'uri://ed-fi.org/RelationDescriptor';

	declare @ElectronicMailTypeDescriptorId int;
	Select top 1 @ElectronicMailTypeDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Home/Personal' AND [Namespace] = 'uri://ed-fi.org/ElectronicMailTypeDescriptor';
	
	declare @StaffClassificationDescriptorIdLeaAdmin int;
	Select top 1 @StaffClassificationDescriptorIdLeaAdmin  = DescriptorId FROM edfi.Descriptor where [Description] = 'LEA Administrator' AND [Namespace] = 'uri://ed-fi.org/StaffClassificationDescriptor';
	
	declare @StaffClassificationDescriptorIdPrincipal int;
	Select top 1 @StaffClassificationDescriptorIdPrincipal  = DescriptorId FROM edfi.Descriptor where [Description] = 'Principal' AND [Namespace] = 'uri://ed-fi.org/StaffClassificationDescriptor';
	
	declare @StaffClassificationDescriptorIdTeacher int;
	Select top 1 @StaffClassificationDescriptorIdTeacher  = DescriptorId FROM edfi.Descriptor where [Description] = 'Teacher' AND [Namespace] = 'uri://ed-fi.org/StaffClassificationDescriptor';
	
	declare @@GradeLevelDescriptorEighthGradeId int;
	Select top 1 @@GradeLevelDescriptorEighthGradeId = DescriptorId FROM edfi.Descriptor where [Description] = 'Eighth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';

	declare @GradeLevelDescriptorIdTenthGradeId int;
	Select top 1 @GradeLevelDescriptorIdTenthGradeId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Tenth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';

	-- ** Users ** --
	--  Parents  --
	declare @@ParentUniqueId1 nvarchar(50) = '000001';
	declare @@ParentPersonalPrefix1 nvarchar(50) = 'Mrs';
	declare @@ParentFirstName1 nvarchar(10) = 'Linda';
	declare @@ParentLastSurname1 nvarchar(10) = 'Olivas';
	declare @@EmailLogin1 nvarchar(50) = 'lolivas_p_test@student1.org';
	declare @@ParentSexDescriptor1 int = @BirthFemaleSexDescriptorId;
	declare @@ParentRelationDescriptor1 int = @RelationdescriptorIdMother;

	declare @@ParentUniqueId2 nvarchar(50) = '000002';
	declare @@ParentPersonalPrefix2 nvarchar(50) = 'Mrs';
	declare @@ParentFirstName2 nvarchar(10) = 'Patricia';
	declare @@ParentLastSurname2 nvarchar(10) = 'Navarro';
	declare @@EmailLogin2 nvarchar(50) = 'pnavarro_p_test@student1.org';
	declare @@ParentSexDescriptor2 int = @BirthFemaleSexDescriptorId;
	declare @@ParentRelationDescriptor2 int = @RelationdescriptorIdMother;

	-- ** Students Identification, Names and Demographics ** --
	declare @@StudentUniqueId1 nvarchar(50) = '000007';
	declare @StudentPrefix1 nvarchar(3) = 'Mrs';
	declare @@StudentFirstName1 nvarchar(10) = 'Julia';
	declare @@StudentMiddleName1 nvarchar(10) = 'K';
	declare @@StudentLastSurname1 nvarchar(10) = 'Hernandez';
	declare @@StudentBirthDate1 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor1 int = @BirthFemaleSexDescriptorId;
		
	declare @@StudentUniqueId2 nvarchar(50) = '000008';
	declare @StudentPrefix2 nvarchar(3) = 'Mr';
	declare @@StudentFirstName2 nvarchar(10) = 'John';
	declare @@StudentMiddleName2 nvarchar(10) = '';
	declare @@StudentLastSurname2 nvarchar(10) = 'Hernandez';
	declare @@StudentBirthDate2 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor2 int = @BirthMaleSexDescriptorId;

	declare @@StudentUniqueId3 nvarchar(50) = '000009';
	declare @StudentPrefix3 nvarchar(3) = 'Mr';
	declare @@StudentFirstName3 nvarchar(10) = 'Eduard';
	declare @@StudentMiddleName3 nvarchar(10) = null;
	declare @@StudentLastSurname3 nvarchar(10) = 'Colling';
	declare @@StudentBirthDate3 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor3 int = @BirthMaleSexDescriptorId;

	declare @@StudentUniqueId4 nvarchar(50) = '000010';
	declare @StudentPrefix4 nvarchar(3) = 'Mrs';
	declare @@StudentFirstName4 nvarchar(10) = 'Fernanda';
	declare @@StudentMiddleName4 nvarchar(10) = null;
	declare @@StudentLastSurname4 nvarchar(10) = 'Jhonson';
	declare @@StudentBirthDate4 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor4 int = @BirthFemaleSexDescriptorId;

	declare @@StudentUniqueId5 nvarchar(50) = '000011';
	declare @StudentPrefix5 nvarchar(3) = 'Mr';
	declare @@StudentFirstName5 nvarchar(10) = 'John';
	declare @@StudentMiddleName5 nvarchar(10) = null;
	declare @@StudentLastSurname5 nvarchar(10) = 'McDonal';
	declare @@StudentBirthDate5 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor5 int = @BirthMaleSexDescriptorId;

	declare @@StudentUniqueId6 nvarchar(50) = '000012';
	declare @StudentPrefix6 nvarchar(3) = 'Mr';
	declare @@StudentFirstName6 nvarchar(10) = 'Alejandro';
	declare @@StudentMiddleName6 nvarchar(10) = null;
	declare @@StudentLastSurname6 nvarchar(10) = 'Juarez';
	declare @@StudentBirthDate6 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor6 int = @BirthMaleSexDescriptorId;

	declare @@StudentUniqueId7 nvarchar(50) = '000013';
	declare @StudentPrefix7 nvarchar(3) = 'Mr';
	declare @@StudentFirstName7 nvarchar(10) = 'Melwin';
	declare @@StudentMiddleName7 nvarchar(10) = null;
	declare @@StudentLastSurname7 nvarchar(10) = 'Barreto';
	declare @@StudentBirthDate7 nvarchar(10) = '2000-07-29';
	declare @@StudentSexDescriptor7 int = @BirthMaleSexDescriptorId;

	
	declare @@StudentUniqueId8 nvarchar(50) = '000014';
	declare @StudentPrefix8 nvarchar(3) = 'Mr';
	declare @@StudentFirstName8 nvarchar(10) = 'Irvin';
	declare @@StudentMiddleName8 nvarchar(10) = null;
	declare @@StudentLastSurname8 nvarchar(10) = 'Hernandez';
	declare @@StudentBirthDate8 nvarchar(10) = '2000-08-29';
	declare @@StudentSexDescriptor8 int = @BirthMaleSexDescriptorId;
	
	declare @@StudentUniqueId9 nvarchar(50) = '000019';
	declare @StudentPrefix9 nvarchar(3) = 'Mr';
	declare @@StudentFirstName9 nvarchar(10) = 'Kevin';
	declare @@StudentMiddleName9 nvarchar(10) = null;
	declare @@StudentLastSurname9 nvarchar(10) = 'Hernandez';
	declare @@StudentBirthDate9 nvarchar(10) = '2000-09-29';
	declare @@StudentSexDescriptor9 int = @BirthMaleSexDescriptorId;
	
	declare @@StudentUniqueId10 nvarchar(50) = '000020';
	declare @StudentPrefix10 nvarchar(3) = 'Ms';
	declare @@StudentFirstName10 nvarchar(10) = 'Alicia';
	declare @@StudentMiddleName10 nvarchar(10) = null;
	declare @@StudentLastSurname10 nvarchar(10) = 'Millan';
	declare @@StudentBirthDate10 nvarchar(10) = '2000-10-210';
	declare @@StudentSexDescriptor10 int = @BirthFemaleSexDescriptorId;

	-- ** Parent Name and Demographics ** --
	declare @@ParentUniqueId3 nvarchar(50) = '000021';
	declare @@ParentPersonalPrefix3 nvarchar(50) = 'Mr';
	declare @@ParentFirstName3 nvarchar(10) = 'Ibrahim';
	declare @@ParentLastSurname3 nvarchar(10) = 'Wang';
	declare @@EmailLogin3 nvarchar(50) = 'wang@mailinator.com';
	declare @@ParentSexDescriptor3 int = @BirthMaleSexDescriptorId;
	declare @@ParentRelationDescriptor3 int = @RelationdescriptorIdFather;

	declare @@ParentUniqueId4 nvarchar(50) = '000022';
	declare @@ParentPersonalPrefix4 nvarchar(50) = 'Mrs';
	declare @@ParentFirstName4 nvarchar(10) = 'Eulalia';
	declare @@ParentLastSurname4 nvarchar(10) = 'Bonet';
	declare @@EmailLogin4 nvarchar(50) = 'bonet@mailinator.com';
	declare @@ParentSexDescriptor4 int = @BirthFemaleSexDescriptorId;
	declare @@ParentRelationDescriptor4 int = @RelationdescriptorIdMother;

	declare @@ParentUniqueId5 nvarchar(50) = '000016';
	declare @@ParentPersonalPrefix5 nvarchar(50) = 'Mr';
	declare @@ParentFirstName5 nvarchar(10) = 'Oriol';
	declare @@ParentLastSurname5 nvarchar(10) = 'Ye';
	declare @@EmailLogin5 nvarchar(50) = 'oriol.ye@mailinator.com';
	declare @@ParentSexDescriptor5 int = @BirthMaleSexDescriptorId;
	declare @@ParentRelationDescriptor5 int = @RelationdescriptorIdFather;

	declare @@ParentUniqueId6 nvarchar(50) = '000017';
	declare @@ParentPersonalPrefix6 nvarchar(50) = 'Mrs';
	declare @@ParentFirstName6 nvarchar(10) = 'Florencia';
	declare @@ParentLastSurname6 nvarchar(10) = 'Ferrer';
	declare @@EmailLogin6 nvarchar(50) = 'ferrer@mailinator.com';
	declare @@ParentSexDescriptor6 int = @BirthFemaleSexDescriptorId;
	declare @@ParentRelationDescriptor6 int = @RelationdescriptorIdMother;

	declare @@ParentUniqueId7 nvarchar(50) = '000018';
	declare @@ParentPersonalPrefix7 nvarchar(50) = 'Mrs';
	declare @@ParentFirstName7 nvarchar(10) = 'Francisca';
	declare @@ParentLastSurname7 nvarchar(10) = 'Rio';
	declare @@EmailLogin7 nvarchar(50) = 'rio@mailinator.com';
	declare @@ParentSexDescriptor7 int = @BirthFemaleSexDescriptorId;
	declare @@ParentRelationDescriptor7 int = @RelationdescriptorIdMother;
	
	-- Adding Students and Parents

	declare @ParentUSI2 int;
	declare @ParentUSI3 int;
	declare @ParentUSI4 int;
	declare @ParentUSI5 int;
	declare @ParentUSI6 int;
	declare @ParentUSI7 int;
	
	print('Inserting students & parents')
	-- Inserting Student1, Parent1 and associating them.
	print('Inserting student 1')

	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix1,@@StudentFirstName1,@@StudentMiddleName1,@@StudentLastSurname1,@@StudentBirthDate1,@@StudentSexDescriptor1,@@StudentUniqueId1)
	
	declare @StudentUSI1 int;
	set @StudentUSI1 = SCOPE_IDENTITY();
	print('Inserting student 8')

	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix8,@@StudentFirstName8,@@StudentMiddleName8,@@StudentLastSurname8,@@StudentBirthDate8,@@StudentSexDescriptor8,@@StudentUniqueId8)
	
	declare @StudentUSI8 int;
	set @StudentUSI8 = SCOPE_IDENTITY();
	print('Inserting student 9')

	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix9,@@StudentFirstName9,@@StudentMiddleName9,@@StudentLastSurname9,@@StudentBirthDate9,@@StudentSexDescriptor9,@@StudentUniqueId9)
	
	declare @StudentUSI9 int;
	set @StudentUSI9 = SCOPE_IDENTITY();
	print('Inserting parent1')

	INSERT INTO [edfi].[Parent] ([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
						 VALUES (@@ParentPersonalPrefix1,@@ParentFirstName1,@@ParentLastSurname1,@@ParentSexDescriptor1,@@ParentUniqueId1);

	declare @ParentUSI1 int;
	set @ParentUSI1 = SCOPE_IDENTITY();
	print('Inserting parent1 association student 1,8,9')

	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
										   VALUES (@ParentUSI1 ,@StudentUSI1 ,@@ParentRelationDescriptor1,1,1,1);
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
										   VALUES (@ParentUSI1 ,@StudentUSI8 ,@@ParentRelationDescriptor1,1,1,1);
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
										   VALUES (@ParentUSI1 ,@StudentUSI9 ,@@ParentRelationDescriptor1,1,1,1);
	print('Inserting parent1 email')

	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI1,@@EmailLogin1 ,1)

	-- Inserting Student2, Parent2 and associating them.
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix2,@@StudentFirstName2,@@StudentMiddleName2,@@StudentLastSurname2,@@StudentBirthDate2,@@StudentSexDescriptor2,@@StudentUniqueId2);
	
	declare @StudentUSI2 int;
	set @StudentUSI2 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix10,@@StudentFirstName10,@@StudentMiddleName10,@@StudentLastSurname10,@@StudentBirthDate10,@@StudentSexDescriptor10,@@StudentUniqueId10)
	
	declare @StudentUSI10 int;
	set @StudentUSI10 = SCOPE_IDENTITY();
		
	INSERT INTO [edfi].[Parent]([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
		     VALUES(@@ParentPersonalPrefix2 ,@@ParentFirstName2 ,@@ParentLastSurname2 ,@@ParentSexDescriptor2 ,@@ParentUniqueId2)

	set @ParentUSI2 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI2 ,@StudentUSI2,@@ParentRelationDescriptor2,1,1,1)
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI2 ,@StudentUSI10,@@ParentRelationDescriptor2,1,1,1)


	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI2,@@EmailLogin2 ,1)
	-- Inserting Student3, Parent3 and associating them.
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix3,@@StudentFirstName3,@@StudentMiddleName3,@@StudentLastSurname3,@@StudentBirthDate3,@@StudentSexDescriptor3,@@StudentUniqueId3);
	
	declare @StudentUSI3 int;
	set @StudentUSI3 = SCOPE_IDENTITY();
		
	INSERT INTO [edfi].[Parent]([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
		     VALUES(@@ParentPersonalPrefix3 ,@@ParentFirstName3 ,@@ParentLastSurname3 ,@@ParentSexDescriptor3 ,@@ParentUniqueId3)

	set @ParentUSI3 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI3 ,@StudentUSI3,@@ParentRelationDescriptor3,1,1,1)
	
	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI3,@@EmailLogin3 ,1)
	-- Inserting Student4, Parent4 and associating them.
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix4,@@StudentFirstName4,@@StudentMiddleName4,@@StudentLastSurname4,@@StudentBirthDate4,@@StudentSexDescriptor4,@@StudentUniqueId4);
	
	declare @StudentUSI4 int;
	set @StudentUSI4 = SCOPE_IDENTITY();
		
	INSERT INTO [edfi].[Parent]([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
		     VALUES(@@ParentPersonalPrefix4 ,@@ParentFirstName4 ,@@ParentLastSurname4 ,@@ParentSexDescriptor4 ,@@ParentUniqueId4)

	set @ParentUSI4 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI4 ,@StudentUSI4,@@ParentRelationDescriptor4,1,1,1)
			
	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI4,@@EmailLogin4 ,1)
-- Inserting Student5, Parent5 and associating them.
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix5,@@StudentFirstName5,@@StudentMiddleName5,@@StudentLastSurname5,@@StudentBirthDate5,@@StudentSexDescriptor5,@@StudentUniqueId5);
	
	declare @StudentUSI5 int;
	set @StudentUSI5 = SCOPE_IDENTITY();
		
	INSERT INTO [edfi].[Parent]([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
		     VALUES(@@ParentPersonalPrefix5 ,@@ParentFirstName5 ,@@ParentLastSurname5 ,@@ParentSexDescriptor5 ,@@ParentUniqueId5)

	set @ParentUSI5 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI5 ,@StudentUSI5,@@ParentRelationDescriptor5,1,1,1)
			
	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI5,@@EmailLogin5 ,1)
	-- Inserting Student6, Parent6 and associating them.
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix6,@@StudentFirstName6,@@StudentMiddleName6,@@StudentLastSurname6,@@StudentBirthDate6,@@StudentSexDescriptor6,@@StudentUniqueId6);
	
	declare @StudentUSI6 int;
	set @StudentUSI6 = SCOPE_IDENTITY();
		
	INSERT INTO [edfi].[Parent]([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
		     VALUES(@@ParentPersonalPrefix6 ,@@ParentFirstName6 ,@@ParentLastSurname6 ,@@ParentSexDescriptor6 ,@@ParentUniqueId6)

	set @ParentUSI6 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI6 ,@StudentUSI6,@@ParentRelationDescriptor6,1,1,1)

	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI6,@@EmailLogin6 ,1)

	-- Inserting Student7, Parent7 and associating them.
	INSERT INTO [edfi].[Student] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[BirthDate],[BirthSexDescriptorId],[StudentUniqueId])
		     VALUES (@StudentPrefix7,@@StudentFirstName7,@@StudentMiddleName7,@@StudentLastSurname7,@@StudentBirthDate7,@@StudentSexDescriptor7,@@StudentUniqueId7);
	
	declare @StudentUSI7 int;
	set @StudentUSI7 = SCOPE_IDENTITY();
		
	INSERT INTO [edfi].[Parent]([PersonalTitlePrefix],[FirstName],[LastSurname],[SexDescriptorId],[ParentUniqueId])
		     VALUES(@@ParentPersonalPrefix7 ,@@ParentFirstName7 ,@@ParentLastSurname7 ,@@ParentSexDescriptor7 ,@@ParentUniqueId7)

	set @ParentUSI7 = SCOPE_IDENTITY();
	
	INSERT INTO [edfi].[StudentParentAssociation] ([ParentUSI],[StudentUSI],[RelationDescriptorId],[PrimaryContactStatus],[LivesWith],[EmergencyContactStatus]) 
			VALUES (@ParentUSI7 ,@StudentUSI7,@@ParentRelationDescriptor7,1,1,1)

	INSERT INTO [edfi].[ParentElectronicMail] ([ElectronicMailTypeDescriptorId],[ParentUSI],[ElectronicMailAddress],[PrimaryEmailAddressIndicator])
		     VALUES (@ElectronicMailTypeDescriptorId,@ParentUSI7,@@EmailLogin7 ,1)


	declare @TelephoneMobileDescriptor int = null        
    Select top 1 @TelephoneMobileDescriptor  =    DescriptorId FROM edfi.Descriptor where [Description] = 'Mobile' AND [Namespace] = 'uri://ed-fi.org/TelephoneNumberTypeDescriptor';


	INSERT INTO [edfi].[StudentEducationOrganizationAssociationTelephone]
			   ([EducationOrganizationId]
			   ,[StudentUSI]
			   ,[TelephoneNumberTypeDescriptorId]
			   ,[TelephoneNumber]
			   ,[OrderOfPriority]
			   ,[CreateDate])
		 VALUES
		 (@HighSchoolId,@StudentUSI1,@TelephoneMobileDescriptor,4567894532,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI2,@TelephoneMobileDescriptor,4253699685,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI3,@TelephoneMobileDescriptor,6565895252,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI4,@TelephoneMobileDescriptor,6598986525,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI5,@TelephoneMobileDescriptor,1125236582,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI6,@TelephoneMobileDescriptor,8954852205,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI7,@TelephoneMobileDescriptor,7878462100,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI8,@TelephoneMobileDescriptor,1712585258,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI9,@TelephoneMobileDescriptor,2556585557,1,GETUTCDATE()),
		 (@HighSchoolId,@StudentUSI10,@TelephoneMobileDescriptor,1525286412,1,GETUTCDATE())
	-- ** Staff & Staff Electronic Mail ** --
	-- Teachers --

	declare @StaffPersonalPrefix1 nvarchar(30) = 'Ms';
	declare @StaffFirstName1 nvarchar(75) = 'Karla ';
	declare @StaffMiddleName1 nvarchar(75) = 'T';
	declare @StaffLastName1 nvarchar(75) = 'Pedraza';
	declare @StaffBirthDate1 date = GETDATE();
	declare @LoginId1 nvarchar(60) = 'kpedraza';
	declare @HighlyQualifiedTeacher1 bit = 0;
	declare @StaffUniqueId1 nvarchar(32) = '000003';
	declare @StaffEmail1 nvarchar(128) = 'kpedraza_t_test@student1.org';
	declare @StaffSexDescriptor1 int = @BirthFemaleSexDescriptorId;
	declare @StaffClassificationDescriptorId1 int = @StaffClassificationDescriptorIdTeacher;
	declare @StaffGradeLevelDescriptorId1 int = @@GradeLevelDescriptorEighthGradeId;
	declare @StaffSchoolId1 int = @HighSchoolId;

	declare @StaffPersonalPrefix2 nvarchar(30) = 'Ms';
	declare @StaffFirstName2 nvarchar(75) = 'Scarlett';
	declare @StaffMiddleName2 nvarchar(75) = 'T';
	declare @StaffLastName2 nvarchar(75) = 'Sarabia';
	declare @StaffBirthDate2 date = GETDATE();
	declare @LoginId2 nvarchar(60) = 'ssarabia';
	declare @HighlyQualifiedTeacher2 bit = 0;
	declare @StaffUniqueId2 nvarchar(32) = '000004';
	declare @StaffEmail2 nvarchar(128) = 'ssarabia_t_test@student1.org';
	declare @StaffSexDescriptor2 int = @BirthFemaleSexDescriptorId;
	declare @StaffClassificationDescriptorId2 int = @StaffClassificationDescriptorIdTeacher;
	declare @StaffGradeLevelDescriptorId2 int = @GradeLevelDescriptorIdTenthGradeId;
	declare @StaffSchoolId2 int = @HighSchoolId;
	
	declare @StaffPersonalPrefix5 nvarchar(30) = 'Ms';
	declare @StaffFirstName5 nvarchar(75) = 'Vania';
	declare @StaffMiddleName5 nvarchar(75) = 'T';
	declare @StaffLastName5 nvarchar(75) = 'Flores';
	declare @StaffBirthDate5 date = GETDATE();
	declare @LoginId5 nvarchar(60) = 'vflores';
	declare @HighlyQualifiedTeacher5 bit = 0;
	declare @StaffUniqueId5 nvarchar(32) = '000024';
	declare @StaffEmail5 nvarchar(128) = 'vflores_t_test@student1.org';
	declare @StaffSexDescriptor5 int = @BirthFemaleSexDescriptorId;
	declare @StaffClassificationDescriptorId5 int = @StaffClassificationDescriptorIdTeacher;
	declare @StaffGradeLevelDescriptorId5 int = @GradeLevelDescriptorIdTenthGradeId;
	declare @StaffSchoolId5 int = @MiddleSchoolId;

	-- Campus Leaders --
	declare @StaffPersonalPrefix3 nvarchar(30) = 'Mr';
	declare @StaffFirstName3 nvarchar(75) = 'Andy';
	declare @StaffMiddleName3 nvarchar(75) = 'CL';
	declare @StaffLastName3 nvarchar(75) = 'Wright';
	declare @StaffBirthDate3 date = GETDATE();
	declare @LoginId3 nvarchar(60) = 'awright';
	declare @HighlyQualifiedTeacher3 bit = 0;
	declare @StaffUniqueId3 nvarchar(32) = '000005';
	declare @StaffEmail3 nvarchar(128) = 'awright_cl_test@student1.org';
	declare @StaffSexDescriptor3 int = @BirthMaleSexDescriptorId;
	declare @StaffClassificationDescriptorId3 int = @StaffClassificationDescriptorIdLeaAdmin;
	declare @StaffSchoolId3MiddleSchoolId int = @MiddleSchoolId; 
	declare @StaffSchoolId3HighSchoolId int = @HighSchoolId; 

	declare @StaffPersonalPrefix4 nvarchar(30) = 'Mr';
	declare @StaffFirstName4 nvarchar(75) = 'Manuel';
	declare @StaffMiddleName4 nvarchar(75) = 'CL';
	declare @StaffLastName4 nvarchar(75) = 'Bartlet';
	declare @StaffBirthDate4 date = GETDATE();
	declare @LoginId4 nvarchar(60) = 'mbartlet';
	declare @HighlyQualifiedTeacher4 bit = 0;
	declare @StaffUniqueId4 nvarchar(32) = '000006';
	declare @StaffEmail4 nvarchar(128) = 'mbartlet_cl_test@student1.org';
	declare @StaffSexDescriptor4 int = @BirthMaleSexDescriptorId;
	declare @StaffClassificationDescriptorId4 int = @StaffClassificationDescriptorIdPrincipal;
	declare @StaffSchoolId4 int = @HighSchoolId;

	--
	--select top 5 StaffUSI,FirstName,LastSurname from [edfi].[Staff] order by CreateDate DESC
	declare @StaffUSI1 int;
	declare @StaffUSI2 int;
	declare @StaffUSI3 int;
	declare @StaffUSI4 int;
	declare @StaffUSI5 int;

	declare @HighestCompletedLevelOfEducationDescriptorId int; 
	Select top 1 @HighestCompletedLevelOfEducationDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Doctorate' AND [Namespace] = 'uri://ed-fi.org/LevelOfEducationDescriptor';

	declare @EmailTypeWork int;
	Select top 1 @EmailTypeWork  = DescriptorId FROM edfi.Descriptor where [Description] = 'Work' AND [Namespace] = 'uri://ed-fi.org/ElectronicMailTypeDescriptor';
	
	declare @ProgramAssignmentDescriptorId int = 1709;
	Select top 1 @ProgramAssignmentDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Regular Education' AND [Namespace] = 'uri://ed-fi.org/ProgramAssignmentDescriptor';
	
	declare @RegisterStaffInSchool nvarchar(10) = format(getDate(), 'yyyy-MM-dd') ;

	--Staff 1
	INSERT INTO [edfi].[Staff] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[SexDescriptorId],[BirthDate],[HighestCompletedLevelOfEducationDescriptorId],[LoginId],[HighlyQualifiedTeacher],[StaffUniqueId])
						 VALUES(@StaffPersonalPrefix1,@StaffFirstName1,@StaffMiddleName1,@StaffLastName1,@StaffSexDescriptor1,@StaffBirthDate1,@HighestCompletedLevelOfEducationDescriptorId,@LoginId1,@HighlyQualifiedTeacher1,@StaffUniqueId1)
	set @StaffUSI1 = SCOPE_IDENTITY();
	--Staff 1 Electronic Mail

	INSERT INTO [edfi].[StaffElectronicMail]([ElectronicMailTypeDescriptorId],[StaffUSI],[ElectronicMailAddress])
									  VALUES(@EmailTypeWork,@StaffUSI1,@StaffEmail1)

	-- Staff 1 School Association
	INSERT INTO [edfi].[StaffSchoolAssociation]([ProgramAssignmentDescriptorId],[SchoolId],[StaffUSI])
										VALUES (@ProgramAssignmentDescriptorId,@StaffSchoolId1,@StaffUSI1);

	-- Staff 1 Education Organization Assigment Association
	INSERT INTO [edfi].[StaffEducationOrganizationAssignmentAssociation] ([BeginDate],[EducationOrganizationId],[StaffClassificationDescriptorId],[StaffUSI])
																  VALUES (@RegisterStaffInSchool,@StaffSchoolId1,@StaffClassificationDescriptorId1,@StaffUSI1)

	--Staff 2
	INSERT INTO [edfi].[Staff] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[SexDescriptorId],[BirthDate],[HighestCompletedLevelOfEducationDescriptorId],[LoginId],[HighlyQualifiedTeacher],[StaffUniqueId])
						 VALUES(@StaffPersonalPrefix2,@StaffFirstName2,@StaffMiddleName2,@StaffLastName2,@StaffSexDescriptor2,@StaffBirthDate2,@HighestCompletedLevelOfEducationDescriptorId,@LoginId2,@HighlyQualifiedTeacher2,@StaffUniqueId2)
	set @StaffUSI2 = SCOPE_IDENTITY();

	--Staff 2 Electronic Mail
	INSERT INTO [edfi].[StaffElectronicMail]([ElectronicMailTypeDescriptorId],[StaffUSI],[ElectronicMailAddress])
									  VALUES(@EmailTypeWork,@StaffUSI2,@StaffEmail2)

	-- Staff 2 School Association
	INSERT INTO [edfi].[StaffSchoolAssociation]([ProgramAssignmentDescriptorId],[SchoolId],[StaffUSI])
										VALUES (@ProgramAssignmentDescriptorId,@StaffSchoolId2,@StaffUSI2);

	-- Staff 2 Education Organization Assigment Association
	INSERT INTO [edfi].[StaffEducationOrganizationAssignmentAssociation] ([BeginDate],[EducationOrganizationId],[StaffClassificationDescriptorId],[StaffUSI])
																  VALUES (@RegisterStaffInSchool,@StaffSchoolId2,@StaffClassificationDescriptorId2,@StaffUSI2)

	--Staff 3 
	--***** NOTE: review the code for campus leader when is associated with more than 1 school
	INSERT INTO [edfi].[Staff] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[SexDescriptorId],[BirthDate],[HighestCompletedLevelOfEducationDescriptorId],[LoginId],[HighlyQualifiedTeacher],[StaffUniqueId])
						 VALUES(@StaffPersonalPrefix3,@StaffFirstName3,@StaffMiddleName3,@StaffLastName3,@StaffSexDescriptor3,@StaffBirthDate3,@HighestCompletedLevelOfEducationDescriptorId,@LoginId3,@HighlyQualifiedTeacher3,@StaffUniqueId3)
	set @StaffUSI3 = SCOPE_IDENTITY();

	--Staff 3 Electronic Mail
	INSERT INTO [edfi].[StaffElectronicMail]([ElectronicMailTypeDescriptorId],[StaffUSI],[ElectronicMailAddress])
									  VALUES(@EmailTypeWork,@StaffUSI3,@StaffEmail3)

	-- Staff 3 School Association 
	INSERT INTO [edfi].[StaffSchoolAssociation]([ProgramAssignmentDescriptorId],[SchoolId],[StaffUSI])
										VALUES (@ProgramAssignmentDescriptorId,@StaffSchoolId3MiddleSchoolId,@StaffUSI3);

	-- Staff 3 Education Organization Assigment Association
	INSERT INTO [edfi].[StaffEducationOrganizationAssignmentAssociation] ([BeginDate],[EducationOrganizationId],[StaffClassificationDescriptorId],[StaffUSI])
																  VALUES (@RegisterStaffInSchool,@StaffSchoolId3MiddleSchoolId,@StaffClassificationDescriptorId3,@StaffUSI3)
																  
	INSERT INTO [edfi].[StaffEducationOrganizationAssignmentAssociation] ([BeginDate],[EducationOrganizationId],[StaffClassificationDescriptorId],[StaffUSI])
																  VALUES (@RegisterStaffInSchool,@StaffSchoolId3HighSchoolId,@StaffClassificationDescriptorId3,@StaffUSI3)

	--Staff 4
	INSERT INTO [edfi].[Staff] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[SexDescriptorId],[BirthDate],[HighestCompletedLevelOfEducationDescriptorId],[LoginId],[HighlyQualifiedTeacher],[StaffUniqueId])
						 VALUES(@StaffPersonalPrefix4,@StaffFirstName4,@StaffMiddleName4,@StaffLastName4,@StaffSexDescriptor4,@StaffBirthDate4,@HighestCompletedLevelOfEducationDescriptorId,@LoginId4,@HighlyQualifiedTeacher4,@StaffUniqueId4)
	set @StaffUSI4 = SCOPE_IDENTITY();

	--Staff 4 Electronic Mail
	INSERT INTO [edfi].[StaffElectronicMail]([ElectronicMailTypeDescriptorId],[StaffUSI],[ElectronicMailAddress])
									  VALUES(@EmailTypeWork,@StaffUSI4,@StaffEmail4)

	-- Staff 4 School Association
	INSERT INTO [edfi].[StaffSchoolAssociation]([ProgramAssignmentDescriptorId],[SchoolId],[StaffUSI])
										VALUES (@ProgramAssignmentDescriptorId,@StaffSchoolId4,@StaffUSI4);

	-- Staff 4 Education Organization Assigment Association
	INSERT INTO [edfi].[StaffEducationOrganizationAssignmentAssociation] ([BeginDate],[EducationOrganizationId],[StaffClassificationDescriptorId],[StaffUSI])
																  VALUES (@RegisterStaffInSchool,@StaffSchoolId4,@StaffClassificationDescriptorId4,@StaffUSI4)

	--Staff 5
	INSERT INTO [edfi].[Staff] ([PersonalTitlePrefix],[FirstName],[MiddleName],[LastSurname],[SexDescriptorId],[BirthDate],[HighestCompletedLevelOfEducationDescriptorId],[LoginId],[HighlyQualifiedTeacher],[StaffUniqueId])
						 VALUES(@StaffPersonalPrefix5,@StaffFirstName5,@StaffMiddleName5,@StaffLastName5,@StaffSexDescriptor5,@StaffBirthDate5,@HighestCompletedLevelOfEducationDescriptorId,@LoginId5,@HighlyQualifiedTeacher5,@StaffUniqueId5)
	set @StaffUSI5 = SCOPE_IDENTITY();

	--Staff 5 Electronic Mail
	INSERT INTO [edfi].[StaffElectronicMail]([ElectronicMailTypeDescriptorId],[StaffUSI],[ElectronicMailAddress])
									  VALUES(@EmailTypeWork,@StaffUSI5,@StaffEmail5)

	-- Staff 5 School Association
	INSERT INTO [edfi].[StaffSchoolAssociation]([ProgramAssignmentDescriptorId],[SchoolId],[StaffUSI])
										VALUES (@ProgramAssignmentDescriptorId,@StaffSchoolId5,@StaffUSI5);

	-- Staff 5 Education Organization Assigment Association
	INSERT INTO [edfi].[StaffEducationOrganizationAssignmentAssociation] ([BeginDate],[EducationOrganizationId],[StaffClassificationDescriptorId],[StaffUSI])
																  VALUES (@RegisterStaffInSchool,@StaffSchoolId5,@StaffClassificationDescriptorId5,@StaffUSI5)


	-- ********************************** --
	-- *** Adding Session & Locations *** --
	-- ********************************** --@@CurrentSchoolYear) + '-01-01'
	-- Current Year
	declare @@CurrentSchoolYear int;
	select @@CurrentSchoolYear = CAST(SchoolYear AS int) from edfi.SchoolYearType where CurrentSchoolYear = 1 -- We're selecting the current school year from the production DB
	declare @@PreviousSchoolYear int = @@CurrentSchoolYear -1;
	declare @@SchoolYearRange nvarchar(10) = CAST(@@PreviousSchoolYear AS varchar(4))+'-'+CAST(@@CurrentSchoolYear AS varchar(4));

	declare @SessionNameSpring nvarchar(50) = @@SchoolYearRange + ' Spring Semester';
	declare @SessionNameFall nvarchar(50) = @@SchoolYearRange + ' Fall Semester';
	declare @SessionNameYear nvarchar(50) = @@SchoolYearRange + ' Year Round';
	declare @SpringBeginDate nvarchar(10) = CONVERT(varchar(10), @@CurrentSchoolYear) + '-01-05';
	declare @SpringEndDate nvarchar(10) = CONVERT(varchar(10), @@CurrentSchoolYear) + '-06-10';
	declare @FallBeginDate nvarchar(10) = CONVERT(varchar(10), @@PreviousSchoolYear) + '-08-16';
	declare @FallEndDate nvarchar(10) = CONVERT(varchar(10), @@PreviousSchoolYear) + '-12-17';
	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%TermDescriptor%';

	--Sessions
	--Highschool
	declare @TermDescriptorIdSpring int; 
	Select top 1 @TermDescriptorIdSpring  = DescriptorId FROM edfi.Descriptor where [Description] = 'Spring Semester' AND [Namespace] = 'uri://ed-fi.org/TermDescriptor';
	declare @TermDescriptorIdFall int;
	Select top 1 @TermDescriptorIdFall  = DescriptorId FROM edfi.Descriptor where [Description] = 'Fall Semester' AND [Namespace] = 'uri://ed-fi.org/TermDescriptor';
	
	print('Inserting sessions')
	--Sessions
	--Highschool
	INSERT INTO [edfi].[Session]([SchoolId],[SchoolYear],[SessionName],[BeginDate],[EndDate],[TermDescriptorId],[TotalInstructionalDays])
						 VALUES (@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameSpring,@SpringBeginDate,@SpringEndDate,@TermDescriptorIdSpring,88)

	INSERT INTO [edfi].[Session] ([SchoolId],[SchoolYear],[SessionName],[BeginDate],[EndDate],[TermDescriptorId],[TotalInstructionalDays])
						  VALUES (@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@FallBeginDate,@FallEndDate,@TermDescriptorIdFall,82)
	--Middlechool
	INSERT INTO [edfi].[Session]([SchoolId],[SchoolYear],[SessionName],[BeginDate],[EndDate],[TermDescriptorId],[TotalInstructionalDays])
						 VALUES (@MiddleSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameSpring,@SpringBeginDate,@SpringEndDate,@TermDescriptorIdSpring,88)

	INSERT INTO [edfi].[Session] ([SchoolId],[SchoolYear],[SessionName],[BeginDate],[EndDate],[TermDescriptorId],[TotalInstructionalDays])
						  VALUES (@MiddleSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@FallBeginDate,@FallEndDate,@TermDescriptorIdFall,82)

	print('Inserting locations')
	--Location Section
	declare @LocalClassroomIdentificationCode1 nvarchar(10) = 'Classroom1';
	declare @LocalClassroomIdentificationCode2 nvarchar(10) = 'Classroom2';
	declare @LocalClassroomIdentificationCode3 nvarchar(10) = 'Classroom3';
	declare @LocalClassroomIdentificationCode4 nvarchar(10) = 'Classroom4';
	declare @LocalClassroomIdentificationCode5 nvarchar(10) = 'Classroom5';
	declare @LocalClassroomIdentificationCode6 nvarchar(10) = 'Classroom6';
	declare @LocalClassroomIdentificationCode7 nvarchar(10) = 'Classroom7';

	--Highschool
	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode1,@HighSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode2,@HighSchoolId,20,18)

	INSERT INTO [edfi].[Location]([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						  VALUES (@LocalClassroomIdentificationCode3,@HighSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode4,@HighSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode5,@HighSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode6,@HighSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode7,@HighSchoolId,20,18)
						   
	--Middlechool
	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode1,@MiddleSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode2,@MiddleSchoolId,20,18)

	INSERT INTO [edfi].[Location]([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						  VALUES (@LocalClassroomIdentificationCode3,@MiddleSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode4,@MiddleSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode5,@MiddleSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode6,@MiddleSchoolId,20,18)

	INSERT INTO [edfi].[Location] ([ClassroomIdentificationCode],[SchoolId],[MaximumNumberOfSeats],[OptimalNumberOfSeats])
						   VALUES (@LocalClassroomIdentificationCode7,@MiddleSchoolId,20,18)
	
	-- ***************************************** --
	-- *** Adding Courses & Courses Offering *** --
	-- ***************************************** --
	--Highschool
	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%academicsubject%';
	
	declare @LocalCourseCode1 nvarchar(10) = 'ART-01-NSD';
	declare @LocalCourseTitle1 nvarchar(50) = 'Arts 1';
	declare @AcademicSubjectDescriptor1 int;
	Select top 1 @AcademicSubjectDescriptor1  = DescriptorId FROM edfi.Descriptor where [Description] = 'English Language Arts' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier1 nvarchar(50) = Concat(@HighSchoolId, 'Trad', @LocalClassroomIdentificationCode1 , 'ART01-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode2 nvarchar(10) = 'SPA-01-NSD';
	declare @LocalCourseTitle2 nvarchar(50) = 'Spanish 1';
	declare @AcademicSubjectDescriptor2 int;
	Select top 1 @AcademicSubjectDescriptor2  = DescriptorId FROM edfi.Descriptor where [Description] = 'Foreign Language and Literature' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier2 nvarchar(50) = Concat(@HighSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'SPA01-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode3 nvarchar(10) = 'GEO-02-NSD';
	declare @LocalCourseTitle3 nvarchar(50) = 'Geography 2';
	declare @AcademicSubjectDescriptor3 int;
	Select top 1 @AcademicSubjectDescriptor3  = DescriptorId FROM edfi.Descriptor where [Description] = 'Social Studies' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier3 nvarchar(50) = Concat(@HighSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'GEO02-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode4 nvarchar(10) = 'ENV-02-NSD';
	declare @LocalCourseTitle4 nvarchar(50) = 'Enviroment 2';
	declare @AcademicSubjectDescriptor4 int;
	Select top 1 @AcademicSubjectDescriptor4  = DescriptorId FROM edfi.Descriptor where [Description] = 'Career and Technical Education' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier4 nvarchar(50) = Concat(@HighSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'ENV02-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode5 nvarchar(10) = 'MAT-03-NSD';
	declare @LocalCourseTitle5 nvarchar(50) = 'Math 3';
	declare @AcademicSubjectDescriptor5 int;
	Select top 1 @AcademicSubjectDescriptor5  = DescriptorId FROM edfi.Descriptor where [Description] = 'Mathematics' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier5 nvarchar(50) = Concat(@HighSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'MAT03-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode6 nvarchar(10) = 'ALG-03-NSD';
	declare @LocalCourseTitle6 nvarchar(50) = 'Algebra 3';
	declare @AcademicSubjectDescriptor6 int;
	Select top 1 @AcademicSubjectDescriptor6  = DescriptorId FROM edfi.Descriptor where [Description] = 'Mathematics' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier6 nvarchar(50) = Concat(@HighSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'ALG03-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode7 nvarchar(10) = 'ENG-04-NSD';
	declare @LocalCourseTitle7 nvarchar(50) = 'English 4';
	declare @AcademicSubjectDescriptor7 int;
	Select top 1 @AcademicSubjectDescriptor7  = DescriptorId FROM edfi.Descriptor where [Description] = 'English' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier7 nvarchar(50) = Concat(@HighSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'ENG04-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));
	
	declare @LocalCourseCode8 nvarchar(10) = 'ALG-04-NSD';
	declare @LocalCourseTitle8 nvarchar(50) = 'Algebra 4';
	declare @AcademicSubjectDescriptor8 int;
	Select top 1 @AcademicSubjectDescriptor8  = DescriptorId FROM edfi.Descriptor where [Description] = 'Mathematics' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier8 nvarchar(50) = Concat(@MiddleSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'ALG04-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @LocalCourseCode9 nvarchar(10) = 'ENG-MS-NSD';
	declare @LocalCourseTitle9 nvarchar(50) = 'English MS';
	declare @AcademicSubjectDescriptor9 int;
	Select top 1 @AcademicSubjectDescriptor9  = DescriptorId FROM edfi.Descriptor where [Description] = 'English' AND [Namespace] = 'uri://ed-fi.org/AcademicSubjectDescriptor';
	declare @SectionIdentifier9 nvarchar(50) = Concat(@MiddleSchoolId,'Trad', @LocalClassroomIdentificationCode1 , 'ENG-MS-NSD' , CONVERT(varchar(10), @@CurrentSchoolYear));

	declare @HighSchoolCourseRequirement int = 1;
	declare @CourseGPAApplicabilityDescriptorId int = 603;
	Select top 1 @CourseGPAApplicabilityDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Applicable' AND [Namespace] = 'uri://ed-fi.org/CourseGPAApplicabilityDescriptor';

	declare @CourseDefinedByDescriptorId int;
	Select top 1 @CourseDefinedByDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'SEA' AND [Namespace] = 'uri://ed-fi.org/CourseDefinedByDescriptor';

	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%DefinedBy%';

	print('Inserting courses')
	--courses
	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode1,@HighSchoolId,@LocalCourseTitle1,1,@AcademicSubjectDescriptor1,@LocalCourseTitle1,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode2,@HighSchoolId,@LocalCourseTitle2,1,@AcademicSubjectDescriptor2,@LocalCourseTitle2,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode3,@HighSchoolId,@LocalCourseTitle3,1,@AcademicSubjectDescriptor3,@LocalCourseTitle3,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode4,@HighSchoolId,@LocalCourseTitle4,1,@AcademicSubjectDescriptor4,@LocalCourseTitle4,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						VALUES (@LocalCourseCode5,@HighSchoolId,@LocalCourseTitle5,1,@AcademicSubjectDescriptor5,@LocalCourseTitle5,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
					     VALUES (@LocalCourseCode6,@HighSchoolId,@LocalCourseTitle6,1,@AcademicSubjectDescriptor6,@LocalCourseTitle6,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode7,@HighSchoolId,@LocalCourseTitle7,1,@AcademicSubjectDescriptor7,@LocalCourseTitle7,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode8,@HighSchoolId,@LocalCourseTitle8,1,@AcademicSubjectDescriptor8,@LocalCourseTitle8,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	INSERT INTO [edfi].[Course] ([CourseCode],[EducationOrganizationId],[CourseTitle],[NumberOfParts],[AcademicSubjectDescriptorId],[CourseDescription],[HighSchoolCourseRequirement],[CourseGPAApplicabilityDescriptorId],[CourseDefinedByDescriptorId])
						 VALUES (@LocalCourseCode9,@MiddleSchoolId,@LocalCourseTitle9,1,@AcademicSubjectDescriptor9,@LocalCourseTitle8,@HighSchoolCourseRequirement,@CourseGPAApplicabilityDescriptorId,@CourseDefinedByDescriptorId)

	--course offering

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode1 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle1,@LocalCourseCode1,@HighSchoolId)

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode2 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle2,@LocalCourseCode2,@HighSchoolId)

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode3 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle3,@LocalCourseCode3,@HighSchoolId)

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode4 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle4,@LocalCourseCode4,@HighSchoolId)

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode5 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle5,@LocalCourseCode5,@HighSchoolId)

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode6 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle6,@LocalCourseCode6,@HighSchoolId)

	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode7 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle7,@LocalCourseCode7,@HighSchoolId)
								 
	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode8 ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle8,@LocalCourseCode8,@HighSchoolId)
		 
	INSERT INTO [edfi].[CourseOffering] ([LocalCourseCode],[SchoolId],[SchoolYear],[SessionName],[LocalCourseTitle],[CourseCode],[EducationOrganizationId])
								 VALUES (@LocalCourseCode9 ,@MiddleSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@LocalCourseTitle9,@LocalCourseCode9,@MiddleSchoolId)

	declare @EducationalEnvironmentDescriptorId int;		
	Select top 1 @EducationalEnvironmentDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Classroom' AND [Namespace] = 'uri://ed-fi.org/EducationalEnvironmentDescriptor';

	print('Inserting sections')
	--Sections
	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode1,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier1,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode1)

	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode2,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier2,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode2)

	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode3,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier3,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode3)

	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode4,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier4,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode4)

	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
					      VALUES (@LocalCourseCode5,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier5,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode5)

	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode6,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier6,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode6)

	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode7,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier7,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode7)
	
	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode8,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier8,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@HighSchoolId,@LocalClassroomIdentificationCode7)
	
	INSERT INTO [edfi].[Section] ([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[SequenceOfCourse],[EducationalEnvironmentDescriptorId],[AvailableCredits],[LocationSchoolId],[LocationClassroomIdentificationCode])
						  VALUES (@LocalCourseCode9,@MiddleSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier9,@SessionNameFall,1,@EducationalEnvironmentDescriptorId,1.000,@MiddleSchoolId,@LocalClassroomIdentificationCode7)
	
	-- ** Student Enrollment Courses **
	declare @@GraduationPlanTypeDescriptorId int = null;      
	declare @@StudentParticipationCodeDescriptorId int = null;     
	declare @IncidentIdentifier1 nvarchar(20) = 'Incident 01';
	declare @IncidentIdentifier2 nvarchar(20) = 'Incident 02';

	declare @@EntryTypeDescriptorId int;
    Select top 1 @@EntryTypeDescriptorId = DescriptorId FROM edfi.Descriptor where [Description] = 'New to education system' AND [Namespace] = 'uri://ed-fi.org/EntryTypeDescriptor';      -- New to education system
    Select top 1 @@GraduationPlanTypeDescriptorId  =        DescriptorId FROM edfi.Descriptor where [Description] = 'Standard' AND [Namespace] = 'uri://ed-fi.org/GraduationPlanTypeDescriptor';            -- Standard
    Select top 1 @@StudentParticipationCodeDescriptorId  =  DescriptorId FROM edfi.Descriptor where [Description] = 'Perpetrator' AND [Namespace] = 'uri://ed-fi.org/StudentParticipationCodeDescriptor';   -- Perpetrator
	
	declare @CalendarCode nvarchar(10) = '11111'; -- Calendar Code
	declare @CalendarTypeDescriptorId int;
    Select top 1 @CalendarTypeDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Student Specific' AND [Namespace] = 'uri://ed-fi.org/CalendarTypeDescriptor';

	declare @ClassroomPositionDescriptorId int;
    Select top 1 @ClassroomPositionDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Teacher of Record' AND [Namespace] = 'uri://ed-fi.org/ClassroomPositionDescriptor';

	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%grading%';

	declare @RegisterStudentInSchool nvarchar(10);
	select @RegisterStudentInSchool = @FallBeginDate;--format(getDate(), 'yyyy-MM-dd');
	
	print('Inserting calendar')
	INSERT INTO [edfi].[Calendar] ([CalendarCode],[SchoolId],[SchoolYear],[CalendarTypeDescriptorId])
						   VALUES (@CalendarCode,@HighSchoolId,@@CurrentSchoolYear,@CalendarTypeDescriptorId)

	INSERT INTO [edfi].[Calendar] ([CalendarCode],[SchoolId],[SchoolYear],[CalendarTypeDescriptorId])
						   VALUES (@CalendarCode,@MiddleSchoolId,@@CurrentSchoolYear,@CalendarTypeDescriptorId)
	declare @AttendanceEventCategoryDescriptorId int = null   
    Select top 1 @AttendanceEventCategoryDescriptorId  =    DescriptorId FROM edfi.Descriptor where [Description] = 'Excused Absence' AND [Namespace] = 'uri://ed-fi.org/AttendanceEventCategoryDescriptor';-- Absent
	declare @TardyEventCategoryDescriptorId int = null        
    Select top 1 @TardyEventCategoryDescriptorId  =    DescriptorId FROM edfi.Descriptor where [Description] = 'Tardy' AND [Namespace] = 'uri://ed-fi.org/AttendanceEventCategoryDescriptor';-- Absent
	declare @UnexcusedAbscenceCategoryDescriptorId int = null        
    Select top 1 @UnexcusedAbscenceCategoryDescriptorId  =    DescriptorId FROM edfi.Descriptor where [Description] = 'Unexcused Absence' AND [Namespace] = 'uri://ed-fi.org/AttendanceEventCategoryDescriptor';-- Absent

	-- Attendance student
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
		    VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-04' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI1,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
				VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-05' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI1,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
				VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-04' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI2,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
				VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-01' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI3,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-01' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI4,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-02' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI4,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-04-01' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI5,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-03-02' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI5,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-02-03' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI5,'The student left the school.')

					INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
		    VALUES (@AttendanceEventCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-04' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI6,'COVID')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
				VALUES (@AttendanceEventCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-05' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI6,'COVID')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
				VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-04' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI7,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
				VALUES (@AttendanceEventCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-01' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI6,'COVID')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-01' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI8,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-02' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI8,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-01' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI7,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-07' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI1,'The student left the school.')
	INSERT INTO [edfi].[StudentSchoolAttendanceEvent] ([AttendanceEventCategoryDescriptorId],[EventDate],[SchoolId],[SchoolYear],[SessionName],[StudentUSI],[AttendanceEventReason])
					VALUES (@UnexcusedAbscenceCategoryDescriptorId,CONVERT(varchar(10), @@CurrentSchoolYear) + '-09-08' ,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SessionNameFall,@StudentUSI1,'The student left the school.')

	--staff
	
	print('Inserting staff section association')
	-- Staff section association 	
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode1,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier1,@SessionNameFall,@StaffUSI1,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)

	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode2,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier2,@SessionNameFall,@StaffUSI1,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	 
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode3,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier3,@SessionNameFall,@StaffUSI1,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode4,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier4,@SessionNameFall,@StaffUSI1,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode5,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier5,@SessionNameFall,@StaffUSI2,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode6,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier6,@SessionNameFall,@StaffUSI2,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode7,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier7,@SessionNameFall,@StaffUSI2,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)

	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode8,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier8,@SessionNameFall,@StaffUSI2,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	
	INSERT INTO [edfi].[StaffSectionAssociation]([LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StaffUSI],[ClassroomPositionDescriptorId],[BeginDate],[EndDate])
     VALUES (@LocalCourseCode9,@MiddleSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier9,@SessionNameFall,@StaffUSI5,@ClassroomPositionDescriptorId,@FallBeginDate,@FallEndDate)
	

	declare @EighthGradeLevelDescriptorId int;
	Select top 1 @EighthGradeLevelDescriptorId = DescriptorId FROM edfi.Descriptor where [Description] = 'Eighth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';
	declare @NinthGradeLevelDescriptorId int;
	Select top 1 @NinthGradeLevelDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Ninth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';
	declare @TenthGradeLevelDescriptorId int;
	Select top 1 @TenthGradeLevelDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Tenth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';
	declare @EleventhGradeLevelDescriptorId int;
	Select top 1 @EleventhGradeLevelDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Eleventh grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';
	declare @TwelfthGradeLevelDescriptorId int;
	Select top 1 @TwelfthGradeLevelDescriptorId  = DescriptorId FROM edfi.Descriptor where [Description] = 'Twelfth grade' AND [Namespace] = 'uri://ed-fi.org/GradeLevelDescriptor';
	
	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%gradeleve%';
	
	print('Inserting student school association')
	--Student School Association
	-- students 1,2,3 high school 9th
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI1,@NinthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI2,@NinthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI3,@NinthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	-- students 4,5 high school 10th
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI4,@TenthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI5,@TenthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	
	-- students 6,7 high school 11th
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI6,@EleventhGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI7,@EleventhGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	
	-- students 8,9 high school 12th
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI8,@TwelfthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@HighSchoolId,@StudentUSI9,@TwelfthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@HighSchoolId,@CalendarCode ,@@CurrentSchoolYear)
	
	-- student 10 middle school 8th
	INSERT INTO [edfi].[StudentSchoolAssociation] ([EntryDate],[SchoolId],[StudentUSI],[EntryGradeLevelDescriptorId],[EntryTypeDescriptorId],[GraduationPlanTypeDescriptorId],[EducationOrganizationId],[CalendarCode],[SchoolYear])
		     VALUES (@RegisterStudentInSchool,@MiddleSchoolId,@StudentUSI10,@EighthGradeLevelDescriptorId ,@@EntryTypeDescriptorId ,@@GraduationPlanTypeDescriptorId ,@MiddleSchoolId,@CalendarCode ,@@CurrentSchoolYear)

	print('Inserting student section association')
	-- Student section association
	/******* Students 9th grade ********/
	-- Student 1
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode1,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier1,@SessionNameFall,@StudentUSI1,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode2,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier2,@SessionNameFall,@StudentUSI1,@FallEndDate,0);

	-- Student 2
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode1,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier1,@SessionNameFall,@StudentUSI2,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode2,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier2,@SessionNameFall,@StudentUSI2,@FallEndDate,0);

	-- Student 3
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode1,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier1,@SessionNameFall,@StudentUSI3,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode2,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier2,@SessionNameFall,@StudentUSI3,@FallEndDate,0);
	
	/******* Students 10th grade ********/
	-- Student 4
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode3,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier3,@SessionNameFall,@StudentUSI4,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode4,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier4,@SessionNameFall,@StudentUSI4,@FallEndDate,0);
	-- Student 5
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode3,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier3,@SessionNameFall,@StudentUSI5,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode4,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier4,@SessionNameFall,@StudentUSI5,@FallEndDate,0);
	
	/******* Students 11th grade ********/
	-- Student 6
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode5,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier5,@SessionNameFall,@StudentUSI6,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode6,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier6,@SessionNameFall,@StudentUSI6,@FallEndDate,0);
	-- Student 7
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode5,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier5,@SessionNameFall,@StudentUSI7,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode6,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier6,@SessionNameFall,@StudentUSI7,@FallEndDate,0);
	
	/******* Students 12th grade ********/
	-- Student 8
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode7,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier7,@SessionNameFall,@StudentUSI8,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode8,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier8,@SessionNameFall,@StudentUSI8,@FallEndDate,0);
	-- Student 9
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode7,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier7,@SessionNameFall,@StudentUSI9,@FallEndDate,0);

	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode8,@HighSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier8,@SessionNameFall,@StudentUSI9,@FallEndDate,0);
	
	/******* Student 8th grade ********/
	-- Student 10
	INSERT INTO [edfi].[StudentSectionAssociation] ([BeginDate],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[EndDate],[HomeroomIndicator])
		     VALUES (@FallBeginDate,@LocalCourseCode9,@MiddleSchoolId,CONVERT(varchar(10), @@CurrentSchoolYear),@SectionIdentifier9,@SessionNameFall,@StudentUSI10,@FallEndDate,0);

	print('Inserting student education org association')
	-- student education organization associations
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI1,@@StudentSexDescriptor1,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI2,@@StudentSexDescriptor2,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI3,@@StudentSexDescriptor3,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI4,@@StudentSexDescriptor4,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI5,@@StudentSexDescriptor5,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI6,@@StudentSexDescriptor6,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI7,@@StudentSexDescriptor7,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI8,@@StudentSexDescriptor8,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@HighSchoolId,@StudentUSI9,@@StudentSexDescriptor9,1);
	INSERT INTO [edfi].[StudentEducationOrganizationAssociation] ([EducationOrganizationId],[StudentUSI],[SexDescriptorId],[HispanicLatinoEthnicity])
		     VALUES (@MiddleSchoolId,@StudentUSI10,@@StudentSexDescriptor10,1);
		
	print('Inserting student electronic mail')
	-- Student education organization electronic mail
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI1,@@StudentFirstName1 + @@StudentLastSurname1+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI2,@@StudentFirstName2 + @@StudentLastSurname2+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI3,@@StudentFirstName3 + @@StudentLastSurname3+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI4,@@StudentFirstName4 + @@StudentLastSurname4+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI5,@@StudentFirstName5 + @@StudentLastSurname5+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI6,@@StudentFirstName6 + @@StudentLastSurname6+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI7,@@StudentFirstName7 + @@StudentLastSurname7+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI8,@@StudentFirstName8 + @@StudentLastSurname8+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@HighSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI9,@@StudentFirstName9 + @@StudentLastSurname9+ '@yesprep.com')
	INSERT INTO [edfi].[StudentEducationOrganizationAssociationElectronicMail] ([EducationOrganizationId],[ElectronicMailTypeDescriptorId],[StudentUSI],[ElectronicMailAddress])
		     VALUES (@MiddleSchoolId,@ElectronicMailTypeDescriptorId,@StudentUSI10,@@StudentFirstName10 + @@StudentLastSurname10+ '@yesprep.com')
	
	-- *********************** --
	-- *** Grading periods *** --
	-- *********************** --

	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%GradingPeriodDesc%';
	--select * from [edfi].[GradingPeriod] where schoolid = 56
	
	declare @GradingPeriodDescriptorIdFirstNineWeeks int;-- = 933;
	Select top 1 @GradingPeriodDescriptorIdFirstNineWeeks  = DescriptorId FROM edfi.Descriptor where [Description] = 'First Nine Weeks' AND [Namespace] = 'uri://ed-fi.org/GradingPeriodDescriptor';
	declare @TotalInstructionalDays1 int = 50;
	declare @PeriodSequence1 int = 1;
	declare @PeriodBeginDate1 date = CONCAT(@@PreviousSchoolYear, '-08-23')
	declare @PeriodEndDate1 date = CONCAT(@@PreviousSchoolYear, '-10-30')
	
	declare @GradingPeriodDescriptorIdSecondNineWeeks int;-- = 940;
	Select top 1 @GradingPeriodDescriptorIdSecondNineWeeks  = DescriptorId FROM edfi.Descriptor where [Description] = 'Second Nine Weeks' AND [Namespace] = 'uri://ed-fi.org/GradingPeriodDescriptor';
	declare @TotalInstructionalDays2 int = 54;
	declare @PeriodSequence2 int = 2;
	declare @PeriodBeginDate2 date = CONCAT(@@PreviousSchoolYear, '-11-02')
	declare @PeriodEndDate2 date = CONCAT(@@PreviousSchoolYear, '-12-17')

	declare @GradingPeriodDescriptorIdThirdNineWeeks int;-- = 947;
	Select top 1 @GradingPeriodDescriptorIdThirdNineWeeks  = DescriptorId FROM edfi.Descriptor where [Description] = 'Third Nine Weeks' AND [Namespace] = 'uri://ed-fi.org/GradingPeriodDescriptor';
	declare @TotalInstructionalDays3 int = 53;
	declare @PeriodSequence3 int = 3;
	declare @PeriodBeginDate3 date = CONCAT(@@CurrentSchoolYear, '-01-19')
	declare @PeriodEndDate3 date = CONCAT(@@CurrentSchoolYear, '-04-01')

	declare @GradingPeriodDescriptorIdFourthNineWeeks int;-- = 938;
	Select top 1 @GradingPeriodDescriptorIdFourthNineWeeks  = DescriptorId FROM edfi.Descriptor where [Description] = 'Fourth Nine Weeks' AND [Namespace] = 'uri://ed-fi.org/GradingPeriodDescriptor';
	declare @TotalInstructionalDays4 int = 38;
	declare @PeriodSequence4 int = 4;
	declare @PeriodBeginDate4 date = CONCAT(@@CurrentSchoolYear, '-04-06')
	declare @PeriodEndDate4 date = CONCAT(@@CurrentSchoolYear, '-05-27')

	declare @GradingPeriodDescriptorIdEndOfYear int;-- = 931;
	Select top 1 @GradingPeriodDescriptorIdEndOfYear  = DescriptorId FROM edfi.Descriptor where [Description] = 'End of Year' AND [Namespace] = 'uri://ed-fi.org/GradingPeriodDescriptor';
	declare @TotalInstructionalDays5 int = 199;
	declare @PeriodSequence5 int = 5;
	declare @PeriodBeginDate5 date = CONCAT(@@PreviousSchoolYear, '-08-24')
	declare @PeriodEndDate5 date = CONCAT(@@CurrentSchoolYear, '-05-27')
	print('Inserting grading periods ')

	declare @SemesterDescriptorId int = null;               
    Select top 1 @SemesterDescriptorId  =                 DescriptorId FROM edfi.Descriptor where [Description] = 'Semester' AND [Namespace] = 'uri://ed-fi.org/GradeTypeDescriptor';   
	
	declare @GradingPeriodDescriptorId int = null;            
    Select top 1 @GradingPeriodDescriptorId  =              DescriptorId FROM edfi.Descriptor where [Description] = 'Grading Period' AND [Namespace] = 'uri://ed-fi.org/GradeTypeDescriptor';      
	
	declare @FirstSemesterDescriptorId int = null;            
    Select top 1 @FirstSemesterDescriptorId  =              DescriptorId FROM edfi.Descriptor where [Description] = 'First Semester' AND [Namespace] = 'uri://ed-fi.org/GradingPeriodDescriptor';               
	declare @TotalInstructionalDays6 int = 104;
	declare @PeriodSequence6 int = 6;
	declare @PeriodBeginDate6 date = CONCAT(@@PreviousSchoolYear, '-08-24')
	declare @PeriodEndDate6 date = CONCAT(@@PreviousSchoolYear, '-12-17')

	INSERT INTO [edfi].[GradingPeriod] ([GradingPeriodDescriptorId],[PeriodSequence],[SchoolId],[SchoolYear],[BeginDate],[EndDate],[TotalInstructionalDays])
								VALUES (@GradingPeriodDescriptorIdFirstNineWeeks,@PeriodSequence1,@HighSchoolId,@@CurrentSchoolYear,@PeriodBeginDate1,@PeriodEndDate1,@TotalInstructionalDays1)

	INSERT INTO [edfi].[GradingPeriod] ([GradingPeriodDescriptorId],[PeriodSequence],[SchoolId],[SchoolYear],[BeginDate],[EndDate],[TotalInstructionalDays])
								VALUES (@GradingPeriodDescriptorIdSecondNineWeeks,@PeriodSequence2,@HighSchoolId,@@CurrentSchoolYear,@PeriodBeginDate2,@PeriodEndDate2,@TotalInstructionalDays2)

	INSERT INTO [edfi].[GradingPeriod] ([GradingPeriodDescriptorId],[PeriodSequence],[SchoolId],[SchoolYear],[BeginDate],[EndDate],[TotalInstructionalDays])
								VALUES (@GradingPeriodDescriptorIdThirdNineWeeks,@PeriodSequence3,@HighSchoolId,@@CurrentSchoolYear,@PeriodBeginDate3,@PeriodEndDate3,@TotalInstructionalDays3)

	INSERT INTO [edfi].[GradingPeriod] ([GradingPeriodDescriptorId],[PeriodSequence],[SchoolId],[SchoolYear],[BeginDate],[EndDate],[TotalInstructionalDays])
								VALUES (@GradingPeriodDescriptorIdFourthNineWeeks,@PeriodSequence4,@HighSchoolId,@@CurrentSchoolYear,@PeriodBeginDate4,@PeriodEndDate4,@TotalInstructionalDays4)

	INSERT INTO [edfi].[GradingPeriod] ([GradingPeriodDescriptorId],[PeriodSequence],[SchoolId],[SchoolYear],[BeginDate],[EndDate],[TotalInstructionalDays])
								VALUES (@GradingPeriodDescriptorIdEndOfYear,@PeriodSequence5,@HighSchoolId,@@CurrentSchoolYear,@PeriodBeginDate5,@PeriodEndDate5,@TotalInstructionalDays5)

	INSERT INTO [edfi].[GradingPeriod] ([GradingPeriodDescriptorId],[PeriodSequence],[SchoolId],[SchoolYear],[BeginDate],[EndDate],[TotalInstructionalDays])
								VALUES (@FirstSemesterDescriptorId,@PeriodSequence6,@HighSchoolId,@@CurrentSchoolYear,@PeriodBeginDate6,@PeriodEndDate6,@TotalInstructionalDays6)

			
	-- *********************** --
	-- ***  Course Grades  *** --
	-- *********************** --
	

		--Student 1
	print('Inserting student grades')
	--SELECT * FROM edfi.Descriptor WHERE NAmespace like '%Gradetype%';
	--select * from [edfi].[GradingPeriod] where schoolid = 56
	
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI1,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI1,90.00)
	
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		     VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI1,98.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI1,89.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		     VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI1,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI1,88.00)
	
	--Student 2 Course 1, 2
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI2,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI2,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI2,98.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI2,89.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI2,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI2,88.00)
	
	--Student 3 Course 3, 4
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI3,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI3,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI3,98.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI3,89.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI3,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI3,88.00)
	
	--Student 4 Course 3, 4
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI4,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI4,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI4,98.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI4,89.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI4,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI4,88.00)
	
	--Student 5 Course 3, 4
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI5,95.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI5,92.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI5,78.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI5,79.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI5,60.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI5,68.00)	
	
	--Student 6 Course 5, 6
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI6,95.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI6,92.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI6,78.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI6,79.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI6,60.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI6,68.00)	
	
	--Student 7 Course 5, 6
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI7,95.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI7,92.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI7,78.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI7,79.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI7,60.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI7,68.00)	
	

	--******first semester grades *********
	
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI1,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI1,90.00)
	

	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI2,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI2,90.00)


	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode1,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier1,@SessionNameFall,@StudentUSI3,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode2,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier2,@SessionNameFall,@StudentUSI3,80.00)


	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
	VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI4,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI4,90.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
	VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode3,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier3,@SessionNameFall,@StudentUSI5,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode4,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier4,@SessionNameFall,@StudentUSI5,90.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
	VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI6,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI6,90.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
	VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode5,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier5,@SessionNameFall,@StudentUSI7,92.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode6,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier6,@SessionNameFall,@StudentUSI7,94.00)



			

 INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI8,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI8,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI8,98.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI8,89.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI8,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI8,88.00)

			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
	VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI8,94.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI8,84.50)


			

 INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI9,90.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdFirstNineWeeks ,@@CurrentSchoolYear,@PeriodSequence1,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI9,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI9,98.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdSecondNineWeeks ,@@CurrentSchoolYear,@PeriodSequence2,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI9,89.00)
			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
				VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI9,80.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
			VALUES (@FallBeginDate,@GradingPeriodDescriptorId ,@GradingPeriodDescriptorIdThirdNineWeeks ,@@CurrentSchoolYear,@PeriodSequence3,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI9,88.00)

			
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
	VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode7,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier7,@SessionNameFall,@StudentUSI9,94.00)
		
	INSERT INTO [edfi].[Grade] ([BeginDate],[GradeTypeDescriptorId],[GradingPeriodDescriptorId],[GradingPeriodSchoolYear],[GradingPeriodSequence],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName],[StudentUSI],[NumericGradeEarned])
		    VALUES (@FallBeginDate,@SemesterDescriptorId ,@FirstSemesterDescriptorId ,@@CurrentSchoolYear,@PeriodSequence6,@LocalCourseCode8,@HighSchoolId,@@CurrentSchoolYear,@SectionIdentifier8,@SessionNameFall,@StudentUSI9,84.50)
	--StudentAcademicRecord GPA
	INSERT INTO [edfi].[StudentAcademicRecord]
           ([EducationOrganizationId],[SchoolYear],[StudentUSI],[TermDescriptorId],[CumulativeEarnedCredits],[CumulativeGradePointAverage])
     VALUES
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI1,@TermDescriptorIdFall,7.000,3.5000),
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI2,@TermDescriptorIdFall,7.000,4.5000),
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI3,@TermDescriptorIdFall,7.000,2.5000),
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI4,@TermDescriptorIdFall,7.000,3.5000),
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI5,@TermDescriptorIdFall,7.000,4.5000),
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI6,@TermDescriptorIdFall,7.000,2.5000),
           (@HighSchoolId,@@CurrentSchoolYear,@StudentUSI7,@TermDescriptorIdFall,7.000,3.5000)
	
	-- schedule
	declare @ClassPeriodName1 nvarchar(60) = '01 - Traditional';
	declare @ClassPeriodName2 nvarchar(60) = '02 - Traditional';
	declare @ClassPeriodName3 nvarchar(60) = '03 - Traditional';
	declare @ClassPeriodName4 nvarchar(60) = '04 - Traditional';
	declare @ClassPeriodName5 nvarchar(60) = '05 - Traditional';
	declare @ClassPeriodName6 nvarchar(60) = '06 - Traditional';
	declare @ClassPeriodName7 nvarchar(60) = '07 - Traditional';

	declare @BellScheduleName nvarchar(60) = 'Normal Schedule';

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName1
           ,@HighSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName2
           ,@HighSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName3
           ,@HighSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName4
           ,@HighSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName5
           ,@HighSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName6
           ,@HighSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName7
           ,@HighSchoolId)
		   
	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName8
           ,@HighSchoolId)
		   
	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName9
           ,@MiddleSchoolId)
	
	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName1
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName2
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName3
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName4
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName5
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName6
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[ClassPeriod]
           ([ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@ClassPeriodName7
           ,@MiddleSchoolId)



	INSERT INTO [edfi].[BellSchedule]
           ([BellScheduleName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName1
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName2
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName3
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName4
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName5
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName6
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName7
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-02')
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-03')
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-04')
           ,@HighSchoolId)
	
	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-05')
           ,@HighSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-06')
           ,@HighSchoolId)




		   select * from [edfi].[BellSchedule] where SchoolId = 62222
		   select * from [edfi].[BellScheduleDate] where SchoolId = 62222

		   
	INSERT INTO [edfi].[BellSchedule]
           ([BellScheduleName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName1
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName2
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName3
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName4
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName5
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName6
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleClassPeriod]
           ([BellScheduleName]
           ,[ClassPeriodName]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,@ClassPeriodName7
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-02')
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-03')
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-04')
           ,@MiddleSchoolId)
	
	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-05')
           ,@MiddleSchoolId)

	INSERT INTO [edfi].[BellScheduleDate]
           ([BellScheduleName]
           ,[Date]
           ,[SchoolId])
     VALUES
           (@BellScheduleName
           ,CONCAT(@@CurrentSchoolYear,'-05','-06')
           ,@MiddleSchoolId)


	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName],[LocalCourseCode],[SchoolId],[SchoolYear],[SectionIdentifier],[SessionName])
     VALUES
           (@ClassPeriodName1 
           ,@LocalCourseCode1
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier1
           ,@SessionNameFall)

	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName2
           ,@LocalCourseCode2
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier2
           ,@SessionNameFall)

	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName3 
           ,@LocalCourseCode3
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier3
           ,@SessionNameFall)

	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName4
           ,@LocalCourseCode4
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier4
           ,@SessionNameFall)

	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName5 
           ,@LocalCourseCode5
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier5
           ,@SessionNameFall)

	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName6
           ,@LocalCourseCode6
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier6
           ,@SessionNameFall)

	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName7
           ,@LocalCourseCode7
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier7
           ,@SessionNameFall)
		
	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName8
           ,@LocalCourseCode8
           ,@HighSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier8
           ,@SessionNameFall)
	
	INSERT INTO [edfi].[SectionClassPeriod]
           ([ClassPeriodName]
           ,[LocalCourseCode]
           ,[SchoolId]
           ,[SchoolYear]
           ,[SectionIdentifier]
           ,[SessionName])
     VALUES
           (@ClassPeriodName9
           ,@LocalCourseCode9
           ,@MiddleSchoolId
           ,@@CurrentSchoolYear
           ,@SectionIdentifier9
           ,@SessionNameFall)
	COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

		DECLARE @Message nvarchar(2048) = ERROR_MESSAGE();
        DECLARE @Severity integer = ERROR_SEVERITY();
        DECLARE @State integer = ERROR_STATE();
		SELECT ERROR_LINE() AS ErrorLine;  
        RAISERROR(@Message, @Severity, @State);
	END CATCH;
END;

