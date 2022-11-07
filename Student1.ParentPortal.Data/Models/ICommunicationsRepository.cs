using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Chat;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Models.Staff;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Models.User;

namespace Student1.ParentPortal.Data.Models
{
    public interface ICommunicationsRepository
    {
        Task<ChatLogHistoryModel> GetThreadByParticipantsAsync(string studentUniqueId, string senderUniqueId, int senderTypeid, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int? unreadMessageCount, int rowsToRetrieve = 15);
        Task<ChatLogItemModel> PersistMessage(ChatLogItemModel model);
        Task<List<UnreadMessageModel>> RecipientUnreadMessages(string recipientUniqueId, int recipientTypeId);
        Task<int> UnreadMessageCount(int studentUsi, string recipientUniqueId, int recipientTypeId, string senderUniqueid, int? senderTypeId);
        Task SetRecipientRead(ChatLogItemModel model);
        Task SetRecipientsRead(string studentUniqueId, string senderUniqueId, int senderTypeid, string recipientUniqueId, int recipientTypeId);
        Task<AllRecipients> GetAllParentRecipients(int? studentUsi, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, string[] validLeadersDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday);
        Task<AllRecipients> GetAllStaffRecipients(int? studentUsi, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, DateTime endOfDayToday, DateTime beginningOfDayToday);
        Task<AllRecipients> GetAllCampusLeaderRecipients(int? studentUsi, string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, string[] validLeadersDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding);        
        Task<List<UnreadMessageModel>> PrincipalRecipientMessages(string recipientUniqueId, int recipientTypeId, int rowsToSkip, int rowsToRetrieve, string searchTerm, bool onlyUnreadMessages);
        Task<int> PrincipalRecipientMessagesCount(string recipientUniqueId, int recipientTypeId, string searchTerm, bool onlyUnreadMessages);
        Task<ParentStudentCountModel> GetFamilyMembersByGradesAndPrograms(int staffUsi, ParentStudentCountFilterModel model, string[] validParentDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday);
        Task<List<ParentStudentsModel>> GetParentsByGradeLevelsAndPrograms(Guid queueId, string personUniqueId, int schoolId, int[] grades, int[] programs, string[] validParentDescriptors, string[] validEmailTypeDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday);
        Task<GroupMessagesQueueLogModel> PersistQueueGroupMessage(GroupMessagesQueueLogModel model);
        Task<GroupMessagesChatLogModel> PersistChatGroupMessage(GroupMessagesChatLogModel model);
        Task<List<GroupMessagesQueueLogModel>> GetGroupMessagesQueuesQueued();
        Task<List<GroupMessagesQueueLogModel>> GetGroupMessagesQueuesQueued(string staffUniqueId, int schoolId, QueuesFilterModel model);
        Task<GroupMessagesQueueLogModel> UpdateGroupMessagesQueue(GroupMessagesQueueLogModel model);
        Task<List<ParentStudentsModel>> GetParentsByGradeLevelsAndSearchTerm(string personUniqueId, string term, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday, int schoolId);
        Task<GroupMessagesQueueLogModel> GetGroupMessagesQueue(Guid Id);
        Task<ChatLogItemModel> EnsureGroupMessageChatLog(Guid groupMessageQueueId, ChatLogItemModel model);
        Task<List<ParentStudentsModel>> GetParentsByPanrentUsisAndGradeLevels(Guid queueId, string personUniqueId, int schoolId, int[] parentUsis, int[] gradeLevels, string[] validParentDescriptors, string[] validEmailTypeDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday);
        Task<int> GetParentsByGradeLevelsAndSearchTermCount(string personUniqueId, string term, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday, int schoolId);
        Task<List<StudentBriefModel>> GetStudentListByGradeLevelsAndSearchTerm(string term, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayToday, DateTime beginningOfDayToday, int schoolId);
        Task<List<StudentBriefModel>> GetStudentListByGradeLevelsProgramsAndSearchTerm(string searchTerm, GradesLevelModel model, string[] validParentDescriptors, DateTime endOfDayTodayWithPadding, DateTime beginningOfDayTodayWithPadding, int schoolId, int[] programs);
        Task<List<UnreadDirectMessageAdvisoryModel>> GetUnAdvisedChatLogs(string[] validEmailTypeDescriptors, string[] validMobileTelephoneNumberTypeDescriptors);
        Task WriteMessageAdvisoryLog(Guid chatLogId, string advisoryTypeSent, string errorMessage);
        Task<bool> ChatLogAdvisoryExists(Guid chatLogId);
    }
}
