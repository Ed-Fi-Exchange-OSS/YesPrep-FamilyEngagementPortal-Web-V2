using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Student1.ParentPortal.Models.Chat;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Models.Staff;
using Student1.ParentPortal.Models.Student;

namespace Student1.ParentPortal.Data.Models.EdFi31
{
    public class CommunicationsRepository : ICommunicationsRepository
    {
        private readonly IEdFi31Context _edFiDb;

        public CommunicationsRepository(IEdFi31Context edFiDb)
        {
            _edFiDb = edFiDb;
        }


        public async Task<ChatLogHistoryModel> GetThreadByParticipantsAsync(string studentUniqueId, string senderUniqueId, int senderTypeid, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int? unreadMessageCount, int rowsToRetrieve = 15)
        {
            var messagesToTake = 0;
            var unreadMessages = unreadMessageCount.HasValue ? unreadMessageCount.Value : await _edFiDb.ChatLogs.CountAsync(x => x.StudentUniqueId == studentUniqueId && x.SenderUniqueId == recipientUniqueId && x.SenderTypeId == recipientTypeId && x.RecipientTypeId == senderTypeid && x.RecipientUniqueId == senderUniqueId);

            messagesToTake = unreadMessages > rowsToRetrieve ? unreadMessages : rowsToRetrieve;

            var dataNeeded = await (from c in _edFiDb.ChatLogs
                                    where c.StudentUniqueId == studentUniqueId
                                          && (c.SenderUniqueId == senderUniqueId && c.RecipientUniqueId == recipientUniqueId && c.RecipientTypeId == recipientTypeId && c.SenderTypeId == senderTypeid
                                          || (c.SenderUniqueId == recipientUniqueId && c.RecipientUniqueId == senderUniqueId && c.RecipientTypeId == senderTypeid && c.SenderTypeId == recipientTypeId))
                                    orderby c.DateSent descending
                                    select c)
                              .Skip(rowsToSkip)
                              .Take(messagesToTake)
                              .ToListAsync();

            dataNeeded.Reverse();

            var returnList = dataNeeded.Select(x => new ChatLogItemModel
            {
                StudentUniqueId = x.StudentUniqueId,
                SenderUniqueId = x.SenderUniqueId,
                RecipientUniqueId = x.RecipientUniqueId,
                TranslatedMessage = x.TranslatedMessage,
                EnglishMessage = x.EnglishMessage,
                DateSent = x.DateSent,
                RecipientTypeId = x.RecipientTypeId,
                SenderTypeId = x.SenderTypeId,
                RecipientHasRead = x.RecipientHasRead,
                TranslatedLanguageCode = x.TranslatedLanguageCode
            }).ToList();

            var result = new ChatLogHistoryModel
            {
                Messages = returnList,
                EndOfMessageHistory = rowsToSkip + rowsToRetrieve >= dataNeeded.Count()
            };

            return result;
        }

        public async Task SetRecipientsRead(string studentUniqueId, string senderUniqueId, int senderTypeid, string recipientUniqueId, int recipientTypeId)
        {

            var unreadMessagesData = await (from c in _edFiDb.ChatLogs
                                            where c.StudentUniqueId == studentUniqueId && c.SenderUniqueId == recipientUniqueId && c.SenderTypeId == recipientTypeId && c.RecipientTypeId == senderTypeid && c.RecipientUniqueId == senderUniqueId
                                            && !c.RecipientHasRead
                                            select c).ToListAsync();
            //  var ctx = ((IObjectContextAdapter)_edFiDb).ObjectContext;
            //   await ctx.RefreshAsync(RefreshMode.StoreWins, _edFiDb.ChatLogs);
            unreadMessagesData.ForEach(x => x.RecipientHasRead = true);
            try
            {
                await _edFiDb.SaveChangesAsync();
            }
            catch (Exception errorUpdating)
            {
                //TODO: revist this workaround. There is a concurrency issue causing EF to not be able to properly update the record in the for some reason.
                foreach (var Message in unreadMessagesData)
                {

                    await _edFiDb.Database.ExecuteSqlCommandAsync("update  ParentPortal.ChatLog set  RecipientHasRead = 1 where  StudentUniqueId = @studentUniqueId and RecipientTypeId = @RecipienTypeId and RecipientUniqueId = @RecipientUniqueId and Id=@Id",
                            new SqlParameter("@studentUniqueId", Message.StudentUniqueId),
                            new SqlParameter("@RecipienTypeId", Message.RecipientTypeId),
                            new SqlParameter("@RecipientUniqueId", Message.RecipientUniqueId),
                            new SqlParameter("@Id", Message.Id)
                            );
                }
                //ctx = ((IObjectContextAdapter)_edFiDb).ObjectContext;
                //await ctx.RefreshAsync(RefreshMode.StoreWins, _edFiDb.ChatLogs);
                //await _edFiDb.SaveChangesAsync();
            }
        }

        public async Task<ChatLogItemModel> PersistMessage(ChatLogItemModel model)
        {
            var persistModel = new ChatLog();
            persistModel.RecipientUniqueId = model.RecipientUniqueId;
            persistModel.SenderUniqueId = model.SenderUniqueId;
            persistModel.StudentUniqueId = model.StudentUniqueId;
            persistModel.EnglishMessage = model.EnglishMessage;
            persistModel.TranslatedMessage = model.TranslatedMessage;
            persistModel.SenderTypeId = model.SenderTypeId;
            persistModel.RecipientTypeId = model.RecipientTypeId;
            persistModel.RecipientHasRead = model.RecipientHasRead;
            persistModel.TranslatedLanguageCode = model.TranslatedLanguageCode;

            var dbModel = _edFiDb.ChatLogs.Add(persistModel);

            await _edFiDb.SaveChangesAsync();

            model.DateSent = persistModel.DateSent;
            model.ChatId = dbModel.Id;

            return model;
        }

        public async Task<int> UnreadMessageCount(int studentUsi, string recipientUniqueId, int recipientTypeId, string senderUniqueid, int? senderTypeId)
        {
            var result = await (from c in _edFiDb.ChatLogs
                                join s in _edFiDb.Students on c.StudentUniqueId equals s.StudentUniqueId
                                where s.StudentUsi == studentUsi && !c.RecipientHasRead
                                && c.RecipientUniqueId == recipientUniqueId && c.RecipientTypeId == recipientTypeId
                                && (senderUniqueid != null ? (c.SenderUniqueId == senderUniqueid && c.SenderTypeId == senderTypeId) : true)
                                select c).CountAsync();

            return result;
        }

        public async Task<List<UnreadMessageModel>> RecipientUnreadMessages(string recipientUniqueId, int recipientTypeId)
        {

            var result = new List<UnreadMessageModel>();

            if (recipientTypeId == ChatLogPersonTypeEnum.Staff.Value)
            {
                result = await (from ssa in _edFiDb.StudentSectionAssociations
                                join staffsa in _edFiDb.StaffSectionAssociations
                                on new { ssa.LocalCourseCode, ssa.SchoolId, ssa.SchoolYear, ssa.SectionIdentifier, ssa.SessionName }
                                equals new { staffsa.LocalCourseCode, staffsa.SchoolId, staffsa.SchoolYear, staffsa.SectionIdentifier, staffsa.SessionName }
                                join s in _edFiDb.Students on ssa.StudentUsi equals s.StudentUsi
                                join staff in _edFiDb.Staffs on staffsa.StaffUsi equals staff.StaffUsi
                                group new { s, staff, staffsa } by s.StudentUniqueId into g
                                select new UnreadMessageModel
                                {
                                    StudentFirstName = g.FirstOrDefault().s.FirstName,
                                    StudentMiddleName = g.FirstOrDefault().s.MiddleName,
                                    StudentLastSurname = g.FirstOrDefault().s.LastSurname,
                                    StudentUniqueId = g.Key,
                                    StudentUsi = g.FirstOrDefault().s.StudentUsi,
                                    UnreadMessageCount = _edFiDb.ChatLogs.Count(x => !x.RecipientHasRead && x.RecipientUniqueId == recipientUniqueId && x.RecipientTypeId == recipientTypeId && x.StudentUniqueId == g.FirstOrDefault().s.StudentUniqueId && x.DateSent > g.FirstOrDefault().staffsa.BeginDate),
                                    PersonTypeId = ChatLogPersonTypeEnum.Staff.Value,
                                    UniqueId = g.FirstOrDefault().staff.StaffUniqueId

                                }).Where(x => x.UnreadMessageCount > 0).ToListAsync();


            }
            else
            {
                result = await (from s in _edFiDb.Students
                                join spa in _edFiDb.StudentParentAssociations on s.StudentUsi equals spa.StudentUsi
                                join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                                group new { s, p } by s.StudentUniqueId into g
                                select new UnreadMessageModel
                                {
                                    StudentFirstName = g.FirstOrDefault().s.FirstName,
                                    StudentMiddleName = g.FirstOrDefault().s.MiddleName,
                                    StudentLastSurname = g.FirstOrDefault().s.LastSurname,
                                    StudentUniqueId = g.Key,
                                    StudentUsi = g.FirstOrDefault().s.StudentUsi,
                                    UnreadMessageCount = _edFiDb.ChatLogs.Count(x => !x.RecipientHasRead && x.RecipientUniqueId == recipientUniqueId && x.RecipientTypeId == recipientTypeId && x.StudentUniqueId == g.FirstOrDefault().s.StudentUniqueId),
                                    PersonTypeId = ChatLogPersonTypeEnum.Parent.Value,
                                    UniqueId = g.FirstOrDefault().p.ParentUniqueId
                                }).Where(x => x.UnreadMessageCount > 0)
                             .ToListAsync();
            }

            return result.ToList();
        }

