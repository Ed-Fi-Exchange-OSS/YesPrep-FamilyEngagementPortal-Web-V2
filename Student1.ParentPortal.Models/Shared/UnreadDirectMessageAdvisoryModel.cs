using System;

namespace Student1.ParentPortal.Models.Shared
{
    public class UnreadDirectMessageAdvisoryModel
    {
        public Guid ChatLogId { get; set; }
        public string StaffUniqueId { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffLastSurname { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastSurname { get; set; }
        public string ParentUniqueId { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastSurname { get; set; }
        public string ParentMobileTelephoneNumber { get; set; }
        public string ParentEdFiHomeElectronicMailAddress { get; set; }
        public int? PreferredMethodOfContactTypeId { get; set; }
        public string ParentProfileTelephoneNumber { get; set; }
        public string ParentProfileElectronicMailAddress { get; set; }
        public string ParentProfileLanguageCode { get; set; }
        public string EnglishMessage { get; set; }
        public string TranslatedMessage { get; set; }
        public DateTime DateSent { get; set; }
        public string TranslatedLanguageCode { get; set; }

    }
}