        public async Task<List<UnreadMessageModel>> PrincipalRecipientMessages(string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, string searchTerm, bool onlyUnreadMessages)
        {
            var baseQuery = (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations

                             join sta in _edFiDb.Staffs on seoaa.StaffUsi equals sta.StaffUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on seoaa.EducationOrganizationId equals ssa.SchoolId
                             join s in _edFiDb.Students on ssa.StudentUsi equals s.StudentUsi
                             //ssa.StudentUsi equals s.StudentUsi 
                             join spa in _edFiDb.StudentParentAssociations on ssa.StudentUsi equals spa.StudentUsi
                             join d in _edFiDb.Descriptors on spa.RelationDescriptorId equals d.DescriptorId

                             join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                             from pf in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             from chl in _edFiDb.ChatLogs
                             where (recipientUniqueId == chl.RecipientUniqueId && chl.SenderUniqueId == p.ParentUniqueId) || (recipientUniqueId == chl.SenderUniqueId && chl.RecipientUniqueId == p.ParentUniqueId)
                             join st in _edFiDb.Students on chl.StudentUniqueId equals st.StudentUniqueId

                             where sta.StaffUniqueId == recipientUniqueId
                             group new { p, st, d, pf } by new { p.ParentUniqueId, st.StudentUniqueId } into g
                             select new UnreadMessageModel
                             {
                                 FirstName = g.FirstOrDefault().p.FirstName,
                                 LastSurname = g.FirstOrDefault().p.LastSurname,
                                 UniqueId = g.FirstOrDefault().p.ParentUniqueId,
                                 Usi = g.FirstOrDefault().p.ParentUsi,
                                 StudentUniqueId = g.FirstOrDefault().st.StudentUniqueId,
                                 StudentUsi = g.FirstOrDefault().st.StudentUsi,
                                 StudentFirstName = g.FirstOrDefault().st.FirstName,
                                 StudentLastSurname = g.FirstOrDefault().st.LastSurname,
                                 RelationToStudent = g.FirstOrDefault().d.Description,
                                 LanguageCode = g.FirstOrDefault().pf.LanguageCode,
                                 OldestMessageDateSent = _edFiDb.ChatLogs.Where(c => (c.RecipientUniqueId == recipientUniqueId && c.SenderUniqueId == g.FirstOrDefault().p.ParentUniqueId) ||
                                                                                (c.SenderUniqueId == recipientUniqueId && c.RecipientUniqueId == g.FirstOrDefault().p.ParentUniqueId)).Max(x => x.DateSent),
                                 UnreadMessageCount = _edFiDb.ChatLogs.Count(x => !x.RecipientHasRead && x.RecipientUniqueId == recipientUniqueId
                                                                                                      && x.RecipientTypeId == recipientTypeId
                                                                                                      && x.SenderUniqueId == g.FirstOrDefault().p.ParentUniqueId
                                                                                                      && x.StudentUniqueId == g.FirstOrDefault().st.StudentUniqueId)
                             });
            baseQuery = baseQuery.Where(x => x.OldestMessageDateSent != null).OrderByDescending(x => x.OldestMessageDateSent);


            if (onlyUnreadMessages)
            {
                baseQuery = baseQuery.Where(x => x.UnreadMessageCount > 0);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                baseQuery = baseQuery.Where(x => x.FirstName.Contains(searchTerm)
                                              || x.LastSurname.Contains(searchTerm)
                                              || x.StudentFirstName.Contains(searchTerm)
                                              || x.StudentLastSurname.Contains(searchTerm)
                                              || (x.FirstName + " " + x.LastSurname).Contains(searchTerm)
                                              || (x.LastSurname + " " + x.FirstName).Contains(searchTerm)
                                              || (x.StudentFirstName + " " + x.StudentLastSurname).Contains(searchTerm)
                                              || (x.StudentLastSurname + " " + x.StudentFirstName).Contains(searchTerm));
            }

            if (rowsToSkip > 0)
            {
                baseQuery = baseQuery.Skip(rowsToSkip);
            }

            if (rowsToRetrieve > 0)
            {
                baseQuery = baseQuery.Take(rowsToRetrieve);
            }



            var query = await baseQuery.ToListAsync();

            return query;
        }

        public async Task<int> PrincipalRecipientMessagesCount(string recipientUniqueId, int recipientTypeId, string searchTerm, bool onlyUnreadMessages)
        {
            var baseQuery = (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                             join sta in _edFiDb.Staffs on seoaa.StaffUsi equals sta.StaffUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on seoaa.EducationOrganizationId equals ssa.SchoolId
                             join s in _edFiDb.Students on ssa.StudentUsi equals s.StudentUsi
                             join spa in _edFiDb.StudentParentAssociations on ssa.StudentUsi equals spa.StudentUsi
                             join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                             where sta.StaffUniqueId == recipientUniqueId
                             group new { p, s } by new { p.ParentUniqueId } into g
                             select new UnreadMessageModel
                             {
                                 FirstName = g.FirstOrDefault().p.FirstName,
                                 LastSurname = g.FirstOrDefault().p.LastSurname,
                                 UniqueId = g.FirstOrDefault().p.ParentUniqueId,
                                 Usi = g.FirstOrDefault().p.ParentUsi,
                                 StudentUniqueId = g.FirstOrDefault().s.StudentUniqueId,
                                 StudentUsi = g.FirstOrDefault().s.StudentUsi,
                                 StudentFirstName = g.FirstOrDefault().s.FirstName,
                                 StudentLastSurname = g.FirstOrDefault().s.LastSurname,
                                 OldestMessageDateSent = _edFiDb.ChatLogs.Where(c => (c.RecipientUniqueId == recipientUniqueId && c.SenderUniqueId == g.FirstOrDefault().p.ParentUniqueId) ||
                                                                               (c.SenderUniqueId == recipientUniqueId && c.RecipientUniqueId == g.FirstOrDefault().p.ParentUniqueId)).Max(x => x.DateSent),
                                 UnreadMessageCount = _edFiDb.ChatLogs.Count(x => !x.RecipientHasRead && x.RecipientUniqueId == recipientUniqueId
                                                                                                      && x.RecipientTypeId == recipientTypeId
                                                                                                      && x.SenderUniqueId == g.FirstOrDefault().p.ParentUniqueId)
                             });

            baseQuery = baseQuery.Where(x => x.OldestMessageDateSent != null);
            if (onlyUnreadMessages)
            {
                baseQuery = baseQuery.Where(x => x.UnreadMessageCount > 0);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                baseQuery = baseQuery.Where(x => x.FirstName.Contains(searchTerm) || x.LastSurname.Contains(searchTerm) || x.StudentFirstName.Contains(searchTerm) || x.StudentLastSurname.Contains(searchTerm));
            }

            var query = await baseQuery.CountAsync();

            return query;
        }

        public async Task<AllRecipients> GetAllStaffRecipients(int? studentUsi, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding)
        {
            // Get all students with their parents and get all related students and their related courses
            var parents = await _edFiDb.StaffChatRecipients.Where(x => x.BeginDate <= endOfDayTodayWithPadding && x.EndDate >= beginningOfDayTodayWithPadding && x.StaffUniqueId == recipientUniqueId && x.ParentUsi != null)
                                                           .GroupBy(x => x.StudentUsi)
                                                           .Select(g => new StudentRecipients
                                                           {
                                                               StudentUsi = g.Key,
                                                               StudentUniqueId = g.FirstOrDefault().StudentUniqueId,
                                                               FirstName = g.FirstOrDefault().StudentFirstName,
                                                               MiddleName = g.FirstOrDefault().StudentMiddleName,
                                                               LastSurname = g.FirstOrDefault().StudentLastSurname,
                                                               MostRecentMessageDate = g.FirstOrDefault().MostRecentMessageDate,
                                                               RelationsToStudent = new List<string> { g.FirstOrDefault().LocalCourseTitle },
                                                               Recipients = g.GroupBy(x => x.ParentUsi).Select(recipient => new RecipientModel
                                                               {
                                                                   Usi = recipient.Key.Value,
                                                                   UniqueId = recipient.FirstOrDefault().ParentUniqueId,
                                                                   FirstName = recipient.FirstOrDefault().ParentFirstName,
                                                                   LastSurname = recipient.FirstOrDefault().ParentLastSurname,
                                                                   PersonTypeId = ChatLogPersonTypeEnum.Parent.Value,
                                                                   ReplyExpectations = recipient.FirstOrDefault().ReplyExpectations != null ? recipient.FirstOrDefault().ReplyExpectations : "",
                                                                   UnreadMessageCount = recipient.FirstOrDefault().UnreadMessageCount.Value,
                                                                   LanguageCode = recipient.FirstOrDefault().LanguageCode
                                                               }).ToList()
                                                           }).OrderBy(x => x.MostRecentMessageDate).Skip(rowsToSkip).Take(rowsToRetrieve).ToListAsync();

            var totalRecipients = await _edFiDb.StaffChatRecipients.Where(x => x.BeginDate <= endOfDayTodayWithPadding && x.EndDate >= beginningOfDayTodayWithPadding && x.StaffUniqueId == recipientUniqueId)
                                                                   .GroupBy(x => x.StudentUsi).CountAsync();

            List<StudentRecipients> result = new List<StudentRecipients>();

            // If they selected a student
            if (studentUsi.HasValue)
                result.Add(parents.FirstOrDefault(x => x.StudentUsi == studentUsi.Value));

            var unreadMessages = parents.Where(x => x.UnreadMessageCount > 0 && (studentUsi.HasValue ? studentUsi.Value != x.StudentUsi : true)).ToList();

            parents = parents.Where(x => !unreadMessages.Any(um => um.StudentUniqueId == x.StudentUniqueId)).ToList();

            var recentMessages = parents.Where(x => (studentUsi.HasValue ? studentUsi.Value != x.StudentUsi : true)
            && x.MostRecentMessageDate != null).OrderByDescending(x => x.MostRecentMessageDate).ToList();

            parents = parents.Where(x => !recentMessages.Any(um => um.StudentUniqueId == x.StudentUniqueId)).ToList();

            var otherMessages = parents.Where(x => (studentUsi.HasValue ? studentUsi.Value != x.StudentUsi : true)).ToList();

            result.AddRange(unreadMessages);
            result.AddRange(recentMessages);
            result.AddRange(otherMessages);

            var model = new AllRecipients
            {
                EndOfData = totalRecipients <= rowsToSkip + rowsToRetrieve,
                StudentRecipients = result
            };

            return model;
        }

        public async Task<AllRecipients> GetAllParentRecipients(int? studentUsi, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, string[] validLeadersDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding)
        {
            var students = await (from spa in _edFiDb.StudentParentAssociations
                                  join p in _edFiDb.Parents
                                  on spa.ParentUsi equals p.ParentUsi
                                  where p.ParentUniqueId == recipientUniqueId
                                  select spa.StudentUsi).ToListAsync();

            //var teachers = await _edFiDb.ParentChatRecipients.Where(x => x.RecipientUniqueId == recipientUniqueId && x.BeginDate <= endOfDayTodayWithPadding && x.EndDate >= beginningOfDayTodayWithPadding && students.Contains(x.StudentUsi))
            //                                                 .OrderBy(x => x.StudentUsi).ToListAsync();

            var teachers = await _edFiDb.ParentChatRecipients.Where(x => x.RecipientUniqueId == recipientUniqueId && students.Contains(x.StudentUsi) )
                                                            .OrderBy(x => x.StudentUsi).ToListAsync();

            var campusLeaders = await _edFiDb.ParentPrincipalsChatRecipients
                                                .Where(x => x.RecipientUniqueId == recipientUniqueId && validLeadersDescriptors.Contains(x.RelationsToStudent) && students.Contains(x.StudentUsi))
                                                .ToListAsync();

            var validLeadersList = validLeadersDescriptors.ToList();
            campusLeaders = campusLeaders.OrderBy(c => validLeadersList.FindIndex(
                            x => x.Equals(c.RelationsToStudent, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            var studentLeaderStaff = campusLeaders.GroupBy(x => x.StudentUsi)
                .Select(x => new StudentRecipients
                {
                    StudentUsi = x.Key,
                    StudentUniqueId = x.FirstOrDefault().StudentUniqueId,
                    FirstName = x.FirstOrDefault().StudentFirstName,
                    MiddleName = x.FirstOrDefault().StudentMiddleName,
                    LastSurname = x.FirstOrDefault().StudentLastSurname,
                    MostRecentMessageDate = x.FirstOrDefault().MostRecentMessageDate,
                    Recipients = x.Select(recipient => new RecipientModel
                    {
                        FirstName = recipient.StaffFirstName,
                        LastSurname = recipient.StaffLastSurname,
                        Usi = recipient.StaffUsi,
                        UniqueId = recipient.StaffUniqueId,
                        PersonTypeId = ChatLogPersonTypeEnum.Staff.Value,
                        RelationsToStudent = new List<string> { recipient.RelationsToStudent },
                        UnreadMessageCount = recipient.UnreadMessageCount.Value
                    }).ToList()
                }).ToList();

            var totalRecipients = teachers.GroupBy(x => x.StudentUsi).Count();

            var studentStaff = teachers.GroupBy(x => x.StudentUsi)
                .Select(x => new StudentRecipients
                {
                    StudentUsi = x.Key,
                    StudentUniqueId = x.FirstOrDefault().StudentUniqueId,
                    FirstName = x.FirstOrDefault().StudentFirstName,
                    MiddleName = x.FirstOrDefault().StudentMiddleName,
                    LastSurname = x.FirstOrDefault().StudentLastSurname,
                    MostRecentMessageDate = x.FirstOrDefault().MostRecentMessageDate,                    
                    Recipients = x.GroupBy(t => t.StaffUsi).Select(recipient => new RecipientModel
                    {
                        FirstName = recipient.FirstOrDefault().FirstName,
                        LastSurname = recipient.FirstOrDefault().LastSurname,
                        Usi = recipient.FirstOrDefault().StaffUsi,
                        ReplyExpectations = recipient.FirstOrDefault().ReplyExpectations,
                        UniqueId = recipient.FirstOrDefault().StaffUniqueId,
                        PersonTypeId = ChatLogPersonTypeEnum.Staff.Value,
                        RelationsToStudent = new List<string> { recipient.FirstOrDefault().RelationsToStudent },
                        UnreadMessageCount = recipient.FirstOrDefault().UnreadMessageCount.Value
                    }).ToList()
                }).OrderBy(x => x.StudentUsi).Skip(rowsToSkip).Take(rowsToRetrieve).ToList();
            if (studentStaff.Count<1)
                    studentStaff = new List<StudentRecipients>();
            if (studentStaff.Count > 0)
                if (studentStaff[0] == null)
                    studentStaff = new List<StudentRecipients>();


                    foreach (var ss in studentStaff)
            {
                if (studentLeaderStaff.Count() > 0)
                {
                    var recipientsLeaders = studentLeaderStaff.FirstOrDefault(x => x.StudentUsi == ss.StudentUsi).Recipients;
                    ss.Recipients.AddRange(recipientsLeaders);
                }
            }

            if (studentUsi.HasValue)
            {
                var studentSelected = studentStaff.FirstOrDefault(x => x.StudentUsi == studentUsi.Value);
                studentStaff.Remove(studentSelected);
                studentStaff.Insert(0, studentSelected);
            }

            List<StudentRecipients> result = new List<StudentRecipients>();

            var unreadMessages = studentStaff.ToList().Where(x => x.UnreadMessageCount > 0).ToList();

            studentStaff = studentStaff.Where(x => !unreadMessages.Any(um => um.StudentUniqueId == x.StudentUniqueId)).ToList();

            var recentMessages = studentStaff.Where(x => x.MostRecentMessageDate != null).OrderByDescending(x => x.MostRecentMessageDate).ToList();

            studentStaff = studentStaff.Where(x => !recentMessages.Any(um => um.StudentUniqueId == x.StudentUniqueId)).ToList();

            result.AddRange(unreadMessages);
            result.AddRange(recentMessages);
            result.AddRange(studentStaff);

            var model = new AllRecipients
            {
                EndOfData = totalRecipients <= rowsToSkip + rowsToRetrieve,
                StudentRecipients = result
            };

            return model;
        }

        public async Task<AllRecipients> GetAllCampusLeaderRecipients(int? studentUsi, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, string[] validLeadersDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding)
        {
            var campusLeaders = await _edFiDb.ParentPrincipalsChatRecipients
                                               .Where(x => x.StaffUniqueId == recipientUniqueId)
                                               .ToListAsync();

            var students = await (from spa in _edFiDb.StudentParentAssociations
                                  join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                                  join ppc in _edFiDb.ParentPrincipalsChatRecipients on spa.StudentUsi equals ppc.StudentUsi
                                  where ppc.StaffUniqueId == recipientUniqueId
                                  select spa.StudentUsi).ToListAsync();

            var teachers = await _edFiDb.ParentChatRecipients.Where(x => x.BeginDate <= endOfDayTodayWithPadding && x.EndDate >= beginningOfDayTodayWithPadding && students.Contains(x.StudentUsi))
             .OrderBy(x => x.StudentUsi).ToListAsync();

            var parents = await _edFiDb.StaffChatRecipients.Where(x => x.BeginDate <= endOfDayTodayWithPadding && x.EndDate >= beginningOfDayTodayWithPadding && students.Contains(x.StudentUsi))
             .OrderBy(x => x.StudentUsi).ToListAsync();

            var validLeadersList = validLeadersDescriptors.ToList();
            campusLeaders = campusLeaders.OrderBy(c => validLeadersList.FindIndex(
                            x => x.Equals(c.RelationsToStudent, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            var studentLeaderStaff = campusLeaders.GroupBy(x => x.StudentUsi)
                .Select(x => new StudentRecipients
                {
                    StudentUsi = x.Key,
                    StudentUniqueId = x.FirstOrDefault().StudentUniqueId,
                    FirstName = x.FirstOrDefault().StudentFirstName,
                    MiddleName = x.FirstOrDefault().StudentMiddleName,
                    LastSurname = x.FirstOrDefault().StudentLastSurname,
                    MostRecentMessageDate = x.FirstOrDefault().MostRecentMessageDate,
                    Recipients = x.Select(recipient => new RecipientModel
                    {
                        FirstName = recipient.StaffFirstName,
                        LastSurname = recipient.StaffLastSurname,
                        Usi = recipient.StaffUsi,
                        UniqueId = recipient.StaffUniqueId,
                        PersonTypeId = ChatLogPersonTypeEnum.Staff.Value,
                        RelationsToStudent = new List<string> { recipient.RelationsToStudent },
                        UnreadMessageCount = recipient.UnreadMessageCount.Value
                    }).ToList()
                }).ToList();

            var totalRecipients = parents.GroupBy(x => x.StudentUsi).Count();

            var studentStaff = parents.GroupBy(x => x.StudentUsi)
                .Select(x => new StudentRecipients
                {
                    StudentUsi = x.Key,
                    StudentUniqueId = x.FirstOrDefault().StudentUniqueId,
                    FirstName = x.FirstOrDefault().StudentFirstName,
                    MiddleName = x.FirstOrDefault().StudentMiddleName,
                    LastSurname = x.FirstOrDefault().StudentLastSurname,
                    MostRecentMessageDate = x.FirstOrDefault().MostRecentMessageDate,
                    Recipients = x.Select(recipient => new RecipientModel
                    {
                        Usi = recipient.ParentUsi.Value,
                        UniqueId = recipient.ParentUniqueId,
                        FirstName = recipient.ParentFirstName,
                        LastSurname = recipient.ParentLastSurname,
                        PersonTypeId = ChatLogPersonTypeEnum.Parent.Value,
                        ReplyExpectations = recipient.ReplyExpectations != null ? recipient.ReplyExpectations : "",
                        UnreadMessageCount = recipient.UnreadMessageCount.Value,
                        LanguageCode = recipient.LanguageCode
                    }).ToList().GetRange(0, 1)
                }).OrderBy(x => x.StudentUsi).Skip(rowsToSkip).Take(rowsToRetrieve).ToList();

            /*foreach (var ss in studentStaff)
            {
                if (studentLeaderStaff.Count() > 0)
                {
                    var recipientsLeaders = studentLeaderStaff.FirstOrDefault(x => x.StudentUsi == ss.StudentUsi).Recipients;
                    ss.Recipients.AddRange(recipientsLeaders);
                }
            }*/

            if (studentUsi.HasValue)
            {
                var studentSelected = studentStaff.FirstOrDefault(x => x.StudentUsi == studentUsi.Value);
                studentStaff.Remove(studentSelected);
                studentStaff.Insert(0, studentSelected);
            }

            List<StudentRecipients> result = new List<StudentRecipients>();

            var unreadMessages = studentStaff.Where(x => x.UnreadMessageCount > 0).ToList();

            studentStaff = studentStaff.Where(x => !unreadMessages.Any(um => um.StudentUniqueId == x.StudentUniqueId)).ToList();

            var recentMessages = studentStaff.Where(x => x.MostRecentMessageDate != null).OrderByDescending(x => x.MostRecentMessageDate).ToList();

            studentStaff = studentStaff.Where(x => !recentMessages.Any(um => um.StudentUniqueId == x.StudentUniqueId)).ToList();

            result.AddRange(unreadMessages);
            result.AddRange(recentMessages);
            result.AddRange(studentStaff);

            var model = new AllRecipients
            {
                EndOfData = totalRecipients <= rowsToSkip + rowsToRetrieve,
                StudentRecipients = result
            };

            return model;
        }

        public async Task SetRecipientRead(ChatLogItemModel model)
        {
            var dbModel = _edFiDb.ChatLogs
                .Where(x => x.RecipientUniqueId == model.RecipientUniqueId &&
                        x.RecipientTypeId == model.RecipientTypeId &&
                        x.SenderUniqueId == model.SenderUniqueId &&
                        x.SenderTypeId == model.SenderTypeId &&
                        x.StudentUniqueId == model.StudentUniqueId)
                .OrderByDescending(x => x.DateSent).FirstOrDefault();

            dbModel.RecipientHasRead = true;

            await _edFiDb.SaveChangesAsync();
        }

        public async Task<ParentStudentCountModel> GetFamilyMembersByGradesAndPrograms(int staffUsi, ParentStudentCountFilterModel model, string[] validParentDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding)
        {
            //Here we got the information for the school first 
            var school = await (from s in _edFiDb.Schools
                                            .Include(x => x.EducationOrganization)
                                join sf in _edFiDb.StaffEducationOrganizationAssignmentAssociations on s.SchoolId equals sf.EducationOrganizationId
                                where sf.StaffUsi == staffUsi && s.SchoolId == model.SchoolId
                                select new
                                {
                                    s.EducationOrganization.EducationOrganizationId,
                                    s.EducationOrganization.NameOfInstitution,
                                    s.LocalEducationAgencyId,
                                    s.SchoolId
                                }).FirstOrDefaultAsync();

            var edorgIds = new List<int> { school.SchoolId, school.LocalEducationAgencyId.Value };

            //ToDo: Fix this query
            var baseQuery = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             from seoapp in _edFiDb.GeneralStudentProgramAssociations.Where(x => x.StudentUsi == spa.StudentUsi && edorgIds.Contains(x.EducationOrganizationId)).DefaultIfEmpty()
                             where sess.SchoolId == model.SchoolId
                                  && validParentDescriptors.Contains(dspa.CodeValue)
                                  && sy.CurrentSchoolYear
                                  && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                  && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate >= beginningOfDayTodayWithPadding || ssa.ExitWithdrawDate == null)
                             select new
                             {
                                 p.ParentUsi,
                                 s.StudentUsi,

                                 //gradeLevels and programs filters
                                 gradeLevels = gld.DescriptorId,
                                 programs = seoapp.ProgramTypeDescriptorId
                             });

            if (model.Grades.Count() > 0)
                baseQuery = baseQuery.Where(x => model.Grades.Contains(x.gradeLevels));

            if (model.Programs.Count() > 0)
                baseQuery = baseQuery.Where(x => model.Programs.Contains(x.programs));

            var groupQuery = (from q in baseQuery
                              group new { q } by new { q.ParentUsi, q.StudentUsi } into g
                              select new
                              {
                                  g.FirstOrDefault().q.ParentUsi,
                                  g.FirstOrDefault().q.StudentUsi
                              });

            var query = await groupQuery.ToListAsync();

            var result = new ParentStudentCountModel()
            {
                CampusName = school.NameOfInstitution,
                FamilyMembersCount = query.GroupBy(x => x.ParentUsi).Count(),
                StudentsCount = query.GroupBy(x => x.StudentUsi).Count()
            };
            return result;
        }

        public async Task<List<ParentStudentsModel>> GetParentsByGradeLevelsAndPrograms(Guid queueId, string personUniqueId, int schoolId, int[] grades, int[] programs, string[] validParentDescriptors, string[] validEmailTypeDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding)
        {
            //get LEA id for the school
            var school = await (from s in _edFiDb.Schools
                                where s.SchoolId == schoolId
                                select new { s.SchoolId, s.LocalEducationAgencyId }).SingleOrDefaultAsync();

            var edOrgIds = new List<int> { schoolId, school.LocalEducationAgencyId.Value };

            var mobileTelephoneDescriptor = (from d in _edFiDb.Descriptors
                                             where d.Namespace == "uri://ed-fi.org/TelephoneNumberTypeDescriptor" && d.CodeValue == "Mobile"
                                             select d).FirstOrDefault();

            var baseQuery = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             join gmql in _edFiDb.GroupMessagesQueueLogs on queueId equals gmql.Id
                             // Left join
                             from pe in _edFiDb.ParentElectronicMails.Where(x => x.ParentUsi == p.ParentUsi).DefaultIfEmpty()
                             from ped in _edFiDb.Descriptors.Where(x => x.DescriptorId == pe.ElectronicMailTypeDescriptorId && validEmailTypeDescriptors.Contains(x.CodeValue)).DefaultIfEmpty()
                             from seoapp in _edFiDb.GeneralStudentProgramAssociations.Where(x => x.StudentUsi == ssa.StudentUsi && edOrgIds.Contains(x.EducationOrganizationId)).DefaultIfEmpty()
                             from ptd in _edFiDb.Descriptors.Where(x => x.DescriptorId == seoapp.ProgramTypeDescriptorId).DefaultIfEmpty()
                             from pp in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             from ppe in _edFiDb.ParentProfileElectronicMails
                                 .Where(x => x.ParentUniqueId == p.ParentUniqueId && x.PrimaryEmailAddressIndicator == true)
                                 .DefaultIfEmpty()
                             from ppt in _edFiDb.ParentTelephones.Where(x =>
                                 x.ParentUsi == p.ParentUsi && x.TelephoneNumberTypeDescriptorId == mobileTelephoneDescriptor.DescriptorId).DefaultIfEmpty()
                             from gmlcl in _edFiDb.GroupMessagesLogChatLogs.Where(x => x.GroupMessagesLogId == gmql.Id ).DefaultIfEmpty()
                             from cl in _edFiDb.ChatLogs.Where(x => x.RecipientUniqueId == p.ParentUniqueId && gmlcl.ChatLogId == x.Id ).DefaultIfEmpty()
                             where ssa.SchoolId == schoolId
                                   && (gmlcl == null || (gmlcl.GroupMessagesLogId == queueId && gmlcl.Status != GroupMessagesStatusEnum.Sent.Value))
                                   && sy.CurrentSchoolYear
                                   && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                   && validParentDescriptors.Contains(dspa.CodeValue)
                                   && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate >= beginningOfDayTodayWithPadding || ssa.ExitWithdrawDate == null)
                             select new
                             {
                                 ParentUsi = p.ParentUsi,
                                 ParentUniqueId = p.ParentUniqueId,
                                 ParentFirstName = p.FirstName,
                                 ParentLastSurname = p.LastSurname,
                                 EdFiEmail = pe.ElectronicMailAddress,
                                 ProfileEmail = ppe.ElectronicMailAddress,
                                 ProfileTelephone = ppt.TelephoneNumber,
                                 LanguageCode = pp.LanguageCode,
                                 // This validation is for prod database, the PreferredMethodOfContactTypeId some times is null
                                 // *Note: In case of the PreferredMethodOfContactTypeId is null should be Email by Default
                                 PreferredMethodOfContactTypeId = pp != null ? pp.PreferredMethodOfContactTypeId : ppt != null ? MethodOfContactTypeEnum.SMS.Value : MethodOfContactTypeEnum.Email.Value,

                                 StudentFirstName = s.FirstName,
                                 StudentLastSurname = s.LastSurname,
                                 StudentUsi = s.StudentUsi,
                                 StudentUniqueId = s.StudentUniqueId,
                                 // Grade and Programs info for filtering,
                                 Grades = ssa.EntryGradeLevelDescriptorId,
                                 Programs = ptd.DescriptorId
                             });


            if (grades.Length > 0)
                baseQuery = baseQuery.Where(x => grades.Contains(x.Grades));

            if (programs.Length > 0)
                baseQuery = baseQuery.Where(x => programs.Contains(x.Programs));

            var query = (from q in baseQuery
                         group new { q } by q.StudentUsi into g
                         select new ParentStudentsModel
                         {
                             ParentUsi = g.FirstOrDefault().q.ParentUsi,
                             ParentUniqueId = g.FirstOrDefault().q.ParentUniqueId,
                             ParentFirstName = g.FirstOrDefault().q.ParentFirstName,
                             ParentLastSurname = g.FirstOrDefault().q.ParentLastSurname,
                             EdFiEmail = g.FirstOrDefault().q.EdFiEmail,
                             ProfileEmail = g.FirstOrDefault().q.ProfileEmail,
                             ProfileTelephone = g.FirstOrDefault().q.ProfileTelephone,
                             LanguageCode = g.FirstOrDefault().q.LanguageCode,
                             PreferredMethodOfContactTypeId = g.FirstOrDefault().q.PreferredMethodOfContactTypeId,
                             StudentFirstName = g.FirstOrDefault().q.StudentFirstName,
                             StudentLastSurname = g.FirstOrDefault().q.StudentLastSurname,
                             StudentUsi = g.FirstOrDefault().q.StudentUsi,
                             StudentUniqueId = g.FirstOrDefault().q.StudentUniqueId
                         });

            var studentsAssociatedWithStaff = await query.ToListAsync();

            return studentsAssociatedWithStaff;
        }

        public async Task<GroupMessagesQueueLogModel> PersistQueueGroupMessage(GroupMessagesQueueLogModel model)
        {
            var persisModel = new GroupMessagesQueueLog
            {
                Type = model.Type,
                QueuedDateTime = model.QueuedDateTime,
                StaffUniqueIdSent = model.StaffUniqueId,
                SchoolId = model.SchoolId,
                Audience = model.Audience,
                FilterParams = model.FilterParams,
                Subject = model.Subject,
                Body = model.BodyMessage,
                SentStatus = model.SentStatus,
                Data = model.Data
            };

            _edFiDb.GroupMessagesQueueLogs.Add(persisModel);

            await _edFiDb.SaveChangesAsync();

            model.Id = persisModel.Id;

            return model;
        }

        public async Task<GroupMessagesChatLogModel> PersistChatGroupMessage(GroupMessagesChatLogModel model)
        {
            var persistModel = new GroupMessagesLogChatLog();
            persistModel.GroupMessagesLogId = model.GroupMessagesLogId;
            persistModel.ChatLogId = model.ChatLogId;
            persistModel.Status = model.Status.Value;
            persistModel.ErrorMessage = model.ErrorMessage;

            _edFiDb.GroupMessagesLogChatLogs.Add(persistModel);

            await _edFiDb.SaveChangesAsync();

            return model;
        }

        public async Task<List<GroupMessagesQueueLogModel>> GetGroupMessagesQueuesQueued()
        {

            var model = await (from gmq in _edFiDb.GroupMessagesQueueLogs
                               where gmq.SentStatus == GroupMessagesStatusEnum.Queued.Value
                                     || (gmq.SentStatus == GroupMessagesStatusEnum.Error.Value && gmq.RetryCount < 4)
                               select new GroupMessagesQueueLogModel
                               {
                                   Id = gmq.Id,
                                   SentStatus = gmq.SentStatus,
                                   FilterParams = gmq.FilterParams,
                                   StaffUniqueId = gmq.StaffUniqueIdSent,
                                   SchoolId = gmq.SchoolId,
                                   QueuedDateTime = gmq.QueuedDateTime,
                                   Type = gmq.Type,
                                   Audience = gmq.Audience,
                                   BodyMessage = gmq.Body,
                                   Subject = gmq.Subject,
                                   Data = gmq.Data,
                                   RetryCount = gmq.RetryCount
                               }).OrderBy(x => x.QueuedDateTime).ToListAsync();
            return model;
        }

        public async Task<List<GroupMessagesQueueLogModel>> GetGroupMessagesQueuesQueued(string staffUniqueId, int schoolId, QueuesFilterModel model)
        {
            List<GroupMessagesQueueLogModel> result = new List<GroupMessagesQueueLogModel>();
            var baseQuery = (from gmq in _edFiDb.GroupMessagesQueueLogs
                             where gmq.StaffUniqueIdSent == staffUniqueId
                             && gmq.SchoolId == schoolId
                             && gmq.Type == GroupMessagesQueueTypeEnum.Group.DisplayName
                             select new GroupMessagesQueueLogModel
                             {
                                 Id = gmq.Id,
                                 SentStatus = gmq.SentStatus,
                                 FilterParams = gmq.FilterParams,
                                 StaffUniqueId = gmq.StaffUniqueIdSent,
                                 SchoolId = gmq.SchoolId,
                                 QueuedDateTime = gmq.QueuedDateTime,
                                 Type = gmq.Type,
                                 Audience = gmq.Audience,
                                 BodyMessage = gmq.Body,
                                 Subject = gmq.Subject,
                                 Data = gmq.Data,
                                 RetryCount = gmq.RetryCount,
                                 DateSent = gmq.DateSent
                             });

            if (model.From != null)
            {
                var fromDate = new DateTime(model.From.Value.Year, model.From.Value.Month, model.From.Value.Day);
                baseQuery = baseQuery.Where(x => x.QueuedDateTime >= fromDate);
            }

            if (model.To != null)
            {
                var toDate = new DateTime(model.To.Value.Year, model.To.Value.Month, model.To.Value.Day, 23, 59, 59);
                baseQuery = baseQuery.Where(x => x.QueuedDateTime <= toDate);
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                baseQuery = baseQuery.Where(x => x.Subject.Contains(model.SearchTerm) || x.BodyMessage.Contains(model.SearchTerm));
            }

            if (!String.IsNullOrEmpty(model.SectionName))
            {
                baseQuery = baseQuery.Where(x => x.Audience.Contains(model.SectionName));
            }

            var query = await baseQuery.OrderByDescending(x => x.QueuedDateTime).ToListAsync();

            result.AddRange(query);

            if (model.GradeLevels.Any() && model.GradeLevels.Length == 1)
            {
                foreach (var q in query)
                {
                    if (q.Type == GroupMessagesQueueTypeEnum.Group.DisplayName)
                    {
                        var queueFilter = JsonConvert.DeserializeObject<GroupMessagesFilterParamsModel>(q.FilterParams);
                        if (queueFilter.gradeLevels.Length > 1 || model.GradeLevels[0] != queueFilter.gradeLevels[0])
                        {
                            result.Remove(q);
                        }

                    }
                }
            }

            if (model.Programs.Any() && model.Programs.Length == 1)
            {
                foreach (var q in query)
                {
                    bool alreadyDeleted = false;
                    if (q.Type == GroupMessagesQueueTypeEnum.Group.DisplayName)
                    {

                        var queueFilter = JsonConvert.DeserializeObject<GroupMessagesFilterParamsModel>(q.FilterParams);
                        if (queueFilter.programs.Length > 1 || model.Programs[0] != queueFilter.programs[0])
                        {
                            result.Remove(q);
                        }

                    }
                }
            }

            return result;
        }

        public async Task<GroupMessagesQueueLogModel> GetGroupMessagesQueue(Guid Id)
        {
            var model = await (from gmq in _edFiDb.GroupMessagesQueueLogs
                               where gmq.Id == Id
                               select new GroupMessagesQueueLogModel
                               {
                                   Id = gmq.Id,
                                   SentStatus = gmq.SentStatus,
                                   FilterParams = gmq.FilterParams,
                                   StaffUniqueId = gmq.StaffUniqueIdSent,
                                   SchoolId = gmq.SchoolId,
                                   QueuedDateTime = gmq.QueuedDateTime,
                                   Type = gmq.Type,
                                   Audience = gmq.Audience,
                                   BodyMessage = gmq.Body,
                                   Subject = gmq.Subject,
                                   Data = gmq.Data,
                                   RetryCount = gmq.RetryCount
                               }).FirstOrDefaultAsync();
            return model;
        }

        public async Task<ChatLogItemModel> EnsureGroupMessageChatLog(Guid groupMessageQueueId, ChatLogItemModel model)
        {
            ChatLogItemModel response;
            //check if there is an existing chat log for the group message by the group message queue. 

            var chatLogItem = (from gmlcl in _edFiDb.GroupMessagesLogChatLogs
                               join cl in _edFiDb.ChatLogs
                               on gmlcl.ChatLogId equals cl.Id
                               where gmlcl.GroupMessagesLogId == groupMessageQueueId
                                     && cl.RecipientUniqueId == model.RecipientUniqueId
                                     && cl.StudentUniqueId == model.StudentUniqueId
                               select cl).Distinct().SingleOrDefault();

            if (chatLogItem == null)
            {
                var persistModel = new ChatLog
                {
                    RecipientUniqueId = model.RecipientUniqueId,
                    SenderUniqueId = model.SenderUniqueId,
                    StudentUniqueId = model.StudentUniqueId,
                    EnglishMessage = model.EnglishMessage,
                    TranslatedMessage = model.TranslatedMessage,
                    SenderTypeId = model.SenderTypeId,
                    RecipientTypeId = model.RecipientTypeId,
                    RecipientHasRead = model.RecipientHasRead,
                    TranslatedLanguageCode = model.TranslatedLanguageCode
                };

                var dbModel = _edFiDb.ChatLogs.Add(persistModel);

                await _edFiDb.SaveChangesAsync();

                response = model;
                response.DateSent = persistModel.DateSent;
                response.ChatId = dbModel.Id;
            }
            else
            {
                response = new ChatLogItemModel
                {
                    RecipientUniqueId = chatLogItem.RecipientUniqueId,
                    SenderUniqueId = chatLogItem.SenderUniqueId,
                    StudentUniqueId = chatLogItem.StudentUniqueId,
                    EnglishMessage = chatLogItem.EnglishMessage,
                    TranslatedMessage = chatLogItem.TranslatedMessage,
                    SenderTypeId = chatLogItem.SenderTypeId,
                    RecipientTypeId = chatLogItem.RecipientTypeId,
                    RecipientHasRead = chatLogItem.RecipientHasRead,
                    TranslatedLanguageCode = chatLogItem.TranslatedLanguageCode,
                    DateSent = chatLogItem.DateSent,
                    ChatId = chatLogItem.Id
                };
            }

            return response;

        }

        public async Task<GroupMessagesQueueLogModel> UpdateGroupMessagesQueue(GroupMessagesQueueLogModel model)
        {
            var updateModel = await (from gmq in _edFiDb.GroupMessagesQueueLogs
                                     where gmq.Id == model.Id
                                     select gmq).FirstOrDefaultAsync();


            updateModel.DateSent = model.DateSent;
            updateModel.SentStatus = model.SentStatus;
            updateModel.RetryCount = model.RetryCount;
            updateModel.Data = model.Data;

            await _edFiDb.SaveChangesAsync();

            var returnModel = new GroupMessagesQueueLogModel
            {
                Id = updateModel.Id,
                Audience = updateModel.Audience,
                SchoolId = updateModel.SchoolId,
                FilterParams = updateModel.FilterParams,
                SentStatus = updateModel.SentStatus,
                StaffUniqueId = updateModel.StaffUniqueIdSent,
                QueuedDateTime = updateModel.QueuedDateTime,
                Type = updateModel.Type,
                Subject = updateModel.Subject,
                BodyMessage = updateModel.Body,
                RetryCount = updateModel.RetryCount,
                DateSent = updateModel.DateSent,
                Data = updateModel.Data
            };
            return returnModel;
        }

        public async Task<List<ParentStudentsModel>> GetParentsByGradeLevelsAndSearchTerm(string personUniqueId, string term, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding, int schoolId)
        {
            int studentUniqueId = 0;
            string firstName = "", lastName = "";
            var searchName = new List<string>();
            var isStudentId = int.TryParse(term, out studentUniqueId);

            if (!string.IsNullOrEmpty(term) && !isStudentId)
            {
                searchName = term.Split(' ').ToList();
                firstName = searchName[0];
                lastName = searchName.Count > 1 ? searchName[1] : string.Empty;
            }


            var queryBase = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             // Left join
                             from pp in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             where ssa.SchoolId == schoolId
                                   && sy.CurrentSchoolYear
                                   && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                   && validParentDescriptors.Contains(dspa.CodeValue)
                                   && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate >= beginningOfDayTodayWithPadding || ssa.ExitWithdrawDate == null)
                             select new
                             {
                                 ParentUsi = p.ParentUsi,
                                 ParentUniqueId = p.ParentUniqueId,
                                 ParentFirstName = p.FirstName,
                                 ParentLastSurname = p.LastSurname,
                                 LanguageCode = pp.LanguageCode,

                                 // This validation is for prod database, the PreferredMethodOfContactTypeId some times is null
                                 PreferredMethodOfContactTypeId = pp.PreferredMethodOfContactTypeId != null ? pp.PreferredMethodOfContactTypeId : 0,
                                 //ParentRelation = spa.EmergencyContactStatus == true ? "Emergency Contact" : dspa.Description,
                                 ParentRelation = dspa.Description,
                                 StudentFirstName = s.FirstName,
                                 StudentLastSurname = s.LastSurname,
                                 StudentUsi = s.StudentUsi,
                                 StudentUniqueId = s.StudentUniqueId,

                                 // Grade and Programs info for filtering,
                                 GradeLevelId = gld.DescriptorId,
                                 GradeLevel = gld.CodeValue
                             });

            if (string.IsNullOrEmpty(term))
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             select q);
            }

            if (isStudentId)
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId) && q.StudentUniqueId == term
                             select q);
            }

            if (!string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(lastName))
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             && (((q.StudentFirstName.Contains(firstName) && q.StudentLastSurname.Contains(lastName)) || (q.StudentFirstName.Contains(lastName) && q.StudentLastSurname.Contains(firstName))) ||
                                 ((q.ParentFirstName.Contains(firstName) && q.ParentLastSurname.Contains(lastName)) || (q.ParentFirstName.Contains(lastName) && q.ParentLastSurname.Contains(firstName))))
                             select q);
            }

            if (!string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             && ((q.StudentFirstName.Contains(firstName) || q.StudentLastSurname.Contains(firstName)) || (q.ParentFirstName.Contains(firstName) || q.ParentLastSurname.Contains(firstName)))
                             select q);
            }

            queryBase = (from q in queryBase
                         group new { q } by q.ParentUsi into g
                         select g.FirstOrDefault().q).OrderBy(x => x.StudentFirstName).ThenByDescending(x => x.StudentLastSurname);

            if (model.SkipRows > 0)
            {
                queryBase = queryBase.Skip(model.SkipRows);
            }

            if (model.PageSize.HasValue)
            {
                queryBase = queryBase.Take(model.PageSize.Value);
            }

            var query = (from q in queryBase
                         select new ParentStudentsModel
                         {
                             ParentUsi = q.ParentUsi,
                             ParentUniqueId = q.ParentUniqueId,
                             ParentFirstName = q.ParentFirstName,
                             ParentLastSurname = q.ParentLastSurname,
                             ParentRelation = q.ParentRelation,
                             LanguageCode = q.LanguageCode,
                             PreferredMethodOfContactTypeId = q.PreferredMethodOfContactTypeId,
                             StudentFirstName = q.StudentFirstName,
                             StudentLastSurname = q.StudentLastSurname,
                             StudentUsi = q.StudentUsi,
                             StudentUniqueId = q.StudentUniqueId,
                             GradeLevel = q.GradeLevel
                         });


            var students = await query.ToListAsync();

            return students;
        }

        public async Task<List<StudentBriefModel>> GetStudentListByGradeLevelsAndSearchTerm(string searchTerm, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding, int schoolId)
        {
            var queryBase = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             // Left join
                             from pp in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             where ssa.SchoolId == schoolId
                                   && sy.CurrentSchoolYear
                                   && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                   && validParentDescriptors.Contains(dspa.CodeValue)
                                   && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate >= beginningOfDayTodayWithPadding || ssa.ExitWithdrawDate == null)
                             select new
                             {
                                 ParentUsi = p.ParentUsi,
                                 ParentUniqueId = p.ParentUniqueId,
                                 ParentFirstName = p.FirstName,
                                 ParentLastSurname = p.LastSurname,
                                 LanguageCode = pp.LanguageCode,

                                 // This validation is for prod database, the PreferredMethodOfContactTypeId some times is null
                                 PreferredMethodOfContactTypeId = pp.PreferredMethodOfContactTypeId != null ? pp.PreferredMethodOfContactTypeId : 0,
                                 //ParentRelation = spa.EmergencyContactStatus == true ? "Emergency Contact" : dspa.Description,
                                 ParentRelation = dspa.Description,
                                 StudentFirstName = s.FirstName,
                                 StudentLastSurname = s.LastSurname,
                                 StudentUsi = s.StudentUsi,
                                 StudentUniqueId = s.StudentUniqueId,
                                 // Grade and Programs info for filtering,
                                 GradeLevelId = gld.DescriptorId,
                                 GradeLevel = gld.CodeValue
                             });

            if (model.Grades.Length > 0)
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             select q);
            }


            int studentUniqueId = 0;
            var isStudentId = int.TryParse(searchTerm, out studentUniqueId);

            // SearchTerm: As of when this functionality was written the SearchTerm should support the searching by a studnet's:
            // -- UniqueId (string)
            // -- Firstname (string)
            // -- Lastname (string)
            var searchValues = searchTerm.Split(' ').ToList();

            if (searchValues.Count > 0)
            {
                foreach (var sval in searchValues)
                {
                    // searchValues could be ["1224498", "Ricardo", "Baez"]
                    queryBase = (from q in queryBase
                                 where (q.StudentFirstName.Contains(sval) || q.StudentLastSurname.Contains(sval) || q.StudentUniqueId.Contains(sval))
                                 || (q.ParentFirstName.Contains(sval) || q.ParentLastSurname.Contains(sval) || q.ParentUniqueId.Contains(sval))
                                 select q);
                }
            }

            queryBase = (from q in queryBase
                         group new { q } by q.StudentUsi into g
                         select g.FirstOrDefault().q).OrderBy(x => x.StudentFirstName).ThenByDescending(x => x.StudentLastSurname);

            if (model.SkipRows > 0)
            {
                queryBase = queryBase.Skip(model.SkipRows);
            }

            if (model.PageSize.HasValue)
            {
                queryBase = queryBase.Take(model.PageSize.Value);
            }


            var query = (from q in queryBase
                         select new StudentBriefModel
                         {
                             FirstName = q.StudentFirstName,
                             LastSurname = q.StudentLastSurname,
                             StudentUsi = q.StudentUsi,
                             StudentUniqueId = q.StudentUniqueId,
                             GradeLevel = q.GradeLevel
                         });


            var students = await query.ToListAsync();

            return students;
        }

        public async Task<List<StudentBriefModel>> GetStudentListByGradeLevelsProgramsAndSearchTerm(string searchTerm, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding, int schoolId, int[] programs)
        {
            var school = await (from s in _edFiDb.Schools
                                where s.SchoolId == schoolId
                                select new { s.SchoolId, s.LocalEducationAgencyId }).FirstOrDefaultAsync();

            var edorgIds = new List<int> { school.SchoolId, school.LocalEducationAgencyId.Value };

            var queryBase = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             // Left join
                             from pp in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             from seoapp in _edFiDb.GeneralStudentProgramAssociations.Where(x => x.StudentUsi == ssa.StudentUsi && edorgIds.Contains(x.EducationOrganizationId)).DefaultIfEmpty()
                             from ptd in _edFiDb.Descriptors.Where(x => x.DescriptorId == seoapp.ProgramTypeDescriptorId).DefaultIfEmpty()

                             where ssa.SchoolId == schoolId
                                   && sy.CurrentSchoolYear
                                   && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                   && validParentDescriptors.Contains(dspa.CodeValue)
                                   && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate == null || ssa.ExitWithdrawDate <= endOfDayTodayWithPadding)
                             select new
                             {
                                 ParentUsi = p.ParentUsi,
                                 ParentUniqueId = p.ParentUniqueId,
                                 ParentFirstName = p.FirstName,
                                 ParentLastSurname = p.LastSurname,
                                 LanguageCode = pp.LanguageCode,

                                 // This validation is for prod database, the PreferredMethodOfContactTypeId some times is null
                                 PreferredMethodOfContactTypeId = pp.PreferredMethodOfContactTypeId != null ? pp.PreferredMethodOfContactTypeId : 0,
                                 //ParentRelation = spa.EmergencyContactStatus == true ? "Emergency Contact" : dspa.Description,
                                 ParentRelation = dspa.Description,
                                 StudentFirstName = s.FirstName,
                                 StudentLastSurname = s.LastSurname,
                                 StudentUsi = s.StudentUsi,
                                 StudentUniqueId = s.StudentUniqueId,
                                 // Grade and Programs info for filtering,
                                 GradeLevelId = gld.DescriptorId,
                                 GradeLevel = gld.CodeValue,
                                 Programs = ptd.DescriptorId
                             });

            // Applying filters to the base query.


            int studentUniqueId = 0;
            var isStudentId = int.TryParse(searchTerm, out studentUniqueId);

            // SearchTerm: As of when this functionality was written the SearchTerm should support the searching by a studnet's:
            // -- UniqueId (string)
            // -- Firstname (string)
            // -- Lastname (string)
            var searchValues = searchTerm.Split(' ').ToList();

            if (searchValues.Count > 0)
            {
                foreach (var sval in searchValues)
                {
                    // searchValues could be ["1224498", "Ricardo", "Baez"]
                    queryBase = (from q in queryBase
                                 where (q.StudentFirstName.Contains(sval) || q.StudentLastSurname.Contains(sval) || q.StudentUniqueId.Contains(sval))
                                 select q);
                }
            }

            if (model.Grades.Length > 0)
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             select q);
            }

            if (programs.Length > 0)
                queryBase = queryBase.Where(x => programs.Contains(x.Programs));


            queryBase = (from q in queryBase
                         group new { q } by q.StudentUsi into g
                         select g.FirstOrDefault().q).OrderBy(x => x.StudentFirstName).ThenByDescending(x => x.StudentLastSurname);

            if (model.SkipRows > 0)
            {
                queryBase = queryBase.Skip(model.SkipRows);
            }

            if (model.PageSize.HasValue)
            {
                queryBase = queryBase.Take(model.PageSize.Value);
            }


            var query = (from q in queryBase
                         select new StudentBriefModel
                         {
                             FirstName = q.StudentFirstName,
                             LastSurname = q.StudentLastSurname,
                             StudentUsi = q.StudentUsi,
                             StudentUniqueId = q.StudentUniqueId,
                             GradeLevel = q.GradeLevel
                         });


            var students = await query.ToListAsync();

            return students;
        }

        //******************************************************






        public async Task<int> GetParentsByGradeLevelsAndSearchTermCount(string personUniqueId, string term, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding, int schoolId)
        {
            int studentUniqueId = 0;
            string firstName = "", lastName = "";
            var searchName = new List<string>();
            var isStudentId = int.TryParse(term, out studentUniqueId);

            if (!string.IsNullOrEmpty(term) && !isStudentId)
            {
                searchName = term.Split(' ').ToList();
                firstName = searchName[0];
                lastName = searchName.Count > 1 ? searchName[1] : string.Empty;
            }



            var queryBase = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             // Left join
                             from pp in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             where ssa.SchoolId == schoolId
                                   && sy.CurrentSchoolYear
                                   && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                   && validParentDescriptors.Contains(dspa.CodeValue)
                                   && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate >= beginningOfDayTodayWithPadding || ssa.ExitWithdrawDate == null)
                             select new
                             {
                                 ParentUsi = p.ParentUsi,
                                 ParentFirstName = p.FirstName,
                                 ParentLastSurname = p.LastSurname,

                                 StudentFirstName = s.FirstName,
                                 StudentLastSurname = s.LastSurname,
                                 StudentUniqueId = s.StudentUniqueId,
                                 // Grade and Programs info for filtering,
                                 GradeLevelId = gld.DescriptorId,
                             });

            if (string.IsNullOrEmpty(term))
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             select q);
            }

            if (isStudentId)
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId) && q.StudentUniqueId == term
                             select q);
            }

            if (!string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(lastName))
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             && (((q.StudentFirstName.Contains(firstName) && q.StudentLastSurname.Contains(lastName)) || (q.StudentFirstName.Contains(lastName) && q.StudentLastSurname.Contains(firstName))) ||
                                 ((q.ParentFirstName.Contains(firstName) && q.ParentLastSurname.Contains(lastName)) || (q.ParentFirstName.Contains(lastName) && q.ParentLastSurname.Contains(firstName))))
                             select q);
            }

            if (!string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                queryBase = (from q in queryBase
                             where model.Grades.Contains(q.GradeLevelId)
                             && ((q.StudentFirstName.Contains(firstName) || q.StudentLastSurname.Contains(firstName)) || (q.ParentFirstName.Contains(firstName) || q.ParentLastSurname.Contains(firstName)))
                             select q);
            }

            queryBase = (from q in queryBase
                         group new { q } by q.ParentUsi into g
                         select g.FirstOrDefault().q).OrderBy(x => x.StudentFirstName);

            var query = (from q in queryBase
                         select new ParentStudentsModel
                         {
                             ParentUsi = q.ParentUsi,
                         });


            var studentsCount = await query.CountAsync();

            return studentsCount;
        }

        public async Task<List<ParentStudentsModel>> GetParentsByPanrentUsisAndGradeLevels(Guid queueId, string personUniqueId, int schoolId, int[] parentUsis, int[] gradeLevels, string[] validParentDescriptors, string[] validEmailTypeDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding)
        {
            var mobileTelephoneDescriptor = (from d in _edFiDb.Descriptors
                                             where d.Namespace == "uri://ed-fi.org/TelephoneNumberTypeDescriptor" && d.CodeValue == "Mobile"
                                             select d).FirstOrDefault();

            var baseQuery = (from p in _edFiDb.Parents
                             join spa in _edFiDb.StudentParentAssociations on p.ParentUsi equals spa.ParentUsi
                             join dspa in _edFiDb.Descriptors on spa.RelationDescriptorId equals dspa.DescriptorId
                             join s in _edFiDb.Students on spa.StudentUsi equals s.StudentUsi
                             join ssa in _edFiDb.StudentSchoolAssociations on s.StudentUsi equals ssa.StudentUsi
                             join gld in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals gld.DescriptorId
                             join studsec in _edFiDb.StudentSectionAssociations
                                        on new { s.StudentUsi, ssa.SchoolId }
                                        equals new { studsec.StudentUsi, studsec.SchoolId }
                             join sess in _edFiDb.Sessions
                                         on new { studsec.SchoolId, studsec.SessionName, studsec.SchoolYear }
                                         equals new { sess.SchoolId, sess.SessionName, sess.SchoolYear }
                             join sy in _edFiDb.SchoolYearTypes on sess.SchoolYear equals sy.SchoolYear
                             // Left join
                             from pe in _edFiDb.ParentElectronicMails.Where(x => x.ParentUsi == p.ParentUsi).DefaultIfEmpty()
                             from ped in _edFiDb.Descriptors.Where(x => x.DescriptorId == pe.ElectronicMailTypeDescriptorId && validEmailTypeDescriptors.Contains(x.CodeValue)).DefaultIfEmpty()
                             from pp in _edFiDb.ParentProfiles.Where(x => x.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             from ppe in _edFiDb.ParentProfileElectronicMails
                                 .Where(x => x.ParentUniqueId == p.ParentUniqueId && x.PrimaryEmailAddressIndicator == true)
                                 .DefaultIfEmpty()
                             from ppt in _edFiDb.ParentTelephones.Where(x =>
                                 x.ParentUsi == p.ParentUsi && x.TelephoneNumberTypeDescriptorId == mobileTelephoneDescriptor.DescriptorId).DefaultIfEmpty()
                             from cl in _edFiDb.ChatLogs.Where(x => x.RecipientUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                             from gmlcl in _edFiDb.GroupMessagesLogChatLogs.Where(x => x.ChatLogId == cl.Id && x.GroupMessagesLogId == queueId).DefaultIfEmpty()
                             where ssa.SchoolId == schoolId
                                   && sy.CurrentSchoolYear
                                   && sess.BeginDate <= endOfDayTodayWithPadding && sess.EndDate >= beginningOfDayTodayWithPadding
                                   && validParentDescriptors.Contains(dspa.CodeValue)
                                   && ssa.EntryDate <= endOfDayTodayWithPadding && (ssa.ExitWithdrawDate >= beginningOfDayTodayWithPadding || ssa.ExitWithdrawDate == null)
                                   && (gmlcl == null || gmlcl.Status != 3)
                             select new
                             {
                                 ParentUsi = p.ParentUsi,
                                 ParentUniqueId = p.ParentUniqueId,
                                 ParentFirstName = p.FirstName,
                                 ParentLastSurname = p.LastSurname,
                                 EdFiEmail = pe.ElectronicMailAddress,
                                 ProfileEmail = ppe.ElectronicMailAddress,
                                 ProfileTelephone = ppt.TelephoneNumber,
                                 LanguageCode = pp.LanguageCode,
                                 // This validation is for prod database, the PreferredMethodOfContactTypeId some times is null
                                 // *Note: In case of the PreferredMethodOfContactTypeId is null should be Email by Default
                                 PreferredMethodOfContactTypeId = pp.PreferredMethodOfContactTypeId != null ? pp.PreferredMethodOfContactTypeId : ppt != null ? MethodOfContactTypeEnum.SMS.Value : MethodOfContactTypeEnum.Email.Value,

                                 StudentFirstName = s.FirstName,
                                 StudentLastSurname = s.LastSurname,
                                 StudentUsi = s.StudentUsi,
                                 StudentUniqueId = s.StudentUniqueId,

                                 // Grade and Programs info for filtering,
                                 Grades = ssa.EntryGradeLevelDescriptorId
                             });


            if (gradeLevels.Length > 0)
                baseQuery = baseQuery.Where(x => gradeLevels.Contains(x.Grades));

            if (parentUsis.Length > 0)
                baseQuery = baseQuery.Where(x => parentUsis.Contains(x.ParentUsi));

            baseQuery = (from q in baseQuery
                         group new { q } by q.ParentUsi into g
                         select g.FirstOrDefault().q);

            var query = (from q in baseQuery
                         select new ParentStudentsModel
                         {
                             ParentUsi = q.ParentUsi,
                             ParentUniqueId = q.ParentUniqueId,
                             ParentFirstName = q.ParentFirstName,
                             ParentLastSurname = q.ParentLastSurname,
                             EdFiEmail = q.EdFiEmail,
                             ProfileEmail = q.ProfileEmail,
                             ProfileTelephone = q.ProfileTelephone,
                             LanguageCode = q.LanguageCode,
                             PreferredMethodOfContactTypeId = q.PreferredMethodOfContactTypeId,
                             StudentFirstName = q.StudentFirstName,
                             StudentLastSurname = q.StudentLastSurname,
                             StudentUsi = q.StudentUsi,
                             StudentUniqueId = q.StudentUniqueId
                         });


            var students = await query.ToListAsync();

            return students;
        }

        public async Task<List<UnreadDirectMessageAdvisoryModel>> GetUnAdvisedChatLogs(string[] validEmailTypeDescriptors, string[] validMobileTelephoneNumberTypeDescriptors)
        {
            _edFiDb.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            var chatLogs = (from ct in _edFiDb.ChatLogs
                            where ct.RecipientHasRead == false
                                  && !_edFiDb.GroupMessagesLogChatLogs.Select(x => x.ChatLogId).Contains(ct.Id)
                                  && !_edFiDb.ChatLogAdvisoryLogs.Select(y => y.ChatLogId).Contains(ct.Id)
                                  && ct.RecipientTypeId == ChatLogPersonTypeEnum.Parent.Value
                            select ct);

            var tempcl = chatLogs.ToList();

            var parentProfileInfo = (from p in _edFiDb.Parents
                                         //from pt in _edFiDb.ParentTelephones.Where(x => x.ParentUsi == p.ParentUsi).DefaultIfEmpty()
                                         //from ptd in _edFiDb.Descriptors.Where(x => x.DescriptorId == pt.TelephoneNumberTypeDescriptorId && validMobileTelephoneNumberTypeDescriptors.Contains(x.CodeValue)).DefaultIfEmpty()
                                     from ptdx in (from pt in _edFiDb.ParentTelephones join dt in _edFiDb.Descriptors on pt.TelephoneNumberTypeDescriptorId equals dt.DescriptorId where pt.ParentUsi == p.ParentUsi && validMobileTelephoneNumberTypeDescriptors.Contains(dt.CodeValue) select pt).DefaultIfEmpty()
                                         //from pe in _edFiDb.ParentElectronicMails.Where(x => x.ParentUsi == p.ParentUsi).DefaultIfEmpty()
                                         //from ped in _edFiDb.Descriptors.Where(x => x.DescriptorId == pe.ElectronicMailTypeDescriptorId && validEmailTypeDescriptors.Contains(x.CodeValue)).DefaultIfEmpty()
                                     from pemx in (from pem in _edFiDb.ParentElectronicMails join dem in _edFiDb.Descriptors on pem.ElectronicMailTypeDescriptorId equals dem.DescriptorId where pem.ParentUsi == p.ParentUsi && validEmailTypeDescriptors.Contains(dem.CodeValue) select pem).DefaultIfEmpty()
                                     from pp in _edFiDb.ParentProfiles.Where(r => r.ParentUniqueId == p.ParentUniqueId).DefaultIfEmpty()
                                     from ppt in _edFiDb.ParentProfileTelephones.Where(x => x.ParentUniqueId == pp.ParentUniqueId && x.TextMessageCapabilityIndicator == true && x.PrimaryMethodOfContact == true).DefaultIfEmpty()
                                     from ppem in _edFiDb.ParentProfileElectronicMails.Where(y => y.ParentUniqueId == pp.ParentUniqueId && y.PrimaryEmailAddressIndicator == true).DefaultIfEmpty()
                                     where chatLogs.Select(c => c.RecipientUniqueId).Contains(p.ParentUniqueId)
                                     select new
                                     {
                                         p.ParentUniqueId,
                                         p.FirstName,
                                         p.LastSurname,
                                         ptdx.TelephoneNumber,
                                         pemx.ElectronicMailAddress,
                                         PreferredMethodOfContactTypeId = (int?)pp.PreferredMethodOfContactTypeId,
                                         ProfileLanguageCode = pp.LanguageCode,
                                         ProfileTelephoneNumber = ppt.TelephoneNumber,
                                         ProfileElectronicMailAddress = ppem.ElectronicMailAddress
                                     });

            var tempppi = parentProfileInfo.ToList();

            var model = await (from c in chatLogs
                               join ppi in parentProfileInfo on c.RecipientUniqueId equals ppi.ParentUniqueId
                               join s in _edFiDb.Staffs on c.SenderUniqueId equals s.StaffUniqueId
                               join stu in _edFiDb.Students on c.StudentUniqueId equals stu.StudentUniqueId
                               select new UnreadDirectMessageAdvisoryModel
                               {
                                   ChatLogId = c.Id,
                                   StaffUniqueId = s.StaffUniqueId,
                                   StaffFirstName = s.FirstName,
                                   StaffLastSurname = s.LastSurname,
                                   StudentFirstName = stu.FirstName,
                                   StudentLastSurname = stu.LastSurname,
                                   ParentUniqueId = ppi.ParentUniqueId,
                                   ParentFirstName = ppi.FirstName,
                                   ParentLastSurname = ppi.LastSurname,
                                   ParentMobileTelephoneNumber = ppi.TelephoneNumber,
                                   ParentEdFiHomeElectronicMailAddress = ppi.ElectronicMailAddress,
                                   PreferredMethodOfContactTypeId = (int?)ppi.PreferredMethodOfContactTypeId,
                                   ParentProfileTelephoneNumber = ppi.ProfileTelephoneNumber,
                                   ParentProfileElectronicMailAddress = ppi.ProfileElectronicMailAddress,
                                   ParentProfileLanguageCode = ppi.ProfileLanguageCode == null ? "en" : ppi.ProfileLanguageCode,
                                   EnglishMessage = c.EnglishMessage,
                                   TranslatedMessage = c.TranslatedMessage,
                                   DateSent = c.DateSent,
                                   TranslatedLanguageCode = c.TranslatedLanguageCode
                               }).ToListAsync();

            return model;
        }
        public async Task WriteMessageAdvisoryLog(Guid chatLogId, string advisoryTypeSent, string message)
        {
            var chatLogAdvisory = new ChatLogAdvisoryLog
            {
                ChatLogId = chatLogId,
                AdvisoryTypeSent = advisoryTypeSent,
                DateTimeSent = DateTime.Now,
                Status = GroupMessagesStatusEnum.Sent.Value,
                Message = message
            };

            _edFiDb.ChatLogAdvisoryLogs.Add(chatLogAdvisory);

            await _edFiDb.SaveChangesAsync();
        }

        public Task<bool> ChatLogAdvisoryExists(Guid chatLogId)
        {
            return _edFiDb.ChatLogAdvisoryLogs.AnyAsync(x => x.ChatLogId == chatLogId);
        }
    }
}
